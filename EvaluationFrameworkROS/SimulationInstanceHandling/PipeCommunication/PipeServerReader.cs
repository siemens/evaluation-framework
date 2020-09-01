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
using System.Threading.Tasks;

namespace EvaluationFrameworkROS
{
    internal class PipeServerReader
    {
        private string pipeName = "ObjectiveValues";
        private int instanceID;
        private List<DataTypes> parameterTypeOrder;

        public PipeServerReader(int _instanceID)
        {
            instanceID = _instanceID;
            pipeName += _instanceID;
            parameterTypeOrder = EvaluationData.EvaluationObjectiveTypeOrder;
        }

        public async Task<List<object>> ReadObjectiveValuesAsync(System.Threading.CancellationToken _cancellationToken)
        {
            List<object> objectiveValues = new List<object>();

            using (NamedPipeServerStream pipeServer = new NamedPipeServerStream(pipeName, PipeDirection.In))
            {
                // asynchronously wait for connection on pipe
                await pipeServer.WaitForConnectionAsync(_cancellationToken).ContinueWith(antecedent =>
                {
                    using (BinaryReader binaryReader = new BinaryReader(pipeServer))
                    {
                        for (int i = 0; i < parameterTypeOrder.Count; i++)
                        {
                            var dataType = parameterTypeOrder[i];
                            if (dataType == DataTypes.intValue)
                            {
                                objectiveValues.Add(binaryReader.ReadInt32());
                            }
                            else if (dataType == DataTypes.floatValue)
                            {
                                objectiveValues.Add(binaryReader.ReadSingle());
                            }
                            else if (dataType == DataTypes.boolValue)
                            {
                                objectiveValues.Add(binaryReader.ReadBoolean());
                            }
                            else if (dataType == DataTypes.intArray)
                            {
                                int arraySize = EvaluationData.EvaluationObjectives[i].selectedVariableSize;
                                int[] tempData = new int[arraySize];
                                for (int j = 0; j < arraySize; j++)
                                    tempData[j] = binaryReader.ReadInt32();
                                objectiveValues.Add(tempData);
                            }
                            else if (dataType == DataTypes.floatArray)
                            {
                                int arraySize = EvaluationData.EvaluationObjectives[i].selectedVariableSize;
                                float[] tempData = new float[arraySize];
                                for (int j = 0; j < arraySize; j++)
                                    tempData[j] = binaryReader.ReadSingle();
                                objectiveValues.Add(tempData);
                            }
                        }
                    }
                }, TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.ExecuteSynchronously);
            }
            return objectiveValues;
        }
    }
}
