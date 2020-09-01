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

[CustomEditor(typeof(ChooseEvaluationParameter))]
[CanEditMultipleObjects]
public class EvaluationParameterEditor : ParameterEditorBase
{
    // variable setting
    private int[] intVariableSetting = new int[] { 0, 0, 0 }; // min, max, step
    private float[] floatVariableSetting = new float[] { 0f, 0f, 0f }; // min, max, step
    private int[][] intArrayVariableSetting;
    private float[][] floatArrayVariableSetting;

    private int arraySize;
    private string unit;

    private void OnEnable()
    {
        chooseParameterObject = (ChooseEvaluationParameter)target;

        LoadScriptableObject();
    }

    // load scriptable object data when selecting the object in the assets folder during the definition process
    protected override void LoadScriptableObject()
    {
        base.LoadScriptableObject();

        if (chooseParameterObject.ParameterHandlerObject != null)
        {
            if (chooseParameterObject.ParameterHandlerObject.GetType() == typeof(IntegerEvaluationParameterHandler))
            {
                LoadIntVariableSetting();
            }
            else if (chooseParameterObject.ParameterHandlerObject.GetType() == typeof(FloatEvaluationParameterHandler))
            {
                LoadFloatVariableSetting();
            }
            else if (chooseParameterObject.ParameterHandlerObject.GetType() == typeof(BoolEvaluationParameterHandler))
            {
                return;
            }
            else if (chooseParameterObject.ParameterHandlerObject.GetType() == typeof(IntegerArrayEvaluationParameterHandler))
            {
                LoadIntArrayVariableSetting();
            }
            else if (chooseParameterObject.ParameterHandlerObject.GetType() == typeof(FloatArrayEvaluationParameterHandler))
            {
                LoadFloatArrayVariableSetting();
            }
        }
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
                EditorGUI.BeginChangeCheck();
                SetComponentDropDown();
                if (EditorGUI.EndChangeCheck())
                    ResetVariableSettings();

                // check if PUBLIC variables are available
                if (_variableChoices.Length < 1)
                {
                    GUILayout.Label("No Public Variables Available, Choose Different Component!",
                        EditorStyles.boldLabel);
                }
                else
                {
                    EditorGUI.BeginChangeCheck();
                    SetVariableDropDown();
                    if (EditorGUI.EndChangeCheck())
                        ResetVariableSettings();

                    if (chooseParameterObject.SelectedVariableType.IsArray)
                        arraySize = EditorGUILayout.IntField("ArraySize:", arraySize);

                    DisplayVariableSettings();
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

    private void ResetVariableSettings()
    {
        intVariableSetting = new int[] { 0, 0, 0 }; // min, max, step
        floatVariableSetting = new float[] { 0f, 0f, 0f }; // min, max, step
        intArrayVariableSetting = null;
        floatArrayVariableSetting = null;
    }

    private void DisplayVariableSettings()
    {
        GUILayout.Label("Set Variable Options for " + chooseParameterObject.SelectedVariable, EditorStyles.boldLabel);

        EditorGUILayout.BeginVertical();
        bool correctVariableSetting;
        if (chooseParameterObject.SelectedVariableType == typeof(int))
        {
            correctVariableSetting = DisplayIntVariableSetting();
        }
        else if (chooseParameterObject.SelectedVariableType == typeof(float))
        {
            correctVariableSetting = DisplayFloatVariableSetting();
        }
        else if (chooseParameterObject.SelectedVariableType == typeof(bool))
        {
            correctVariableSetting = DisplayBoolVariableSetting();
        }
        else if (chooseParameterObject.SelectedVariableType == typeof(int[]))
        {
            correctVariableSetting = DisplayIntArrayVariableSetting();
        }
        else if (chooseParameterObject.SelectedVariableType == typeof(float[]))
        {
            correctVariableSetting = DisplayFloatArrayVariableSetting();
        }
        else
        {
            GUILayout.Label("Selected Type of Variable is Not Supported (yet)!",
                    EditorStyles.boldLabel);
            correctVariableSetting = false;
        }

        EditorGUILayout.EndVertical();

        unit = EditorGUILayout.TextField("(optional) Unit: ", unit);

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUI.BeginDisabledGroup(!correctVariableSetting);
        if (GUILayout.Button(correctVariableSetting ? "Save and Add Parameter to Manager" : "Invalid Parameter Setting"))
        {
            _changesSaved = true;
            // rename scriptable object
            string assetName = chooseParameterObject.GameObjectOfInterest.name + ", " +
                    chooseParameterObject.SelectedComponent + ", " + chooseParameterObject.SelectedVariable;
            RenameScriptableObject(assetName);
            chooseParameterObject.SaveSelectedVariableOptions();

            // save changes
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(chooseParameterObject);
        }
        EditorGUI.EndDisabledGroup();
    }

    #region TypeSpecificVariableSettings
    private void LoadIntVariableSetting()
    {
        var integerObject = (IntegerEvaluationParameterHandler)chooseParameterObject.ParameterHandlerObject;
        intVariableSetting[0] = integerObject.variableSettings[0];
        intVariableSetting[1] = integerObject.variableSettings[1];
        intVariableSetting[2] = integerObject.variableSettings[2];
    }

    private void LoadFloatVariableSetting()
    {
        var floatObject = (FloatEvaluationParameterHandler)chooseParameterObject.ParameterHandlerObject;
        floatVariableSetting[0] = floatObject.variableSettings[0];
        floatVariableSetting[1] = floatObject.variableSettings[1];
        floatVariableSetting[2] = floatObject.variableSettings[2];
    }

    private void LoadIntArrayVariableSetting()
    {
        var integerArrayObject = (IntegerArrayEvaluationParameterHandler)chooseParameterObject.ParameterHandlerObject;
        InitializeVariableArray("int", arraySize);

        for (int i = 0; i < arraySize; i++)
        {
            intArrayVariableSetting[i] = integerArrayObject.variableSettings[i];
        }
    }

    private void LoadFloatArrayVariableSetting()
    {
        var floatArrayObject = (FloatArrayEvaluationParameterHandler)chooseParameterObject.ParameterHandlerObject;
        InitializeVariableArray("float", arraySize);

        for (int i = 0; i < arraySize; i++)
        {
            floatArrayVariableSetting[i] = floatArrayObject.variableSettings[i];
        }
    }

    private bool DisplayIntVariableSetting()
    {
        var integerObject = new IntegerEvaluationParameterHandler(
            chooseParameterObject.GameObjectOfInterest,
            chooseParameterObject.SelectedComponent,
            chooseParameterObject.SelectedVariable,
            chooseParameterObject.SelectedVariableType,
            intVariableSetting, 1, unit);

        intVariableSetting[0] = EditorGUILayout.IntField("Min:", intVariableSetting[0]);
        intVariableSetting[1] = EditorGUILayout.IntField("Max:", intVariableSetting[1]);
        intVariableSetting[2] = EditorGUILayout.IntField("Step:", intVariableSetting[2]);

        // save choice
        integerObject.variableSettings = intVariableSetting;
        chooseParameterObject.ParameterHandlerObject = integerObject;

        return CheckIntVariableSetting(intVariableSetting);
    }

    private bool DisplayFloatVariableSetting()
    {
        var floatObject = new FloatEvaluationParameterHandler(
            chooseParameterObject.GameObjectOfInterest,
            chooseParameterObject.SelectedComponent,
            chooseParameterObject.SelectedVariable,
            chooseParameterObject.SelectedVariableType,
            floatVariableSetting, 1, unit);

        floatVariableSetting[0] = EditorGUILayout.FloatField("Min:", floatVariableSetting[0]);
        floatVariableSetting[1] = EditorGUILayout.FloatField("Max:", floatVariableSetting[1]);
        floatVariableSetting[2] = EditorGUILayout.FloatField("Step:", floatVariableSetting[2]);
        // save choice
        floatObject.variableSettings = floatVariableSetting;
        chooseParameterObject.ParameterHandlerObject = floatObject;

        return CheckFloatVariableSetting(floatVariableSetting);
    }

    private bool DisplayBoolVariableSetting()
    {
        // create corresponding object
        var boolObject = new BoolEvaluationParameterHandler(
            chooseParameterObject.GameObjectOfInterest,
            chooseParameterObject.SelectedComponent,
            chooseParameterObject.SelectedVariable,
            chooseParameterObject.SelectedVariableType,
            1, unit);

        // save choice
        chooseParameterObject.ParameterHandlerObject = boolObject;

        return true;
    }

    private bool DisplayIntArrayVariableSetting()
    {
        bool correctVariableSetting = true;

        if (intArrayVariableSetting == null || intArrayVariableSetting.Length != arraySize)
        {
            InitializeVariableArray("int", arraySize);
        }

        var integerArrayObject = new IntegerArrayEvaluationParameterHandler(
            chooseParameterObject.GameObjectOfInterest,
            chooseParameterObject.SelectedComponent,
            chooseParameterObject.SelectedVariable,
            chooseParameterObject.SelectedVariableType,
            intArrayVariableSetting, 1, unit);

        for (int i = 0; i < arraySize; i++)
        {
            intArrayVariableSetting[i][0] = EditorGUILayout.IntField("Min:", intArrayVariableSetting[i][0]);
            intArrayVariableSetting[i][1] = EditorGUILayout.IntField("Max:", intArrayVariableSetting[i][1]);
            intArrayVariableSetting[i][2] = EditorGUILayout.IntField("Step:", intArrayVariableSetting[i][2]);
            correctVariableSetting = CheckIntVariableSetting(intArrayVariableSetting[i]);
            EditorGUILayout.Space();
        }
        // save choice
        integerArrayObject.variableSettings = intArrayVariableSetting;
        integerArrayObject.selectedVariableSize = arraySize;
        chooseParameterObject.ParameterHandlerObject = integerArrayObject;

        return correctVariableSetting;
    }

    private bool DisplayFloatArrayVariableSetting()
    {
        bool correctVariableSetting = true;

        if (floatArrayVariableSetting == null || floatArrayVariableSetting.Length != arraySize)
        {
            InitializeVariableArray("float", arraySize);
        }

        var floatArrayObject = new FloatArrayEvaluationParameterHandler(
            chooseParameterObject.GameObjectOfInterest,
            chooseParameterObject.SelectedComponent,
            chooseParameterObject.SelectedVariable,
            chooseParameterObject.SelectedVariableType,
            floatArrayVariableSetting, 1, unit);

        for (int i = 0; i < arraySize; i++)
        {
            floatArrayVariableSetting[i][0] = EditorGUILayout.FloatField("Min:", floatArrayVariableSetting[i][0]);
            floatArrayVariableSetting[i][1] = EditorGUILayout.FloatField("Max:", floatArrayVariableSetting[i][1]);
            floatArrayVariableSetting[i][2] = EditorGUILayout.FloatField("Step:", floatArrayVariableSetting[i][2]);
            correctVariableSetting = CheckFloatVariableSetting(floatArrayVariableSetting[i]);
            EditorGUILayout.Space();
        }
        // save choice
        floatArrayObject.variableSettings = floatArrayVariableSetting;
        floatArrayObject.selectedVariableSize = arraySize;
        chooseParameterObject.ParameterHandlerObject = floatArrayObject;

        return correctVariableSetting;
    }

    private bool CheckIntVariableSetting(int[] _variableSetting)
    {
        if (_variableSetting[0] > _variableSetting[1] || _variableSetting[2] <= 0)
            return false;

        return true;
    }

    private bool CheckFloatVariableSetting(float[] _variableSetting)
    {
        if (_variableSetting[0] > _variableSetting[1] || _variableSetting[2] <= 0)
            return false;

        return true;
    }

    private void InitializeVariableArray(string _arrayType, int _arraySize)
    {
        if (_arrayType == "int")
        {
            intArrayVariableSetting = new int[_arraySize][];
            for (int i = 0; i < _arraySize; i++)
                intArrayVariableSetting[i] = new int[] { 0, 0, 0 };
        }
        else if (_arrayType == "float")
        {
            floatArrayVariableSetting = new float[_arraySize][];
            for (int i = 0; i < _arraySize; i++)
                floatArrayVariableSetting[i] = new float[] { 0f, 0f, 0f };
        }
    }
    #endregion
}