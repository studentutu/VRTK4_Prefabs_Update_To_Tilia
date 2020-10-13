using System;
using System.Collections.Generic;
using UnityEngine;
using static OVRHand;

namespace FusedVR 
{
    /// <summary>
    /// This class represents a mapping for the control scheme used for Hand Tracking
    /// </summary>
    public class HandsController : InputControl 
    {

        [Tooltip("Customizable mapping of Finger to Trigger Input Button")]
        [SerializeField] private HandFinger triggerFinger = HandFinger.Index;
        
        [Tooltip("Customizable mapping of Finger to Grip Input Button")]
        [SerializeField] private HandFinger gripFinger = HandFinger.Middle;
        
        [Tooltip("Customizable mapping of Finger to A Input Button")]
        [SerializeField] private HandFinger aFinger = HandFinger.Ring;
        
        [Tooltip("Customizable mapping of Finger to B Input Button")]
        [SerializeField] private HandFinger bFinger = HandFinger.Pinky;

        private Dictionary<Button, System.Func<HandFinger>> strategy = null;

        protected Dictionary<Button, Func<HandFinger>> Strategy
        {
            get
            {
                if (strategy == null)
                {
                    strategy = new Dictionary<Button, Func<HandFinger>>()
                    {
                        {Button.Trigger, OnButtonTrigger},
                        {Button.Grip, OnButtonGrip},
                        {Button.A, OnButtonA},
                        {Button.B, OnButtonB},

                    };
                }

                return strategy;
            }
        }

        private HandFinger OnButtonTrigger()
        {
            return triggerFinger;
        }
        
        private HandFinger OnButtonGrip()
        {
            return gripFinger;
        }
        
        private HandFinger OnButtonA()
        {
            return aFinger;
        }
        
        private HandFinger OnButtonB()
        {
            return bFinger;
        }

        /// <summary>
        /// Get whether the correct finger has been pinched based on the abstract input button
        /// </summary>
        /// <param name="h">Which hand is the button on</param>
        /// <param name="b">Which button is being pressed</param>
        /// <returns>A bool indicating whether the given button has been pressed</returns>
        public override bool GetButton(Hand h, Button b) 
        {
            GameObject handObj = (h == Hand.Left) ? leftHand : rightHand;
            OVRHand hand = handObj.GetComponent<OVRHand>();
            HandFinger finger = FingerMap(b);

            if ( hand && hand.GetFingerConfidence(finger) == TrackingConfidence.High ) 
            {               
                return hand.GetFingerIsPinching(finger);
            }

            return false; //null check for hand
        }

        /// <summary>
        /// Get a float value the finger pinch strength based on the abstract input button
        /// </summary>
        /// <param name="h">Which hand is the axis on</param>
        /// <param name="b">Which button is being pressed</param>
        /// <returns>A float from 0-1 indicating how much the input has been pressed</returns>
        public override float GetAxis(Hand h, Button b) 
        {
            GameObject handObj = (h == Hand.Left) ? leftHand : rightHand;
            OVRHand hand = handObj.GetComponent<OVRHand>();
            HandFinger finger = FingerMap(b);

            if ( hand && hand.GetFingerConfidence(finger) == TrackingConfidence.High ) 
            {
                return hand.GetFingerPinchStrength(finger);
            }

            return 0f;
        }

        /// <summary>
        /// A Mapping from the abstract button input to each finger
        /// </summary>
        /// <param name="b">Which button should be mapped</param>
        /// <returns>An OVRHand.HandFinger corresponding to the button</returns>
        private HandFinger FingerMap(Button b) 
        {
            if (Strategy.ContainsKey(b))
            {
                return Strategy[b]();
            }
            
            return HandFinger.Index; //default to Index Finger even though all buttons have been mapped
        }
    }
}