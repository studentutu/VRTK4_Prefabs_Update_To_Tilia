using UnityEngine;

namespace Adrenak.SUI {
    [RequireComponent(typeof(Canvas))]
    public class SpatialInputCanvas : MonoBehaviour {
        void Start() => SpatialInputModule.Instance.RegisterCanvas(GetComponent<Canvas>());
    }
}
