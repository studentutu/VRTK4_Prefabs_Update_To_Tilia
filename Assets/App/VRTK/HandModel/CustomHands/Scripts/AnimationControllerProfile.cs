using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tilia.VRTK.Animations
{
    [CreateAssetMenu(menuName = "AnimatorStateMachine/AnimatorProfile", fileName = "Profile",order = 4)]
    public class AnimationControllerProfile : ScriptableObject
    {
        [SerializeField] private List<AnimationToPLay> allAnimations = new List<AnimationToPLay>();

        public List<AnimationToPLay> AllAnimations => allAnimations;
    }
}
