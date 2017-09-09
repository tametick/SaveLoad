using UnityEngine;
using System.Collections;

namespace CompleteProject {
	[System.Serializable]
	public class CameraFollowData: IData {
		public Vector3 position;
	}

	public class CameraFollow : MonoBehaviour, ISavable {
		CameraFollowData data;

		#region IData implementation

		public IData GetData () {
			return data;
		}

		public void LoadData (IData d) {
			var oldData = (CameraFollowData)d;

			transform.position = oldData.position;

			data = oldData;
		}

		#endregion

		public Transform target;
		// The position that that camera will be following.
		public float smoothing = 5f;
		// The speed with which the camera will be following.


		Vector3 offset;
		// The initial offset from the target.


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
			data.position.Set (transform.position.x, transform.position.y, transform.position.z);
		}
	}
}