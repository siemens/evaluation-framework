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

[CustomEditor(typeof(ChooseROSParameter))]
[CanEditMultipleObjects]
public class ROSParameterEditor : ParameterEditorBase
{
    private void OnEnable()
    {
        chooseParameterObject = (ChooseROSParameter)target;

        LoadScriptableObject();
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginDisabledGroup(_changesSaved);
        if (chooseParameterObject.GameObjectOfInterest == null)
        {
            // set type of selected object
            gameObjectOfInterest = EditorGUILayout.ObjectField("Choose Game Object",
                                                               gameObjectOfInterest,
                                                               typeof(GameObject),
                                                               true);
            chooseParameterObject.GameObjectOfInterest = (GameObject)gameObjectOfInterest;
            EditorGUILayout.Space();
        }
        else
        {
            GUILayout.Label("Selected Game Object: " + chooseParameterObject.GameObjectOfInterest.name, EditorStyles.boldLabel);
            if (GUILayout.Button("Change Game Object"))
            {
                chooseParameterObject.GameObjectOfInterest = null;
                gameObjectOfInterest = null;
                return;
            }
            // generate all components of the selected GameObject
            chooseParameterObject.HandleGameObjectComponents();
            _componentChoices = chooseParameterObject.ComponentOptionStrings;

            // check if SCRIPT components are available
            if (_componentChoices.Length < 1)
            {
                GUILayout.Label("No Components Available, Choose Different GameObject!",
                    EditorStyles.boldLabel);
            }
            else
            {
                SetComponentDropDown();

                // check if PUBLIC variables are available
                if (_variableChoices.Length < 1)
                {
                    GUILayout.Label("No Public Variables Available, Choose Different Component!",
                        EditorStyles.boldLabel);
                }
                else
                {
                    SetVariableDropDown();

                    DisplaySaveButton();
                }
            }
        }
        EditorGUI.EndDisabledGroup();

        if (GUI.changed)
        {
            // save changes
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(chooseParameterObject);
        }
    }

    private void DisplaySaveButton()
    {
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        if (GUILayout.Button("Save and Add ROS Parameter to Manager"))
        {
            chooseParameterObject.ParameterHandlerObject =
                new ROSParameterHandler(chooseParameterObject.GameObjectOfInterest,
                                          chooseParameterObject.SelectedComponent,
                                          chooseParameterObject.SelectedVariable,
                                          chooseParameterObject.SelectedVariableType);
            _changesSaved = true;
            // rename scriptable object
            string assetName = chooseParameterObject.GameObjectOfInterest.name + ", " +
                    chooseParameterObject.SelectedComponent.GetType() + ", " + chooseParameterObject.SelectedVariable;
            RenameScriptableObject(assetName);
            chooseParameterObject.SaveSelectedVariableOptions();

            // save changes
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(chooseParameterObject);
        }
    }
}
