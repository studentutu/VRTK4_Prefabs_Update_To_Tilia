using System;

namespace Tillia.VRTKUI.Prefabs.CameraRig.UnityXRCameraRig.Input
{
    using UnityEngine;
    using Malimbe.PropertySerializationAttribute;
    using Malimbe.XmlDocumentationAttribute;
    using Zinnia.Action;

    /// <summary>
    /// Listens for the specified key state and emits the appropriate action.
    /// </summary>
    [Obsolete]
    public class UnityButtonAction : BooleanAction
    {
        [SerializeField] private string Info = "Enable to use button for as ContinuousStream";
        
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