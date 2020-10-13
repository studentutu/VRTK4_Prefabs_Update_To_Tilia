using UnityEngine;
using Zinnia.Action;

namespace FusedVR.VRTK 
{
    /// <summary>
    /// A child class of VRTK Float Action that maps OVR Input to VRTK Actions
    /// </summary>
    public class OVRAxis : FloatAction 
    {

        [Tooltip("Which hand is this button on")]
        [SerializeField] private InputControl.Hand hand;
        [Tooltip("Which button does this object represent")]
        [SerializeField] private InputControl.Button button = InputControl.Button.Trigger; //default axis

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
            Receive(manager.GetAxis(hand, button)); //sends axis data to VRTK
        }
    }
}

