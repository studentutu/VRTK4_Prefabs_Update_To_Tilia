using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Tilia.Indicators.ObjectPointers;
using UnityEngine;
using UnityEngine.EventSystems;
using Zinnia.Cast;
using Zinnia.Data.Type;
using Zinnia.Pointer;


namespace UnityEngine.XR.Interaction.Toolkit.UI
{
    [DisallowMultipleComponent]
    public class VRTK_Set_Interactions_XR_Toolkit : MonoBehaviour
    {
        public class EventDataToPointer
        {
            public XRRayInteractor pointer;
            public PointerEventData EventData;
            public RaycastResult RaycastResult;
            public bool lastInputReady = false;
            public GameObject lastPressed;
            public bool InputReady = false;
            public EventDataToPointer(XRRayInteractor pointer, EventSystem es, Camera cam)
            {
                this.pointer = pointer;
                EventData = new PointerEventData(es);
                EventData.position = new Vector2(cam.pixelWidth / 2, cam.pixelHeight / 2);
            }
        }
        
        [SerializeField] private PointerFacade pointerFacade;
        [SerializeField] private XRRayInteractor interactor;
        [SerializeField] private XRController controller;
        
        private List<Vector3> temporalList = new List<Vector3>();
        private ObjectPointer.EventData eventData;
        private PointsCast.EventData pointerCastEventData;
        private XRUIInputModule asXruiInputModule;
        [NonSerialized]
        protected List<RaycastResult> m_RaycastResultCache = new List<RaycastResult>();

        private int checkFrames = 3;
        [NonSerialized]
        private EventDataToPointer dataToPointer = null;
        private EventDataToPointer DataToPointer
        {
            get
            {
                if (dataToPointer == null)
                {
                    dataToPointer = new EventDataToPointer(interactor, EventSystem.current, WorldCanvasSetter.EventCamera);
                }

                return dataToPointer;
            }
        }

        private MethodInfo actualMethod = null;

        private MethodInfo ActualMethod
        {
            get
            {
                if (actualMethod == null)
                {
                    // Type[] types = { typeof(TrackedDeviceModel).MakeByRefType() };
                    var bindingFlag = BindingFlags.NonPublic | BindingFlags.Instance;
                    actualMethod = typeof(UIInputModule).GetMethod("ProcessTrackedDevice",bindingFlag); //, null,types, null);
                }

                return actualMethod;
            }
        }

        private void InvokeReflexionMethod(ref TrackedDeviceModel UIMOdule)
        {
            //  asXruiInputModule.ProcessTrackedDevice(ref UIMOdule,true);
            var actualArray = new System.Object[]{ UIMOdule, true};
            if (ActualMethod != null)
            {
                ActualMethod.Invoke(asXruiInputModule, actualArray);
                UIMOdule = (TrackedDeviceModel) actualArray[0];
                var allPoints = UIMOdule.raycastPoints;
                if (allPoints.Count > 1)
                {
                    Debug.DrawLine(allPoints[0],allPoints[1]);
                }
            }
        }
        
        private MethodInfo actualMethodHandle = null;

        private MethodInfo ActualMethodHandle 
        {
            get
            {
                if (actualMethodHandle == null)
                {
                    var bindingFlag = BindingFlags.NonPublic | BindingFlags.Instance;
                    actualMethodHandle = typeof(BaseInputModule).GetMethod("HandlePointerExitAndEnter",bindingFlag); //, null,types, null);
                }

                return actualMethodHandle;
            }
        }

        private void InvokeReflexionMethodHandle( PointerEventData currentPointerData, GameObject newEnterTarget)
        {
            //  asXruiInputModule.HandlePointerExitAndEnter(currentPointerData,newEnterTarget);
            var actualArray = new System.Object[]{ currentPointerData, newEnterTarget};
            if (asXruiInputModule == null)
            {
                asXruiInputModule = EventSystem.current.GetComponent<XRUIInputModule>();
            }
            if (ActualMethodHandle != null)
            {
                ActualMethodHandle.Invoke(asXruiInputModule, actualArray);
            }
        }

