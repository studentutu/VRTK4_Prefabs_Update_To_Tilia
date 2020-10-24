using UnityEngine;
using System.Collections.Generic;

namespace CodingJar.MultiScene
{
	/// <summary>
	/// A UniqueObject that is represented by a Scene and an ID in the Scene.
	/// </summary>
	[System.Serializable]
	public partial struct UniqueObject
	{
		public AmsSceneReference	scene;
		public string				fullPath;
		public string				componentName;
		public int					componentIndex;
		public int					editorLocalId;
		public string				editorPrefabRelativePath;

		// So we can version and auto-upgrade
		[SerializeField, HideInInspector]	private int		version;

		private static int	CurrentSerializedVersion = 1;
		private static List<Component>	_reusableComponentsList = new List<Component>();

		/// <summary>
		/// Resolve a cross-scene reference if possible.
		/// </summary>
		/// <returns>The cross-scene referenced object if it's possible</returns>
		private Object	RuntimeResolve()
		{
			var scene = this.scene.scene;

			if ( !scene.IsValid() )
				return null;

			// Try to find the Object
			GameObject gameObject = GameObjectEx.FindBySceneAndPath( scene, fullPath );
			if ( !gameObject )
				return null;

			if ( string.IsNullOrEmpty(componentName) )
				return gameObject;

			// This is the old method where we didn't store the component index (deprecated)
			if ( version < 1 )
			{
				Component oldStyleComponent = gameObject.GetComponent( componentName );
				if ( componentIndex < 0 || oldStyleComponent )
					return oldStyleComponent;
			}

			// Get the component and index
			System.Type type = System.Type.GetType( componentName, false, true );
			if ( type != null )
			{
				gameObject.GetComponents( type, _reusableComponentsList );
				if ( componentIndex < _reusableComponentsList.Count )
					return _reusableComponentsList[componentIndex];
			}

			return null;
		}

		internal Object		Resolve()
		{
			Object obj = RuntimeResolve();

#if UNITY_EDITOR
			if ( !obj && !Application.isPlaying )
				obj = EditorResolveSlow();
#endif

			return obj;
		}


		public override string ToString()
		{
			System.Type type = string.IsNullOrEmpty(componentName) ? null : System.Type.GetType( componentName, false, true );
			return string.Format( "{0}'{1}' ({2} #{3})", scene.name, fullPath, type != null ? type.FullName : "GameObject", componentIndex );
		}

	} // struct
} // namespace 
