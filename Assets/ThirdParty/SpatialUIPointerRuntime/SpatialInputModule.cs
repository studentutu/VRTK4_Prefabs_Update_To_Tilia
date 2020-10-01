using System;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

namespace Adrenak.SUI 
{
    public class SpatialInputModule : BaseInputModule 
    {
        public class EventDataToPointer
        {
            public SpatialInputPointer pointer;
            public PointerEventData EventData;
            public RaycastResult RaycastResult;
            public bool lastInputReady = false;
            public GameObject lastPressed;
            public bool InputReady => pointer.input;
            public EventDataToPointer(SpatialInputPointer pointer, EventSystem es, Camera cam)
            {
                this.pointer = pointer;
                EventData = new PointerEventData(es);
                EventData.position = new Vector2(cam.pixelWidth / 2, cam.pixelHeight / 2);
            }
        }
        private static SpatialInputModule instance;
        private static bool Quit = false;
        
        private List<EventDataToPointer> listOfPointers = new List<EventDataToPointer>();
        private Camera eventCamera;
        private EventDataToPointer dataToPointer;
        private int ProcessedThisFrame = 0;
        
        protected Camera EventCamera
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

        public void AddPointer(SpatialInputPointer pointer)
        {
            RemovePointer(pointer);
            var newDataPointer = new EventDataToPointer(pointer, EventSystem.current, EventCamera);
            listOfPointers.Add(newDataPointer);
        }
        
        public void RemovePointer(SpatialInputPointer pointer)
        {
            InputReady();
            for (int i = 0; i < listOfPointers.Count; i++)
            {
                if (listOfPointers[i].pointer == pointer)
                {
                    listOfPointers.RemoveAt(i);
                    i--;
                }
            }
        }

        protected virtual bool InputReady()
        {
            // Clean All NUll
            for (int i = 0; i < listOfPointers.Count; i++)
            {
                if (listOfPointers[i].pointer == null )
                {
                    listOfPointers.RemoveAt(i);
                    i--;
                }
            }
            for (int i = 0; i < listOfPointers.Count; i++)
            {
                if (listOfPointers[i].pointer != null && listOfPointers[i].pointer)
                {
                    return true;
                }
            }
            return false;
        }
        
        public EventDataToPointer GetPointerData(SpatialInputPointer forPointer)
        {
            for (int i = 0; i < listOfPointers.Count; i++)
            {
                if (listOfPointers[i].pointer == forPointer)
                {
                    return listOfPointers[i];
                }
            }
            return null;
        }

        private static void OnQuit()
        {
            Quit = true;
        }

        public static SpatialInputModule Instance 
        {
            get
            {
                Application.quitting -= OnQuit;
                Application.quitting += OnQuit;
                if (Quit)
                {
                    return null;
                }

                if (EventSystem.current == null)
                    EventSystem.current = FindObjectOfType<EventSystem>();

                if(EventSystem.current == null)
                    EventSystem.current = new GameObject("EventSystem").AddComponent<EventSystem>();

                if (instance == null) 
                    instance = FindObjectOfType<SpatialInputModule>();

                if (instance == null) 
                    instance = EventSystem.current.gameObject.AddComponent<SpatialInputModule>();

                return instance;
            }
        }

        protected override void Awake() 
        {
            base.Awake();
            CreateEventCamera();
            InitializeCanvases();
        }

        private void CreateEventCamera() 
        {
            if (eventCamera == null) 
            {
                var go = new GameObject("SUI Event Camera");
                go.hideFlags = HideFlags.DontSave;
                eventCamera = go.AddComponent<Camera>();
                eventCamera.fieldOfView = 5f;
                eventCamera.nearClipPlane = 0.01f;
                eventCamera.clearFlags = CameraClearFlags.Nothing;
                eventCamera.enabled = false;
            }
        }

        private void InitializeCanvases() 
        {
            var canvases = FindObjectsOfType<Canvas>();
            foreach (var canvas in canvases)
            {
                InitializeCanvas(canvas, EventCamera);
            }
        }

        private void InitializeCanvas(Canvas canvas, Camera eventCamera) 
        {
            if (canvas == null)
            {
                return;
            }

            if(canvas.renderMode != RenderMode.WorldSpace) 
            {
                Debug.LogWarning($"Canvas on {canvas.gameObject} GameObject is not in WorldSpace more and will not work with spatial input");
                return;
            }
            // Ensure each canvas has WorldCanvasSetter
            var component = canvas.gameObject.GetComponent<WorldCanvasSetter>();
            if (component == null)
            {
                canvas.gameObject.AddComponent<WorldCanvasSetter>();
            }

            canvas.worldCamera = eventCamera;
        }

        public void Update()
        {
            if (ProcessedThisFrame == 0)
            {
                Process();
            }

            ProcessedThisFrame = 0;
        }

