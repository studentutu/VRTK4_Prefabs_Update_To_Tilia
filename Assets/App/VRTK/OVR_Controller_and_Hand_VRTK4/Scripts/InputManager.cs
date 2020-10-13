using System;
using System.Collections.Generic;
using UnityEngine;
using Zinnia.Tracking.CameraRig;

namespace FusedVR
{
    /// <summary>
    ///  Singleton Class that manages context switching between Controllers and Hand Tracking.
    ///  To be used, to access the current input chosen by the user. 
    /// </summary>
    public class InputManager : MonoBehaviour
    {
        public static event Action<InputManager> PingMe;
       
        [Tooltip("OVR Camera Rig Linked Alias Connection for VRTK")]
        [SerializeField] private LinkedAliasAssociationCollection ovrRig;

        [Tooltip("Hand Tracking Input Control Scheme")]
        [SerializeField] private InputControl hands;
        [Tooltip("Physical Controller Input Control Scheme")]
        [SerializeField] private InputControl controllers;
        
        private OVRPlugin.Controller currControl = OVRPlugin.Controller.None; //variable to keep tracking of current input mechanism
        
        // Start is called before the first frame update
        private void Start() 
        {
            currControl = OVRPlugin.GetActiveController(); //set active
            Swap(currControl); //set defaults
        }

        // Update is called once per frame
        private void Update() 
        {
            OVRPlugin.Controller control = OVRPlugin.GetActiveController(); //get current controller scheme
            if (currControl != control) 
            { 
                //if current controller (hand or controller) is different from previous
                Swap(control); //swap shown controllers (hand or controller)
                currControl = control; //save current controller (hand or controller) scheme
            }
            PingMe?.Invoke(this);
        }

        /// <summary>
        /// Swaps the visibility of controllers depending on which controller a user has selected
        /// </summary>
        /// <param name="controller">Which controller is the active controller at the present</param>
        private void Swap(OVRPlugin.Controller controller) 
        {
            bool swap = (controller == OVRPlugin.Controller.Hands); //if hands then true otherwise false
            hands.Show(swap);
            controllers.Show(!swap);

            if ( swap && ovrRig ) 
            { // if hands, because the hand data forward is incorrectly offset by 90 degrees
                ovrRig.LeftController.transform.localRotation = Quaternion.Euler(0f , 90f , 0f);
                ovrRig.RightController.transform.localRotation = Quaternion.Euler(0f, 90f, 0f);
            } else if ( ovrRig ) 
            { //if controllers, reset back to identity
                ovrRig.LeftController.transform.localRotation = Quaternion.identity;
                ovrRig.RightController.transform.localRotation = Quaternion.identity;
            }
        }
        

        /// <summary>
        /// Get a float value representing how much a button has been pressed from the active control scheme
        /// </summary>
        /// <param name="hand">Which hand is the axis on</param>
        /// <param name="button">Which button is being pressed</param>
        /// <returns>A float from 0-1 indicating how much the input has been pressed</returns>
        public float GetAxis(InputControl.Hand hand , InputControl.Button button) 
        {
            InputControl control = (currControl == OVRPlugin.Controller.Hands) ? hands : controllers;
            return control.GetAxis(hand, button);
        }

        /// <summary>
        /// Get whether or not a button has been pressed from the active control scheme
        /// </summary>
        /// <param name="hand">Which hand is the button on</param>
        /// <param name="button">Which button is being pressed</param>
        /// <returns>A bool indicating whether the given button has been pressed</returns>
        public bool GetButton(InputControl.Hand hand, InputControl.Button button) 
        {
            InputControl control = (currControl == OVRPlugin.Controller.Hands) ? hands : controllers;
            return control.GetButton(hand , button);
        }
    }
}
