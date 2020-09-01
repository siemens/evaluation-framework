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
using Newtonsoft.Json;
using System.IO;

public class ParameterStorageBase : ScriptableObject
{
    protected GameObject mainFrameworkGameObject;
    public ParameterHandler[] parameterHandlerArray;

    protected static string frameworkBaseFolder;
    protected static string evaluationName;
    protected static readonly string evaluationsSubFolder = "Evaluations/";
    protected static readonly string parameterFilesSubFolder = "ParameterFiles/";
    protected static readonly string executableSubFolder = "Executables/";

    public void InitalizeGameObject()
    {
        mainFrameworkGameObject = GameObject.Find("EvaluationFramework");
    }

    public void InitializeParameterArray(ParameterHandler[] _paramArray)
    {
        parameterHandlerArray = _paramArray;
    }

    public void SetFrameworkBaseFolder(string _folderName)
    {
        frameworkBaseFolder = _folderName;
    }

    public void SetEvaluationName(string _evaluationName)
    {
        evaluationName = _evaluationName;
        string dir = Path.Combine(frameworkBaseFolder, evaluationsSubFolder, evaluationName);
        Directory.CreateDirectory(dir);
    }

    public void SaveToJson(ParameterHandler[] _parameterArray, string _fileName)
    {
        string dir = Path.Combine(frameworkBaseFolder, evaluationsSubFolder, evaluationName, parameterFilesSubFolder);

        Directory.CreateDirectory(dir);
        string json = JsonConvert.SerializeObject(_parameterArray, Formatting.Indented);
        File.WriteAllText(dir + "/" + _fileName + ".json", json);
    }
}