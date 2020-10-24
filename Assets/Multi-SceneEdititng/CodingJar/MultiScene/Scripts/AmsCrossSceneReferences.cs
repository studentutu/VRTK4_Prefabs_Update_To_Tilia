using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using CodingJar;

using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Reflection;
#endif

namespace CodingJar.MultiScene
{
    /// <summary>
	/// Holds cross-scene references for a particular scene.
    /// </summary>
	[ExecuteInEditMode]
    public sealed class AmsCrossSceneReferences : MonoBehaviour
#if UNITY_EDITOR
		, ISerializationCallbackReceiver
#endif
    {
        [SerializeField]    private List<RuntimeCrossSceneReference>	_crossSceneReferences = new List<RuntimeCrossSceneReference>();
		[SerializeField, HideInInspector]	private List<GameObject>	_realSceneRootsForPostBuild = new List<GameObject>();
		
		private List<RuntimeCrossSceneReference>	_referencesToResolve = new List<RuntimeCrossSceneReference>();

		/// <summary>
		/// An event that you can register with in order to receive notifications that a cross-scene reference was restored.
		/// </summary>
		public static event System.Action<RuntimeCrossSceneReference>	CrossSceneReferenceRestored;

		/// <summary>
		/// Return the Singleton for a given scene (there is one per Scene).
		/// </summary>
		/// <param name="scene">The Scene to obtain the singleton for</param>
		/// <returns>The per-scene singleton</returns>
		public static AmsCrossSceneReferences	GetSceneSingleton( Scene scene, bool bCreateIfNotFound )
		{
			var multiSceneSetup = GameObjectEx.GetSceneSingleton<AmsMultiSceneSetup>( scene, bCreateIfNotFound );
			if ( !multiSceneSetup )
				return null;

			var existing = multiSceneSetup.gameObject.GetComponent<AmsCrossSceneReferences>();
			if ( existing )
				return existing;
			else if ( bCreateIfNotFound )
				return multiSceneSetup.gameObject.AddComponent<AmsCrossSceneReferences>();

			return null;
		}

		/// <summary>
		/// Adds a cross-scene reference.
		/// </summary>
		/// <param name="reference"></param>
		public void AddReference( RuntimeCrossSceneReference reference )
		{
			int index = _crossSceneReferences.FindIndex( reference.IsSameSource );
			if ( index >= 0 )
			{
				_crossSceneReferences[index] = reference;
			}
			else
			{
				_crossSceneReferences.Add( reference );
			}
		}

		/// <summary>
		/// Remove all of the stored cross-scene references that reference 'toScene'.
		/// </summary>
		public void ResetCrossSceneReferences( Scene toScene )
		{
			_crossSceneReferences.RemoveAll( x => (x.toScene.scene == toScene) );
		}

		void Awake()
		{
			//AmsDebug.Log( this, "{0}.Awake() Scene: {1}. IsLoaded: {2}. Path: {3}. Frame: {4}. Root Count: {5}", GetType().Name, gameObject.scene.name, gameObject.scene.isLoaded, gameObject.scene.path, Time.frameCount, gameObject.scene.rootCount );

			// We need to queue our cross-scene references super early in case we get merged.
			_referencesToResolve.Clear();
			_referencesToResolve.AddRange( _crossSceneReferences );
		}

		void Start()
		{
			AmsDebug.Log( this, "{0}.Start() Scene: {1}. IsLoaded: {2}. Path: {3}. Frame: {4}. Root Count: {5}", GetType().Name, gameObject.scene.name, gameObject.scene.isLoaded, gameObject.scene.path, Time.frameCount, gameObject.scene.rootCount );

#if UNITY_EDITOR
            EditorCheckSceneRename();
#endif

            // A build might have just been performed, in that case clean-up the leftovers.
            PerformPostBuildCleanup();

			// For some reason in Awake(), the scene we belong to isn't considered "loaded"?!  We must resolve our cross-scene references here.
			ResolvePendingCrossSceneReferences();

			// Register to these callbacks only once
			AmsMultiSceneSetup.OnStart -= HandleNewSceneLoaded;
			AmsMultiSceneSetup.OnStart += HandleNewSceneLoaded;
			AmsMultiSceneSetup.OnDestroyed -= HandleSceneDestroyed;
			AmsMultiSceneSetup.OnDestroyed += HandleSceneDestroyed;
		}

		void OnDestroy()
		{
			AmsMultiSceneSetup.OnStart -= HandleNewSceneLoaded;
			AmsMultiSceneSetup.OnDestroyed -= HandleSceneDestroyed;
		}

		/// <summary>
		/// Whenever another scene is loaded, we have another shot at resolving more cross-scene references
		/// </summary>
		/// <param name="sceneSetup">The AmsMultiSceneSetup that was loaded</param>
		private void HandleNewSceneLoaded( AmsMultiSceneSetup sceneSetup )
		{
			var loadedScene = sceneSetup.gameObject.scene;
			if ( !loadedScene.isLoaded )
				Debug.LogErrorFormat( this, "{0} Received HandleNewSceneLoaded from scene {1} which isn't considered loaded.  The scene MUST be considered loaded by this point", GetType().Name, loadedScene.name );

			// Restore any references to this newly loaded scene
			foreach( var xRef in _crossSceneReferences )
			{
				if ( !_referencesToResolve.Contains(xRef) && xRef.toScene.scene == loadedScene )
					_referencesToResolve.Add( xRef );
			}

			if ( _referencesToResolve.Count > 0 )
			{
				AmsDebug.Log( this, "Scene {0} Loaded. {1} Cross-Scene References (in total) from Cross-Scene Manager in {2} are queued for resolve.", loadedScene.name, _referencesToResolve.Count, gameObject.scene.name );
				ConditionalResolveReferences( _referencesToResolve );
			}
		}

		/// <summary>
		/// Whenever a scene is destroyed, we will receive this callback.  In the editor, we can remember that we may be about to lose a cross-scene reference.
		/// </summary>
		/// <param name="sceneSetup"></param>
		private void HandleSceneDestroyed( AmsMultiSceneSetup sceneSetup )
		{
			var destroyedScene = sceneSetup.gameObject.scene;
			if ( !destroyedScene.IsValid() )
				return;

			// If our own scene is being destroyed, we don't need to do anymore work
			if ( destroyedScene == gameObject.scene )
				return;

			// Remove all of the pending refs for that scene.
			_referencesToResolve.RemoveAll( x => x.toScene.scene == destroyedScene );

			// Now we re-add all of the relevant refs to pending.  They'll be re-resolved when the scene is loaded again.
			var allRelevantRefs = _crossSceneReferences.Where( x => x.toScene.scene == destroyedScene );
			_referencesToResolve.AddRange( allRelevantRefs );
		}

		/// <summary>
		/// This is a co-routine for waiting until a given scene is loaded, then performing a cross-scene reference resolve
		/// </summary>
		/// <param name="scene">The scene to guarantee loaded</param>
		System.Collections.IEnumerator	CoWaitForSceneLoadThenResolveReferences( Scene scene )
		{
			if ( !Application.isPlaying )
			{
				Debug.LogErrorFormat( this, "CoWaitForSceneLoadThenResolveReferences called, but we're not playing. Co-routines do not work reliably in the Editor!" );
				yield break;
			}

			if ( !scene.IsValid() )
				yield break;

			while ( !scene.isLoaded )
				yield return null;

			ResolvePendingCrossSceneReferences();
		}

		[ContextMenu("Retry Pending Resolves")]
		public void ResolvePendingCrossSceneReferences()
		{
			ConditionalResolveReferences( _referencesToResolve );
		}

		[ContextMenu("Retry ALL Resolves")]
		private void RetryAllResolves()
		{
			_referencesToResolve.Clear();
			_referencesToResolve.AddRange( _crossSceneReferences );
			
			ResolvePendingCrossSceneReferences();
		}

		private void ConditionalResolveReferences( List<RuntimeCrossSceneReference> references )
		{
			for(int i = references.Count - 1 ; i >= 0 ; --i)
			{
				var xRef = references[i];

				try
				{
					var fromScene = xRef.fromScene;
					var toScene= xRef.toScene;
					bool bIsReady = fromScene.isLoaded && toScene.isLoaded;

					AmsDebug.Log( this, "{0}.ConditionalResolveReferences() Scene: {1}. xRef: {2}. isReady: {3}. fromSceneLoaded: {4}. toSceneLoaded: {5}.", GetType().Name, gameObject.scene.name, xRef, bIsReady, fromScene.isLoaded, toScene.isLoaded );

					if ( bIsReady )
					{
						// Remove it from our list (assuming it goes through)
						references.RemoveAt(i);

						AmsDebug.Log( this, "Restoring Cross-Scene Reference {0}", xRef );
						xRef.Resolve();

						// Notify any listeners
						if ( CrossSceneReferenceRestored != null )
							CrossSceneReferenceRestored( xRef );
					}
				}
				catch ( System.Exception ex )
				{
					Debug.LogException( ex, this );
				}
			}
		}

		void PerformPostBuildCleanup()
		{
			if ( Application.isEditor && !Application.isPlaying && _realSceneRootsForPostBuild.Count > 0 )
			{
				GameObject[] newSceneRoots = gameObject.scene.GetRootGameObjects();
				foreach( GameObject root in newSceneRoots )
				{
					if ( !_realSceneRootsForPostBuild.Contains(root) )
					{
						AmsDebug.LogWarning( this, "Destroying '{0}/{1}' since we've determined it's a temporary for a cross-scene reference", gameObject.scene.name, root.name );
						DestroyImmediate( root );
					}
				}

				_realSceneRootsForPostBuild.Clear();
			}
		}

#if UNITY_EDITOR
		/// <summary>
		/// Callback that this object has been deserialized which can happen on a level-load, or on a hot recompile
		/// </summary>
		public void OnAfterDeserialize()
		{
			// We should register for this callback (but only once).
			AmsMultiSceneSetup.OnStart -= HandleNewSceneLoaded;
			AmsMultiSceneSetup.OnStart += HandleNewSceneLoaded;

			AmsMultiSceneSetup.OnDestroyed -= HandleSceneDestroyed;
			AmsMultiSceneSetup.OnDestroyed += HandleSceneDestroyed;
		}

		/// <summary>
		/// Callback that this object is about to be serialized which can happen when saving, or when simply inspecting the object.
		/// </summary>
		public void OnBeforeSerialize()
		{
			if ( !BuildPipeline.isBuildingPlayer )
				return;

			if ( EditorSceneManager.sceneCount < 2 )
				return;

			// Give us one last shot...
			ResolvePendingCrossSceneReferences();

			// Warn about any references that were not yet resolved...
			foreach( var xRef in _referencesToResolve )
			{
				Debug.LogWarningFormat( "Did not resolve Cross-Scene Reference during build: {0}", xRef );
			}

			// This deserves an explanation.  This gets serialized right before we do a build IF this scene is active
			// at the time of building.  The problem is, when we come back from the build, we get duplicate prefabs in
			// the scene since the cross-scene reference was still active.  So what we wanna do, is delete those when
			// we come back from the build.
			gameObject.scene.GetRootGameObjects( _realSceneRootsForPostBuild  );
		}

        void EditorCheckSceneRename()
        {
            // Did this happen through a rename?
            if (!Application.isEditor || !gameObject.scene.IsValid() || !gameObject.scene.isLoaded)
                return;

            // Go through each cross-scene reference and see if any are assigned to the wrong "from scene" (should always be this)
            for (int i = 0; i < _crossSceneReferences.Count; ++i)
            {
                var xRef = _crossSceneReferences[i];
                if (xRef.fromScene.IsValid() && xRef.fromScene.isLoaded)
                    continue;

                var oldScene = xRef.fromScene;

                // Re-assign this scene...
                xRef.fromScene = new AmsSceneReference(gameObject.scene);

                // Now resolve and see if that works...
                Object obj = xRef.fromObject;
                if (obj)
                {
                    _crossSceneReferences[i] = xRef;
                    Debug.LogWarningFormat(this, "Fixing up probable cross-scene reference duplication. Old Scene = {0}. New Scene = {1}", oldScene.editorPath, xRef.fromScene.editorPath);
                    EditorUtility.SetDirty(this);

                    _referencesToResolve.Add(xRef);
                }
                else
                {
                    // Revert that change
                    xRef.fromScene = oldScene;
                }
            }
        }
#endif

	} // class
} // namespace
