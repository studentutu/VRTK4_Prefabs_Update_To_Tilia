using System;
using System.Collections;
using System.Collections.Generic;
using Tilia.Indicators.ObjectPointers;
using Tilia.Interactions.Interactables.Interactors;
using Tilia.Trackers.PseudoBody;
using Tilia.VRTK.Animations;
using Tilia.VRTKUI;
using UnityEngine;
using UnityEngine.Events;

namespace Services
{
    /// <summary>
    /// Contains the animations for the head, hands,body, pointers, UI pointers
    /// </summary>
    [DisallowMultipleComponent]
    public class VRTK_References : MonoBehaviour
    {
        [SerializeField] private GameObject headSetAlias;
        [SerializeField] private PseudoBodyFacade body;
        
        [SerializeField] private AnimatorOnHands leftHandAnimator;
        [SerializeField] private AnimatorOnHands rightHandAnimator;

        [SerializeField] private Transform leftAttachedPoint;
        [SerializeField] private Transform rightAttachedPoint;

        [SerializeField] private VRTK4_UIPointer leftHandUI_Pointer;
        [SerializeField] private VRTK4_UIPointer rightHandUI_Pointer;
        
        [SerializeField] private PointerFacade leftHand_Pointer;
        [SerializeField] private PointerFacade rightHand_Pointer;

        [SerializeField] private InteractorFacade leftHandInteractor;
        [SerializeField] private InteractorFacade rightHandInteractor;

        [SerializeField] private UnityEngine.Events.UnityEvent OnEnableAction = new UnityEvent();
        [SerializeField] private UnityEngine.Events.UnityEvent OnDisableAction = new UnityEvent();
        
        public GameObject HeadSetAlias => headSetAlias;
        public PseudoBodyFacade Body => body;
        public AnimatorOnHands LeftHandAnimator => leftHandAnimator;
        public AnimatorOnHands RightHandAnimator => rightHandAnimator;
        public Transform LeftAttachedPoint => leftAttachedPoint;
        public Transform RightAttachedPoint => rightAttachedPoint;
        public VRTK4_UIPointer LeftHandUiPointer => leftHandUI_Pointer;
        public VRTK4_UIPointer RightHandUiPointer => rightHandUI_Pointer;
        public PointerFacade LeftHandPointer => leftHand_Pointer;
        public PointerFacade RightHandPointer => rightHand_Pointer;

        public InteractorFacade LeftHandInteractor => leftHandInteractor;
        public InteractorFacade RightHandInteractor => rightHandInteractor;
        

        private void OnEnable()
        {
            OnEnableAction?.Invoke();
        }

        private void OnDisable()
        {
            OnDisableAction?.Invoke();
        }
    }
}