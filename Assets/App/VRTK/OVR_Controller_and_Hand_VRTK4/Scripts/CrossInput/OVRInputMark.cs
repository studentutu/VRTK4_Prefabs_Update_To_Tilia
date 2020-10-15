using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FusedVR
{
    [DisallowMultipleComponent]
    public class OVRInputMark : MonoBehaviour
    {
        public enum ButtonOrHand
        {
            None,
            RemoteControl,
            ActualHand
        }

        public OVRHand ActualOvrHand => actualOVRHand;
        
#pragma warning disable
        // Just a visual indicator
        [SerializeField] private ButtonOrHand typeOfMark = ButtonOrHand.None;
#pragma warning restore


        [SerializeField] private OVRHand actualOVRHand = null;
    }
}
