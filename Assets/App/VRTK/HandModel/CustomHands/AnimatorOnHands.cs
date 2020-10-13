using System.Collections;
using System.Collections.Generic;
using Scripts.Common.AnimationParameter;
using Scripts.Common.StateMachine;
using UnityEngine;

namespace Tillia.VRTK.Animations
{
    [DisallowMultipleComponent]
    public class AnimatorOnHands : MonoBehaviour
    {
        
        [SerializeField] private AnimationControllerProfile animationProfile = null;
        [SerializeField] private StateControllerView changeAnimatorComponent = null;
        
        private AnimationEnum currentAnimation;
        private bool isLocked = false;

        private void Start()
        {
            UpdateAnimStates(AnimationEnum.Relaxed);
        }

        private void Update()
        {
            if (isLocked)
            {
                return;
            }
            UpdateAnimStates(currentAnimation);
        }
        
        [UnityEngine.Scripting.Preserve]
        public void LockAnimationState(bool lockAnimation)
        {
            isLocked = lockAnimation;
        }
        
        [UnityEngine.Scripting.Preserve]
        public void ChangeAnimationProfile(AnimationControllerProfile newProfile)
        {
            animationProfile = newProfile;
        }
        
        public void UpdateAnimStates(AnimationEnum enumToUse)
        {
            var profile = animationProfile.AllAnimations;
            for (int i = 0; i < profile.Count; i++)
            {
                if (profile[i].enumOfAnimation == enumToUse )
                {
                    currentAnimation = enumToUse;
                    if (!isLocked)
                    {
                        isLocked = profile[i].LockAnimation;
                    }

                    changeAnimatorComponent.TryChangeState(profile[i].parameter);
                    return;
                }
            }
        }
        
        [UnityEngine.Scripting.Preserve]
        public void UpdateAnimStates(StateAndParameters animationToUse)
        {
            var profile = animationProfile.AllAnimations;

            for (int i = 0; i < profile.Count; i++)
            {
                if (profile[i].parameter == animationToUse )
                {
                    currentAnimation = profile[i].enumOfAnimation;
                    if (!isLocked)
                    {
                        isLocked = profile[i].LockAnimation;
                    }

                    changeAnimatorComponent.TryChangeState(profile[i].parameter);
                    return;
                }
            }
        }
        
        // private void OculusHandTrackingNoController()
        // {
        //     float InputValueRateChange(bool isDown, float value)
        //     {
        //         float rateDelta = Time.deltaTime * INPUT_RATE_CHANGE;
        //         float sign = isDown ? 1.0f : -1.0f;
        //         return Mathf.Clamp01(value + rateDelta * sign);
        //     }
        //     
        //     int m_animLayerIndexThumb = -1;// m_animator.GetLayerIndex(ANIM_LAYER_NAME_POINT);
        //     int m_animLayerIndexPoint = -1; // m_animator.GetLayerIndex(ANIM_LAYER_NAME_THUMB);
        //     
        //     Need to get the HandPose from thw actual Hand
        //     Pose
        //     HandPoseId handPoseId = grabPose.PoseId;
        //     m_animator.SetInteger(m_animParamIndexPose, (int)handPoseId);
        //     
        //     // Flex
        //     // blend between open hand and fully closed fist
        //     float flex = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, m_controller);
        //     m_animator.SetFloat(m_animParamIndexFlex, flex);
        //     
        //     // Point
        //     bool canPoint = !grabbing || grabPose.AllowPointing;
        //     float point = canPoint ? m_pointBlend : 0.0f;
        //     m_animator.SetLayerWeight(m_animLayerIndexPoint, point);
        //     
        //     // Thumbs up
        //     bool canThumbsUp = !grabbing || grabPose.AllowThumbsUp;
        //     float thumbsUp = canThumbsUp ? m_thumbsUpBlend : 0.0f;
        //     m_animator.SetLayerWeight(m_animLayerIndexThumb, thumbsUp);
        //     
        //     m_animator.SetFloat("Pinch", pinch);
        // }
    }
}
