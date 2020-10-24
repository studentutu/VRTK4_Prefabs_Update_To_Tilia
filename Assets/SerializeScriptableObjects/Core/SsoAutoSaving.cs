using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Sso.Core
{
    public class SsoAutoSaving : UnityEditor.AssetModificationProcessor
    {
       static string[] OnWillSaveAssets( string[] filenames )
		{
			// Do we have any pending actions? If so, execute them now.
			// This prevents a ctrl-s spam causing us to lose cross-scene references on second save.
			if ( EditorApplication.delayCall != null )
			{
				var delayCall = EditorApplication.delayCall;
				EditorApplication.delayCall = null;

				delayCall();
			}

			// Check if we're saving any scenes
			List<Scene> savingScenes = new List<Scene>();
			foreach( var filename in filenames )
			{
				var scene = EditorSceneManager.GetSceneByPath(filename);
				if ( scene.IsValid() )
				{
					savingScenes.Add(scene);
				}
			}

			// Weird Unity issue where it doesn't come in here with a filename when saving a new scene (or a non-dirty scene).
			bool bIsSaveNewScene = (filenames.Length < 1);
			if ( bIsSaveNewScene )
			{
				savingScenes.Add( EditorSceneManager.GetActiveScene() );
			}

			HandleSavingScenes( savingScenes );
			HandleCrossSceneReferences( savingScenes );

			return filenames;
		}

		private static void HandleSavingScenes( IList<Scene> scenes )
		{
			// We need to create an AmsMultiSceneSetup singleton in every scene.  This is how we keep track of Awake scenes and
			// it also allows us to use cross-scene references.
			// foreach( var scene in scenes )
			// {
			// 	if ( !scene.isLoaded )
			// 		continue;
			//
			// 	var sceneSetup = GameObjectEx.GetSceneSingleton<AmsMultiSceneSetup>( scene, true );
			// 	sceneSetup.OnBeforeSerialize();
			// }
		}

		public static void HandleCrossSceneReferences( IList<Scene> scenes )
		{
			
		}
    }
}
