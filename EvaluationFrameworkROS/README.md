[EvaluationFrameworkROS.sln](https://github.com/siemens/evaluation-framework/blob/master/EvaluationFrameworkROS/EvaluationFrameworkROS.sln) is the .NET solution of the [Evaluation Framework](https://github.com/siemens/evaluation-framework)
with the extension of communicating with a [ROS](https://www.ros.org/) system via [ROS\#](https://github.com/siemens/ros-sharp)

## Usage ##

* Navigate to the folder [EvaluationFrameworkROS](https://github.com/siemens/evaluation-framework/tree/master/EvaluationFrameworkROS)
* On any connected (via [ROS\#](https://github.com/siemens/ros-sharp)) ROS system, two commands have to be executed:
	- websocket node: `roslaunch rosbridge_server rosbridge_websocket.launch`
	- any MoveIt! node, e.g.: `roslaunch panda_moveit_config demo.launch`
* From the command line, execute: `bin\[Debug/Release]\netcoreapp3.1\EvaluationFrameworkROS.exe`
* Command line options: 
  * `-evaluationName NameOfEvaluationToBeSimulated` [mandatory] --> name of folder where evaluation space files are stored.
  * `--demonstration` [optional] --> show a Unity simulation of a specific parameter combination.
  * `-parameterID IntegerParameterID` [mandatory if --demonstration] --> Integer ID of the parameter combination to demonstrate. Its ID is its index in the grid of evaluation parameters.
  * `-timeScale FloatTimeScale` [mandatory if --demonstration] --> Float value for the Unity time scale at which the demonstration simulation is executed
  
For a concrete example, please have a look at this [application example](https://github.com/siemens/evaluation-framework/wiki/ROS-Incorporation).

## External Dependencies ##

* [Newtonsoft Json.NET](https://github.com/JamesNK/Newtonsoft.Json) (MIT License)
* [RosBridgeClient](https://github.com/siemens/ros-sharp/tree/master/Libraries/RosBridgeClient) 
of the open source library [ROS\#](https://github.com/siemens/ros-sharp) (Apache 2.0 License)

---

© Siemens AG, 2020

Author: Michael Dyck (m.dyck@gmx.net)
