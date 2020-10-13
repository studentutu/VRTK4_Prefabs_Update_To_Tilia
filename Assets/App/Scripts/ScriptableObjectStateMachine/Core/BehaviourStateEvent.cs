using System;
using System.Collections.Generic;
using Scripts.Common.AnimationParameter;
using UnityEngine;

namespace Scripts.Common.StateMachine
{
    [CreateAssetMenu(menuName = "AnimatorStateMachine/AnimatorBinder", fileName = "Binder",order = 1)]
    public class BehaviourStateEvent : ScriptableObject
    {
        //---------------------------------------------------------------------
        // Internal
        //---------------------------------------------------------------------
        
        private readonly List<BehaviourStateEventListener> listeners = new List<BehaviourStateEventListener>();
        
        //---------------------------------------------------------------------
        // Public
        //---------------------------------------------------------------------

        public void Register(BehaviourStateEventListener l)
        {
            listeners.Add(l);
        }

        public void UnRegister(BehaviourStateEventListener l)
        {
            listeners.Remove(l);
        }

        public void Raise(StateAndParameters data)
        {
	        if (data == null)
	        {
		        Debug.LogError(" Raising empty Data! ");
		        return;
	        }

	        for (var i = 0; i < listeners.Count; i++)
            {
#if UNITY_EDITOR
	            try
	            {
		            if (listeners[i] != null)
		            {
			            listeners[i].Response(data);
		            }
		            else
		            {
			            Debug.LogError(" listener is null when data " + data.name);
		            }
	            }
	            catch (Exception e)
	            {
		            if (listeners[i] == null)
		            {
			            Debug.LogError(" listener is null when data " + data.name + " and it caused exception");
		            }
		            else
		            {
			            Debug.LogError(listeners[i].name + " with " + data.name + " caused exception");
		            }
		            throw e;
	            }

#else
                listeners[i].Response(data);
#endif
            }
        }
    }
}