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

using RosSharp.RosBridgeClient;
using RosSharp.RosBridgeClient.MessageTypes.Std;
using RosSharp.RosBridgeClient.MessageTypes.Moveit;
using RosSharp.RosBridgeClient.MessageTypes.Shape;
using RosSharp.RosBridgeClient.MessageTypes.Geometry;

namespace EvaluationFrameworkROS
{
    internal static class CollisionObjectHandler
    {
        // 
        // BOX
        //
        private static string boxID = "Box";
        private static float unityBoxLength = 0.6f;
        private static float unityBoxWidth = 0.4f;
        private static float unityBoxHeight = 0.4f;
        private static float unityTopDepth = 0.03f;
        private static float unityBoxStrength = 0.01f;
        private static Quaternion unityBoxRotation = new Quaternion(0.0436194, 0, 0, 0.9990482);
        private static float unityBoxRotationAngle = 5f;

        //
        // INDUCTER
        //
        private static string inducterID = "Inducter";
        private static float unityInducterLength = 2.36f;
        private static float unityInducterWidth = 1.0f;
        private static float unityInducterBottomHeight = 0.02f;
        private static float unityInducterBottomOffset = 0.1f;
        private static float unityInducterHeight = 0.15f;
        private static float unityInducterSideStrength = 0.15f;
        private static Quaternion unityInducterRotation = new Quaternion(0, 0, 0, 1);

        //
        // SCREEN
        //
        private static string screenID = "Screen";
        private static float unityScreenLength = 0.604f;
        private static float unityScreenWidth = 0.017f;
        private static float unityScreenHeight = 0.45f;
        private static Quaternion unityScreenRotation = new Quaternion(0, 0, 0.3583679, 0.9335804);

        //
        // ARCH
        //
        private static string archID = "Arch";
        private static float unityArchLength = 1.3f;
        private static float unityArchWidth = 0.15f;
        private static float unityArchHeight = 0.38f;
        private static float unityTopArchStrength = 0.08f;
        private static float unitySideArchStrength = 0.15f;
        private static Quaternion unityArchRotation = new Quaternion(0, 0, 0, 1);

        //
        // STICK
        //
        private static string stickID = "Stick";
        private static float unityStickLength = 0.08f;
        private static float unityStickWidth = 0.08f;
        private static float unityStickHeight = 0.3f;
        private static Quaternion unityStickRotation = new Quaternion(0, 0, 0.3826834, 0.9238795);

        public static CollisionObject AddCollisionObject(string _object, float _centerX, float _centerY, float _centerZ, bool _useSpecialBoxGeometry)
        {
            switch (_object)
            {
                case "Box":
                    return AddBoxCollisionObject(_centerX, _centerY, _centerZ, _useSpecialBoxGeometry);
                case "Screen":
                    return AddScreenCollisionObject(_centerX, _centerY, _centerZ);
                case "Inducter":
                    return AddInducterCollisionObject(_centerX, _centerY, _centerZ);
                case "Arch":
                    return AddArchCollisionObject(_centerX, _centerY, _centerZ);
                case "Stick":
                    return AddStickCollisionObject(_centerX, _centerY, _centerZ);
                default:
                    return null;
            }
        }

