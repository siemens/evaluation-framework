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

using System.Diagnostics;

namespace EvaluationFrameworkROS
{
    /*
     * Authors: Alexander Mezhov, Georg Jung
     * Changes: The code was adapted to work with the given command line arguments and simplified for this application
     * Source: https://gist.github.com/AlexMAS/276eed492bc989e13dcce7c78b9e179d,
     *         https://gist.github.com/georg-jung/3a8703946075d56423e418ea76212745#file-processasynchelper-cs
     */
    internal class ExecutableProcessHandler
    {
        private int instanceID;
        // timeout waiting for executable process to exit before killing it [ms]
        private int executableTimeout = 60000; // 1 minunte per executable

        public ExecutableProcessHandler(int _instanceID)
        {
            instanceID = _instanceID;
        }

        public ProcessResult LaunchExecutable()
        {
            var result = new ProcessResult();

            using (Process exeProcess = new Process())
            {
                string processIDCommandLineArgument = "-pipeID";
                string timeScaleCommandLineArgument = "-timeScale";

                exeProcess.StartInfo.FileName = EvaluationData.PathToExecutable;
                exeProcess.StartInfo.Arguments += processIDCommandLineArgument + " " + instanceID;
                if (EvaluationData.UseROSCommunication)
                {
                    exeProcess.StartInfo.Arguments += " -useROS";
                    for (int paramID = 0; paramID < EvaluationData.NumberOfROSParameters; paramID++)
                    {
                        var rosParamType = EvaluationData.ROSParameterTyperOrder[paramID];
                        if (rosParamType == DataTypes.intArray || rosParamType == DataTypes.floatArray)
                        {
                            var rosParam = (float[])EvaluationData.ROSParameterValues[instanceID][paramID];
                            exeProcess.StartInfo.Arguments += " -rosParam ";
                            exeProcess.StartInfo.Arguments += rosParam.Length;
                        }
                        else if (rosParamType == DataTypes.jaggedIntArray || rosParamType == DataTypes.jaggedFloatArray)
                        {
                            var rosParam = (float[][])EvaluationData.ROSParameterValues[instanceID][paramID];
                            exeProcess.StartInfo.Arguments += " -rosParam ";
                            exeProcess.StartInfo.Arguments += rosParam.Length;
                            exeProcess.StartInfo.Arguments += ",";
                            exeProcess.StartInfo.Arguments += rosParam[0].Length;
                        }
                    }
                }
                exeProcess.StartInfo.Arguments += " -nolog";
                if (!EvaluationData.DemonstrationMode)
                    exeProcess.StartInfo.Arguments += " -batchmode -nographics";
                else
                    exeProcess.StartInfo.Arguments += " " + timeScaleCommandLineArgument + " " + EvaluationData.UnityTimeScale;

                exeProcess.StartInfo.UseShellExecute = true;
                exeProcess.StartInfo.RedirectStandardOutput = false;
                exeProcess.StartInfo.RedirectStandardError = false;


                bool isStarted = exeProcess.Start();
                if (!isStarted)
                {
                    result.ExitCode = exeProcess.ExitCode;
                    return result;
                }

                if (!exeProcess.WaitForExit(executableTimeout))
                {
                    try
                    {
                        // Kill hung process
                        exeProcess.Kill();
                    }
                    catch
                    {
                        // ignored
                    }
                }

                result.ExitCode = exeProcess.ExitCode;

                return result;
            }
        }

        public struct ProcessResult
        {
            public int? ExitCode;
        }
    }
}
