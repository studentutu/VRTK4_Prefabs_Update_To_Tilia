using System;
using System.Collections;
using System.Collections.Generic;
using Tilia.Indicators.ObjectPointers;
using UnityEngine;
using Zinnia.Pointer;
using UnityEvent = Zinnia.Pointer.ObjectPointer.UnityEvent;

namespace Tilia.VRTK
{
    public class PointerFacadeSourced : MonoBehaviour
    {
        [SerializeField] protected PointerFacade[] Sources;
        
        /// <summary>
        /// Emitted when the <see cref="ObjectPointer"/> becomes active.
        /// </summary>
        [Header("Pointer Events")]
        public UnityEvent Activated = new UnityEvent();
        
        /// <summary>
        /// Emitted when the <see cref="ObjectPointer"/> becomes inactive.
        /// </summary>
        public UnityEvent Deactivated = new UnityEvent();
        
        /// <summary>
        /// Emitted when the <see cref="ObjectPointer"/> collides with a new target.
        /// </summary>
        public UnityEvent Entered = new UnityEvent();
        
        /// <summary>
        /// Emitted when the <see cref="ObjectPointer"/> stops colliding with an existing target.
        /// </summary>
        public UnityEvent Exited = new UnityEvent();
        /// <summary>
        /// Emitted when the <see cref="ObjectPointer"/> changes its hovering position over an existing target.
        /// </summary>
        public UnityEvent HoverChanged = new UnityEvent();
        /// <summary>
        /// Emitted whenever <see cref="Select"/> is called.
        /// </summary>
        public UnityEvent Selected = new UnityEvent();

        protected virtual void OnEnable()
        {
            for (int i = 0; i < Sources.Length; i++)
            {
                if (Sources[i] != null)
                {
                    Sources[i].Activated.AddListener(OnActivated);
                    Sources[i].Deactivated.AddListener(OnDeactivated);
                    Sources[i].Entered.AddListener(OnEntered);
                    Sources[i].Exited.AddListener(OnExited);
                    Sources[i].HoverChanged.AddListener(OnHoverChanged);
                    Sources[i].Selected.AddListener(OnSelected);
                }
            }
        }

        protected virtual void OnDisable()
        {
            for (int i = 0; i < Sources.Length; i++)
            {
                if (Sources[i] != null)
                {
                    Sources[i].Activated.RemoveListener(OnActivated);
                    Sources[i].Deactivated.RemoveListener(OnDeactivated);
                    Sources[i].Entered.RemoveListener(OnEntered);
                    Sources[i].Exited.RemoveListener(OnExited);
                    Sources[i].HoverChanged.RemoveListener(OnHoverChanged);
                    Sources[i].Selected.RemoveListener(OnSelected);
                }
            }
        }

        [UnityEngine.Scripting.Preserve]
        public void OnActivated(ObjectPointer.EventData eventData)
        {
            Activated.Invoke(eventData);
        }
        
        [UnityEngine.Scripting.Preserve]
        public void OnDeactivated(ObjectPointer.EventData eventData)
        {
            Deactivated.Invoke(eventData);
        }
        
        [UnityEngine.Scripting.Preserve]
        public void OnEntered(ObjectPointer.EventData eventData)
        {
            Entered.Invoke(eventData);
        }
        
        [UnityEngine.Scripting.Preserve]
        public void OnExited(ObjectPointer.EventData eventData)
        {
            Exited.Invoke(eventData);
        }
        
        [UnityEngine.Scripting.Preserve]
        public void OnHoverChanged(ObjectPointer.EventData eventData)
        {
            HoverChanged.Invoke(eventData);
        }
        
        [UnityEngine.Scripting.Preserve]
        public void OnSelected(ObjectPointer.EventData eventData)
        {
            Selected.Invoke(eventData);
        }
    }
}
