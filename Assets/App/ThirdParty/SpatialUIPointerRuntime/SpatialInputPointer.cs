using System;
using UnityEngine;

namespace Adrenak.SUI 
{
    public class SpatialInputPointer : MonoBehaviour 
    {
        public bool input;
        public bool debugDraw;

        private void OnEnable()
        {
            SpatialInputModule.Instance.AddPointer(this);
        }

        private void OnDisable()
        {
            if (SpatialInputModule.Instance != null)
            {
                SpatialInputModule.Instance.RemovePointer(this);
            }
        }

        private void OnDrawGizmos()
        {
            if (debugDraw)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawRay(transform.position, transform.forward * 10000);
            }
        }
    }
}
