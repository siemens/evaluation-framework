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
using System.Linq;
using System.Threading;
using RosSharp.RosBridgeClient;
using RosSharp.RosBridgeClient.MessageTypes.Actionlib;
using RosSharp.RosBridgeClient.MessageTypes.Geometry;
using RosSharp.RosBridgeClient.MessageTypes.Moveit;
using RosSharp.RosBridgeClient.MessageTypes.Sensor;
using RosSharp.RosBridgeClient.MessageTypes.Std;

namespace EvaluationFrameworkROS
{
    internal class MoveItClient
    {
        static readonly string uri = "ws://192.168.56.102:9090";
        static readonly string actionName = "move_group";

        private RosSocket rosSocket;
        private MoveGroupActionClient moveGroupActionClient;

        private JointGroup jointGroup;
        private string plannerID = "";

        private double[] initialPosition = new double[] { 0, 0, 0, -1.571, 0, 1.571, 0.785, 0.035, 0.035 };

        // Cartesian position on path
        private Point aboveBox = new Point(0.4544, 0.099, 0.54);
        private Point inBox = new Point(0.4544, 0.1404, 0.2);
        private Point aboveInducter = new Point(-0.27, -0.55, 0.54);
        private Point onInducter = new Point(-0.27, -0.55, 0.24);
        private Quaternion orientation = new Quaternion(0.924, -0.383, 0, 0);
        private Pose[] posesOnPath;

        private int instanceID;

        public Dictionary<int, Dictionary<int, object>> trajectoryDict;

        private ManualResetEvent OnServiceReceived = new ManualResetEvent(false);

        private float[] boxPosition = new float[] { 0.4544f, 0.197262f, -0.009377f };
        private float[] screenPosition = new float[] { 0.241415f, -0.493228f, 0.805f };
        private float[] inducterPosition = new float[] { 0.05f, -0.863f, 0.03f };
        private float archXPosition = -0.745f;

        public MoveItClient()
        {
            rosSocket = new RosSocket(new RosSharp.RosBridgeClient.Protocols.WebSocketNetProtocol(uri), RosSocket.SerializerEnum.Newtonsoft_JSON);

            /*
             * TRAJECTORY SETTING
             */
            jointGroup = new JointGroup
            {
                name = "panda_arm_hand",
                jointNames = new string[] { "panda_joint1", "panda_joint2", "panda_joint3",
                                            "panda_joint4", "panda_joint5", "panda_joint6",
                                            "panda_joint7", "panda_finger_joint1", "panda_finger_joint2" }
            };

            /*
             * ENVIRONMENT SETTING
             */
            AddCollisionObjects("Box", boxPosition[0], boxPosition[1], boxPosition[2]);
            AddCollisionObjects("Screen", screenPosition[0], screenPosition[1], screenPosition[2]);
            AddCollisionObjects("Inducter", inducterPosition[0], inducterPosition[1], inducterPosition[2]);
            AddCollisionObjects("Arch", archXPosition, inducterPosition[1], inducterPosition[2]);
            AddCollisionObjects("Stick", archXPosition, inducterPosition[1], inducterPosition[2]);

            /*
             * INITIALIZE TRAJECTORY DICTIONARY
             */
            trajectoryDict = new Dictionary<int, Dictionary<int, object>>();

            /*
             * INITIALIZE CLIENT
             */
            moveGroupActionClient = new MoveGroupActionClient(actionName, jointGroup, plannerID, rosSocket);
            moveGroupActionClient.PlanOnly = false;
            moveGroupActionClient.Initialize();

            /*
             * MOVE TO INITIAL POSITION
             */
            MoveToInitialPosition();

            /*
             * PLANNER PARAMETRIZATION
             */
            //plannerID = "ChompDefault";
            //SetPlannerParameters();
            //GetPlannerParameters();
        }

