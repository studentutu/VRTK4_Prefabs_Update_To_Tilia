namespace VRTK.UnityEventHelper
{
    using UnityEngine;
    using UnityEngine.Events;
    using System;

    [AddComponentMenu("VRTK/Scripts/Utilities/Unity Events/VRTK_UIPointer_UnityEvents")]
    public sealed class VRTK_UIPointer_UnityEvents : VRTK_UnityEvents<VRTK_UIPointer>
    {
        [Serializable]
        public sealed class UIPointerEvent : UnityEvent<object, UIPointerEventArgs> { }
        [Serializable]
        public sealed class UIPointerEventDirect : UnityEvent<VRTK_UIPointer> { }


        public UIPointerEvent OnUIPointerElementEnter = new UIPointerEvent();
        public UIPointerEvent OnUIPointerElementExit = new UIPointerEvent();
        public UIPointerEvent OnUIPointerElementClick = new UIPointerEvent();
        public UIPointerEvent OnUIPointerElementDragStart = new UIPointerEvent();
        public UIPointerEvent OnUIPointerElementDragEnd = new UIPointerEvent();
        public UIPointerEventDirect OnActivationButtonPressed = new UIPointerEventDirect();
        public UIPointerEventDirect OnActivationButtonReleased = new UIPointerEventDirect();
        public UIPointerEventDirect OnSelectionButtonPressed = new UIPointerEventDirect();
        public UIPointerEventDirect OnSelectionButtonReleased = new UIPointerEventDirect();

        protected override void AddListeners(VRTK_UIPointer component)
        {
            component.UIPointerElementEnter += UIPointerElementEnter;
            component.UIPointerElementExit += UIPointerElementExit;
            component.UIPointerElementClick += UIPointerElementClick;
            component.UIPointerElementDragStart += UIPointerElementDragStart;
            component.UIPointerElementDragEnd += UIPointerElementDragEnd;
            component.ActivationButtonPressed += ActivationButtonPressed;
            component.ActivationButtonReleased += ActivationButtonReleased;
            component.SelectionButtonPressed += SelectionButtonPressed;
            component.SelectionButtonReleased += SelectionButtonReleased;
        }

        protected override void RemoveListeners(VRTK_UIPointer component)
        {
            component.UIPointerElementEnter -= UIPointerElementEnter;
            component.UIPointerElementExit -= UIPointerElementExit;
            component.UIPointerElementClick -= UIPointerElementClick;
            component.UIPointerElementDragStart -= UIPointerElementDragStart;
            component.UIPointerElementDragEnd -= UIPointerElementDragEnd;
            component.ActivationButtonPressed -= ActivationButtonPressed;
            component.ActivationButtonReleased -= ActivationButtonReleased;
            component.SelectionButtonPressed -= SelectionButtonPressed;
            component.SelectionButtonReleased -= SelectionButtonReleased;
        }

        private void UIPointerElementEnter(object o, UIPointerEventArgs e)
        {
            OnUIPointerElementEnter.Invoke(o, e);
        }

        private void UIPointerElementExit(object o, UIPointerEventArgs e)
        {
            OnUIPointerElementExit.Invoke(o, e);
        }

        private void UIPointerElementClick(object o, UIPointerEventArgs e)
        {
            OnUIPointerElementClick.Invoke(o, e);
        }

        private void UIPointerElementDragStart(object o, UIPointerEventArgs e)
        {
            OnUIPointerElementDragStart.Invoke(o, e);
        }

        private void UIPointerElementDragEnd(object o, UIPointerEventArgs e)
        {
            OnUIPointerElementDragEnd.Invoke(o, e);
        }

        private void ActivationButtonPressed(VRTK_UIPointer pointer)
        {
            OnActivationButtonPressed.Invoke(pointer);
        }

        private void ActivationButtonReleased(VRTK_UIPointer pointer)
        {
            OnActivationButtonReleased.Invoke(pointer);
        }

        private void SelectionButtonPressed(VRTK_UIPointer pointer)
        {
            OnSelectionButtonPressed.Invoke(pointer);
        }

        private void SelectionButtonReleased(VRTK_UIPointer pointer)
        {
            OnSelectionButtonReleased.Invoke(pointer);
        }
    }
}