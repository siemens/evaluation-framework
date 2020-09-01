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
    internal class ROSParameterPipeServerWriter
    {
        private string pipeName = "ROSParameters";
        private int instanceID;
        private object[] rosParameters;
        private List<DataTypes> parameterTypeOrder;

        public ROSParameterPipeServerWriter(int _instanceID)
        {
            instanceID = _instanceID;
            pipeName += _instanceID;

            if (EvaluationData.DemonstrationMode)
                rosParameters = EvaluationData.DemonstrationROSParameterSetting;
            else
                rosParameters = EvaluationData.ROSParameterValues[instanceID];

            parameterTypeOrder = EvaluationData.ROSParameterTyperOrder;
        }

        public async Task WriteROSParametersAsync(System.Threading.CancellationToken _cancellationToken)
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
                                binaryWriter.Write((int)rosParameters[i]);
                            }
                            else if (dataType == DataTypes.floatValue)
                            {
                                binaryWriter.Write((float)rosParameters[i]);
                            }
                            else if (dataType == DataTypes.boolValue)
                            {
                                binaryWriter.Write((bool)rosParameters[i]);
                            }
                            if (dataType == DataTypes.intArray)
                            {
                                var rosParamIntArray = (int[])rosParameters[i];
                                foreach (var intParam in rosParamIntArray)
                                    binaryWriter.Write(intParam);
                            }
                            else if (dataType == DataTypes.floatArray)
                            {
                                var rosParamFloatArray = (float[])rosParameters[i];
                                foreach (var floatParam in rosParamFloatArray)
                                    binaryWriter.Write(floatParam);
                            }
                            else if (dataType == DataTypes.jaggedIntArray)
                            {
                                var rosParamJaggedIntArray = (int[][])rosParameters[i];
                                foreach (var paramIntArray in rosParamJaggedIntArray)
                                    foreach (var intParam in paramIntArray)
                                        binaryWriter.Write(intParam);
                            }
                            else if (dataType == DataTypes.jaggedFloatArray)
                            {
                                var rosParamJaggedFloatArray = (float[][])rosParameters[i];
                                foreach (var paramFloatArray in rosParamJaggedFloatArray)
                                    foreach (var floatParam in paramFloatArray)
                                        binaryWriter.Write(floatParam);
                            }
                        }
                    }
                }, TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.ExecuteSynchronously);
            }
        }
    }
}