        private static CollisionObject AddBoxCollisionObject(float _centerX, float _centerY, float _centerZ, bool _useSpecialBoxGeometry)
        {
            var objectHeader = new Header();
            objectHeader.Update();
            objectHeader.frame_id = "/world";

            SolidPrimitive back = new SolidPrimitive
            {
                type = SolidPrimitive.BOX,
                dimensions = new double[] { unityBoxWidth, unityBoxStrength, unityBoxHeight },
            };

            SolidPrimitive front = new SolidPrimitive
            {
                type = SolidPrimitive.BOX,
                dimensions = new double[] { unityBoxWidth, unityBoxStrength, unityBoxHeight },
            };

            SolidPrimitive right = new SolidPrimitive
            {
                type = SolidPrimitive.BOX,
                dimensions = new double[] { unityBoxStrength, unityBoxLength, unityBoxHeight },
            };

            SolidPrimitive left = new SolidPrimitive
            {
                type = SolidPrimitive.BOX,
                dimensions = new double[] { unityBoxStrength, unityBoxLength, unityBoxHeight },
            };

            SolidPrimitive bottom = new SolidPrimitive
            {
                type = SolidPrimitive.BOX,
                dimensions = new double[] { unityBoxWidth, unityBoxLength, unityBoxStrength },
            };

            SolidPrimitive topSide = new SolidPrimitive
            {
                type = SolidPrimitive.BOX,
                dimensions = new double[] { unityTopDepth, unityBoxLength, unityBoxStrength },
            };

            SolidPrimitive topFrontBack = new SolidPrimitive
            {
                type = SolidPrimitive.BOX,
                dimensions = new double[] { unityBoxWidth, unityTopDepth, unityBoxStrength },
            };

            Pose boxPoseBack = new Pose
            {
                orientation = unityBoxRotation,
                position = new Point(_centerX, _centerY - unityBoxLength / 2, _centerZ + unityBoxHeight / 2),
            };
            boxPoseBack.position.y -= System.Math.Sin(unityBoxRotationAngle * System.Math.PI / 180) * unityBoxHeight / 2;
            boxPoseBack.position.z -= System.Math.Sin(unityBoxRotationAngle * System.Math.PI / 180) * unityBoxLength / 2;

            Pose boxPoseFront = new Pose
            {
                orientation = unityBoxRotation,
                position = new Point(_centerX, _centerY + unityBoxLength / 2, _centerZ + unityBoxHeight / 2),
            };
            boxPoseFront.position.y -= System.Math.Sin(unityBoxRotationAngle * System.Math.PI / 180) * unityBoxHeight / 2;
            boxPoseFront.position.z += System.Math.Sin(unityBoxRotationAngle * System.Math.PI / 180) * unityBoxLength / 2;

            Pose boxPoseLeft = new Pose
            {
                orientation = unityBoxRotation,
                position = new Point(_centerX + unityBoxWidth / 2, _centerY, _centerZ + unityBoxHeight / 2),
            };
            boxPoseLeft.position.y -= System.Math.Sin(unityBoxRotationAngle * System.Math.PI / 180) * unityBoxHeight / 2;

            Pose boxPoseRight = new Pose
            {
                orientation = unityBoxRotation,
                position = new Point(_centerX - unityBoxWidth / 2, _centerY, _centerZ + unityBoxHeight / 2),
            };
            boxPoseRight.position.y -= System.Math.Sin(unityBoxRotationAngle * System.Math.PI / 180) * unityBoxHeight / 2;

            Pose boxPoseBottom = new Pose
            {
                orientation = unityBoxRotation,
                position = new Point(_centerX, _centerY, _centerZ),
            };

            Pose boxPoseTopRight = new Pose
            {
                orientation = unityBoxRotation,
                position = new Point(_centerX - unityBoxWidth / 2 + unityTopDepth / 2, _centerY, _centerZ + unityBoxHeight),
            };
            boxPoseTopRight.position.y -= System.Math.Sin(unityBoxRotationAngle * System.Math.PI / 180) * unityBoxHeight;

            Pose boxPoseTopLeft = new Pose
            {
                orientation = unityBoxRotation,
                position = new Point(_centerX + unityBoxWidth / 2 - unityTopDepth / 2, _centerY, _centerZ + unityBoxHeight),
            };
            boxPoseTopLeft.position.y -= System.Math.Sin(unityBoxRotationAngle * System.Math.PI / 180) * unityBoxHeight;

            Pose boxPoseTopFront = new Pose
            {
                orientation = unityBoxRotation,
                position = new Point(_centerX, _centerY + unityBoxLength / 2 - unityTopDepth / 2, _centerZ + unityBoxHeight),
            };
            boxPoseTopFront.position.y -= System.Math.Sin(unityBoxRotationAngle * System.Math.PI / 180) * unityBoxHeight;
            boxPoseTopFront.position.z += System.Math.Sin(unityBoxRotationAngle * System.Math.PI / 180) * unityBoxLength / 2;

            Pose boxPoseTopBack = new Pose
            {
                orientation = unityBoxRotation,
                position = new Point(_centerX, _centerY - unityBoxLength / 2 + unityTopDepth / 2, _centerZ + unityBoxHeight),
            };
            boxPoseTopBack.position.y -= System.Math.Sin(unityBoxRotationAngle * System.Math.PI / 180) * unityBoxHeight;
            boxPoseTopBack.position.z -= System.Math.Sin(unityBoxRotationAngle * System.Math.PI / 180) * unityBoxLength / 2;

            CollisionObject collisionObjectMsg = new CollisionObject();
            collisionObjectMsg.header = objectHeader;
            collisionObjectMsg.id = boxID;
            SolidPrimitive[] boxPrimitives;
            Pose[] boxPrimitivePoses;
            if (_useSpecialBoxGeometry)
            {
                boxPrimitives = new SolidPrimitive[] { bottom, left, right, back, front,
                                                       topSide, topSide, topFrontBack, topFrontBack };
                boxPrimitivePoses = new Pose[] { boxPoseBottom, boxPoseLeft, boxPoseRight, boxPoseBack, boxPoseFront,
                                                 boxPoseTopRight, boxPoseTopLeft, boxPoseTopFront, boxPoseTopBack };
            }
            else
            {
                boxPrimitives = new SolidPrimitive[] { bottom, left, right, back, front };
                boxPrimitivePoses = new Pose[] { boxPoseBottom, boxPoseLeft, boxPoseRight, boxPoseBack, boxPoseFront };
            }

            collisionObjectMsg.primitives = boxPrimitives;
            collisionObjectMsg.primitive_poses = boxPrimitivePoses;
            collisionObjectMsg.operation = CollisionObject.ADD;

            return collisionObjectMsg;
        }

