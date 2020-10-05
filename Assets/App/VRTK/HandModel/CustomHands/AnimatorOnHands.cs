using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MapstarVr
{
    [DisallowMultipleComponent]
    public class AnimatorOnHands : MonoBehaviour
    {
        public const string ANIM_LAYER_NAME_POINT = "Point Layer";
        public const string ANIM_LAYER_NAME_THUMB = "Thumb Layer";
        public const string ANIM_PARAM_NAME_FLEX = "Flex";
        public const string ANIM_PARAM_NAME_POSE = "Pose";
        public const float THRESH_COLLISION_FLEX = 0.9f;

        public const float INPUT_RATE_CHANGE = 20.0f;

        public const float COLLIDER_SCALE_MIN = 0.01f;
        public const float COLLIDER_SCALE_MAX = 1.0f;
        public const float COLLIDER_SCALE_PER_SECOND = 1.0f;

        public const float TRIGGER_DEBOUNCE_TIME = 0.05f;
        public const float THUMB_DEBOUNCE_TIME = 0.15f;

        [SerializeField]
        private Animator m_animator = null;

        private Collider[] m_colliders = null;
        private bool m_collisionEnabled = true;


        private int m_animLayerIndexThumb = -1;
        private int m_animLayerIndexPoint = -1;
        private int m_animParamIndexFlex = -1;
        private int m_animParamIndexPose = -1;

        private bool m_isPointing = false;
        private bool m_isGivingThumbsUp = false;
        private float m_pointBlend = 0.0f;
        private float m_thumbsUpBlend = 0.0f;

        private float m_collisionScaleCurrent = 0.0f;

        private bool isLocked = false;
        private List<int> allPreviousAnimation = new List<int>(8);
        
        private void Start()
        {
            // Collision starts disabled. We'll enable it for certain cases such as making a fist.
            m_colliders = this.GetComponentsInChildren<Collider>().Where(childCollider => !childCollider.isTrigger).ToArray();
            CollisionEnable(false);

            // Get animator layer indices by name, for later use switching between hand visuals
            m_animLayerIndexPoint = m_animator.GetLayerIndex(ANIM_LAYER_NAME_POINT);
            m_animLayerIndexThumb = m_animator.GetLayerIndex(ANIM_LAYER_NAME_THUMB);
            m_animParamIndexFlex = Animator.StringToHash(ANIM_PARAM_NAME_FLEX);
            m_animParamIndexPose = Animator.StringToHash(ANIM_PARAM_NAME_POSE);
        }


        private void Update()
        {
            m_pointBlend = InputValueRateChange(m_isPointing, m_pointBlend);
            m_thumbsUpBlend = InputValueRateChange(m_isGivingThumbsUp, m_thumbsUpBlend);

            CollisionEnable(true);

            UpdateAnimStates();
        }

        private void LateUpdate()
        {
            // Hand's collision grows over a short amount of time on enable, rather than snapping to on, to help somewhat with interpenetration issues.
            if (m_collisionEnabled && m_collisionScaleCurrent + Mathf.Epsilon < COLLIDER_SCALE_MAX)
            {
                m_collisionScaleCurrent = Mathf.Min(COLLIDER_SCALE_MAX, m_collisionScaleCurrent + Time.deltaTime * COLLIDER_SCALE_PER_SECOND);
                for (int i = 0; i < m_colliders.Length; ++i)
                {
                    Collider collider = m_colliders[i];
                    collider.transform.localScale = new Vector3(m_collisionScaleCurrent, m_collisionScaleCurrent, m_collisionScaleCurrent);
                }
            }
        }

        private float InputValueRateChange(bool isDown, float value)
        {
            float rateDelta = Time.deltaTime * INPUT_RATE_CHANGE;
            float sign = isDown ? 1.0f : -1.0f;
            return Mathf.Clamp01(value + rateDelta * sign);
        }

        private void UpdateAnimStates()
        {
            // Need to get the HandPose from thw actual Hand
            // Pose
            // HandPoseId handPoseId = grabPose.PoseId;
            // m_animator.SetInteger(m_animParamIndexPose, (int)handPoseId);
            //
            // // Flex
            // // blend between open hand and fully closed fist
            // float flex = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, m_controller);
            // m_animator.SetFloat(m_animParamIndexFlex, flex);
            //
            // // Point
            // bool canPoint = !grabbing || grabPose.AllowPointing;
            // float point = canPoint ? m_pointBlend : 0.0f;
            // m_animator.SetLayerWeight(m_animLayerIndexPoint, point);
            //
            // // Thumbs up
            // bool canThumbsUp = !grabbing || grabPose.AllowThumbsUp;
            // float thumbsUp = canThumbsUp ? m_thumbsUpBlend : 0.0f;
            // m_animator.SetLayerWeight(m_animLayerIndexThumb, thumbsUp);
            //
            //
            // m_animator.SetFloat("Pinch", pinch);
        }
        

        private void CollisionEnable(bool enabled)
        {
            if (m_collisionEnabled == enabled)
            {
                return;
            }
            m_collisionEnabled = enabled;

            if (enabled)
            {
                m_collisionScaleCurrent = COLLIDER_SCALE_MIN;
                for (int i = 0; i < m_colliders.Length; ++i)
                {
                    Collider collider = m_colliders[i];
                    collider.transform.localScale = new Vector3(COLLIDER_SCALE_MIN, COLLIDER_SCALE_MIN, COLLIDER_SCALE_MIN);
                    collider.enabled = true;
                }
            }
            else
            {
                m_collisionScaleCurrent = COLLIDER_SCALE_MAX;
                for (int i = 0; i < m_colliders.Length; ++i)
                {
                    Collider collider = m_colliders[i];
                    collider.enabled = false;
                    collider.transform.localScale = new Vector3(COLLIDER_SCALE_MIN, COLLIDER_SCALE_MIN, COLLIDER_SCALE_MIN);
                }
            }
        }
    }
}
