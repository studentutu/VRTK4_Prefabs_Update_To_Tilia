using UnityEngine;

namespace Adrenak.SUI 
{
    public class KeyboardPointerInput : MonoBehaviour 
    {
        [SerializeField] SpatialInputPointer pointer;
        [SerializeField] KeyCode keyCode;

        private void Update() 
        {
            pointer.input = Input.GetKey(keyCode);
        }
    }
}
