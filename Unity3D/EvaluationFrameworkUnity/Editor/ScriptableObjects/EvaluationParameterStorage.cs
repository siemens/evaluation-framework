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

using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class EvaluationParameterStorage : ParameterStorageBase
{
    public ChooseEvaluationParameter[] scriptObjInstances;
    private EvaluationValueReader mainFrameworkComponent;

    public void InitalizeScriptObjInstances(ChooseEvaluationParameter[] _evalParamInstances)
    {
        scriptObjInstances = _evalParamInstances;
    }

    public void AddParameterArraysToComponents()
    {
        InitalizeGameObject();

        mainFrameworkComponent = mainFrameworkGameObject.GetComponent<EvaluationValueReader>();
        if (mainFrameworkComponent == null)
            mainFrameworkComponent = mainFrameworkGameObject.AddComponent<EvaluationValueReader>();

        mainFrameworkComponent.evaluationParameters.Clear();
        for (int i = 0; i < parameterHandlerArray.Length; i++)
        {
            mainFrameworkComponent.evaluationParameters.Add(parameterHandlerArray[i]);
        }
    }

    public void BuildExecutable()
    {
        string dir = Path.Combine(frameworkBaseFolder, evaluationsSubFolder, evaluationName, executableSubFolder);
        Directory.CreateDirectory(dir);

        PlayerSettings.runInBackground = true;
        PlayerSettings.usePlayerLog = false;
        PlayerSettings.SetApiCompatibilityLevel(BuildTargetGroup.Standalone, ApiCompatibilityLevel.NET_4_6);

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
        {
            scenes = new string[] { }, // empty: use currently open scene!
            locationPathName = dir + "WindowsBuild.exe", // --> if different name is chose, the other two framework components have to change the name too
            target = BuildTarget.StandaloneWindows/*,
            options = BuildOptions.Development | BuildOptions.AllowDebugging | BuildOptions.EnableHeadlessMode*/
        };

        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
        }

        if (summary.result == BuildResult.Failed)
        {
            Debug.LogError("Build failed");
        }
    }
}
