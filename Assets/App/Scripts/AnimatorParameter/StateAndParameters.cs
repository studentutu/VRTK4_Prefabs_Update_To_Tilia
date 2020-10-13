using System;
using System.Collections.Generic;
using Scripts.Common.AnimationParameter.Data;
using UnityEngine;

namespace Scripts.Common.AnimationParameter
{
    public interface IState
    {
        AnimationParamSerialiazable[] CurrentParameters { get; }
    }
#if UNITY_EDITOR
    public interface IStateEditoHelper
    {
        void CopyParamsFrom(UnityEditor.Animations.AnimatorController toCopyFrom);
    }
#endif

    [System.Serializable]
    public class StateAndParameters : ScriptableObject, IState
#if UNITY_EDITOR
        , IStateEditoHelper
#endif
    {
	    
        [SerializeField]
        private AnimationParamSerialiazable[] parameters = new AnimationParamSerialiazable[0];

        [System.NonSerialized] private AnimationParamSerialiazable[] runtimeData = null;
		
        public AnimationParamSerialiazable[] CurrentParameters
        {
	        get
	        {
		        if(runtimeData == null || runtimeData.Length == 0)
		        {
			        runtimeData = new AnimationParamSerialiazable[parameters.Length];
			        for (int i = 0; i < parameters.Length; i++)
			        {
				        runtimeData[i] = new AnimationParamSerialiazable();
				        parameters[i].CopyTo(runtimeData[i]);
			        }
		        }
				return runtimeData;
	        }
		}

        public void SetParameters(AnimationParamSerialiazable[] copyFrom)
        {
	        runtimeData = copyFrom;
        }
        
        public void SetParameters(List<AnimationParamSerialiazable> copyFrom)
        {
	        runtimeData = copyFrom.ToArray();
        }
        
        public AnimationParamSerialiazable[] GetInitialParameters()
        {
	        AnimationParamSerialiazable[] param = new AnimationParamSerialiazable[parameters.Length];
	        for (int i = 0; i < parameters.Length; i++)
	        {
		        param[i] = new AnimationParamSerialiazable();
		        parameters[i].CopyTo(param[i]);
	        }
	        return parameters;
        }

#if UNITY_EDITOR
        // only Needed in Editor for copy data
        [System.NonSerialized] private Dictionary<string, AnimationParamSerialiazable> editorHelperDictionary = new Dictionary<string, AnimationParamSerialiazable>();
        [System.NonSerialized] private List<AnimationParamSerialiazable> editorHelperList = new List<AnimationParamSerialiazable>();

        void IStateEditoHelper.CopyParamsFrom(UnityEditor.Animations.AnimatorController animatorToCpyParametersfrom)
        {
            // This SHould Be in IState!
            if (animatorToCpyParametersfrom != null)
            {
                editorHelperDictionary.Clear();
                editorHelperList.Clear();
                editorHelperList.AddRange(parameters);
                foreach (var item in editorHelperList)
                {
                    editorHelperDictionary.Add(item.keyName, item);
                }
	            // Save previous
	            var dictCopy = new Dictionary<string,AnimationParamSerialiazable>();
	            foreach (var item in editorHelperDictionary)
	            {
		            var copy = new AnimationParamSerialiazable();
		            item.Value.CopyTo(copy); 
		            dictCopy.Add(item.Value.keyName, copy);
	            }
	            
                editorHelperList.Clear();
                AnimationParamSerialiazable temporalParam = null;
                for (int i = 0; i < animatorToCpyParametersfrom.parameters.Length; i++)
                {
                    if (!editorHelperDictionary.TryGetValue(animatorToCpyParametersfrom.parameters[i].name, out temporalParam))
                    {
                        temporalParam = new AnimationParamSerialiazable();
                    }

                    temporalParam.boolValue = animatorToCpyParametersfrom.parameters[i].defaultBool;
                    temporalParam.floatValue = animatorToCpyParametersfrom.parameters[i].defaultFloat;
                    temporalParam.intValue = animatorToCpyParametersfrom.parameters[i].defaultInt;
                    temporalParam.keyName = animatorToCpyParametersfrom.parameters[i].name;
                    temporalParam.nameHash = animatorToCpyParametersfrom.parameters[i].nameHash;
                    switch (animatorToCpyParametersfrom.parameters[i].type)
                    {
	                    case AnimatorControllerParameterType.Float:
							temporalParam.type = AnimationParamSerialiazable.AnimationType.Float;
		                    break;
	                    case AnimatorControllerParameterType.Int:
		                    temporalParam.type = AnimationParamSerialiazable.AnimationType.Int;
		                    break;
	                    case AnimatorControllerParameterType.Bool:
		                    temporalParam.type = AnimationParamSerialiazable.AnimationType.Bool;
		                    break;
	                    case AnimatorControllerParameterType.Trigger:
		                    temporalParam.type = AnimationParamSerialiazable.AnimationType.Trigger;
		                    break;
                    }
					
                    if (dictCopy.TryGetValue(animatorToCpyParametersfrom.parameters[i].name, out var prevValue) &&
                        prevValue != null)
                    {
	                    temporalParam.boolValue = prevValue.boolValue;
	                    temporalParam.floatValue = prevValue.floatValue;
	                    temporalParam.intValue = prevValue.intValue;
                    }
	                
                    editorHelperList.Add(temporalParam);
                }
                parameters = editorHelperList.ToArray();
            }
        }
#endif
    }

}