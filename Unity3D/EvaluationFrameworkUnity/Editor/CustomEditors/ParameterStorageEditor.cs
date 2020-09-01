/*
© Siemens AG, 2020
Author: Michael Dyck (m.dyck@gmx.net)

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

<http://www.apache.org/licenses/LICENSE-2.0>.

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ParameterStorageBase), true)]
public class ParameterStorageEditor : Editor
{
    private ParameterStorageBase parameterStorage;

    private void OnEnable()
    {
        parameterStorage = (ParameterStorageBase)target;

        if (target.GetType() == typeof(EvaluationParameterStorage))
            parameterStorage = (EvaluationParameterStorage)parameterStorage;
        else if (target.GetType() == typeof(ObjectiveValueStorage))
            parameterStorage = (ObjectiveValueStorage)parameterStorage;
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.LabelField("Evaluation Framework: Parameter Storage");

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        
        EditorGUI.indentLevel++;

        for (int i = 0; i < parameterStorage.parameterHandlerArray.Length; i++)
        {
            EditorGUI.BeginDisabledGroup(true);
            if (target.GetType() == typeof(EvaluationParameterStorage))
            {
                EditorGUILayout.ObjectField("ScriptableObject: ", 
                                            ((EvaluationParameterStorage)parameterStorage).scriptObjInstances[i],
                                            typeof(GameObject),
                                            true);
            }
            else if (target.GetType() == typeof(ObjectiveValueStorage))
            {
                EditorGUILayout.ObjectField("ScriptableObject: ", 
                                            ((ObjectiveValueStorage)parameterStorage).scriptObjInstances[i],
                                            typeof(GameObject),
                                            true);
            }
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Space();

            var paramHandl = parameterStorage.parameterHandlerArray[i];
            EditorGUILayout.LabelField("GameObject: " + paramHandl.selectedGameObjectName);
            EditorGUILayout.LabelField("Component: " + paramHandl.selectedComponentName);
            EditorGUILayout.LabelField("Variable: " + paramHandl.selectedVariable);
            EditorGUILayout.LabelField("Variable Type: " + paramHandl.selectedVariableTypeString);

            EditorGUILayout.Space();
            EditorGUILayout.Space();
        }

        EditorGUI.indentLevel = 0;

        if (GUI.changed)
        {
            // save changes
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(parameterStorage);
        }
    }
}
