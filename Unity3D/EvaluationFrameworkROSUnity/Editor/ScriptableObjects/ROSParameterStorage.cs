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

public class ROSParameterStorage : ParameterStorageBase
{
    public ChooseROSParameter[] scriptObjInstances;
    private ROSParameterReader mainFrameworkComponent;

    public void InitalizeScriptObjInstances(ChooseROSParameter[] _rosParameterInstances)
    {
        scriptObjInstances = _rosParameterInstances;
    }

    public void AddParameterArraysToComponents()
    {
        InitalizeGameObject();

        mainFrameworkComponent = mainFrameworkGameObject.GetComponent<ROSParameterReader>();
        if (mainFrameworkComponent == null)
            mainFrameworkComponent = mainFrameworkGameObject.AddComponent<ROSParameterReader>();

        mainFrameworkComponent.rosParameters.Clear();
        for (int i = 0; i < parameterHandlerArray.Length; i++)
        {
            mainFrameworkComponent.rosParameters.Add(parameterHandlerArray[i]);
        }
    }
}
