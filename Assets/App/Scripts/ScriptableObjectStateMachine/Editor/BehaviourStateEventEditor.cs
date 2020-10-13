#if UNITY_EDITOR
using Scripts.Common.AnimationParameter;
using UnityEditor;
using UnityEngine;

namespace Scripts.Common.StateMachine.Editor
{
    [CustomEditor(typeof(BehaviourStateEvent))]
    public class BehaviourStateEventEditor : UnityEditor.Editor
    {
        private Object property;
        
        public override void OnInspectorGUI()
        {
            GUI.enabled = Application.isPlaying;
            BehaviourStateEvent e = target as BehaviourStateEvent;
            property = EditorGUILayout.ObjectField("Data", property, typeof(StateAndParameters), false);
            if (GUILayout.Button("Raise"))
            {
                if (e != null)
                {
                    e.Raise(property as StateAndParameters);
                }
            }
        }
    }
}
#endif