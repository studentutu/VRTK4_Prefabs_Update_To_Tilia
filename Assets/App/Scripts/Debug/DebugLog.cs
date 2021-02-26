using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DebugComponentExtension
{
    [UnityEngine.Scripting.Preserve]
    public static void DebugLogErrorComponent(this MonoBehaviour component)
    {
        Debug.LogError("Error : on component" + component.name, component.gameObject);
    }
}

namespace Services
{
    public class DebugLog : MonoBehaviour
    {
        [UnityEngine.Scripting.Preserve]
        public void LogError(MonoBehaviour context)
        {
            context.DebugLogErrorComponent();
        }
        
        [UnityEngine.Scripting.Preserve]
        public void LogError(System.Object context)
        {
            Debug.LogError("Error : " + JsonUtility.ToJson(context));
        }
    }
}