        public override void Process()
        {
            ProcessedThisFrame++;
            var inputReady = InputReady();
            if (!inputReady)
            {
                for (int i = 0; i < listOfPointers.Count; i++)
                {
                    dataToPointer = listOfPointers[i];
                    if (dataToPointer.pointer == null || !dataToPointer.lastInputReady)
                    {
                        continue;
                    }
                    Release(dataToPointer);
                    dataToPointer.lastInputReady = false;
                }
                return;
            }

            for (int i = 0; i < listOfPointers.Count; i++)
            {
                if (listOfPointers[i].pointer == null)
                {
                    continue;
                }

                dataToPointer = listOfPointers[i];
                SpatialInputPointer currentPointer = dataToPointer.pointer;
                PointEventCamera(currentPointer.transform);

                eventSystem.RaycastAll(dataToPointer.EventData, m_RaycastResultCache);
                dataToPointer.EventData.pointerCurrentRaycast = FindFirstRaycast(m_RaycastResultCache);
                m_RaycastResultCache.Clear();

                dataToPointer.RaycastResult = dataToPointer.EventData.pointerCurrentRaycast;

                HandlePointerExitAndEnter(dataToPointer.EventData, dataToPointer.RaycastResult.gameObject);
                ExecuteEvents.Execute(dataToPointer.EventData.pointerDrag, dataToPointer.EventData, ExecuteEvents.dragHandler);
               
                if (dataToPointer.InputReady) 
                {
                    if (!dataToPointer.lastInputReady)
                    {
                        Down(dataToPointer);
                    }
                    else
                    {
                        Hold(dataToPointer);
                    }
                }
                else 
                {
                    if (dataToPointer.lastInputReady)
                    {
                        Release(dataToPointer);
                    }
                }

                dataToPointer.lastInputReady = dataToPointer.InputReady;
            }
        }

        private void PointEventCamera(Transform pointer) 
        {
            if (pointer == null) return;

            EventCamera.transform.parent = pointer;
            EventCamera.transform.localPosition = Vector3.zero;
            EventCamera.transform.localEulerAngles = Vector3.zero;
        }

        private void Down(EventDataToPointer dataToPointer) 
        {
            dataToPointer.EventData.pointerPressRaycast = dataToPointer.EventData.pointerCurrentRaycast;
            var target = dataToPointer.EventData.pointerPressRaycast.gameObject;

            if(dataToPointer.lastPressed != null) 
            {
                ExecuteEvents.Execute(dataToPointer.lastPressed, dataToPointer.EventData, ExecuteEvents.deselectHandler);
                dataToPointer.lastPressed = null;
            }

            var pressed = ExecuteEvents.GetEventHandler<IPointerClickHandler>(target);
            dataToPointer.EventData.pointerPress = pressed;
            ExecuteEvents.Execute(dataToPointer.EventData.pointerPress, dataToPointer.EventData, ExecuteEvents.pointerDownHandler);

            var dragged = ExecuteEvents.GetEventHandler<IDragHandler>(target);
            dataToPointer.EventData.pointerDrag = dragged;
            ExecuteEvents.Execute(dataToPointer.EventData.pointerDrag, dataToPointer.EventData, ExecuteEvents.beginDragHandler);

            dataToPointer.lastPressed = pressed;
        }

        private void Hold(EventDataToPointer dataToPointer) 
        {
            dataToPointer.EventData.pointerPressRaycast = dataToPointer.EventData.pointerCurrentRaycast;
            var target = dataToPointer.EventData.pointerPressRaycast.gameObject;

            var pressed = ExecuteEvents.GetEventHandler<IPointerClickHandler>(target);
            dataToPointer.EventData.pointerPress = pressed;
            ExecuteEvents.Execute(dataToPointer.EventData.pointerPress, dataToPointer.EventData, ExecuteEvents.pointerDownHandler);

            var draggedObject = ExecuteEvents.GetEventHandler<IDragHandler>(target);
            dataToPointer.EventData.pointerDrag = draggedObject;
            ExecuteEvents.Execute(dataToPointer.EventData.pointerDrag, dataToPointer.EventData, ExecuteEvents.beginDragHandler);
        }

        private void Release(EventDataToPointer dataToPointer) 
        {
            var target = dataToPointer.EventData.pointerCurrentRaycast.gameObject;
            if (target == null) return;

            var released = ExecuteEvents.GetEventHandler<IPointerClickHandler>(target);

            if (dataToPointer.EventData.pointerPress == released) 
                ExecuteEvents.Execute(dataToPointer.EventData.pointerPress, dataToPointer.EventData, ExecuteEvents.pointerClickHandler);

            ExecuteEvents.Execute(dataToPointer.EventData.pointerPress, dataToPointer.EventData, ExecuteEvents.pointerUpHandler);
            ExecuteEvents.Execute(dataToPointer.EventData.pointerDrag,dataToPointer. EventData, ExecuteEvents.endDragHandler);

            dataToPointer.EventData.pointerCurrentRaycast.Clear();
            dataToPointer.EventData.pointerPress = null;
            dataToPointer.EventData.pointerDrag = null;
        }

        public virtual void RegisterCanvas(Canvas canvas)
        {
            InitializeCanvas(canvas, EventCamera);
        }
    }
}