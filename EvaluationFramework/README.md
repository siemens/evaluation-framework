[EvaluationFramework.sln](https://github.com/siemens/evaluation-framework/blob/master/EvaluationFramework/EvaluationFramework.sln) is the .NET solution of the [Evaluation Framework](https://github.com/siemens/evaluation-framework)

## Usage ##

* Navigate to the folder [EvaluationFramework](https://github.com/siemens/evaluation-framework/tree/master/EvaluationFramework)
* From the command line, execute: `bin\[Debug/Release]\netcoreapp3.1\EvaluationFramework.exe`
* Command line options: 
  * `-evaluationName NameOfEvaluationToBeSimulated` [mandatory] --> name of folder where evaluation space files are stored.
  * `--demonstration` [optional] --> show a Unity simulation of a specific parameter combination.
  * `-parameterID IntegerParameterID` [mandatory if --demonstration] --> Integer ID of the parameter combination to demonstrate. Its ID is its index in the grid of evaluation parameters.
  * `-timeScale FloatTimeScale` [mandatory if --demonstration] --> Float value for the Unity time scale at which the demonstration simulation is executed

For a concrete example, please have a look at this [application example](https://github.com/siemens/evaluation-framework/wiki/Demonstration-Project).

## External Dependencies ##

* [Newtonsoft Json.NET](https://github.com/JamesNK/Newtonsoft.Json) (MIT License)

---

© Siemens AG, 2020

Author: Michael Dyck (m.dyck@gmx.net)
