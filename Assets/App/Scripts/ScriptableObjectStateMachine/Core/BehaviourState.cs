﻿using Scripts.Common.AnimationParameter;
using UnityEngine;
#pragma warning disable

namespace Scripts.Common.StateMachine
{
    /// <summary>
    /// Animator state machine state behaviour.
    /// Sends events on Enter, Update, Exit.
    /// </summary>
    [ExecuteAlways]
    public class BehaviourState : StateMachineBehaviour
    {
        //---------------------------------------------------------------------
        // Editor
        //---------------------------------------------------------------------
        
        [Header("Data")] 
        [SerializeField] private StateAndParameters data;
        
        [Header("Events")]
        [SerializeField] private BehaviourStateEvent onStateEnter;
        [SerializeField] private BehaviourStateEvent onStateExit;
        // [SerializeField] private BehaviourStateEvent onStateUpdate;
        
        
        //---------------------------------------------------------------------
        // Public
        //---------------------------------------------------------------------
        // public StateAndParameters Data => data;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (onStateEnter != null)
            {
                onStateEnter.Raise(data);
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (onStateExit != null)
            {
                onStateExit.Raise(data);
            }
        }

        // public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        // {
        //     if (onStateUpdate != null)
        //     {
        //         onStateUpdate.Raise(data);
        //     }
        // }

        // public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        // {
        // }
        //
        // public override void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        // {
        // }
    }
}