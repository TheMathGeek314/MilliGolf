using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace MilliGolf {
    class CameraMods {
		public static List<CameraLockArea> lockZoneList = new();

        public static void Initialize() {
            On.CameraLockArea.OnTriggerEnter2D += CameraLockArea_OnTriggerEnter2D;
            On.CameraLockArea.OnTriggerStay2D += CameraLockArea_OnTriggerStay2D;
			On.CameraLockArea.OnTriggerExit2D += CameraLockArea_OnTriggerExit2D;
        }

		private static void CameraLockArea_OnTriggerEnter2D(On.CameraLockArea.orig_OnTriggerEnter2D orig, CameraLockArea self, Collider2D otherCollider) {
			if(callPrivateMethod_bool(self, "IsInApplicableGameState") && otherCollider.CompareTag("Player")) {
				Vector3 heroPos = otherCollider.gameObject.transform.position;
				CameraTarget camTarget = getPrivateVariable_cameraTarget(self, "camTarget");
				if(getPrivateVariable_collider2D(self, "box2d") != null) {
					float leftSideX = getPrivateVariable_float(self, "leftSideX");
					if(heroPos.x > leftSideX - 1f && heroPos.x < leftSideX + 1f) {
						camTarget.enteredLeft = true;
					}
					else {
						camTarget.enteredLeft = false;
					}
					float rightSideX = getPrivateVariable_float(self, "rightSideX");
					if(heroPos.x > rightSideX - 1f && heroPos.x < rightSideX + 1f) {
						camTarget.enteredRight = true;
					}
					else {
						camTarget.enteredRight = false;
					}
					float topSideY = getPrivateVariable_float(self, "topSideY");
					if(heroPos.y > topSideY - 2f && heroPos.y < topSideY + 2f) {
						camTarget.enteredTop = true;
					}
					else {
						camTarget.enteredTop = false;
					}
					float botSideY = getPrivateVariable_float(self, "botSideY");
					if(heroPos.y > botSideY - 1f && heroPos.y < botSideY + 1f) {
						camTarget.enteredBot = true;
					}
					else {
						camTarget.enteredBot = false;
					}
				}
				if(MilliGolf.ballCam == 0) {
					GameCameras.instance.cameraController.LockToArea(self);
				}
				else {
					if(!lockZoneList.Contains(self)) {
						lockZoneList.Add(self);
					}
				}
			}
		}

		private static void CameraLockArea_OnTriggerStay2D(On.CameraLockArea.orig_OnTriggerStay2D orig, CameraLockArea self, Collider2D otherCollider) {
			if(callPrivateMethod_bool(self, "IsInApplicableGameState") && otherCollider.CompareTag("Player")) {
				if(MilliGolf.ballCam == 0) {
					GameCameras.instance.cameraController.LockToArea(self);
				}
				else {
					if(!lockZoneList.Contains(self)) {
						lockZoneList.Add(self);
					}
				}
			}
		}

		private static void CameraLockArea_OnTriggerExit2D(On.CameraLockArea.orig_OnTriggerExit2D orig, CameraLockArea self, Collider2D otherCollider) {
			if(otherCollider.CompareTag("Player")) {
				Vector3 heroPos = otherCollider.gameObject.transform.position;
				CameraTarget camTarget = getPrivateVariable_cameraTarget(self, "camTarget");
				if(getPrivateVariable_collider2D(self, "box2d") != null) {
					float leftSideX = getPrivateVariable_float(self, "leftSideX");
					if(heroPos.x > leftSideX - 1f && heroPos.x < leftSideX + 1f) {
						camTarget.exitedLeft = true;
					}
					else {
						camTarget.exitedLeft = false;
					}
					float rightSideX = getPrivateVariable_float(self, "rightSideX");
					if(heroPos.x > rightSideX - 1f && heroPos.x < rightSideX + 1f) {
						camTarget.exitedRight = true;
					}
					else {
						camTarget.exitedRight = false;
					}
					float topSideY = getPrivateVariable_float(self, "topSideY");
					if(heroPos.y > topSideY - 2f && heroPos.y < topSideY + 2f) {
						camTarget.exitedTop = true;
					}
					else {
						camTarget.exitedTop = false;
					}
					float botSideY = getPrivateVariable_float(self, "botSideY");
					if(heroPos.y > botSideY - 1f && heroPos.y < botSideY + 1f) {
						camTarget.exitedBot = true;
					}
					else {
						camTarget.exitedBot = false;
					}
				}
				if(MilliGolf.ballCam == 0) {
					GameCameras.instance.cameraController.ReleaseLock(self);
				}
				else {
					if(lockZoneList.Contains(self)) {
						lockZoneList.Remove(self);
					}
				}
			}
		}

		public static Collider2D getPrivateVariable_collider2D(CameraLockArea gameObject, string variableName) {
			FieldInfo field = typeof(CameraLockArea).GetField(variableName, BindingFlags.NonPublic | BindingFlags.Instance);
			return (Collider2D)field.GetValue(gameObject);
		}

		public static CameraTarget getPrivateVariable_cameraTarget(CameraLockArea gameObject, string variableName) {
			FieldInfo field = typeof(CameraLockArea).GetField(variableName, BindingFlags.NonPublic | BindingFlags.Instance);
			return (CameraTarget)field.GetValue(gameObject);
		}

		public static float getPrivateVariable_float(CameraLockArea gameObject, string variableName) {
			FieldInfo field = typeof(CameraLockArea).GetField(variableName, BindingFlags.NonPublic | BindingFlags.Instance);
			return (float)field.GetValue(gameObject);
		}

		public static bool callPrivateMethod_bool(CameraLockArea gameObject, string methodName) {
			MethodInfo method = typeof(CameraLockArea).GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
			return (bool)method.Invoke(gameObject, null);
		}
    }
}
