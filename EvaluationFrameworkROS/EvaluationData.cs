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

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace EvaluationFrameworkROS
{
    internal static class EvaluationData
    {
        #region PublicProperties
        public static string PathToExecutable { get; set; }

        public static CultureInfo ParsingCultureInfo { get; set; }

        public static int NumberOfParameterCombinations { get; set; }
        public static int NumberOfObjectives { get; set; }
        public static int NumberOfROSParameters { get; set; }

        public static float NumberOfSuccessfullExecutions { get; set; }
        public static float NumberOfCancelledExecutions { get; set; }

        public static bool DemonstrationMode { get; set; }
        public static string UnityTimeScale { get; set; }
        public static List<object> DemonstrationEvaluationParameterSetting { get; set; }
        public static object[] DemonstrationROSParameterSetting { get; set; }
        public static int DemonstrationParameterID { get; set; }

        public static List<Parameter> EvaluationParameters { get; set; }
        public static List<DataTypes> EvaluationParameterTypeOrder { get; set; }
        public static List<List<object>> EvaluationParameterGrid { get; set; }

        public static List<Parameter> EvaluationObjectives { get; set; }
        public static List<DataTypes> EvaluationObjectiveTypeOrder { get; set; }
        public static object[][] EvaluationObjectiveValues { get; set; }

        public static bool UseROSCommunication { get; set; }
        public static List<Parameter> ROSParameters { get; set; }
        public static List<DataTypes> ROSParameterTyperOrder { get; set; }
        public static object[][] ROSParameterValues { get; set; }
        #endregion

        public static void ParseData(string _baseDirectory, string _pathToExecutable)
        {
            PathToExecutable = _pathToExecutable;
            ParsingCultureInfo = new CultureInfo("en-US", false);
            ParsingCultureInfo.NumberFormat.NumberDecimalSeparator = ".";

            string evalParamJson = "EvaluationParameterSet.json";
            EvaluationParameters = GetParameters(_baseDirectory, evalParamJson);
            EvaluationParameterTypeOrder = GetParameterTypeOrder(EvaluationParameters);
            EvaluationParameterGrid = GetEvaluationParameterGrid(EvaluationParameters);

            NumberOfSuccessfullExecutions = 0;
            NumberOfCancelledExecutions = 0;

            string objValJson = "ObjectiveValueSet.json";
            EvaluationObjectives = GetParameters(_baseDirectory, objValJson);
            EvaluationObjectiveTypeOrder = GetParameterTypeOrder(EvaluationObjectives);
            EvaluationObjectiveValues = new object[EvaluationParameterGrid.Count][];
            NumberOfObjectives = EvaluationObjectiveTypeOrder.Count;

            string rosParamJson = "ROSParameterSet.json";
            if (FileExists(_baseDirectory, rosParamJson))
            {
                UseROSCommunication = true;
                ROSParameters = GetParameters(_baseDirectory, rosParamJson);
                ROSParameterTyperOrder = GetParameterTypeOrder(ROSParameters);
                NumberOfROSParameters = ROSParameterTyperOrder.Count;
            }

            // set demonstration parameters
            if (DemonstrationMode)
            {
                NumberOfParameterCombinations = 1;
                DemonstrationEvaluationParameterSetting = EvaluationParameterGrid[DemonstrationParameterID];
            }
            // all possible combinations of evaluation parameters
            else
                NumberOfParameterCombinations = EvaluationParameterGrid.Count;

            if (UseROSCommunication)
                ROSParameterValues = new object[NumberOfParameterCombinations][];
        }

        #region DataParsingMethods
        private static List<Parameter> GetParameters(string _baseDirectory, string _fileName)
        {
            string parameterFileDirectory = GetParameterFileDirectory(_baseDirectory);
            List<Parameter> parameters;

            string file = Path.Combine(parameterFileDirectory, _fileName);
            parameters = JsonConvert.DeserializeObject<List<Parameter>>(File.ReadAllText(file));

            return parameters;
        }

        private static bool FileExists(string _baseDirectory, string _fileName)
        {
            return File.Exists(Path.Combine(GetParameterFileDirectory(_baseDirectory), _fileName));
        }

        private static string GetParameterFileDirectory(string _baseDirectory)
        {
            string parameterFileDirName = "ParameterFiles";
            return Path.Combine(_baseDirectory, parameterFileDirName);
        }

        private static List<DataTypes> GetParameterTypeOrder(List<Parameter> _parameters)
        {
            List<DataTypes> parameterTypeOrder = new List<DataTypes>();
            foreach (var param in _parameters)
            {
                var variableType = param.selectedVariableTypeString;
                if (variableType == typeof(int).ToString())
                {
                    parameterTypeOrder.Add(DataTypes.intValue);
                }
                else if (variableType == typeof(float).ToString())
                {
                    parameterTypeOrder.Add(DataTypes.floatValue);
                }
                else if (variableType == typeof(bool).ToString())
                {
                    parameterTypeOrder.Add(DataTypes.boolValue);
                }
                else if (variableType == typeof(int[]).ToString())
                {
                    parameterTypeOrder.Add(DataTypes.intArray);
                }
                else if (variableType == typeof(float[]).ToString())
                {
                    parameterTypeOrder.Add(DataTypes.floatArray);
                }
                else if (variableType == typeof(int[][]).ToString())
                {
                    parameterTypeOrder.Add(DataTypes.jaggedIntArray);
                }
                else if (variableType == typeof(float[][]).ToString())
                {
                    parameterTypeOrder.Add(DataTypes.jaggedFloatArray);
                }
            }
            return parameterTypeOrder;
        }

        private static List<List<object>> GetEvaluationParameterGrid(List<Parameter> _parameters)
        {
            List<object[]> evaluationParameterArray = new List<object[]>();

            foreach (var parameter in _parameters)
            {
                var variableType = parameter.selectedVariableTypeString;
                if (variableType == typeof(int).ToString())
                {
                    evaluationParameterArray.Add(ParseVariableSettingsAsString<int>(parameter, false, 0));
                }
                else if (variableType == typeof(float).ToString())
                {
                    evaluationParameterArray.Add(ParseVariableSettingsAsString<float>(parameter, false, 0));
                }
                else if (variableType == typeof(bool).ToString())
                {
                    evaluationParameterArray.Add(new string[] { "True", "False" });
                }
                else if (variableType == typeof(int[]).ToString())
                {
                    string[][] arrayVariableSettings = new string[parameter.selectedVariableSize][];
                    for (int i = 0; i < parameter.selectedVariableSize; i++)
                    {
                        arrayVariableSettings[i] = ParseVariableSettingsAsString<int>(parameter, true, i);
                    }
                    evaluationParameterArray.Add(arrayVariableSettings.CartesianProduct().ToArray());
                }
                else if (variableType == typeof(float[]).ToString())
                {
                    string[][] arrayVariableSettings = new string[parameter.selectedVariableSize][];
                    for (int i = 0; i < parameter.selectedVariableSize; i++)
                    {
                        arrayVariableSettings[i] = ParseVariableSettingsAsString<float>(parameter, true, i);
                    }
                    evaluationParameterArray.Add(arrayVariableSettings.CartesianProduct().ToArray());
                }
            }
            return evaluationParameterArray.CartesianProduct();
        }

        private static string[] ParseVariableSettingsAsString<T>(Parameter _parameter, bool _isArrayVariable, int _index)
        {
            EvaluationParameterSetting<T> parameterSetting;
            if (_isArrayVariable)
                parameterSetting = new EvaluationParameterSetting<T>(_parameter.variableSettings.ToObject<T[][]>());
            else
                parameterSetting = new EvaluationParameterSetting<T>(_parameter.variableSettings.ToObject<T[]>());

            if (parameterSetting.GetObjType.IsArray)
            {
                return RangeArray(parameterSetting.arrayValues[_index]).ToArray();
            }

            return RangeArray(parameterSetting.min,
                              parameterSetting.max,
                              parameterSetting.step).ToArray();
        }

        private static IEnumerable<string> RangeArray<T>(T[] _array)
        {
            return RangeArray(_array[0], _array[1], _array[2]);
        }

        private static IEnumerable<string> RangeArray<T>(T _min, T _max, T _step)
        {
            dynamic i;
            dynamic arrMin = _min;
            dynamic arrMax = _max;
            dynamic arrStep = _step;

            for (i = arrMin; i <= arrMax; i += arrStep)
                yield return i.ToString(ParsingCultureInfo);

            if (i != arrMax + arrStep) // added only because max should be returned as last item
                yield return arrMax.ToString(ParsingCultureInfo);
        }
        #endregion
    }
}
