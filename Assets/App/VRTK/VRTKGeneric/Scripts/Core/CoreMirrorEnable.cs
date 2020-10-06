using System;
using System.Collections;
using System.Collections.Generic;
using Tilia.CameraRigs.TrackedAlias;
using UnityEngine;
using Zinnia.Tracking.CameraRig;

namespace Tillia.VRTK
{
    [DisallowMultipleComponent]
    public class CoreMirrorEnable : MonoBehaviour
    {
       [SerializeField] private List<GameObject> SourceObjects = new List<GameObject>();
       [SerializeField] private List<GameObject> TargetObjects = new List<GameObject>();
       [SerializeField] private TrackedAliasFacade RigAlias = null;

       private void OnValidate()
       {
           if (SourceObjects.Count != TargetObjects.Count)
           {
               for (int i = 0; i < SourceObjects.Count; i++)
               {
                   TargetObjects.Add(null);
               }
           }
       }

       private void Awake()
       {
           RigAlias.TrackedAliasChanged.AddListener(ProcessMirror);
       }

       private void OnDestroy()
       {
           RigAlias.TrackedAliasChanged.RemoveListener(ProcessMirror);
       }

       
       private void ProcessMirror(LinkedAliasAssociationCollection enabledObject)
       {
           var targetObj = enabledObject.gameObject;
           for (int j = 0; j < SourceObjects.Count; j++)
           {
               if (SourceObjects[j] == targetObj)
               {
                   TargetObjects[j].SetActive(targetObj.activeInHierarchy);
                   Cursor.visible = false;
                   StartCoroutine(WaitForAFrame());
               }
               else
               {
                   TargetObjects[j].SetActive(false);
               }
           }
       }

       private IEnumerator WaitForAFrame()
       {
           yield return new WaitForSeconds(0.5f);
           Cursor.lockState = CursorLockMode.Confined;
           yield return new WaitForSeconds(3f);
           Cursor.lockState = CursorLockMode.Locked;

       }
    }
}
