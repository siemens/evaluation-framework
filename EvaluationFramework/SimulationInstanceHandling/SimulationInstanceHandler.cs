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

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EvaluationFramework
{
    internal class SimulationInstanceHandler
    {
        private PipeServerWriter pipeServerWriter;
        private PipeServerReader pipeServerReader;
        private ExecutableProcessHandler executableProcessHandler;
        private int instanceID;

        public List<object> simulationInstanceObjective;
        private List<object> simulationInstanceFailure = new List<object> { };

        public SimulationInstanceHandler(int _instanceID)
        {
            instanceID = _instanceID;

            // create pipe servers
            pipeServerWriter = new PipeServerWriter(_instanceID);
            pipeServerReader = new PipeServerReader(_instanceID);
            executableProcessHandler = new ExecutableProcessHandler(_instanceID);

            for (int objectiveID = 0; objectiveID < EvaluationData.NumberOfObjectives; objectiveID++)
                simulationInstanceFailure.Add("NaN");
        }

        public void Execute(CancellationTokenSource _cancellationTokenSource, CancellationToken _cancellationToken)
        {
            // start pipe communcation to send evaluation parameters (asynchronous)
            Task writingEvaluationParametersTask = pipeServerWriter.WriteEvaluationParametersAsync(_cancellationToken);

            // start pipe communcation to read objective values (asynchronous)
            Task<List<object>> readingObjectiveValuesTask = pipeServerReader.ReadObjectiveValuesAsync(_cancellationToken);

            // get result of executable process
            var executableProcessResult = executableProcessHandler.LaunchExecutable();

            // wait for pipe communication tasks to finish
            Task.WaitAll(writingEvaluationParametersTask, readingObjectiveValuesTask);

            // if executable process was killed or not exited properly --> dispose pipe tasks
            if (executableProcessResult.ExitCode != 0)
            {
                _cancellationTokenSource.Cancel();
                EvaluationData.NumberOfCancelledExecutions += 1;
                Console.WriteLine("Execution {0} canceled!", instanceID);

                simulationInstanceObjective = simulationInstanceFailure;
            }
            else
            {
                _cancellationTokenSource.Dispose();
                EvaluationData.NumberOfSuccessfullExecutions += 1;
                Console.WriteLine("Execution {0} finished successfully!", instanceID);

                // get simulation instance objective
                simulationInstanceObjective = readingObjectiveValuesTask.Result;
            }

            EvaluationFrameworkMain.WriteExecutionInfo();
        }
    }
}
