# Evaluation Framework

The [Evaluation Framework](https://github.com/siemens/evaluation-framework) is a set of open source software libraries and tools in [C\# ](https://docs.microsoft.com/de-de/dotnet/csharp/csharp), implemented in .[NET](https://www.microsoft.com/net), and [Python](https://www.python.org/) for efficiently evaluating simulation models of mechatronic systems, in particular by using the game engine [Unity](https://unity3d.com/) as simulation environment, and visualizing the evaluation results in an interactive GUI.

## Idea - Virtual Prototyping ##

The idea of the Evaluation Framework is based on the concept of **Virtual Prototyping**, see e.g. [Wikipedia](https://en.wikipedia.org/wiki/Virtual_prototyping#:~:text=Virtual%20prototyping%20is%20a%20method,to%20making%20a%20physical%20prototype.) or [this publication](https://www.researchgate.net/publication/220517566_Definition_and_Review_of_Virtual_Prototyping). More specifically, the Evaluation Framework serves as a tool for design space exploration in Virtual Prototyping. 

In Virtual Prototyping, (usually 3D) simulation models of e.g. mechatronic systems are examined and evaluated with the goal of optimizing the production costs, time-to-market, and the product itself. Without the need for physical prototypes, different product designs from different engineering specialities (mechanics, electronics, control, software development) can be tested and evaluated early in the production process. In order to do so, the following parameters have to be specified:

* Design alternatives of the product ot be tested, e.g. design of mechanical and electrical components, controller structures, etc.. In the Evaluation Framework these parameters are referred to as **EvaluationParamters**
* Decision criteria the product is evaluated against, e.g. product weight for specific mechanical designs, electric energy consumption with different electrical components, stability of various controllers, etc.. In the Evaluation Framework these parameters are referred to as **ObjectiveValues**

Both the **EvaluationParameters** and **ObjectiveValues** together define the complete **Evaluation Space**. The following GIF shows an example of possible **EvaluationParameters** and **ObjectiveValues** in the case of a simulation model of a plane.

gif

Hence, in Virtual Prototyping, countless design alternatives, i.e. **EvaluationParameters**, of simulation models of mechatronic systems (or other products) are simulated and the specified decision criteria, i.e. **ObjectiveValues**, are used to evalute - and finally optimize - the performance of the respective product designs.

## Implementation - 3 Evaluation Framework Components ##

The Evaluation Framework realizes this idea of performing Virtual Prototyping through three distinct software components. All of these components can also be used individually:

img

* **Definition of Evaluation Space**: In Unity it is very easy to build physics simulations, and hence Unity serves as a great tool for simulation of mechatronic systems. The first component of the Evaluation Framework, i.e. [EvaluationFrameworkUnity](https://github.com/siemens/evaluation-framework/tree/master/Unity3D/EvaluationFrameworkUnity) [EvaluationFrameworkROSUnity](https://github.com/siemens/evaluation-framework/tree/master/Unity3D/EvaluationFrameworkROSUnity), is a Unity addon that allows users to intuitively and easily define **EvaluationParamters** and **ObjectiveValues** from any Unity simulation model. In the end, the specified **EvaluationSpace** is automatically saved into two .json-files and the Unity simulation is built into a standalone application (executable).
* **Execution and Evaluation**: The second component of the Evaluation Framework is implemented as a .[NET](https://www.microsoft.com/net) solution in C\#, see [EvaluationFramework](https://github.com/siemens/evaluation-framework/tree/master/EvaluationFramework) and [EvaluationFrameworkROS](https://github.com/siemens/evaluation-framework/tree/master/EvaluationFrameworkROS). This component efficiently simulates the Unity project for all combinations of the previously defined **EvaluationParamters**. In other words, the .NET solution repeatedly calls the executable of the Unity simulation and performs a brute-force evaluation of the complete parameter space and retrieves the **ObjectiveValues** from the executable. In order to speed up this process, the underlying hardware is used to its maximum capacity, as one simulation can run per CPU core, thus parallelizing the brute-force evaluation. Finally, the results are stored in a table-formatted .txt-file containing all **EvaluationParameter** values and the corresponding **ObjectiveValue** results.
* **Point Cloud Visualization**: The third component of the Evaluation Framework visualizes the evaluation results obtained from the second step in an interactive Design Space Exploration GUI. The corresponding source code can be found in the [VisualizationGUI](https://github.com/siemens/evaluation-framework/tree/master/VisualizationGUI) folder. The GUI has multiple interactive elements that allow the user to inspect the **Evaluation Space** from different perspectives, compare **ObjectiveValues** for different combinations of **EvaluationParameters** and identify conflicting design alternatives and product solutions.

---

## Contents ##

* [EvaluationFramework](https://github.com/siemens/evaluation-framework/tree/master/EvaluationFramework): .NET solution of the Evaluation Framework
* [EvaluationFrameworkROS](https://github.com/siemens/evaluation-framework/tree/master/EvaluationFrameworkROS): .NET solution of the Evaluation Framework with the extension of communicating with a [ROS](https://www.ros.org/) system via [ROS\#](https://github.com/siemens/ros-sharp)
* [Unity3D](https://github.com/siemens/evaluation-framework/tree/master/Unity3D): Source code of the Evaluation Framework connected to the game engine [Unity](https://unity3d.com/)
* [VisualizationGUI](https://github.com/siemens/evaluation-framework/tree/master/VisualizationGUI): [Python](https://www.python.org/) code of the GUI visualizing the evaluation results

[Here](https://github.com/siemens/evaluation-framework/wiki/Demonstration-Project) is an application example illustrating what can be done with the Evaluation Framework.

## Releases ##

In addition to the source code, [Releases](https://github.com/siemens/evaluation-framework/releases) contain 
builds of [EvaluationFramework](https://github.com/siemens/evaluation-framework/tree/master/EvaluationFramework) 
and [EvaluationFrameworkROS](https://github.com/siemens/evaluation-framework/tree/master/EvaluationFrameworkROS)
in **Debug** and **Release** configuration

Please get the latest development version directly from the [tip of this master branch](https://github.com/siemens/evaluation-framework).

## Licensing ##

The [Evaluation Framework](https://github.com/siemens/evaluation-framework) is open source under the [Apache 2.0 license](http://www.apache.org/licenses/LICENSE-2.0) and is free for commercial use.

## External Dependencies ##

[EvaluationFramework](https://github.com/siemens/evaluation-framework/tree/master/EvaluationFramework), 
[EvaluationFrameworkROS](https://github.com/siemens/evaluation-framework/tree/master/EvaluationFrameworkROS),
[EvaluationFrameworkUnity](https://github.com/siemens/evaluation-framework/tree/master/EvaluationFrameworkUnity) and 
[EvaluationFrameworkROSUnity](https://github.com/siemens/evaluation-framework/tree/master/EvaluationFrameworkROSUnity) require:
* [Newtonsoft Json.NET](https://github.com/JamesNK/Newtonsoft.Json) (MIT License)

[EvaluationFrameworkROS](https://github.com/siemens/evaluation-framework/tree/master/EvaluationFrameworkROS) additionally requires:
* [RosBridgeClient](https://github.com/siemens/ros-sharp/tree/master/Libraries/RosBridgeClient) of the open source library [ROS\#](https://github.com/siemens/ros-sharp) (Apache 2.0 License)

## Software Version Requirements ##

The [Evaluation Framework](https://github.com/siemens/evaluation-framework) was developed and tested with the following software versions:
* [EvaluationFrameworkUnity](https://github.com/siemens/evaluation-framework/tree/master/EvaluationFrameworkUnity) and [EvaluationFrameworkROSUnity](https://github.com/siemens/evaluation-framework/tree/master/EvaluationFrameworkROSUnity) were tested with **[Unity](https://unity3d.com/) Version 2019.3.3f1** and **[Newtonsoft Json.NET](https://github.com/JamesNK/Newtonsoft.Json) 12.0.3**
* [EvaluationFramework](https://github.com/siemens/evaluation-framework/tree/master/EvaluationFramework) and [EvaluationFrameworkROS](https://github.com/siemens/evaluation-framework/tree/master/EvaluationFrameworkROS) were developed with **.[NET](https://www.microsoft.com/net) Core 3.1.300** and **[Visual Studio](https://visualstudio.microsoft.com/) 2019, 16.6.0**, both were tested with **[Newtonsoft Json.NET](https://github.com/JamesNK/Newtonsoft.Json) 12.0.3**
* [EvaluationFrameworkROS](https://github.com/siemens/evaluation-framework/tree/master/EvaluationFrameworkROS) was successfully used with **[ROS\#](https://github.com/siemens/ros-sharp) Version 1.6, Release of 12-20-2019** alongside [ROS Kinetic](http://wiki.ros.org/kinetic)
* [VisualizationGUI](https://github.com/siemens/evaluation-framework/tree/master/VisualizationGUI) was implemented and tested in **[Python](https://www.python.org/) 3.7.7**, using **[tkinter](https://docs.python.org/3/library/tkinter.html) 8.6**, **[pandas](https://pandas.pydata.org/) 1.0.3**, **[NumPy](https://numpy.org/) 1.18.1** and **[Matplotlib](https://matplotlib.org/) 3.1.3**

## Platform Support ##

* The [Evaluation Framework](https://github.com/siemens/evaluation-framework) was developed for Windows and has not yet been tested on other platforms.

## Contribution and Feedback ##

We, the [project team](https://github.com/siemens/evaluation-framework/wiki/Contributers-and-Acknowledgements) of the Evaluation Framework, are excited to receive feedback from you on how to improve and further extend the Evaluation Framework. For more information on contributing to this project, check out the [contribution guidelines](https://github.com/siemens/evaluation-framework/blob/master/CONTRIBUTING.md). Please feel free to open [issues](https://github.com/siemens/evaluation-framework/issues) if you find bugs, want to request new features or simply have the urge to ask a question regarding the Evaluation Framework. We are always happy to hear from you! Alternatively, for any feedback, requests or questions, please contact the author Michael Dyck (m.dyck@gmx.net), or - if by any means you do not get a response from him - contact Dr. Martin Bischoff (martin.bischoff@siemens.com). 

#### We are also curious to here, how you intend to use - or are already using - the Evaluation Framework. If you like, come tell us more about your project [here](https://github.com/siemens/evaluation-framework/issues/1). ####

## Further Info ##

* [Read the Wiki](https://github.com/siemens/evaluation-framework/wiki)
* [Contact the Author](mailto:m.dyck@gmx.net)
* [Contributors and Acknowledgements](https://github.com/siemens/evaluation-framework/wiki/Contributers-and-Acknowledgements)

---

© Siemens AG, 2020

Author: Michael Dyck (m.dyck@gmx.net)
