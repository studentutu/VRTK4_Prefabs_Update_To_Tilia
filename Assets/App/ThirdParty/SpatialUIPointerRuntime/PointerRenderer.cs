using UnityEngine;

namespace Adrenak.SUI 
{
    public class PointerRenderer : MonoBehaviour 
    {
        public SpatialInputPointer pointer;
        public LineRenderer lineRenderer = null;
        public Transform marker = null;
        public float maxLength = Mathf.Infinity;
        public float defaultLength = 1;

        void Update() 
        {
            Ray selectionRay = new Ray(transform.position, transform.forward);
            Render(selectionRay);
        }

        public void Render(Ray ray)
        {
            var data = SpatialInputModule.Instance.GetPointerData(pointer);
            var result = data.RaycastResult;
            float distance;

            if (result.isValid) 
            {
                distance = Mathf.Min(result.distance, maxLength);
                if (marker != null) 
                {
                    if (!marker.gameObject.activeInHierarchy)
                    {
                        marker.gameObject.SetActive(true);
                    }
                    marker.position = transform.position + transform.forward * distance;
                    marker.LookAt(transform);
                }
            }
            else 
            {
                distance = defaultLength;
                if (marker != null) 
                {
                    if (marker.gameObject.activeInHierarchy)
                    {
                        marker.gameObject.SetActive(false);
                    }
                    marker.LookAt(transform);
                }
            }

            if (lineRenderer != null) 
            {
                lineRenderer.SetPosition(0, ray.origin);
                lineRenderer.SetPosition(1, ray.origin + ray.direction * distance);
            }
        }
    }
}