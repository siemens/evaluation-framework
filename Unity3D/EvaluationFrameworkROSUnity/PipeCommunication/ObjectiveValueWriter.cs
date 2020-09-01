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

public class ObjectiveValueWriter : MonoBehaviour
{
    [HideInInspector]
    public List<ParameterHandler> objectiveParameters =
        new List<ParameterHandler>();

    private List<ObjectiveValueReceiver> receivers =
        new List<ObjectiveValueReceiver>();

    private List<object> objectiveValues =
        new List<object>();

    private readonly string pipeName = "ObjectiveValues";
    private string fullPipeName;

    private void Awake()
    {
        CommandLineArgumentParser.ParseCommandLineArguments();
        CommandLineArgumentParser.SetTimeScale();
        fullPipeName = pipeName + CommandLineArgumentParser.pipeID;

        CreateObjectiveValueReceivers();
    }

    private void OnApplicationQuit()
    {
        RetrieveAllObjectiveValues();

        using (NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", fullPipeName, PipeDirection.Out))
        {
            pipeClient.Connect();

            if (pipeClient.IsConnected)
            {
                using (BinaryWriter binaryWriter = new BinaryWriter(pipeClient))
                {
                    for (int i = 0; i < receivers.Count; i++)
                    {
                        var objValType = objectiveParameters[i].selectedVariableTypeString;

                        // INT
                        if (objValType == typeof(int).ToString())
                            binaryWriter.Write((int)objectiveValues[i]);
                        // FLOAT
                        else if (objValType == typeof(float).ToString())
                            binaryWriter.Write((float)objectiveValues[i]);
                        // BOOL
                        else if (objValType == typeof(bool).ToString())
                            binaryWriter.Write((bool)objectiveValues[i]);
                        // INT ARRAY
                        else if (objValType == typeof(int[]).ToString())
                        {
                            int[] intArray = (int[])objectiveValues[i];
                            for (int j = 0; j < objectiveParameters[i].selectedVariableSize; j++)
                                binaryWriter.Write(intArray[j]);
                        }
                        // FLOAT ARRAY
                        else if (objValType == typeof(float[]).ToString())
                        {
                            float[] floatArray = (float[])objectiveValues[i];
                            for (int j = 0; j < objectiveParameters[i].selectedVariableSize; j++)
                                binaryWriter.Write(floatArray[j]);
                        }
                    }
                }
            }
        }
    }

    private void CreateObjectiveValueReceivers()
    {
        for (int i = 0; i < objectiveParameters.Count; i++)
        {
            var objVal = objectiveParameters[i];
            receivers.Add(
               new ObjectiveValueReceiver(objVal.selectedGameObject,
                                          objVal.selectedComponent,
                                          objVal.selectedVariable));
        }
    }

    private void RetrieveAllObjectiveValues()
    {
        foreach (var receiver in receivers)
            objectiveValues.Add(receiver.RetrieveObjectiveValue());
    }
}