        private static CollisionObject AddScreenCollisionObject(float _centerX, float _centerY, float _centerZ)
        {
            var objectHeader = new Header();
            objectHeader.Update();
            objectHeader.frame_id = "/world";

            SolidPrimitive screen = new SolidPrimitive
            {
                type = SolidPrimitive.BOX,
                dimensions = new double[] { unityScreenLength, unityScreenWidth, unityScreenHeight },
            };

            Pose screenPose = new Pose
            {
                orientation = unityScreenRotation,
                position = new Point(_centerX, _centerY, _centerZ),
            };

            CollisionObject collisionObjectMsg = new CollisionObject();
            collisionObjectMsg.header = objectHeader;
            collisionObjectMsg.id = screenID;
            collisionObjectMsg.primitives = new SolidPrimitive[] { screen }; // --> not required for moving object
            collisionObjectMsg.primitive_poses = new Pose[] { screenPose };
            collisionObjectMsg.operation = CollisionObject.ADD;

            return collisionObjectMsg;
        }

        private static CollisionObject AddInducterCollisionObject(float _centerX, float _centerY, float _centerZ)
        {
            var objectHeader = new Header();
            objectHeader.Update();
            objectHeader.frame_id = "/world";

            SolidPrimitive inducterSide = new SolidPrimitive
            {
                type = SolidPrimitive.BOX,
                dimensions = new double[] { unityInducterLength, unityInducterSideStrength, unityInducterHeight },
            };

            SolidPrimitive inducterBottom = new SolidPrimitive
            {
                type = SolidPrimitive.BOX,
                dimensions = new double[] { unityInducterLength, unityInducterWidth, unityInducterBottomHeight },
            };

            Pose inducterFrontPose = new Pose
            {
                orientation = unityInducterRotation,
                position = new Point(_centerX, _centerY, _centerZ),
            };
            inducterFrontPose.position.y -= (unityInducterWidth / 2 + unityInducterSideStrength / 2);
            inducterFrontPose.position.z -= (unityInducterHeight / 2 - unityInducterBottomOffset);

            Pose inducterBackPose = new Pose
            {
                orientation = unityInducterRotation,
                position = new Point(_centerX, _centerY, _centerZ),
            };
            inducterBackPose.position.y += (unityInducterWidth / 2 + unityInducterSideStrength / 2);
            inducterBackPose.position.z -= (unityInducterHeight / 2 - unityInducterBottomOffset);

            Pose inducterBottomPose = new Pose
            {
                orientation = unityInducterRotation,
                position = new Point(_centerX, _centerY, _centerZ),
            };

            CollisionObject collisionObjectMsg = new CollisionObject();
            collisionObjectMsg.header = objectHeader;
            collisionObjectMsg.id = inducterID;
            collisionObjectMsg.primitives = new SolidPrimitive[] { inducterSide, inducterSide, inducterBottom }; // --> not required for moving object
            collisionObjectMsg.primitive_poses = new Pose[] { inducterFrontPose, inducterBackPose, inducterBottomPose };
            collisionObjectMsg.operation = CollisionObject.ADD;

            return collisionObjectMsg;
        }

