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
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

public class ChooseParameterBase : ScriptableObject
{
    public UnityEvent SaveEvent;

    public bool settingsSaved = false;

    public Dictionary<string, int> arrayVariableSizes = new Dictionary<string, int>(); 

    // PROPERTIES
    public GameObject GameObjectOfInterest { get; set; }
    public string[] ComponentOptionStrings { get; set; }
    public Component[] ComponentOptions { get; set; }
    public string[] VariableOptionStrings { get; set; }
    public Type[] TypesOfVariableOptions { get; set; }
    public Component SelectedComponent { get; set; }
    public Type SelectedVariableType { get; set; }
    public string SelectedVariable { get; set; }
    public ParameterHandler ParameterHandlerObject { get; set; } 

    public virtual void OnEnable()
    {
        if (SaveEvent == null)
            SaveEvent = new UnityEvent();
    }

    private Component[] GetSelectedGameObjectScriptComponents(GameObject _evalGameObject)
    {
        return _evalGameObject.GetComponents<MonoBehaviour>();
    }

    public void HandleGameObjectComponents()
    {
        Component[] components = GetSelectedGameObjectScriptComponents(GameObjectOfInterest);
        int numOptions = components.Length;
        ComponentOptionStrings = new string[numOptions];
        ComponentOptions = new Component[numOptions];
        // fill component options
        for (int i = 0; i < numOptions; i++)
        {
            ComponentOptions[i] = components[i];
            ComponentOptionStrings[i] = components[i].GetType().ToString();
        }
    }

    public void HandleComponentProperties()
    {
        const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
        FieldInfo[] fields = SelectedComponent.GetType().GetFields(flags);
        PropertyInfo[] properties = SelectedComponent.GetType().GetProperties(flags);

        int numFields = fields.Length + properties.Length;
        VariableOptionStrings = new string[numFields];
        TypesOfVariableOptions = new Type[numFields];

        for (int i = 0; i < numFields; i++)
        {
            if (i < fields.Length)
            {
                TypesOfVariableOptions[i] = fields[i].FieldType;
                VariableOptionStrings[i] = fields[i].Name;
            }
            else
            {
                TypesOfVariableOptions[i] = properties[i - fields.Length].PropertyType;
                VariableOptionStrings[i] = properties[i - fields.Length].Name;
            }
        }
    }

    public void SaveSelectedVariableOptions()
    {
        settingsSaved = true;
        SaveEvent.Invoke();
    }
}

