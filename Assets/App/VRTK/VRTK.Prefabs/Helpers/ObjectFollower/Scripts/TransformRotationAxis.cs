using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zinnia.Data.Type;
using Zinnia.Extension;
using Zinnia.Tracking.Follow.Modifier.Property;

namespace Tilia.VRTK
{
    public class TransformRotationAxis : PropertyModifier
    {
        [NonSerialized] private Quaternion? prevRot = null;
        
        /// <summary>
        /// Determines which axes to rotate.
        /// </summary>
        [SerializeField]
        public Vector3State FollowOnAxis = Vector3State.True;

        /// <summary>
        /// Optionally will smooth the rotation
        /// </summary>
        [SerializeField]
        [Range(0,20)]
        protected float smoothOutRotationSeconds = 0;

       
        /// <summary>
        /// Sets the <see cref="FollowOnAxis"/> x value.
        /// </summary>
        /// <param name="value">The value to set to.</param>
        public void SetFollowOnAxisX(bool value)
        {
            FollowOnAxis = new Vector3State(value, FollowOnAxis.yState, FollowOnAxis.zState);
        }

        /// <summary>
        /// Sets the <see cref="FollowOnAxis"/> y value.
        /// </summary>
        /// <param name="value">The value to set to.</param>
        public void SetFollowOnAxisY(bool value)
        {
            FollowOnAxis = new Vector3State(FollowOnAxis.xState, value, FollowOnAxis.zState);
        }

        /// <summary>
        /// Sets the <see cref="FollowOnAxis"/> z value.
        /// </summary>
        /// <param name="value">The value to set to.</param>
        public void SetFollowOnAxisZ(bool value)
        {
            FollowOnAxis = new Vector3State(FollowOnAxis.xState, FollowOnAxis.yState, value);
        }

        
        /// <summary>
        /// Modifies the target rotation to match the given source rotation.
        /// </summary>
        /// <param name="source">The source to utilize in the modification.</param>
        /// <param name="target">The target to modify.</param>
        /// <param name="offset">The offset of the target against the source when modifying.</param>
        protected override void DoModify(GameObject source, GameObject target, GameObject offset = null)
        {
            var sourceRotation = source.transform.rotation.eulerAngles;
            var targetRotation = target.transform.rotation.eulerAngles;

            float xDegree = FollowOnAxis.xState ? sourceRotation.x: targetRotation.x;
            float yDegree = FollowOnAxis.yState ? sourceRotation.y: targetRotation.y;
            float zDegree = FollowOnAxis.zState ? sourceRotation.z: targetRotation.z;

            var sourceQuaternion = Quaternion.Euler(xDegree, yDegree, zDegree);
            if (smoothOutRotationSeconds > 0)
            {   
                sourceQuaternion = SmoothOutRotation(sourceQuaternion, prevRot == null? target.transform.rotation:prevRot.Value);
            }

            if (IsNew(sourceQuaternion, target.transform.rotation))
            {
                if (offset == null)
                {
                    target.transform.rotation = sourceQuaternion;
                }
                else
                {
                    target.transform.rotation = sourceQuaternion * Quaternion.Inverse(offset.transform.localRotation);
                }

                prevRot = target.transform.rotation;
            }
        }

        private static Quaternion SmoothOutRotation(Quaternion newRotation, Quaternion oldRot)
        {
            var resultInterm = Quaternion.Slerp(oldRot, newRotation, Time.deltaTime);
            var isNew = IsNew(resultInterm, oldRot);
            return isNew ? resultInterm : oldRot;
        }

        private static bool IsNew(Quaternion newRotation, Quaternion oldRot)
        {
            return !newRotation.eulerAngles.magnitude.ApproxEquals(oldRot.eulerAngles.magnitude,0.015f);
        }
    }
}