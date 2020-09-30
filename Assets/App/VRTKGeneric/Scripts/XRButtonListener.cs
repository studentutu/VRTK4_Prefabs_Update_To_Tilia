using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace UnityEngine.UI
{
    [DisallowMultipleComponent]
    public class XRButtonListener : Button
    {
        public enum ButtonStates
        {
            /// <summary>
            /// The UI object can be selected.
            /// </summary>
            Normal,

            /// <summary>
            /// The UI object is highlighted.
            /// </summary>
            Highlighted,

            /// <summary>
            /// The UI object is pressed.
            /// </summary>
            Pressed,

            /// <summary>
            /// The UI object is selected
            /// </summary>
            Selected,

            /// <summary>
            /// The UI object cannot be selected.
            /// </summary>
            Disabled,
        }
        
        [Serializable]
        public class SelectionStateClass : UnityEvent<ButtonStates, Button>
        {
        }
        
        [SerializeField] public SelectionStateClass ButtonChangeStateEvent = new SelectionStateClass();

        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            base.DoStateTransition(state, instant);
            switch (currentSelectionState)
            {
                case SelectionState.Normal:
                    ButtonChangeStateEvent?.Invoke(ButtonStates.Normal, this);
                    break;
                case SelectionState.Highlighted:
                    ButtonChangeStateEvent?.Invoke(ButtonStates.Highlighted, this);
                    break;
                case SelectionState.Pressed:
                    ButtonChangeStateEvent?.Invoke(ButtonStates.Pressed, this);
                    break;
                case SelectionState.Selected:
                    ButtonChangeStateEvent?.Invoke(ButtonStates.Selected, this);
                    break;
                case SelectionState.Disabled:
                    ButtonChangeStateEvent?.Invoke(ButtonStates.Disabled, this);
                    break;
                default:
                    ButtonChangeStateEvent?.Invoke(ButtonStates.Normal, this);
                    break;
            }
        }
    }
}
