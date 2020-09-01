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

public class ObjectiveValueStorage : ParameterStorageBase
{
    public ChooseObjectiveValue[] scriptObjInstances;
    private ObjectiveValueWriter mainFrameworkComponent;

    public void InitalizeScriptObjInstances(ChooseObjectiveValue[] _objValueInstances)
    {
        scriptObjInstances = _objValueInstances;
    }

    public void AddParameterArraysToComponents()
    {
        InitalizeGameObject();

        mainFrameworkComponent = mainFrameworkGameObject.GetComponent<ObjectiveValueWriter>();
        if (mainFrameworkComponent == null)
            mainFrameworkComponent = mainFrameworkGameObject.AddComponent<ObjectiveValueWriter>();

        mainFrameworkComponent.objectiveParameters.Clear();
        for (int i = 0; i < parameterHandlerArray.Length; i++)
        {
            mainFrameworkComponent.objectiveParameters.Add(parameterHandlerArray[i]);
        }
    }
}