        private static CollisionObject AddArchCollisionObject(float _centerX, float _centerY, float _centerZ)
        {
            var objectHeader = new Header();
            objectHeader.Update();
            objectHeader.frame_id = "/world";

            SolidPrimitive archTop = new SolidPrimitive
            {
                type = SolidPrimitive.BOX,
                dimensions = new double[] { unityArchWidth, unityArchLength, unityTopArchStrength },
            };

            SolidPrimitive archSide = new SolidPrimitive
            {
                type = SolidPrimitive.BOX,
                dimensions = new double[] { unitySideArchStrength, unityArchWidth, unityArchHeight },
            };

            Pose archTopPose = new Pose
            {
                orientation = unityArchRotation,
                position = new Point(_centerX, _centerY, _centerZ + unityArchHeight / 2 - unityTopArchStrength / 2),
            };
            archTopPose.position.z += (unityArchHeight / 2 + unityInducterBottomOffset);

            Pose archLeftPose = new Pose
            {
                orientation = unityArchRotation,
                position = new Point(_centerX, _centerY - (unityArchLength / 2 - unityArchWidth / 2), _centerZ),
            };
            archLeftPose.position.z += (unityArchHeight / 2 + unityInducterBottomOffset);

            Pose archRightPose = new Pose
            {
                orientation = unityArchRotation,
                position = new Point(_centerX, _centerY + (unityArchLength / 2 - unityArchWidth / 2), _centerZ),
            };
            archRightPose.position.z += (unityArchHeight / 2 + unityInducterBottomOffset);

            CollisionObject collisionObjectMsg = new CollisionObject();
            collisionObjectMsg.header = objectHeader;
            collisionObjectMsg.id = archID;
            collisionObjectMsg.primitives = new SolidPrimitive[] { archTop, archSide, archSide }; // --> not required for moving object
            collisionObjectMsg.primitive_poses = new Pose[] { archTopPose, archLeftPose, archRightPose };
            collisionObjectMsg.operation = CollisionObject.ADD;

            return collisionObjectMsg;
        }

        private static CollisionObject AddStickCollisionObject(float _centerX, float _centerY, float _centerZ)
        {
            var objectHeader = new Header();
            objectHeader.Update();
            objectHeader.frame_id = "/world";

            SolidPrimitive stick = new SolidPrimitive
            {
                type = SolidPrimitive.BOX,
                dimensions = new double[] { unityStickLength, unityStickWidth, unityStickHeight },
            };

            Pose stickPose = new Pose
            {
                orientation = unityStickRotation,
                position = new Point(_centerX, _centerY + unityArchLength / 2, _centerZ + (unityArchHeight - unityStickHeight) / 2),
            };
            stickPose.position.y += System.Math.Sqrt(System.Math.Pow(unityStickWidth, 2) + System.Math.Pow(unityStickLength, 2)) / 2;
            stickPose.position.z += (unityArchHeight / 2 + unityInducterBottomOffset);

            CollisionObject collisionObjectMsg = new CollisionObject();
            collisionObjectMsg.header = objectHeader;
            collisionObjectMsg.id = stickID;
            collisionObjectMsg.primitives = new SolidPrimitive[] { stick }; // --> not required for moving object
            collisionObjectMsg.primitive_poses = new Pose[] { stickPose };
            collisionObjectMsg.operation = CollisionObject.ADD;

            return collisionObjectMsg;
        }
    }
}
