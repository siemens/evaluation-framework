# Evaluation Framework

The [Evaluation Framework](https://github.com/siemens/evaluation-framework) is a set of open source software libraries and tools in [C\# ](https://docs.microsoft.com/de-de/dotnet/csharp/csharp), implemented in .[NET](https://www.microsoft.com/net), and [Python](https://www.python.org/) for efficiently evaluating simulation models of mechatronic systems, in particular by using the game engine [Unity](https://unity3d.com/) as simulation environment, and visualizing the evaluation results in an interactive GUI.

The Evaluation Framework is the result of [Michael Dyck's](https://github.com/MD-cyb3) Master's thesis entitled 'Development of a Simulation-Based Evaluation Framework for Mechatronic Systems', which he did in the course of his Master's studies 'Robotics, Cognition, Intelligence' at [Technical University of Munich (TUM)](https://www.tum.de/en/), (Germany). The thesis was written together with [Dr. Martin Bischoff](https://github.com/MartinBischoff) from Siemens AG, as well as [Jonis Kiesbye]() from the [Chair of Astronautics (LRT)](https://www.lrg.tum.de/en/lrt/home/) and [Jonas Wittmann](https://github.com/jonasTUM) from the [Chair of Applied Mechaincs (AM)](https://www.mw.tum.de/en/am/home/) of [TUM](https://www.tum.de/en/).

[Here](https://github.com/siemens/evaluation-framework/wiki/Demonstration-Project) is an application example illustrating what can be done with the Evaluation Framework.

## Contents ##

* [EvaluationFramework](https://github.com/siemens/evaluation-framework/tree/master/EvaluationFramework): .NET solution of the Evaluation Framework
* [EvaluationFrameworkROS](https://github.com/siemens/evaluation-framework/tree/master/EvaluationFrameworkROS): .NET solution of the Evaluation Framework with the extension of communicating with a [ROS](https://www.ros.org/) system via [ROS\#](https://github.com/siemens/ros-sharp)
* [Unity3D](https://github.com/siemens/evaluation-framework/tree/master/Unity3D): Source code of the Evaluation Framework connected to the game engine [Unity](https://unity3d.com/)
* [VisualizationGUI](https://github.com/siemens/evaluation-framework/tree/master/VisualizationGUI): [Python](https://www.python.org/) code of the GUI visualizing the evaluation results

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
