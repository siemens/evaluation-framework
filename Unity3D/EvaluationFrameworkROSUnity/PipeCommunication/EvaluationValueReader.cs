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
using System.IO;
using System.IO.Pipes;
using UnityEngine;

public class EvaluationValueReader : MonoBehaviour
{
    [HideInInspector]
    public List<ParameterHandler> evaluationParameters =
        new List<ParameterHandler>();

    private List<EvaluationValueWriter> writers =
        new List<EvaluationValueWriter>();

    private List<object> evaluationValues =
        new List<object>();

    private readonly string pipeName = "EvaluationValues";
    private string fullPipeName;

    private void Awake()
    {
        CommandLineArgumentParser.ParseCommandLineArguments();
        fullPipeName = pipeName + CommandLineArgumentParser.pipeID;

        CreateEvaluationValueWriters();

        using (NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", fullPipeName, PipeDirection.In))
        {
            pipeClient.Connect();

            if (pipeClient.IsConnected)
            {
                using (BinaryReader binaryReader = new BinaryReader(pipeClient))
                {
                    for (int i = 0; i < writers.Count; i++)
                    {
                        var evalParamType = evaluationParameters[i].selectedVariableTypeString;

                        // INT
                        if (evalParamType == typeof(int).ToString())
                            evaluationValues.Add(binaryReader.ReadInt32());
                        // FLOAT
                        else if (evalParamType == typeof(float).ToString())
                            evaluationValues.Add(binaryReader.ReadSingle());
                        // BOOL
                        else if (evalParamType == typeof(bool).ToString())
                            evaluationValues.Add(binaryReader.ReadBoolean());
                        // INT ARRAY
                        else if (evalParamType == typeof(int[]).ToString())
                        {
                            int[] intArrayValue = new int[evaluationParameters[i].selectedVariableSize];
                            for (int j = 0; j < intArrayValue.Length; j++)
                                intArrayValue[j] = binaryReader.ReadInt32();

                            evaluationValues.Add(intArrayValue);
                        }
                        // FLOAT ARRAY
                        else if (evalParamType == typeof(float[]).ToString())
                        {
                            float[] floatArrayValue = new float[evaluationParameters[i].selectedVariableSize];
                            for (int j = 0; j < floatArrayValue.Length; j++)
                                floatArrayValue[j] = binaryReader.ReadSingle();

                            evaluationValues.Add(floatArrayValue);
                        }
                    }
                }
            }
        }

        AssignAllTargetVariables();
    }

    private void CreateEvaluationValueWriters()
    {
        for (int i = 0; i < evaluationParameters.Count; i++)
        {
            var evalParam = evaluationParameters[i];
            writers.Add(
               new EvaluationValueWriter(evalParam.selectedGameObject,
                                         evalParam.selectedComponent,
                                         evalParam.selectedVariable));
        }
    }

    private void AssignAllTargetVariables()
    {
        for (int i = 0; i < writers.Count; i++)
        {
            var writer = writers[i];
            var evalParam = evaluationParameters[i];
            // input arguments come in the same order as assets are listed in evaluationParameters
            if (evalParam.selectedVariableTypeString == typeof(int).ToString())
            {
                writer.AssignTargetValue((int)evaluationValues[i]);
            }
            else if (evalParam.selectedVariableTypeString == typeof(float).ToString())
            {
                writer.AssignTargetValue((float)evaluationValues[i]);
            }
            else if (evalParam.selectedVariableTypeString == typeof(bool).ToString())
            {
                writer.AssignTargetValue((bool)evaluationValues[i]);
            }
            else if (evalParam.selectedVariableTypeString == typeof(int[]).ToString())
            {
                writer.AssignTargetValue((int[])evaluationValues[i]);
            }
            else if (evalParam.selectedVariableTypeString == typeof(float[]).ToString())
            {
                writer.AssignTargetValue((float[])evaluationValues[i]);
            }
        }
    }
}
