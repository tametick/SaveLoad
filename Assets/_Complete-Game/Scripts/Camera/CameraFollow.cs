using UnityEngine;
using System.Collections;

namespace CompleteProject {
	[System.Serializable]
	public class CameraFollowData: IData {
		public Vector3 position;
	}

	public class CameraFollow : Savable {
		#region ISavable implementation

		public override void LoadData (IData d) {
			var oldData = d as CameraFollowData;

			transform.position = oldData.position;

			data = oldData;
		}

		#endregion

		// The position that that camera will be following.
		public Transform target;
		// The speed with which the camera will be following.
		public float smoothing = 5f;

		// The initial offset from the target.
		Vector3 offset;

		void Start () {
			data = new CameraFollowData ();
			// Calculate the initial offset.
			offset = transform.position - target.position;
		}

		void FixedUpdate () {
			// Create a postion the camera is aiming for based on the offset from the target.
			Vector3 targetCamPos = target.position + offset;

			// Smoothly interpolate between the camera's current position and it's target position.
			transform.position = Vector3.Lerp (transform.position, targetCamPos, smoothing * Time.deltaTime);
			(data as CameraFollowData).position.Set (transform.position.x, transform.position.y, transform.position.z);
		}
	}
}