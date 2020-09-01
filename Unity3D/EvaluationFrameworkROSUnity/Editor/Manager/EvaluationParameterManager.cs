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
using System.Collections.Generic;

public class EvaluationParameterManager : EditorWindow
{
    private readonly string saveParameterButton = "Save Evaluation Parameters";
    private readonly string selectBaseFolderButton = "Specify Path To Framework-Base-Folder";
    private readonly string startBuildProcessButton = "Build Executable";
    private readonly string finishSelectionProcess = "Finish Selection Process";
    private readonly string removeParameter = "Remove Parameter";

    private readonly string baseFolder = "Assets/EvaluationFrameworkROSUnity/";
    private readonly string scriptableObjectSubFolder = "EvaluationParameterStorage/";
    private readonly string scriptableObjectSavedSubFolder = "ScriptableObjects/";

    private string frameworkBaseFolder;
    private string evaluationName = "Evaluation";

    public static List<ChooseEvaluationParameter> assets = new List<ChooseEvaluationParameter>();
    private EvaluationParameterStorage storageAsset;

    public static bool useROS = false;
    public static bool parametersSaved = false;
    private bool executableBuilt = false;
    private bool folderSelected = false;

    private Vector2 scrollPos;

    private GUIStyle largeLabelStyle = new GUIStyle();
    private GUIStyle smallLabelStyle = new GUIStyle();
    private GUIStyle smallLabelStyleCenter = new GUIStyle();
    private GUIStyle variableLabelStyle = new GUIStyle();

    // static variables
    private static readonly string gameObjectName = "EvaluationFramework";
    private static ChooseEvaluationParameter currentAsset;
    public static List<string> objStrings = new List<string>();
    public static List<ParameterHandler> parameterList = new List<ParameterHandler>();

    // Add menu named "My Window" to the Window menu
    [MenuItem("Evaluation Framework/Launch")]
    private static void ShowWindow()
    {
        // Get existing open window or if none, make a new one:
        EvaluationParameterManager window = (EvaluationParameterManager)GetWindow(typeof(EvaluationParameterManager));
        window.Show();

        // create empty game object
        if (GameObject.Find(gameObjectName) == null)
            _ = new GameObject(gameObjectName);
    }

    // Event Callback for saving variables
    public static void SaveParametersEventHandler()
    {
        int lastElement = objStrings.Count - 1;
        objStrings[lastElement] = currentAsset.name;
        parameterList[lastElement] = currentAsset.ParameterHandlerObject;
    }

