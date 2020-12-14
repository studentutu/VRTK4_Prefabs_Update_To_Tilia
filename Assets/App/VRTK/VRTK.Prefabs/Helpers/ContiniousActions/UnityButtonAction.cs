using System;

namespace Tilia.VRTKUI.Prefabs.CameraRig.UnityXRCameraRig.Input
{
    using UnityEngine;
    using Zinnia.Action;

    /// <summary>
    /// Listens for the specified key state and emits the appropriate action.
    /// </summary>
    public class UnityButtonAction : BooleanAction
    {
        #pragma warning disable
        [SerializeField] private string Info = "Enable to use button for as ContinuousStream";
        #pragma warning restore
        
        private bool continuousStream = false;
        
        [UnityEngine.Scripting.Preserve]
        public void ContinuousStream(bool enable)
        {
            continuousStream = enable;
            if (!enable)
            {
                Receive(false);
            }
        }

        private void Update()
        {
            if (continuousStream)
            {
                Receive(true);
            }
        }
    }
}