using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace CompleteProject {
	public class SaveGameManager : MonoBehaviour {
		const string saveName = "SavedGame";
		const string saveKey = "1";
		const string loadKey = "2";

		[System.Serializable]
		public class SaveData {
			const string separator = "%SP%";

			public List<string> datas;

			public SaveData () {
				datas = new List<string> ();
			}

			internal void Clear () {
				datas.Clear ();
			}

			internal void Add (IData d) {
				datas.Add (JsonUtility.ToJson (d));
			}

			public override string ToString () {
				string s = "";
				foreach (string data in datas) {
					if (s.Length > 0) {
						s += separator;
					}
					s += data;
				}
				return s;
			}

			public void FromString (string s) {
				Clear ();
				foreach (string dataString in Regex.Split(s,separator)) {
					datas.Add (dataString);
				}
			}

			// removed and returns first ISata
			internal T ShiftData<T> () {
				var d = datas [0];
				datas.RemoveAt (0);
				return JsonUtility.FromJson<T> (d);
			}
		}

		SaveData saveData;

		public GameObject mainCamera;
		public GameObject player;

		void Start () {
			saveData = new SaveData ();
		}

		void Update () {
			
			if (Input.GetKeyUp (saveKey)) {
				Time.timeScale = 0;

				// clear old saved data & deserialize current game state 
				saveData.Clear ();
				saveData.Add (mainCamera.GetComponent<CameraFollow> ().GetData ());
				saveData.Add (player.GetComponent<PlayerMovement> ().GetData ());
				saveData.Add (player.GetComponent<PlayerHealth> ().GetData ());
				saveData.Add (player.GetComponentInChildren<PlayerShooting> ().GetData ());

				var saveString = saveData.ToString ();
				Time.timeScale = 1;

				// write game state to storage
				PlayerPrefs.SetString (saveName, saveString);
				print ("saved\n" + saveString);
			} else if (Input.GetKeyUp (loadKey)) {
				Time.timeScale = 0;

				// get serialized game state from storage & deserialize it
				var loadString = PlayerPrefs.GetString (saveName);
				saveData.FromString (loadString);

				// load into current game
				mainCamera.GetComponent<CameraFollow> ().LoadData (saveData.ShiftData<CameraFollowData> ());
				player.GetComponent<PlayerMovement> ().LoadData (saveData.ShiftData<PlayerMovementData> ());
				player.GetComponent<PlayerHealth> ().LoadData (saveData.ShiftData<PlayerHealthData> ());
				player.GetComponentInChildren<PlayerShooting> ().LoadData (saveData.ShiftData<PlayerShootingData> ());

				Time.timeScale = 1;
				print ("loaded\n" + loadString);
			}
		}
	}
}