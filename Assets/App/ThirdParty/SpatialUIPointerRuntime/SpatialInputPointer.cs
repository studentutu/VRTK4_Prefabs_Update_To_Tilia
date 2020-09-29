using System;
using UnityEngine;

namespace Adrenak.SUI 
{
    public class SpatialInputPointer : MonoBehaviour 
    {
        public bool input;
        public bool debugDraw;
        public bool holdInput;

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

        void Update()
        {
            if (!input)
            {
                input = holdInput;
            }
            if(debugDraw)
                Debug.DrawRay(transform.position, transform.forward * 10000, Color.blue, Time.deltaTime);
        }
    }
}
