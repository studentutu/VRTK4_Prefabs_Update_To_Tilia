using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Adrenak.SUI
{
    [DisallowMultipleComponent]
    public class WorldCanvasSetter : MonoBehaviour
    {
        [SerializeField] private Canvas currentCanvas;

        private void OnEnable()
        {
            if (currentCanvas == null)
            {
                currentCanvas = GetComponent<Canvas>();
            }

            SpatialInputModule.Instance.RegisterCanvas(currentCanvas);
        }
    }
}