        protected static RaycastResult FindFirstRaycast(List<RaycastResult> candidates)
        {
            for (var i = 0; i < candidates.Count; ++i)
            {
                if (candidates[i].gameObject == null)
                    continue;

                return candidates[i];
            }
            return new RaycastResult();
        }
        

        private void Update()
        {
            if (asXruiInputModule == null)
            {
                asXruiInputModule = EventSystem.current.GetComponent<XRUIInputModule>();
            }

            if (checkFrames < 0 && DataToPointer.lastInputReady)
            {
                checkFrames = 3;
                DataToPointer.lastInputReady = false;
            }

            checkFrames--;
            // if (asXruiInputModule != null)
            // {
            //     TrackedDeviceModel UIMOdule;
            //     asXruiInputModule.GetTrackedDeviceModel(interactor, out UIMOdule);
            //     var prev = UIMOdule.orientation;
            //     var newOne = prev * Quaternion.Euler(45, 45, 45);
            //     UIMOdule.orientation = newOne;
            //     UIMOdule.orientation = prev;
            //     UIMOdule.select = true;
            //     // asXruiInputModule.Process();
            //     interactor.UpdateUIModel(ref UIMOdule);
            //     UIMOdule.select = true;
            //     InvokeReflexionMethod(ref UIMOdule);
            // }
        }

        private void FillInEventData()
        {
            if (pointerCastEventData == null)
            {
                pointerCastEventData = new PointsCast.EventData();
                pointerCastEventData.Clear();
            }
            pointerCastEventData.Clear();
            pointerCastEventData.IsValid = true;
            
            if (eventData == null)
            {
                eventData = new ObjectPointer.EventData();
                eventData.Transform = transform;
                eventData.CurrentHoverDuration = 0.1f;
            }
            eventData.Clear();
            eventData.UseLocalValues = false;
            eventData.ScaleOverride = Vector3.one;
            eventData.Direction = pointerFacade.transform.forward;
            eventData.Origin = pointerFacade.Configuration.ObjectPointer.Origin.transform.position;
            eventData.PositionOverride = eventData.Origin;
            eventData.RotationOverride = pointerFacade.Configuration.ObjectPointer.Origin.transform.rotation;
        }

        [UnityEngine.Scripting.Preserve]
        public void HoverEnterXRToolkit(XRBaseInteractable interactable)
        {
            var listOfAllHoveredInteractors = interactable.hoveringInteractors;
            temporalList.Clear();
            XRRayInteractor asRay;
            RaycastHit hit;
            foreach (var item in listOfAllHoveredInteractors)
            {
                asRay = item as XRRayInteractor;
                if (asRay != null)
                {
                    asRay.GetCurrentRaycastHit(out hit);
                    if (hit.transform != null)
                    {
                        temporalList.Add(hit.point);
                    }
                }
            }

            FillInEventData();
            var data = eventData;
            data.CurrentHoverDuration = 0.1f;
            data.IsCurrentlyHovering = true;
            data.CurrentPointsCastData = pointerCastEventData;
            data.CurrentPointsCastData.Points = new HeapAllocationFreeReadOnlyList<Vector3>(temporalList,0,temporalList.Count);
            pointerFacade.Configuration.EmitHoverChanged(data);
        }
        
        [UnityEngine.Scripting.Preserve]
        public void OnHoverExitFromXRToolkit(XRBaseInteractable interactable)
        {
            FillInEventData();
            var data = eventData;
            data.CurrentHoverDuration = 0.9f;
            data.IsCurrentlyHovering = false;
            data.CurrentPointsCastData = pointerCastEventData;
            data.CurrentPointsCastData.Points = new HeapAllocationFreeReadOnlyList<Vector3>(temporalList,0,temporalList.Count);
            pointerFacade.Configuration.EmitHoverChanged(data);
        }
        
