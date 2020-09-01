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
using System;
using UnityEngine;

[Serializable]
public class ParameterHandler
{
    [JsonIgnore] public GameObject selectedGameObject;
    public string selectedGameObjectName;
    [JsonIgnore] public Component selectedComponent;
    public string selectedComponentName;
    public string selectedVariable;
    [JsonIgnore] public Type selectedVariableType;
    public string selectedVariableTypeString;
    public int selectedVariableSize;
    public string selectedVariableUnit;

    // base constructor
    public ParameterHandler(GameObject _gameObject, Component _component,
        string _variable, Type _type, int _variableSize, string _unit = "")
    {
        selectedGameObject = _gameObject;
        selectedGameObjectName = _gameObject.name;
        selectedComponent = _component;
        selectedComponentName = _component.GetType().ToString();
        selectedVariable = _variable;
        selectedVariableType = _type;
        selectedVariableTypeString = _type.ToString();
        selectedVariableSize = _variableSize;
        selectedVariableUnit = _unit;
    }
}

public class IntegerEvaluationParameterHandler : ParameterHandler
{
    public int[] variableSettings = new int[3];

    public IntegerEvaluationParameterHandler(GameObject _gameObject, Component _component,
        string _variable, Type _type, int[] _intSettings, int _variableSize, string _unit = "")
        : base(_gameObject, _component, _variable, _type, _variableSize, _unit)
    {
        variableSettings = _intSettings;
    }
}

public class IntegerArrayEvaluationParameterHandler : ParameterHandler
{
    public int[][] variableSettings;

    public IntegerArrayEvaluationParameterHandler(GameObject _gameObject, Component _component,
        string _variable, Type _type, int[][] _intSettings, int _variableSize, string _unit = "")
        : base(_gameObject, _component, _variable, _type, _variableSize, _unit)
    {
        variableSettings = _intSettings;
    }
}

public class FloatEvaluationParameterHandler : ParameterHandler
{
    public float[] variableSettings = new float[3];

    public FloatEvaluationParameterHandler(GameObject _gameObject, Component _component,
        string _variable, Type _type, float[] _floatSettings, int _variableSize, string _unit = "")
        : base(_gameObject, _component, _variable, _type, _variableSize, _unit)
    {
        variableSettings = _floatSettings;
    }
}

public class FloatArrayEvaluationParameterHandler : ParameterHandler
{
    public float[][] variableSettings;

    public FloatArrayEvaluationParameterHandler(GameObject _gameObject, Component _component,
        string _variable, Type _type, float[][] _floatSettings, int _variableSize, string _unit = "")
        : base(_gameObject, _component, _variable, _type, _variableSize, _unit)
    {
        variableSettings = _floatSettings;
    }
}

public class BoolEvaluationParameterHandler : ParameterHandler
{
    public BoolEvaluationParameterHandler(GameObject _gameObject, Component _component,
        string _variable, Type _type, int _variableSize, string _unit = "")
        : base(_gameObject, _component, _variable, _type, _variableSize, _unit)
    {
    }
}

public class ObjectiveValueHandler : ParameterHandler
{
    // base constructor
    public ObjectiveValueHandler(GameObject _gameObject, Component _component,
        string _variable, Type _type, int _variableSize, string _unit = "")
        : base(_gameObject, _component, _variable, _type, _variableSize, _unit)
    {
    }
}

public class ROSParameterHandler : ParameterHandler
{
    public ROSParameterHandler(GameObject _gameObject, Component _component,
        string _variable, Type _type)
        : base(_gameObject, _component, _variable, _type, 0, "")
    {
    }
}
