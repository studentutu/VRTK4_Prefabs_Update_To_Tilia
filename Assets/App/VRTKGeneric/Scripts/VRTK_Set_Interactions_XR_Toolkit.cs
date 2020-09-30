using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Adrenak.SUI;
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
        [SerializeField] private PointerFacade pointerFacade;
        [SerializeField] private XRRayInteractor interactor;
        [SerializeField] private SpatialInputPointer SpatialUIpointer;
        
        private List<Vector3> temporalList = new List<Vector3>();
        private ObjectPointer.EventData eventData;
        private PointsCast.EventData pointerCastEventData;
        private XRUIInputModule asXrUiModule;

        private XRUIInputModule XRuiInputModuleRef
        {
            get
            {
                if (asXrUiModule == null)
                {
                    asXrUiModule = EventSystem.current.GetComponent<XRUIInputModule>();
                }

                return asXrUiModule;
            }
        }


        private void Update()
        {
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
            Debug.LogWarning("On Hover Enter");
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
            Debug.LogWarning("On Hover Exit");

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
            Debug.LogWarning("On Select Enter");

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
            Debug.LogWarning("On Select Exit");

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
        public void OnPress(bool enable)
        {
            SpatialUIpointer.input = enable;
        }
    }
}
