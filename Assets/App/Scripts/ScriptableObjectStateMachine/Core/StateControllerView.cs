using System;
using System.Collections.Generic;
using Scripts.Common.AnimationParameter;
using Scripts.Common.AnimationParameter.Data;
using UnityEngine;

namespace Scripts.Common.StateMachine
{
    /// <summary>
    /// State machine class
    /// </summary>
    public class StateControllerView : MonoBehaviour
    {
        //---------------------------------------------------------------------
        // Editor
        //---------------------------------------------------------------------

        [SerializeField] private bool isAllowingTransitionToSelf;
        [SerializeField] private int ignoreFirstNSelfTransitions = 0;
        [SerializeField] protected Animator animatorForStateController = null;
        public Action<StateAndParameters> onStateChanged;

        //---------------------------------------------------------------------
        // Properties
        //---------------------------------------------------------------------

        public StateAndParameters CurrentState { get; private set; }
		
        //---------------------------------------------------------------------
        // Internal
        //---------------------------------------------------------------------

        protected static Dictionary<AnimationParamSerialiazable.AnimationType, Action<Animator, AnimationParamSerialiazable>>
            strategy = new Dictionary<AnimationParamSerialiazable.AnimationType, Action<Animator, AnimationParamSerialiazable>>
            {
                {AnimationParamSerialiazable.AnimationType.Trigger, OnTrigger},
                {AnimationParamSerialiazable.AnimationType.Bool, OnBool},
                {AnimationParamSerialiazable.AnimationType.Float, OnFloat},
                {AnimationParamSerialiazable.AnimationType.Int, OnInt},
                {AnimationParamSerialiazable.AnimationType.LayerWeight, OnInt}
            };

        private int? currentNumberOfIgnores = null;

        protected int CurrentNumberOfIgnores
        {
	        get
	        {
		        if (currentNumberOfIgnores == null)
		        {
			        currentNumberOfIgnores = ignoreFirstNSelfTransitions;
		        }

		        return currentNumberOfIgnores.Value;
	        }
	        set
	        {
		        currentNumberOfIgnores = value;
	        }
        }
        //---------------------------------------------------------------------
        // Public
        //---------------------------------------------------------------------
		
        [UnityEngine.Scripting.Preserve]
        public virtual void TryChangeState(StateAndParameters nextState)
        {
	        if (!isAllowingTransitionToSelf && CurrentState == nextState)
	        {
#if UNITY_EDITOR
		        Debug.LogWarning(name + " is already in : " + nextState.name);
#endif
		        return;
	        }
	        
	        // Allow transition to itself, but ignore first n-self-transitions
	        if (CurrentState == nextState  && CurrentNumberOfIgnores > 0)
	        {
#if UNITY_EDITOR
		        Debug.LogWarning("Ignoring first n self transitions " + CurrentNumberOfIgnores);
#endif
		        CurrentNumberOfIgnores--;
		        return;
	        }
	        
            SetAllParamsFromState(animatorForStateController, nextState);
        }

        // Do not effect the animator transitions, only the invocation of Try Change State!
        public void SetAllowTransitionToItself(bool allow)
        {
            isAllowingTransitionToSelf = allow;
        }
        
        public void SetSelfTransitionsIgnoreNFirstTimes(int ignoreFirst)
        {
	        CurrentNumberOfIgnores = ignoreFirst;
        }

        /// <summary>
        /// invoked from Animator through the GameEvent Scriptable Object!
        /// You can use StateEnterActions instead
        /// </summary>
        [UnityEngine.Scripting.Preserve]
        public void OnEnterState(StateAndParameters data)
        {
            CurrentState = data;
            onStateChanged?.Invoke(CurrentState);
        }
        
        /// <summary>
        /// invoked from Animator through the GameEvent Scriptable Object!
        /// You can use StateEnterActions instead
        /// </summary>
        [UnityEngine.Scripting.Preserve]
        public void OnExitState(StateAndParameters data)
        {
	        if (CurrentState == data)
	        {
		        CurrentState = null;
	        }
        }

        //---------------------------------------------------------------------
        // Helpers
        //---------------------------------------------------------------------

        private static void OnTrigger(Animator animatorToUSe, AnimationParamSerialiazable parameter)
        {
            if (parameter.triggerValue)
            {
                animatorToUSe.SetTrigger(parameter.nameHash);
            }
            else
            {
                animatorToUSe.ResetTrigger(parameter.nameHash);
            }
        }

        private static void OnBool(Animator animatorToUSe, AnimationParamSerialiazable parameter)
        {
            animatorToUSe.SetBool(parameter.nameHash, parameter.boolValue);
        }

        private static void OnFloat(Animator animatorToUSe, AnimationParamSerialiazable parameter)
        {
            animatorToUSe.SetFloat(parameter.nameHash, parameter.floatValue);
        }

        private static void OnInt(Animator animatorToUSe, AnimationParamSerialiazable parameter)
        {
            animatorToUSe.SetInteger(parameter.nameHash, parameter.intValue);
        }
        
        private static void OnLayerWeight(Animator animatorToUSe, AnimationParamSerialiazable parameter)
        {
	        animatorToUSe.SetLayerWeight(parameter.indexOfLayer, parameter.floatValue);
        }

        private static void SetAllParamsFromState(Animator animatorToUse, IState stateToGetparametersFrom)
        {
            if (animatorToUse != null && stateToGetparametersFrom != null)
            {
	            var runtimeData = stateToGetparametersFrom.CurrentParameters;
                for (var i = 0; i < runtimeData.Length; i++)
                {
                    if (strategy.ContainsKey(runtimeData[i].type) && runtimeData[i] != null)
                    {
                        strategy[runtimeData[i].type]
                                (animatorToUse, runtimeData[i]);
                    }
                }
            }
        }
    }
}
