using Tilia.Input.UnityInputManager;
using Zinnia.Action;

namespace VRTK.Prefabs.CameraRig.SimulatedCameraRig
{
    using UnityEngine;
    using UnityEngine.UI;
    using VRTK.Prefabs.CameraRig.UnityXRCameraRig.Input;

    /// <summary>
    /// Sets up the key binding display.
    /// </summary>
    public class KeyBindingDisplay : MonoBehaviour
    {
        /// <summary>
        /// The Text component to apply the instructions text to.
        /// </summary>
        // [Serialized]
        // [field: DocumentedByXml, Restricted]

        public Text KeyBindingText;

        /// <summary>
        /// The action for handling forward.
        /// </summary>
        // [Serialized]
        // [field: DocumentedByXml, Restricted]
        public UnityInputManagerButtonAction Forward;

        /// <summary>
        /// The action for handling backward.
        /// </summary>
        // [Serialized]
        // [field: DocumentedByXml, Restricted]
        public UnityInputManagerButtonAction Backward;

        /// <summary>
        /// The action for handling strafeLeft.
        /// </summary>
        // [Serialized]
        // [field: DocumentedByXml, Restricted]
        public UnityInputManagerButtonAction StrafeLeft;

        /// <summary>
        /// The action for handling strafeRight.
        /// </summary>
        // [Serialized]
        // [field: DocumentedByXml, Restricted]
        public UnityInputManagerButtonAction StrafeRight;

        /// <summary>
        /// The action for handling button1.
        /// </summary>
        // [Serialized]
        // [field: DocumentedByXml, Restricted]
        public UnityInputManagerButtonAction Button1;

        /// <summary>
        /// The action for handling button2.
        /// </summary>
        // [Serialized]
        // [field: DocumentedByXml, Restricted]
        public UnityInputManagerButtonAction Button2;

        /// <summary>
        /// The action for handling button3.
        /// </summary>
        // [Serialized]
        // [field: DocumentedByXml, Restricted]
        public UnityInputManagerButtonAction Button3;

        /// <summary>
        /// The action for handling switchToPlayer.
        /// </summary>
        // [Serialized]
        // [field: DocumentedByXml, Restricted]
        public UnityInputManagerButtonAction SwitchToPlayer;

        /// <summary>
        /// The action for handling switchToLeftController.
        /// </summary>
        // [Serialized]
        // [field: DocumentedByXml, Restricted]
        public UnityInputManagerButtonAction SwitchToLeftController;

        /// <summary>
        /// The action for handling switchToRightController.
        /// </summary>
        // [Serialized]
        // [field: DocumentedByXml, Restricted]
        public UnityInputManagerButtonAction SwitchToRightController;

        /// <summary>
        /// The action for handling resetPlayer.
        /// </summary>
        // [Serialized]
        // [field: DocumentedByXml, Restricted]
        public UnityInputManagerButtonAction ResetPlayer;

        /// <summary>
        /// The action for handling resetControllers.
        /// </summary>
        // [Serialized]
        // [field: DocumentedByXml, Restricted]
        public UnityInputManagerButtonAction ResetControllers;

        /// <summary>
        /// The action for handling toggleInstructions.
        /// </summary>
        // [Serialized]
        // [field: DocumentedByXml, Restricted]
        public UnityInputManagerButtonAction ToggleInstructions;

        /// <summary>
        /// The action for handling lockCursorToggle.
        /// </summary>
        // [Serialized]
        // [field: DocumentedByXml, Restricted]
        public UnityInputManagerButtonAction LockCursorToggle;
        
        /// <summary>
        /// The action for handling lockCursorToggle.
        /// </summary>
        // [Serialized]
        // [field: DocumentedByXml, Restricted]
        public UnityInputManagerButtonAction TogglePointers;
        
        /// <summary>
        /// The instructions text.
        /// </summary>
        protected const string Instructions = @"<b>Simulator Key Bindings</b>

<b>Movement:</b>
Forward: {0}
Backward: {1}
Strafe Left: {2}
Strafe Right: {3}

<b>Buttons</b>
Button 1: {4}
Button 2: {5}
Button 3: {6}

<b>Object Control</b>
Move PlayArea: {7}
Move Left Controller: {8}
Move Right Controller: {9}
Reset Player: Position {10}
Reset Controller Position: {11}

<b>Misc</b>
Toggle Help Window: {12}
Lock Mouse Cursor: {13}
Toggle Pointer: {14}";

        protected virtual void OnEnable()
        {
            KeyBindingText.text = string.Format(
                Instructions,
                Forward.KeyCode,
                Backward.KeyCode,
                StrafeLeft.KeyCode,
                StrafeRight.KeyCode,
                Button1.KeyCode,
                Button2.KeyCode,
                Button3.KeyCode,
                SwitchToPlayer.KeyCode,
                SwitchToLeftController.KeyCode,
                SwitchToRightController.KeyCode,
                ResetPlayer.KeyCode,
                ResetControllers.KeyCode,
                ToggleInstructions.KeyCode,
                LockCursorToggle.KeyCode,
                TogglePointers.KeyCode
                );
        }
    }
}