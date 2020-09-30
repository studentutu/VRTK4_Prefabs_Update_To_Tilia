using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.XR.Interaction.Toolkit.UI
{
    public class WorldCanvasSetter : MonoBehaviour
    {
        private static Camera eventCamera;
        private static List<Canvas> canvases = new List<Canvas>();

        public static Camera EventCamera
        {
            get
            {
                if (eventCamera == null)
                {
                    CreateEventCamera();
                    InitializeCanvases();
                }

                return eventCamera;
            }
        }

        private static void CreateEventCamera()
        {
            if (eventCamera == null)
            {
                var go = new GameObject("Disabled UI Event Camera");
                go.hideFlags = HideFlags.DontSave;
                eventCamera = go.AddComponent<Camera>();
                eventCamera.fieldOfView = 5f;
                eventCamera.nearClipPlane = 0.01f;
                eventCamera.clearFlags = CameraClearFlags.Nothing;
                eventCamera.enabled = false;
            }
        }

        private static void InitializeCanvases()
        {
            var canvases = FindObjectsOfType<Canvas>();
            foreach (var canvas in canvases)
            {
                InitializeCanvas(canvas);
            }
        }

        private static void InitializeCanvas(Canvas canvas)
        {
            if (canvas.renderMode != RenderMode.WorldSpace)
            {
                Debug.LogWarning(
                    $"Canvas on {canvas.gameObject} GameObject is not in WorldSpace more and will not work with spatial input");
                return;
            }

            canvas.worldCamera = EventCamera;
        }

        [SerializeField] private Canvas currentCanvas;

        private void OnEnable()
        {
            canvases.Add(currentCanvas);
            InitializeCanvas(currentCanvas);
        }

        private void OnDisable()
        {
            canvases.Remove(currentCanvas);
        }
    }
}
