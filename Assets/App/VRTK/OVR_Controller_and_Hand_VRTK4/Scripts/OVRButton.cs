using UnityEngine;
using Zinnia.Action;

namespace FusedVR.VRTK 
{
    /// <summary>
    /// A child class of VRTK Boolean Action that maps OVR Input to VRTK Actions
    /// </summary>
    public class OVRButton : BooleanAction 
    {
        
        [Tooltip("Which hand is this button on")]
        public InputControl.Hand hand;
        [Tooltip("Which button does this object represent")]
        public InputControl.Button button = InputControl.Button.A; //default button
        
        protected override void OnEnable()
        {
            base.OnEnable();
            InputManager.PingMe += Ping;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            InputManager.PingMe -= Ping;

        }

        /// Called once per Frame from OVR_CROS_HANDS_InputManager
        private void Ping(InputManager manager) 
        {
            Receive(manager.GetButton(hand, button)); //send button data to VRTK
        }
    }
}