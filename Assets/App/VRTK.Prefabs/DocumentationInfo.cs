using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRTK.Prefabs
{
    
    public class DocumentationInfo : MonoBehaviour
    {
        [SerializeField, TextArea(10,50)]
        private string Info;
    }
}
