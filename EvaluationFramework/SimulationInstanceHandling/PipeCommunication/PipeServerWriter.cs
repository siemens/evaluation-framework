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

namespace EvaluationFramework
{
    internal class PipeServerWriter
    {
        private string pipeName = "EvaluationValues";
        private int instanceID;
        private List<object> parameterCombination;
        private List<DataTypes> parameterTypeOrder;

        public PipeServerWriter(int _instanceID)
        {
            instanceID = _instanceID;
            pipeName += _instanceID;

            if (EvaluationData.DemonstrationMode)
                parameterCombination = EvaluationData.DemonstrationParameterSetting;
            else 
                parameterCombination = EvaluationData.EvaluationParameterGrid[instanceID];

            parameterTypeOrder = EvaluationData.EvaluationParameterTypeOrder;
        }

        public async Task WriteEvaluationParametersAsync(System.Threading.CancellationToken _cancellationToken)
        {
            using (NamedPipeServerStream pipeServer = new NamedPipeServerStream(pipeName, PipeDirection.Out))
            {
                // asynchronously wait for connection on pipe
                await pipeServer.WaitForConnectionAsync(_cancellationToken).ContinueWith(antecedent =>
                {
                    using (BinaryWriter binaryWriter = new BinaryWriter(pipeServer))
                    {
                        for (int i = 0; i < parameterTypeOrder.Count; i++)
                        {
                            var dataType = parameterTypeOrder[i];
                            if (dataType == DataTypes.intValue)
                            {
                                binaryWriter.Write(int.Parse((string)parameterCombination[i], 
                                    EvaluationData.ParsingCultureInfo));
                            }
                            else if (dataType == DataTypes.floatValue)
                            {
                                binaryWriter.Write(float.Parse((string)parameterCombination[i], 
                                    EvaluationData.ParsingCultureInfo));
                            }
                            else if (dataType == DataTypes.boolValue)
                            {
                                binaryWriter.Write(bool.Parse((string)parameterCombination[i]));
                            }
                            if (dataType == DataTypes.intArray)
                            {
                                var paramComb = ((List<string>)parameterCombination[i]).ToArray();
                                foreach (var param in paramComb)
                                    binaryWriter.Write(int.Parse(param, EvaluationData.ParsingCultureInfo));
                            }
                            else if (dataType == DataTypes.floatArray)
                            {
                                var paramComb = ((List<string>)parameterCombination[i]).ToArray();
                                foreach (var param in paramComb)
                                    binaryWriter.Write(float.Parse(param, EvaluationData.ParsingCultureInfo));
                            }
                        }
                    }
                }, TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.ExecuteSynchronously);
            }
        }
    }
}