        [UnityEngine.Scripting.Preserve]
        public void OnSelectEnterFromXRToolkit(XRBaseInteractable interactable)
        {
            var listOfAllHoveredInteractors = interactable.hoveringInteractors;
            temporalList.Clear();
            XRRayInteractor asRay;
            RaycastHit hit;
            foreach (var item in listOfAllHoveredInteractors)
            {
                asRay = item as XRRayInteractor;
                if (asRay != null)
                {
                    asRay.GetCurrentRaycastHit(out hit);
                    if (hit.transform != null)
                    {
                        temporalList.Add(hit.point);
                    }
                }
            }
            FillInEventData();
            var data = eventData;
            data.CurrentHoverDuration = 0.1f;
            data.IsCurrentlyHovering = true;
            data.IsCurrentlyActive = true;
            data.CurrentPointsCastData = pointerCastEventData;
            data.CurrentPointsCastData.Points = new HeapAllocationFreeReadOnlyList<Vector3>(temporalList,0,temporalList.Count);
            pointerFacade.Configuration.EmitHoverChanged(data);
        }
        
        [UnityEngine.Scripting.Preserve]
        public void OnSelectExitFromXRToolkit(XRBaseInteractable interactable)
        {
            FillInEventData();
            var data = eventData;
            data.CurrentHoverDuration = 0.9f;
            data.IsCurrentlyHovering = false;
            data.IsCurrentlyActive = false;
            data.CurrentPointsCastData = pointerCastEventData;
            data.CurrentPointsCastData.Points = new HeapAllocationFreeReadOnlyList<Vector3>(temporalList,0,temporalList.Count);
            pointerFacade.Configuration.EmitHoverChanged(data);
        }

        [UnityEngine.Scripting.Preserve]
        public void OnPress()
        {
            checkFrames = 3;
            DataToPointer.InputReady = true;
            var currentPointer = DataToPointer.pointer;
            PointEventCamera(currentPointer.transform);

            EventSystem.current.RaycastAll(DataToPointer.EventData, m_RaycastResultCache);
            DataToPointer.EventData.pointerCurrentRaycast = FindFirstRaycast(m_RaycastResultCache);
            m_RaycastResultCache.Clear();

            DataToPointer.RaycastResult = DataToPointer.EventData.pointerCurrentRaycast;

            InvokeReflexionMethodHandle(DataToPointer.EventData, DataToPointer.RaycastResult.gameObject);
            ExecuteEvents.Execute(DataToPointer.EventData.pointerDrag, DataToPointer.EventData, ExecuteEvents.dragHandler);
               
            if (DataToPointer.InputReady) 
            {
                if (!DataToPointer.lastInputReady)
                {
                    Down(DataToPointer);
                }
                else
                {
                    Hold(DataToPointer);
                }
            }
            else 
            {
                if (DataToPointer.lastInputReady)
                {
                    Release(DataToPointer);
                }
            }

            DataToPointer.lastInputReady = DataToPointer.InputReady;
            // interactor.GetHoverTargets(hoverinteractables);
            // if (hoverinteractables.Count > 0)
            // {
            //     EventSystem.current.SetSelectedGameObject(hoverinteractables[0].gameObject);
            //     ExecuteEvents.Execute(dataToPointer.EventData.pointerDrag,dataToPointer. EventData, ExecuteEvents.endDragHandler);
            //
            //     // EventSystem.current.
            // }
        }
        
        private void PointEventCamera(Transform pointer) 
        {
            if (pointer == null) return;

            WorldCanvasSetter.EventCamera.transform.parent = pointer;
            WorldCanvasSetter.EventCamera.transform.localPosition = Vector3.zero;
            WorldCanvasSetter.EventCamera.transform.localEulerAngles = Vector3.zero;
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
    }
}
