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

using System;
using UnityEditor;

public class ParameterEditorBase : Editor
{
    protected ChooseParameterBase chooseParameterObject;

    protected UnityEngine.Object gameObjectOfInterest;

    protected string[] _componentChoices;
    protected int _componentChoiceIndex = 0;
    protected string[] _variableChoices;
    protected int _variableChoiceIndex = 0;

    protected bool _changesSaved = false;

    protected virtual void LoadScriptableObject()
    {
        gameObjectOfInterest = chooseParameterObject.GameObjectOfInterest;
        if (chooseParameterObject.SelectedComponent != null)
            _componentChoiceIndex = Array.IndexOf(chooseParameterObject.ComponentOptions,
                                                  chooseParameterObject.SelectedComponent);

        if (chooseParameterObject.SelectedVariable != null)
            _variableChoiceIndex = Array.IndexOf(chooseParameterObject.VariableOptionStrings,
                                                 chooseParameterObject.SelectedVariable);

        if (chooseParameterObject.settingsSaved)
            _changesSaved = true;
    }

    protected void SetComponentDropDown()
    {
        // DropDown menu for choosing desired component
        _componentChoiceIndex = EditorGUILayout.Popup("Choose Component",
                                                      _componentChoiceIndex,
                                                      _componentChoices);
        EditorGUILayout.Space();
        // set type of selected component
        chooseParameterObject.SelectedComponent = chooseParameterObject.ComponentOptions[_componentChoiceIndex];
        // generate all variables of this component
        chooseParameterObject.HandleComponentProperties();
        _variableChoices = chooseParameterObject.VariableOptionStrings;
    }

    protected void SetVariableDropDown()
    {
        // DropDown menu for choosing desired variable
        _variableChoiceIndex = EditorGUILayout.Popup("Choose Variable",
                                                     _variableChoiceIndex,
                                                     _variableChoices);
        EditorGUILayout.Space();
        // set selected variable itself and its type
        chooseParameterObject.SelectedVariable =
            chooseParameterObject.VariableOptionStrings[_variableChoiceIndex];
        chooseParameterObject.SelectedVariableType =
            chooseParameterObject.TypesOfVariableOptions[_variableChoiceIndex];
    }

    protected void RenameScriptableObject(string _name)
    {
        // rename scriptable object
        string assetPath = AssetDatabase.GetAssetPath(chooseParameterObject.GetInstanceID());
        AssetDatabase.RenameAsset(assetPath, _name);
    }
}
