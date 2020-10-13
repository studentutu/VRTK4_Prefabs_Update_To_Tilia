using System.Collections.Generic;
using Scripts.Common.AnimationParameter.Data;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine.UIElements;

namespace Scripts.Common.AnimationParameter
{

    [CustomPropertyDrawer(typeof(AnimationParamSerialiazable), false)]
    public class EditorForStateParameters : PropertyDrawer
    {
        private Dictionary<string, bool> allMyPropsEnabled = new Dictionary<string, bool>();
        private Dictionary<string, ContainerForPropery> allMyProps = new Dictionary<string, ContainerForPropery>();
        
        private class ContainerForPropery
        {
            public AnimationParamSerialiazable.AnimationType type;
            public Object property;
            public List<GUIContent> allCOntents = new List<GUIContent>();

            public void Clear()
            {
                property = null;
                allCOntents.Clear();
            }
        }
        
        private void OnEnable()
        {
            allMyPropsEnabled.Clear();
            allMyProps.Clear();
        }

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            OnEnable();
            return base.CreatePropertyGUI(property);
        }

        public override void OnGUI(Rect position, SerializedProperty serCallBaseProperty, GUIContent label)
        {
            bool isEnabled = false;
            if (!allMyPropsEnabled.TryGetValue(serCallBaseProperty.propertyPath, out isEnabled))
            {
                allMyPropsEnabled.Add(serCallBaseProperty.propertyPath, isEnabled);
            }

            // Indent label
            // label.text = "      " + label.text;
            Rect onlyToggle2 = new Rect(position.x, position.y + 2, position.width, position.height);
            // Get keyName
            SerializedProperty targetProp = serCallBaseProperty.FindPropertyRelative(nameof(AnimationParamSerialiazable.keyName));
            string nameKey = targetProp.stringValue;

            GUI.Box(onlyToggle2, "", "toolbarDropDown"); // see https://gist.github.com/MadLittleMods/ea3e7076f0f59a702ecb
            position.y += 4;
            position.x += 4;
            Rect onlyToggle = new Rect(position.x, position.y, 100, EditorGUIUtility.singleLineHeight);
            isEnabled = GUI.Toggle(onlyToggle, isEnabled, nameKey);
            
            allMyPropsEnabled[serCallBaseProperty.propertyPath] = isEnabled;
            if (isEnabled)
            {
                EditorGUI.BeginProperty(position, label, serCallBaseProperty);
                EditorGUI.BeginChangeCheck();
                // Using BeginProperty / EndProperty on the parent property means that
                // prefab override logic works on the entire property.
                position.y += 4 + EditorGUIUtility.singleLineHeight;

                // Draw label
                // Rect pos = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
                Rect targetRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);


                // Get nameHash
                SerializedProperty NameHashProp = serCallBaseProperty.FindPropertyRelative(nameof(AnimationParamSerialiazable.nameHash));
                

                EditorGUI.PropertyField(targetRect, targetProp); // keyName
                targetRect = new Rect(position.x, targetRect.max.y + EditorGUIUtility.standardVerticalSpacing, position.width, EditorGUIUtility.singleLineHeight);

                EditorGUI.PropertyField(targetRect, NameHashProp); // nameHash
                targetRect = new Rect(position.x, targetRect.max.y + EditorGUIUtility.standardVerticalSpacing, position.width, EditorGUIUtility.singleLineHeight);

                SerializedProperty typeEnum = serCallBaseProperty.FindPropertyRelative(nameof(AnimationParamSerialiazable.type));
                int currentValue = typeEnum.enumValueIndex;
                var asEnum = (AnimationParamSerialiazable.AnimationType)currentValue;
                ContainerForPropery contaienrOfStuff = null;
                if (!allMyProps.TryGetValue(serCallBaseProperty.propertyPath, out contaienrOfStuff))
                {
                    contaienrOfStuff = new ContainerForPropery();
                    contaienrOfStuff.type = asEnum;
                    allMyProps.Add(serCallBaseProperty.propertyPath, contaienrOfStuff);
                }

                contaienrOfStuff.type = asEnum;

                EditorGUI.PropertyField(targetRect, typeEnum); // type

                targetRect = new Rect(position.x, targetRect.max.y + EditorGUIUtility.standardVerticalSpacing, position.width, EditorGUIUtility.singleLineHeight);
                List<SerializedProperty> customPropertyFiedlFromEnum = new List<SerializedProperty>();
                switch (asEnum)
                {
                    case AnimationParamSerialiazable.AnimationType.Bool:
                        contaienrOfStuff.Clear();
                        customPropertyFiedlFromEnum.Add(
                            serCallBaseProperty.FindPropertyRelative(nameof(AnimationParamSerialiazable.boolValue))
                        );
                        break;
                    case AnimationParamSerialiazable.AnimationType.Float:
                        contaienrOfStuff.Clear();

                        customPropertyFiedlFromEnum.Add(
                            serCallBaseProperty.FindPropertyRelative(nameof(AnimationParamSerialiazable.floatValue))
                        );
                        break;
                    case AnimationParamSerialiazable.AnimationType.Int:
                        contaienrOfStuff.Clear();

                        customPropertyFiedlFromEnum.Add(
                            serCallBaseProperty.FindPropertyRelative(nameof(AnimationParamSerialiazable.intValue))
                        );
                        break;
                    case AnimationParamSerialiazable.AnimationType.Trigger:
                        contaienrOfStuff.Clear();

                        customPropertyFiedlFromEnum.Add(
                            serCallBaseProperty.FindPropertyRelative(nameof(AnimationParamSerialiazable.triggerValue))
                        );
                        break;
                    case AnimationParamSerialiazable.AnimationType.LayerWeight:
                        var layerFloatProp = serCallBaseProperty.
                                FindPropertyRelative(nameof(AnimationParamSerialiazable.floatValue));
                        customPropertyFiedlFromEnum.Add(layerFloatProp);
                        var propIndexOfLayer =
                            serCallBaseProperty.FindPropertyRelative(
                                nameof(AnimationParamSerialiazable.indexOfLayer));
                        
                        contaienrOfStuff.property = EditorGUI.ObjectField(targetRect,"Get Layer from", contaienrOfStuff.property, 
                            typeof(AnimatorController),true);
                        targetRect = new Rect(position.x, targetRect.max.y + EditorGUIUtility.standardVerticalSpacing, position.width, EditorGUIUtility.singleLineHeight);

                        if (contaienrOfStuff.property != null)
                        {
                            contaienrOfStuff.allCOntents.Clear();
                            int indexInLayers = 0;
                            foreach (var layers in (contaienrOfStuff.property as AnimatorController).layers)
                            {
                                var newCOntent = new GUIContent();
                                newCOntent.text = layers.name;
                                newCOntent.tooltip = "index "+ indexInLayers.ToString();
                                contaienrOfStuff.allCOntents.Add(newCOntent);
                                indexInLayers++;
                            }

                            propIndexOfLayer.intValue = EditorGUI.Popup(targetRect,new GUIContent("Layer :"),
                                propIndexOfLayer.intValue,contaienrOfStuff.allCOntents.ToArray()
                            );
                        }
                        else
                        {
                            customPropertyFiedlFromEnum.Remove(layerFloatProp);
                            EditorGUI.TextField(targetRect, string.Format("Layer: {0} Weight: {1}",propIndexOfLayer.intValue, layerFloatProp.floatValue ));
                        }
                        targetRect = new Rect(position.x, targetRect.max.y + EditorGUIUtility.standardVerticalSpacing, position.width, EditorGUIUtility.singleLineHeight);

                        break;
                }
                if (customPropertyFiedlFromEnum.Count > 0)
                {
                    foreach (var prop in customPropertyFiedlFromEnum)
                    {
                        EditorGUI.PropertyField(targetRect, prop);
                    }
                }
                else
                {
                    GUIContent methodlabel = new GUIContent("Set Type for (" + label.text + ")");
                    // Rect methodRect = new Rect(position.x, targetRect.max.y + EditorGUIUtility.standardVerticalSpacing, position.width, EditorGUIUtility.singleLineHeight);
                    // Method select button
                    EditorGUI.PrefixLabel(targetRect, GUIUtility.GetControlID(FocusType.Passive), methodlabel);

                }
                if (!string.IsNullOrEmpty(nameKey))
                {
                    if (NameHashProp.intValue == default(int) || NameHashProp.intValue != Animator.StringToHash(nameKey))
                    {
                        NameHashProp.intValue = Animator.StringToHash(nameKey);
                        serCallBaseProperty.serializedObject.ApplyModifiedProperties();
                        serCallBaseProperty.serializedObject.Update();
                        // return;
                    }
                }
                if (EditorGUI.EndChangeCheck())
                {
                    serCallBaseProperty.serializedObject.ApplyModifiedProperties();
                    serCallBaseProperty.serializedObject.Update();
                }
                // Set indent back to what it was
                EditorGUI.EndProperty();
            }
        }

        // Unity Layout!
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float lineheight = EditorGUIUtility.standardVerticalSpacing + EditorGUIUtility.singleLineHeight;
            float height = lineheight * 4 + EditorGUIUtility.singleLineHeight * 1.5f;
            
            ContainerForPropery contaienrOfStuff = null;
            if (allMyProps.TryGetValue(property.propertyPath, out contaienrOfStuff))
            {
                if (contaienrOfStuff.type == AnimationParamSerialiazable.AnimationType.LayerWeight)
                {
                    height = lineheight * 6 + EditorGUIUtility.singleLineHeight * 1.5f; // 2 additional object fields
                }
            }
            // height += 8;
            bool isEnabled = false;
            if (!allMyPropsEnabled.TryGetValue(property.propertyPath, out isEnabled))
            {
                allMyPropsEnabled.Add(property.propertyPath, isEnabled);
            }
            if (!isEnabled)
            {
                height = EditorGUIUtility.singleLineHeight + 8;
            }
            return height;
        }
    }
}