    private void OnGUI()
    {
        // font styles
        largeLabelStyle.fontSize = 14;
        largeLabelStyle.fontStyle = FontStyle.Bold;
        smallLabelStyle.fontSize = 12;
        smallLabelStyleCenter.fontSize = 12;
        smallLabelStyleCenter.alignment = TextAnchor.MiddleCenter;
        smallLabelStyleCenter.normal.background = Texture2D.whiteTexture;
        variableLabelStyle.fontSize = 11;
        variableLabelStyle.fixedWidth = 1;

        bool ableToAddParam = objStrings.Count == 0 ||
            (objStrings.Count > 0 && objStrings[objStrings.Count - 1] != "Empty");

        EditorGUILayout.LabelField("Evaluation Framework: Parameter Manager", largeLabelStyle);

        EditorGUILayout.Space();

        scrollPos =
            EditorGUILayout.BeginScrollView(scrollPos);

        EditorGUI.BeginDisabledGroup(parametersSaved);

        useROS = EditorGUILayout.Toggle("Use ROS Communication", useROS);

        EditorGUI.BeginDisabledGroup(!ableToAddParam);
        if (GUILayout.Button(ableToAddParam ? "Add Parameter" : "Fill Parameter Object First"))
        {
            if (objStrings.Count == 0)
            {
                CreateMyAsset();
                objStrings.Add("Empty");
                parameterList.Add(null);
            }
            else if (objStrings[objStrings.Count - 1] != null)
            {
                CreateMyAsset();
                objStrings.Add("Empty");
                parameterList.Add(null);
            }
        }
        EditorGUI.EndDisabledGroup();

        EditorGUILayout.Space();

        for (var i = 0; i < objStrings.Count; i++)
        {
            EditorGUILayout.LabelField(string.Format("{0}. Evaluation Parameter", i + 1),
                largeLabelStyle);

            if (GUILayout.Button(removeParameter))
            {
                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(assets[i]));
                assets.RemoveAt(i);
                objStrings.RemoveAt(i);
                parameterList.RemoveAt(i);
                i--;
                continue;
            }

            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField(objStrings[i], smallLabelStyle);

            EditorGUILayout.Space();
            if (objStrings[i] != "Empty")
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.LabelField("Parameter Information", largeLabelStyle);

                EditorGUI.indentLevel++;
                EditorGUILayout.LabelField("GameObject: " + parameterList[i].selectedGameObject, smallLabelStyle);
                EditorGUILayout.LabelField("Component: " + parameterList[i].selectedComponent.GetType(), smallLabelStyle);
                EditorGUILayout.LabelField("Variable: " + parameterList[i].selectedVariable, smallLabelStyle);

                EditorGUILayout.LabelField("Variable Settings:", smallLabelStyle);
                EditorGUI.indentLevel++;
                EditorGUILayout.BeginHorizontal();
                DisplayVariableSettings(i);
                EditorGUILayout.EndHorizontal();

                EditorGUI.indentLevel = 0;

                EditorGUILayout.Space();
            }
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        if (objStrings.Count != 0 && objStrings[objStrings.Count - 1] != "Empty")
        {
            if (GUILayout.Button(selectBaseFolderButton))
            {
                frameworkBaseFolder =
                    EditorUtility.OpenFolderPanel("Specify Path to Framework Base Folder", "", "");
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                folderSelected = true;
            }

            if (folderSelected)
            {
                EditorGUILayout.LabelField("Enter Name of Current Evaluation", largeLabelStyle);
                evaluationName = EditorGUILayout.TextField(evaluationName, smallLabelStyleCenter);
                EditorGUILayout.Space();

                if (GUILayout.Button(saveParameterButton))
                {
                    parametersSaved = true;
                    CreateEvaluationParameterStorage();
                    SaveParametersIntoJson();
                    if (useROS)
                        ROSParameterManager.ShowROSParameterManagerWindow();
                    else
                        ObjectiveValueManager.ShowObjectiveValueManagerWindow();

                    EditorGUILayout.Space();
                }
            }
            EditorGUI.EndDisabledGroup();

            if (parametersSaved)
            {
                EditorGUI.BeginDisabledGroup(executableBuilt || !ObjectiveValueManager.objectiveValuesSaved || 
                                             (useROS && !ROSParameterManager.rosParametersSaved));
                if (GUILayout.Button(ObjectiveValueManager.objectiveValuesSaved
                                     ? startBuildProcessButton : "Choose Objective Values First"))
                {
                    StartBuildProcess();

                    EditorGUILayout.Space();
                }

                EditorGUI.EndDisabledGroup();
            }

            if (executableBuilt)
            {
                if (GUILayout.Button(finishSelectionProcess))
                {
                    FinishSelectionProcess();
                }
            }
        }

