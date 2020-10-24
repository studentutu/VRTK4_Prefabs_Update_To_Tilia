using UnityEngine;
using UnityEditor;

using System.Collections.Generic;

using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

namespace CodingJar.MultiScene.Editor
{
	public static class AmsPlaymodeHandler
	{
		[UnityEditor.InitializeOnLoadMethod]
		private static void SaveCrossSceneReferencesBeforePlayInEditMode()
		{
			EditorApplication.playmodeStateChanged += () =>
				{
					if ( !EditorApplication.isPlaying && EditorApplication.isPlayingOrWillChangePlaymode )
					{
						List<Scene> allScenes = new List<Scene>( EditorSceneManager.sceneCount );
						for (int i = 0 ; i < EditorSceneManager.sceneCount ; ++i)
						{
							var scene = EditorSceneManager.GetSceneAt(i);
							if ( scene.IsValid() && scene.isLoaded )
								allScenes.Add( scene );
						}

						AmsDebug.Log( null, "Handling Cross-Scene Referencing for Playmode" );
						AmsSaveProcessor.HandleCrossSceneReferences( allScenes );
					}
				};
		}

	} // class
} // namespace