        public void PlanCartesianPath(int _instanceID, float _sockelOffsetX, float _sockelOffsetY, float _sockelOffsetZ, bool _useSpecialBoxGeometry)
        {
            instanceID = _instanceID;
            _sockelOffsetY /= 100;
            /*
             * MOVE OBSTACLES BASED ON SOCKEL POSITION OFFSET
             */
            MoveCollisionObjects(_sockelOffsetX, _sockelOffsetY, _sockelOffsetZ, _useSpecialBoxGeometry);

            /*
             * ADJUST POSITIONS ON PATH BASED ON SOCKEL POSITION OFFSET
             */
            posesOnPath = new Pose[]
            {
                new Pose(new Point(aboveBox.x - _sockelOffsetZ, aboveBox.y - _sockelOffsetX, aboveBox.z - _sockelOffsetY),
                         orientation),
                new Pose(new Point(inBox.x - _sockelOffsetZ, inBox.y - _sockelOffsetX, inBox.z - _sockelOffsetY),
                         orientation),
                new Pose(new Point(aboveBox.x - _sockelOffsetZ, aboveBox.y - _sockelOffsetX, aboveBox.z - _sockelOffsetY + 0.20),
                         orientation),
                new Pose(new Point(aboveInducter.x - _sockelOffsetZ, aboveInducter.y - _sockelOffsetX, aboveInducter.z - _sockelOffsetY),
                         orientation),
                new Pose(new Point(onInducter.x - _sockelOffsetZ, onInducter.y - _sockelOffsetX, onInducter.z - _sockelOffsetY),
                         orientation),
                new Pose(new Point(aboveInducter.x - _sockelOffsetZ, aboveInducter.y - _sockelOffsetX, aboveInducter.z - _sockelOffsetY),
                         orientation),
                new Pose(new Point(aboveBox.x - _sockelOffsetZ, aboveBox.y - _sockelOffsetX, aboveBox.z - _sockelOffsetY),
                         orientation),
            };

            /*
             * PLAN IN CARTESIAN SPACE
             */
            var getCartesianPathRequest = new GetCartesianPathRequest();
            getCartesianPathRequest.header.Update();
            getCartesianPathRequest.group_name = "panda_arm";

            var robotJointState = new JointState
            {
                header = new Header(),
                name = jointGroup.jointNames,
                position = initialPosition
            };
            getCartesianPathRequest.start_state.joint_state = robotJointState;

            getCartesianPathRequest.link_name = "panda_link8";
            getCartesianPathRequest.waypoints = posesOnPath;
            getCartesianPathRequest.avoid_collisions = true;
            getCartesianPathRequest.max_step = 0.01;

            rosSocket.CallService<GetCartesianPathRequest, GetCartesianPathResponse>("/compute_cartesian_path",
                                                                                     ServiceCallHandler, getCartesianPathRequest);
            OnServiceReceived.WaitOne();
            OnServiceReceived.Reset();
        }

        public void Terminate()
        {
            /*
             * TERMINATE CLIENT
             */
            moveGroupActionClient.Terminate();
            rosSocket.Close();
        }

        private void AddCollisionObjects(string _object, float _centerX, float _centerY, float _centerZ, bool _useSpecialBoxGeometry = false)
        {
            CollisionObject collisionObjectMsg = CollisionObjectHandler.AddCollisionObject(_object, _centerX, _centerY, _centerZ, _useSpecialBoxGeometry);

            // Publication:
            string publicationID = rosSocket.Advertise<CollisionObject>("collision_object");
            rosSocket.Publish(publicationID, collisionObjectMsg);
        }

        private void MoveCollisionObjects(float _sockelOffsetX, float _sockelOffsetY, float _sockelOffsetZ, bool _useSpecialBoxGeometry)
        {
            // BOX
            float[] boxPos = new float[]
            {
                boxPosition[0] - _sockelOffsetZ,
                boxPosition[1] - _sockelOffsetX,
                boxPosition[2] - _sockelOffsetY,
            };
            // SCREEN
            float[] screenPos = new float[]
            {
                screenPosition[0] - _sockelOffsetZ,
                screenPosition[1] - _sockelOffsetX,
                screenPosition[2] - _sockelOffsetY,
            };
            // INDUCTER
            float[] inducterPos = new float[]
            {
                inducterPosition[0] - _sockelOffsetZ,
                inducterPosition[1] - _sockelOffsetX,
                inducterPosition[2] - _sockelOffsetY,
            };
            // ARCH & STICK
            float archXPos = archXPosition - _sockelOffsetZ;

            AddCollisionObjects("Box", boxPos[0], boxPos[1], boxPos[2], _useSpecialBoxGeometry : _useSpecialBoxGeometry);
            AddCollisionObjects("Screen", screenPos[0], screenPos[1], screenPos[2]);
            AddCollisionObjects("Inducter", inducterPos[0], inducterPos[1], inducterPos[2]);
            AddCollisionObjects("Arch", archXPos, inducterPos[1], inducterPos[2]);
            AddCollisionObjects("Stick", archXPos, inducterPos[1], inducterPos[2]);
        }

