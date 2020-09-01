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
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EvaluationFramework
{
    public class EvaluationFrameworkMain
    {
        private static string evaluationName;
        private static string executableFile;
        private static string baseDirectory;
        private static SimulationInstanceHandler[] instanceHandlerArray;
        private static Task[] taskArray;

        private static int totalNumberOfRows;
        private static int numberOfRows;
        private static int lowestRowOnWindow;

        public static void Main(string[] _args)
        {
            // Clear the screen.
            Console.Clear();
            numberOfRows = Console.WindowHeight;
            totalNumberOfRows = Console.BufferHeight;
            lowestRowOnWindow = numberOfRows - 1;

            ParseCommandLineArguments(_args);

            GetPathToExecutable();
            EvaluationData.ParseData(baseDirectory, executableFile);

            taskArray = new Task[EvaluationData.NumberOfParameterCombinations];

            CreateSimulationInstanceHandlers();

            WriteExecutionInfo();

            // default min threads corresponds to number of cores of machine
            // best idea to run at max this number of tasks in parallel
            int minThreads, completionPortThreads;
            ThreadPool.GetMinThreads(out minThreads, out completionPortThreads);

            var cancellationOnFailureTokenSource = new CancellationTokenSource();
            var failureToken = cancellationOnFailureTokenSource.Token;

            for (int instanceID = 0; instanceID < EvaluationData.NumberOfParameterCombinations; instanceID++)
            {
                var simulationInstanceHandler = instanceHandlerArray[instanceID];

                // execute
                taskArray[instanceID] = new Task(() => simulationInstanceHandler.Execute(cancellationOnFailureTokenSource,
                                                                                         failureToken));
            }

            TaskExtensions.StartAndWaitAllThrottled(taskArray, minThreads);

            if (!EvaluationData.DemonstrationMode)
            {
                SaveSimulationInstanceObjectives();

                // Write to textfile
                SaveSimulationObjectiveToFile(EvaluationData.NumberOfParameterCombinations);
            }
        }

        public static void WriteExecutionInfo()
        {
            WriteAt0(string.Format("#### Finished {0:0.##}% of all Executions... {1} Executions Finished, {2} Executions Cancelled, {3} Total Executions! ####",
                100 * (EvaluationData.NumberOfCancelledExecutions + EvaluationData.NumberOfSuccessfullExecutions) / EvaluationData.NumberOfParameterCombinations,
                EvaluationData.NumberOfSuccessfullExecutions, EvaluationData.NumberOfCancelledExecutions, EvaluationData.NumberOfParameterCombinations));
        }

        public static void WriteAt0(string s)
        {
            numberOfRows = Console.WindowHeight;
            totalNumberOfRows = Console.BufferHeight;
            int currentRow = Console.CursorTop;

            Console.SetCursorPosition(0, lowestRowOnWindow);
            Console.Write(new string(' ', Console.WindowWidth));

            if (currentRow >= numberOfRows - 2)
                lowestRowOnWindow = currentRow + 1;

            if (lowestRowOnWindow >= totalNumberOfRows - 1)
            {
                lowestRowOnWindow = totalNumberOfRows - 1;
                currentRow = lowestRowOnWindow - 2;
            }

            try
            {
                Console.SetCursorPosition(0, lowestRowOnWindow);
                Console.Write(s);
                Console.SetCursorPosition(0, currentRow);
            }
            catch (ArgumentOutOfRangeException e)
            {
                Console.Clear();
                Console.WriteLine(e.Message);
            }
        }

        private static void GetPathToExecutable()
        {
            baseDirectory = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\", "Evaluations", evaluationName));

            executableFile = Path.Combine(baseDirectory, "Executables", "WindowsBuild.exe");
        }

        private static void ParseCommandLineArguments(string[] _args)
        {
            bool gotEvaluationName = false;
            bool gotTimeScale = false;
            bool gotParameterID = false;

            EvaluationData.DemonstrationMode = false;

            for (int i = 0; i < _args.Length; i++)
            {
                string arg = _args[i];
                if (arg == "--demonstration")
                {
                    EvaluationData.DemonstrationMode = true;
                    // only one simulation is to be run
                    EvaluationData.NumberOfParameterCombinations = 1;
                }
                else if (arg == "-evaluationName")
                {
                    gotEvaluationName = true;
                    evaluationName = _args[i + 1];
                    i++;
                }
                else if (arg == "-timeScale")
                {
                    gotTimeScale = true;
                    EvaluationData.UnityTimeScale = _args[i + 1];
                    i++;
                }
                else if (arg == "-parameterID")
                {
                    gotParameterID = true;
                    EvaluationData.DemonstrationParameterID = int.Parse(_args[i + 1], EvaluationData.ParsingCultureInfo);
                    i++;
                }
            }

            if (!gotEvaluationName || (EvaluationData.DemonstrationMode && !(gotTimeScale && gotParameterID)) 
                                   || (!EvaluationData.DemonstrationMode && gotTimeScale && gotParameterID))
            {
                Console.WriteLine("Commandline Usage of the Evaluation Framework's Execution and Evaluation Component:");
                Console.WriteLine("\t-evaluationName NameOfEvaluationToBeSimulated [mandatory] --> name of folder where evaluation space files are stored.");
                Console.WriteLine("\t--demonstration [optional] --> show a Unity simulation of a specific parameter combination.");
                Console.WriteLine("\t\t-parameterID [mandatory if --demonstration] --> Integer ID of the parameter combination to demonstrate. Its ID is its index in the grid of evaluation parameters.");
                Console.WriteLine("\t\t-timeScale [mandatory if --demonstration] --> Float value for the Unity time scale at which the demonstration simulation is executed.");
                Environment.Exit(-1);
            }
        }

        private static void CreateSimulationInstanceHandlers()
        {
            Console.WriteLine("Creating SimulationInstanceHandlers...");
            instanceHandlerArray = new SimulationInstanceHandler[EvaluationData.NumberOfParameterCombinations];

            for (int instanceID = 0; instanceID < EvaluationData.NumberOfParameterCombinations; instanceID++)
            {
                instanceHandlerArray[instanceID] = new SimulationInstanceHandler(instanceID);
            }
        }

        private static void SaveSimulationInstanceObjectives()
        {
            // save objective values of all SimulationInstanceHandlers
            for (int instanceID = 0; instanceID < EvaluationData.NumberOfParameterCombinations; instanceID++)
            {
                var simulationInstanceObjective = instanceHandlerArray[instanceID].simulationInstanceObjective;
                EvaluationData.EvaluationObjectiveValues[instanceID] = simulationInstanceObjective.ToArray();
            }
        }

        private static void SaveSimulationObjectiveToFile(int _numParamCombinations)
        {
            Thread.CurrentThread.CurrentCulture = EvaluationData.ParsingCultureInfo;

            Directory.CreateDirectory(Path.Combine(baseDirectory, "Results"));
            string fileName = Path.Combine(baseDirectory, "Results", "Results.txt");
            // write first line with parameter names
            string[] evaluationParameterNames = EvaluationData.EvaluationParameters.Select(
                param => string.Format(param.selectedVariable + " [{0}]", param.selectedVariableUnit)).ToArray();
            string[] evaluationObjectiveNames = EvaluationData.EvaluationObjectives.Select(
                param => string.Format(param.selectedVariable + " [{0}]", param.selectedVariableUnit)).ToArray();

            int numOfParameters = EvaluationData.EvaluationParameters.Count;
            int numOfObjectives = EvaluationData.NumberOfObjectives;

            using (StreamWriter outputFile = new StreamWriter(fileName))
            {
                outputFile.Write(string.Join(";", evaluationParameterNames));
                outputFile.Write("|");
                outputFile.Write(string.Join(";", evaluationObjectiveNames));
                outputFile.Write(outputFile.NewLine);


                for (int instanceID = 0; instanceID < _numParamCombinations; instanceID++)
                {
                    string line = "";
                    for (int i = 0; i < numOfParameters; i++)
                    {
                        var param = EvaluationData.EvaluationParameterGrid[instanceID][i];
                        if (param is IList<string>)
                        {
                            var paramArray = ((List<string>)param).ToArray();
                            line += string.Join(",", paramArray);
                        }
                        else
                            line += param;

                        if (i != numOfParameters - 1)
                            line += ";";
                    }

                    outputFile.Write(line);
                    outputFile.Write("|");

                    line = "";
                    for (int i = 0; i < numOfObjectives; i++)
                    {
                        var param = EvaluationData.EvaluationObjectiveValues[instanceID][i];
                        if (param is IList<int>)
                        {
                            var paramArray = (int[])param;
                            line += string.Join(",", paramArray);
                        }
                        else if (param is IList<float>)
                        {
                            var paramArray = (float[])param;
                            line += string.Join(",", paramArray);
                        }
                        else
                            line += param;

                        if (i != numOfObjectives - 1)
                            line += ";";
                    }

                    outputFile.Write(line);
                    outputFile.Write(outputFile.NewLine);
                }
            }
        }
    }
}
