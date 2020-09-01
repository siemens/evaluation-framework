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

using UnityEngine;

public class ROSParameterWriter
{
    protected GameObject targetGameObject;
    protected Component targetComponent;
    protected string targetVariableName;

    // base constructor
    public ROSParameterWriter(GameObject _gameObject,
                              Component _component,
                              string _variableName)
    {
        targetGameObject = _gameObject;
        targetComponent = _component;
        targetVariableName = _variableName;
    }

    public void AssignTargetValue(int _variableValue)
    {
        try
        {
            targetComponent.GetType().GetField(targetVariableName).SetValue(targetComponent, _variableValue);
        }
        catch (System.NullReferenceException)
        {
            targetComponent.GetType().GetProperty(targetVariableName).SetValue(targetComponent, _variableValue);
        }
    }

    public void AssignTargetValue(float _variableValue)
    {
        try
        {
            targetComponent.GetType().GetField(targetVariableName).SetValue(targetComponent, _variableValue);
        }
        catch (System.NullReferenceException)
        {
            targetComponent.GetType().GetProperty(targetVariableName).SetValue(targetComponent, _variableValue);
        }
    }

    public void AssignTargetValue(bool _variableValue)
    {
        try
        {
            targetComponent.GetType().GetField(targetVariableName).SetValue(targetComponent, _variableValue);
        }
        catch (System.NullReferenceException)
        {
            targetComponent.GetType().GetProperty(targetVariableName).SetValue(targetComponent, _variableValue);
        }
    }

    public void AssignTargetValue(int[] _variableValue)
    {
        try
        {
            targetComponent.GetType().GetField(targetVariableName).SetValue(targetComponent, _variableValue);
        }
        catch (System.NullReferenceException)
        {
            targetComponent.GetType().GetProperty(targetVariableName).SetValue(targetComponent, _variableValue);
        }
    }

    public void AssignTargetValue(float[] _variableValue)
    {
        try
        {
            targetComponent.GetType().GetField(targetVariableName).SetValue(targetComponent, _variableValue);
        }
        catch (System.NullReferenceException)
        {
            targetComponent.GetType().GetProperty(targetVariableName).SetValue(targetComponent, _variableValue);
        }
    }

    public void AssignTargetValue(int[][] _variableValue)
    {
        try
        {
            targetComponent.GetType().GetField(targetVariableName).SetValue(targetComponent, _variableValue);
        }
        catch (System.NullReferenceException)
        {
            targetComponent.GetType().GetProperty(targetVariableName).SetValue(targetComponent, _variableValue);
        }
    }

    public void AssignTargetValue(float[][] _variableValue)
    {
        try
        {
            targetComponent.GetType().GetField(targetVariableName).SetValue(targetComponent, _variableValue);
        }
        catch (System.NullReferenceException)
        {
            targetComponent.GetType().GetProperty(targetVariableName).SetValue(targetComponent, _variableValue);
        }
    }
}
