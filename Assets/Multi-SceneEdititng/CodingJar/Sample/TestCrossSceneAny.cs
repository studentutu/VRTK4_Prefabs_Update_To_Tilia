using Scripts.Common.AnimationParameter;
using UnityEngine;

namespace CodingJar.MultiScene.Samples
{
    public class TestCrossSceneAny : MonoBehaviour
    {
        [SerializeField] private Collider FromAnotherScene;
        [SerializeField] private StateAndParameters parameter;
    }
}