        private void MoveToInitialPosition()
        {
            moveGroupActionClient.PlanGoal(initialPosition);
            moveGroupActionClient.SendGoal();

            /*
             * GET FEEDBACK, STATUS, RESULT
             */
            do
            {
                Console.WriteLine("Status: " + moveGroupActionClient.GetStatusString());
                Console.WriteLine("Feedback: " + moveGroupActionClient.GetFeedbackString());
                Thread.Sleep(200);
            } while (moveGroupActionClient.goalStatus.status != GoalStatus.SUCCEEDED);
            Console.WriteLine("Status: " + moveGroupActionClient.GetStatusString());

            Thread.Sleep(500);
        }

        private void ServiceCallHandler(GetCartesianPathResponse message)
        {
            float[][] posArray = message.solution.joint_trajectory.points
                                 .Select(point => point.positions
                                         .Select(pos => (float)pos)
                                         .ToArray())
                                 .ToArray();
            float[][] velArray = message.solution.joint_trajectory.points
                                 .Select(point => point.velocities
                                         .Select(vel => (float)vel)
                                         .ToArray())
                                 .ToArray();
            float[][] accArray = message.solution.joint_trajectory.points
                                 .Select(point => point.accelerations
                                         .Select(acc => (float)acc)
                                         .ToArray())
                                 .ToArray();
            float[] timeArray = message.solution.joint_trajectory.points
                                .Select(point => point.time_from_start.secs + point.time_from_start.nsecs / 1e9f)
                                .ToArray();
            float fractionOfTrajectory = (float)message.fraction * 100;

            trajectoryDict.Add(instanceID, new Dictionary<int, object>
            {
                { 0, fractionOfTrajectory },
                { 1, timeArray },
                { 2, posArray },
                { 3, velArray },
                { 4, accArray },
            });

            OnServiceReceived.Set();
        }

        private void SetPlannerParameters()
        {
            PlannerParams _params = new PlannerParams
            {
                keys = new string[] { "type", "max_nearest_neighbors" },
                values = new string[] { "geometric::PRM", "5" }
            };

            var setPlannerParamsRequest = new SetPlannerParamsRequest
            {
                planner_config = plannerID,
                group = jointGroup.name,
                _params = _params,
                replace = false,
            };
            rosSocket.CallService<SetPlannerParamsRequest, SetPlannerParamsResponse>("/set_planner_params", ServiceCallHandler, setPlannerParamsRequest);
            OnServiceReceived.WaitOne();
            OnServiceReceived.Reset();
        }

        private void ServiceCallHandler(SetPlannerParamsResponse message)
        {
            Console.WriteLine(message);
            OnServiceReceived.Set();
        }

        private void GetPlannerParameters()
        {
            var getPlannerParamsRequest = new GetPlannerParamsRequest
            {
                planner_config = plannerID,
                group = jointGroup.name,
            };
            rosSocket.CallService<GetPlannerParamsRequest, GetPlannerParamsResponse>("/get_planner_params", ServiceCallHandler, getPlannerParamsRequest);
            OnServiceReceived.WaitOne();
            OnServiceReceived.Reset();
        }

        private void ServiceCallHandler(GetPlannerParamsResponse message)
        {
            Console.WriteLine(message._params.values[0]);
            Console.WriteLine(message._params.values[1]);
            OnServiceReceived.Set();
        }
    }
}
