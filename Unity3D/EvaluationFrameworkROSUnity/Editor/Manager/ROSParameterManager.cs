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

using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class ROSParameterManager : EditorWindow
{
    private readonly string finishedChoosingROSParameters = "Done Choosing ROS Parameters";
    private readonly string removeParameter = "Remove Parameter";

    private readonly string baseFolder = "Assets/EvaluationFrameworkROSUnity/";
    private readonly string scriptableObjectSubFolder = "ROSParameterStorage/";
    private readonly string scriptableObjectSavedSubFolder = "ScriptableObjects/";

    public static List<ChooseROSParameter> assets = new List<ChooseROSParameter>();
    private ROSParameterStorage storageAsset;

    private Vector2 scrollPos;

    private GUIStyle largeLabelStyle = new GUIStyle();
    private GUIStyle smallLabelStyle = new GUIStyle();
    private GUIStyle variableLabelStyle = new GUIStyle();

    // static variables
    private static ChooseROSParameter currentAsset;
    public static List<string> objStrings = new List<string>();
    public static List<ParameterHandler> parameterList = new List<ParameterHandler>();
    public static bool rosParametersSaved = false;

    public static void ShowROSParameterManagerWindow()
    {
        // Get existing open window or if none, make a new one: (docked to EvaluationParameterManager Window)
        ROSParameterManager window = GetWindow<ROSParameterManager>(new Type[] { typeof(EvaluationParameterManager) });
        window.Show();
    }


    // Event Callback for saving variables
    public static void SaveROSParametersEventHandler()
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
        variableLabelStyle.fontSize = 11;
        variableLabelStyle.fixedWidth = 1;

        bool ableToAddParam = objStrings.Count == 0 ||
            (objStrings.Count > 0 && objStrings[objStrings.Count - 1] != "Empty");

        EditorGUILayout.LabelField("Evaluation Framework: ROS Parameter Manager", largeLabelStyle);

        EditorGUILayout.Space();

        scrollPos =
            EditorGUILayout.BeginScrollView(scrollPos);

        EditorGUI.BeginDisabledGroup(!ableToAddParam);
        if (GUILayout.Button(ableToAddParam ? "Add ROS Parameter Value" : "Fill Object First"))
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
            EditorGUILayout.LabelField(string.Format("{0}. ROS Parameter", i + 1),
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
                EditorGUILayout.LabelField("ROS Parameter Information", largeLabelStyle);

                EditorGUI.indentLevel++;
                EditorGUILayout.LabelField("GameObject: " + parameterList[i].selectedGameObject,
                    smallLabelStyle);
                EditorGUILayout.LabelField("Component: " + parameterList[i].selectedComponent.GetType(),
                    smallLabelStyle);
                EditorGUILayout.LabelField("Variable: " + parameterList[i].selectedVariable,
                    smallLabelStyle);

                EditorGUI.indentLevel = 0;

                EditorGUILayout.Space();
            }
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        if (objStrings.Count != 0 && objStrings[objStrings.Count - 1] != "Empty")
        {
            if (GUILayout.Button(finishedChoosingROSParameters))
            {
                rosParametersSaved = true;
                CreateROSParameterStorage();
                SaveParametersIntoJson();
                ObjectiveValueManager.ShowObjectiveValueManagerWindow();
                GetWindow(typeof(ROSParameterManager)).Close();
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

        currentAsset = CreateInstance<ChooseROSParameter>();
        assets.Add(currentAsset);

        AssetDatabase.CreateAsset(currentAsset, dir + "ParameterAsset.asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = currentAsset;
    }

    private void CreateROSParameterStorage()
    {
        string dir = baseFolder + scriptableObjectSubFolder;
        string parentDir = baseFolder;

        if (!AssetDatabase.IsValidFolder(dir))
            AssetDatabase.CreateFolder(parentDir, scriptableObjectSavedSubFolder);

        storageAsset = CreateInstance<ROSParameterStorage>();
        storageAsset.InitializeParameterArray(parameterList.ToArray());
        storageAsset.InitalizeScriptObjInstances(assets.ToArray());
        storageAsset.AddParameterArraysToComponents();

        AssetDatabase.CreateAsset(storageAsset, dir + "ROSParameterStorage.asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = storageAsset;
    }

    private void SaveParametersIntoJson()
    {
        storageAsset.SaveToJson(parameterList.ToArray(), "ROSParameterSet");
    }
}
