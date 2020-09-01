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

using UnityEditor;

public class ResetFramework : EditorWindow
{
    // Add menu
    [MenuItem("Evaluation Framework/Reset")]
    private static void Reset()
    {
        DeleteAllAssets();
        ResetManager();
    }

    public static void DeleteAllAssets()
    {
        string[] folders = new string[] { "Assets/EvaluationFrameworkUnity/EvaluationParameterStorage",
                                          "Assets/EvaluationFrameworkUnity/ObjectiveValueStorage" };
        // search for all ScriptObject called ScriptObj
        string[] guids = AssetDatabase.FindAssets("t:ScriptableObject", folders);
        foreach (string guid in guids)
        {
            AssetDatabase.DeleteAsset(AssetDatabase.GUIDToAssetPath(guid));
        }
    }

    public static void ResetManager()
    {
        EvaluationParameterManager.assets.Clear();
        EvaluationParameterManager.objStrings.Clear();
        EvaluationParameterManager.parameterList.Clear();
        EvaluationParameterManager.parametersSaved = false;

        ObjectiveValueManager.assets.Clear();
        ObjectiveValueManager.objStrings.Clear();
        ObjectiveValueManager.parameterList.Clear();
        ObjectiveValueManager.objectiveValuesSaved = false;

        ObjectiveValueManager window = GetWindow<ObjectiveValueManager>();
        window.Close();
    }
}
