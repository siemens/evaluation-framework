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

public class ROSParameterReader : MonoBehaviour
{
    [HideInInspector]
    public List<ParameterHandler> rosParameters =
        new List<ParameterHandler>();

    private List<ROSParameterWriter> writers =
        new List<ROSParameterWriter>();

    private List<object> rosParameterValues =
        new List<object>();

    private readonly string pipeName = "ROSParameters";
    private string fullPipeName;

    private void Awake()
    {
        CommandLineArgumentParser.ParseCommandLineArguments();

        if (!CommandLineArgumentParser.useROSPipe)
            return;

        fullPipeName = pipeName + CommandLineArgumentParser.pipeID;

        CreateROSParameterWriters();

        using (NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", fullPipeName, PipeDirection.In))
        {
            pipeClient.Connect();

            if (pipeClient.IsConnected)
            {
                using (BinaryReader binaryReader = new BinaryReader(pipeClient))
                {
                    int arrSizeIndex = 0;
                    for (int i = 0; i < writers.Count; i++)
                    {
                        var rosParamType = rosParameters[i].selectedVariableTypeString;

                        // INT
                        if (rosParamType == typeof(int).ToString())
                            rosParameterValues.Add(binaryReader.ReadInt32());
                        // FLOAT
                        else if (rosParamType == typeof(float).ToString())
                            rosParameterValues.Add(binaryReader.ReadSingle());
                        // BOOL
                        else if (rosParamType == typeof(bool).ToString())
                            rosParameterValues.Add(binaryReader.ReadBoolean());
                        // INT ARRAY
                        else if (rosParamType == typeof(int[]).ToString())
                        {
                            int[] intArrayValue = new int[CommandLineArgumentParser.arraySizes[arrSizeIndex]];
                            for (int j = 0; j < intArrayValue.Length; j++)
                                intArrayValue[j] = binaryReader.ReadInt32();

                            rosParameterValues.Add(intArrayValue);
                            arrSizeIndex++;
                        }
                        // FLOAT ARRAY
                        else if (rosParamType == typeof(float[]).ToString())
                        {
                            float[] floatArrayValue = new float[CommandLineArgumentParser.arraySizes[arrSizeIndex]];
                            for (int j = 0; j < floatArrayValue.Length; j++)
                                floatArrayValue[j] = binaryReader.ReadSingle();

                            rosParameterValues.Add(floatArrayValue);
                            arrSizeIndex++;
                        }
                        // JAGGED INT ARRAY
                        else if (rosParamType == typeof(int[][]).ToString())
                        {
                            int[][] jaggedIntArrayValue = new int[CommandLineArgumentParser.arraySizes[arrSizeIndex]][];
                            arrSizeIndex++;
                            for (int j = 0; j < jaggedIntArrayValue.Length; j++)
                            {
                                jaggedIntArrayValue[j] = new int[CommandLineArgumentParser.arraySizes[arrSizeIndex]];
                                for (int k = 0; k < jaggedIntArrayValue[j].Length; k++)
                                    jaggedIntArrayValue[j][k] = binaryReader.ReadInt32();
                            }

                            rosParameterValues.Add(jaggedIntArrayValue);
                            arrSizeIndex++;
                        }
                        // JAGGED FLOAT ARRAY
                        else if (rosParamType == typeof(float[][]).ToString())
                        {
                            float[][] jaggedFloatArrayValue = new float[CommandLineArgumentParser.arraySizes[arrSizeIndex]][];
                            arrSizeIndex++;
                            for (int j = 0; j < jaggedFloatArrayValue.Length; j++)
                            {
                                jaggedFloatArrayValue[j] = new float[CommandLineArgumentParser.arraySizes[arrSizeIndex]];
                                for (int k = 0; k < jaggedFloatArrayValue[j].Length; k++)
                                    jaggedFloatArrayValue[j][k] = binaryReader.ReadSingle();
                            }

                            rosParameterValues.Add(jaggedFloatArrayValue);
                            arrSizeIndex++;
                        }
                    }
                }
            }
        }

        AssignAllTargetVariables();
    }

    private void CreateROSParameterWriters()
    {
        for (int i = 0; i < rosParameters.Count; i++)
        {
            var rosParam = rosParameters[i];
            writers.Add(
               new ROSParameterWriter(rosParam.selectedGameObject,
                                      rosParam.selectedComponent,
                                      rosParam.selectedVariable));
        }
    }

    private void AssignAllTargetVariables()
    {
        for (int i = 0; i < writers.Count; i++)
        {
            var writer = writers[i];
            var evalParam = rosParameters[i];
            // input arguments come in the same order as assets are listed in evaluationParameters
            if (evalParam.selectedVariableTypeString == typeof(int).ToString())
            {
                writer.AssignTargetValue((int)rosParameterValues[i]);
            }
            else if (evalParam.selectedVariableTypeString == typeof(float).ToString())
            {
                writer.AssignTargetValue((float)rosParameterValues[i]);
            }
            else if (evalParam.selectedVariableTypeString == typeof(bool).ToString())
            {
                writer.AssignTargetValue((bool)rosParameterValues[i]);
            }
            else if (evalParam.selectedVariableTypeString == typeof(int[]).ToString())
            {
                writer.AssignTargetValue((int[])rosParameterValues[i]);
            }
            else if (evalParam.selectedVariableTypeString == typeof(float[]).ToString())
            {
                writer.AssignTargetValue((float[])rosParameterValues[i]);
            }
            else if (evalParam.selectedVariableTypeString == typeof(int[][]).ToString())
            {
                writer.AssignTargetValue((int[][])rosParameterValues[i]);
            }
            else if (evalParam.selectedVariableTypeString == typeof(float[][]).ToString())
            {
                writer.AssignTargetValue((float[][])rosParameterValues[i]);
            }
        }
    }
}
