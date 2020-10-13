using System;
using Scripts.Common.AnimationParameter;

namespace Tillia.VRTK.Animations
{
    [Serializable]
    public class AnimationToPLay
    {
        public StateAndParameters parameter;
        public AnimationEnum enumOfAnimation = AnimationEnum.Relaxed;
        public bool LockAnimation = false;
    }
}