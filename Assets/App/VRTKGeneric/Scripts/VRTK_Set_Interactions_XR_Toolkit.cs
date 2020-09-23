using System.Collections;
using System.Collections.Generic;
using Tilia.Indicators.ObjectPointers;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Zinnia.Cast;
using Zinnia.Data.Type;
using Zinnia.Pointer;

namespace VRTK.Prefabs.Helpers.XRToolkit
{
    [DisallowMultipleComponent]
    public class VRTK_Set_Interactions_XR_Toolkit : MonoBehaviour
    {
        [SerializeField] private PointerFacade pointerFacade;
        private List<Vector3> temporalList = new List<Vector3>();
        private ObjectPointer.EventData eventData;
        private PointsCast.EventData pointerCastEventData;
        
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
    }
}