        EditorGUILayout.EndScrollView();
    }

    private void CreateMyAsset()
    {
        string dir = baseFolder + scriptableObjectSubFolder + scriptableObjectSavedSubFolder;
        string parentDir = baseFolder + scriptableObjectSubFolder;
        if (!AssetDatabase.IsValidFolder(dir))
            AssetDatabase.CreateFolder(parentDir, scriptableObjectSavedSubFolder);

        currentAsset = CreateInstance<ChooseEvaluationParameter>();
        assets.Add(currentAsset);

        AssetDatabase.CreateAsset(currentAsset, dir + "ParameterAsset.asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = currentAsset;
    }

    private void DisplayVariableSettings(int _i)
    {
        if (assets[_i].ParameterHandlerObject.GetType() == typeof(IntegerEvaluationParameterHandler))
        {
            var param = (IntegerEvaluationParameterHandler)assets[_i].ParameterHandlerObject;
            EditorGUILayout.LabelField("Min: " + param.variableSettings[0] +
                                       "; Max: " + param.variableSettings[1] +
                                       "; Step: " + param.variableSettings[2],
                                       smallLabelStyle);
        }
        else if (assets[_i].ParameterHandlerObject.GetType() == typeof(FloatEvaluationParameterHandler))
        {
            var param = (FloatEvaluationParameterHandler)assets[_i].ParameterHandlerObject;
            EditorGUILayout.LabelField("Min: " + param.variableSettings[0] +
                                       "; Max: " + param.variableSettings[1] +
                                       "; Step: " + param.variableSettings[2],
                                       smallLabelStyle);
        }
        else if (assets[_i].ParameterHandlerObject.GetType() == typeof(BoolEvaluationParameterHandler))
        {
            EditorGUILayout.LabelField("True / False", smallLabelStyle);
        }
        else if (assets[_i].ParameterHandlerObject.GetType() == typeof(IntegerArrayEvaluationParameterHandler))
        {
            var param = (IntegerArrayEvaluationParameterHandler)assets[_i].ParameterHandlerObject;
            EditorGUILayout.BeginVertical();
            for (int i = 0; i < param.variableSettings.Length; i++)
            {
                EditorGUILayout.LabelField("Min: " + param.variableSettings[i][0] +
                                           "; Max: " + param.variableSettings[i][1] +
                                           "; Step: " + param.variableSettings[i][2],
                                           smallLabelStyle);
            }
            EditorGUILayout.EndVertical();
        }
        else if (assets[_i].ParameterHandlerObject.GetType() == typeof(FloatArrayEvaluationParameterHandler))
        {
            var param = (FloatArrayEvaluationParameterHandler)assets[_i].ParameterHandlerObject;
            EditorGUILayout.BeginVertical();
            for (int i = 0; i < param.variableSettings.Length; i++)
            {
                EditorGUILayout.LabelField("Min: " + param.variableSettings[i][0] +
                                           "; Max: " + param.variableSettings[i][1] +
                                           "; Step: " + param.variableSettings[i][2],
                                           smallLabelStyle);
            }
            EditorGUILayout.EndVertical();
        }
    }

    private void CreateEvaluationParameterStorage()
    {
        string dir = baseFolder + scriptableObjectSubFolder;
        string parentDir = baseFolder;

        if (!AssetDatabase.IsValidFolder(dir))
            AssetDatabase.CreateFolder(parentDir, scriptableObjectSavedSubFolder);

        storageAsset = CreateInstance<EvaluationParameterStorage>();
        storageAsset.SetFrameworkBaseFolder(frameworkBaseFolder);
        storageAsset.SetEvaluationName(evaluationName);
        storageAsset.InitializeParameterArray(parameterList.ToArray());
        storageAsset.InitalizeScriptObjInstances(assets.ToArray());
        storageAsset.AddParameterArraysToComponents();

        AssetDatabase.CreateAsset(storageAsset, dir + "EvaluationParameterStorage.asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = storageAsset;
    }

    private void SaveParametersIntoJson()
    {
        storageAsset.SaveToJson(parameterList.ToArray(), "EvaluationParameterSet");
    }

    private void StartBuildProcess()
    {
        storageAsset.BuildExecutable();
        executableBuilt = true;
    }

    private void FinishSelectionProcess()
    {
        ResetFramework.DeleteAllAssets();
        ResetFramework.ResetManager();
        GetWindow(typeof(EvaluationParameterManager)).Close();
    }
}