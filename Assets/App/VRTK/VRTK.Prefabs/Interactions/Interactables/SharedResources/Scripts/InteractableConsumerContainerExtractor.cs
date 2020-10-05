using System;
using Tilia.Interactions.Interactables.Interactables;
using UnityEngine.Events;

namespace Tillia.VRTKUI.Prefabs.Interactions.Interactables
{
    using UnityEngine;
    using Zinnia.Data.Operation.Extraction;

    /// <summary>
    /// Extracts the <see cref="GameObject"/> of the <see cref="InteractableFacade.ConsumerContainer"/>.
    /// </summary>
    public class InteractableConsumerContainerExtractor : GameObjectExtractor<InteractableFacade,InteractableConsumerContainerExtractor.ExtractorGameObjectFromInteractableFacade >
    {
        [Serializable]
        public class ExtractorGameObjectFromInteractableFacade: UnityEvent<GameObject>
        {
            
        }
        /// <summary>
        /// Extracts the <see cref="GameObject"/> of the <see cref="InteractableFacade.ConsumerContainer"/>.
        /// </summary>
        /// <param name="interactable">The Interactable to extract from.</param>
        /// <returns>The related <see cref="GameObject"/>.</returns>
        public override GameObject Extract(InteractableFacade interactable)
        {
            if (interactable == null)
            {
                Result = null;
                return null;
            }

            Result = interactable.MeshContainer;
            return base.Extract();
        }

        /// <summary>
        /// Extracts the <see cref="GameObject"/> of the <see cref="InteractableFacade.ConsumerContainer"/>.
        /// </summary>
        /// <param name="interactable">The Interactable to extract from.</param>
        public override void DoExtract(InteractableFacade interactable)
        {
            Extract(interactable);
        }

        protected override GameObject ExtractValue()
        {
            return Extract(Source);
        }
    }
}