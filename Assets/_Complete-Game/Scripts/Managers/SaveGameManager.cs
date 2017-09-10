﻿using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using System;

namespace CompleteProject {
	public class SaveGameManager : MonoBehaviour {
		internal static SaveGameManager instance;

		const string saveName = "SavedGame";
		const string startSaveName = "GameStart";
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

		public GameObject player;
		public GameObject mainCamera;
		public GameObject enemyManager;
		public GameObject scoreText;

		public GameObject HellephantPrefab;
		public GameObject ZomBearPrefab;
		public GameObject ZomBunnyPrefab;

		void Start () {
			instance = this;
			saveData = new SaveData ();
			Save (startSaveName);
		}

		internal void Restart () {
			EnemyManager.Clear ();
			Load (startSaveName);
		}

		string Save (string name) {
			Time.timeScale = 0;
			// clear old saved data & deserialize current game state 
			saveData.Clear ();
			saveData.Add (scoreText.GetComponent<ScoreManager> ().GetData ());
			saveData.Add (enemyManager.GetComponent<EnemyManager> ().GetData ());
			saveData.Add (mainCamera.GetComponent<CameraFollow> ().GetData ());
			saveData.Add (player.GetComponent<PlayerMovement> ().GetData ());
			saveData.Add (player.GetComponent<PlayerHealth> ().GetData ());
			saveData.Add (player.GetComponentInChildren<PlayerShooting> ().GetData ());

			// enemies
			foreach (GameObject enemy in EnemyManager.enemies) {
				// only save live enemies 
				if (enemy.GetComponent<EnemyHealth> ().currentHealth > 0) {
					var enemyTypeData = new EnemyTypeData ();
					if (enemy.name.ToLower ().StartsWith ("zombear")) {
						enemyTypeData.enemyType = EnemyType.ZomBear.ToString ();
					} else if (enemy.name.ToLower ().StartsWith ("zombunny")) {
						enemyTypeData.enemyType = EnemyType.ZomBunny.ToString ();
					} else {
						enemyTypeData.enemyType = EnemyType.Hellephant.ToString ();
					}

					saveData.Add (enemyTypeData);
					saveData.Add (enemy.GetComponent<EnemyMovement> ().GetData ());
					saveData.Add (enemy.GetComponent<EnemyHealth> ().GetData ());
					saveData.Add (enemy.GetComponent<EnemyAttack> ().GetData ());
				}
			}

			var saveString = saveData.ToString ();
			Time.timeScale = 1;
			// write game state to storage
			PlayerPrefs.SetString (name, saveString);
			return saveString;
		}

		string Load (string name) {
			Time.timeScale = 0;
			// get serialized game state from storage & deserialize it
			var loadString = PlayerPrefs.GetString (name);
			saveData.FromString (loadString);

			// load into current game
			scoreText.GetComponent<ScoreManager> ().LoadData (saveData.ShiftData<ScoreManagerData> ());
			var em = enemyManager.GetComponent<EnemyManager> ();
			em.LoadData (saveData.ShiftData<EnemyManagerData> ());
			mainCamera.GetComponent<CameraFollow> ().LoadData (saveData.ShiftData<CameraFollowData> ());
			player.GetComponent<PlayerMovement> ().LoadData (saveData.ShiftData<PlayerMovementData> ());
			player.GetComponent<PlayerHealth> ().LoadData (saveData.ShiftData<PlayerHealthData> ());
			player.GetComponentInChildren<PlayerShooting> ().LoadData (saveData.ShiftData<PlayerShootingData> ());

			// enemies
			var enemies = em.LiveEnemies;

			// clear and instantiate one from the saved game
			EnemyManager.Clear ();
			for (int e = 0; e < enemies; e++) {
				EnemyTypeData enemyTypeData = saveData.ShiftData<EnemyTypeData> ();
				GameObject enemy = null;
				EnemyType t = (EnemyType)Enum.Parse (typeof(EnemyType), enemyTypeData.enemyType);
				switch (t) {
				case EnemyType.Hellephant:
					enemy = GameObject.Instantiate (HellephantPrefab);
					break;
				case EnemyType.ZomBear:
					enemy = GameObject.Instantiate (ZomBearPrefab);
					break;
				case EnemyType.ZomBunny:
					enemy = GameObject.Instantiate (ZomBunnyPrefab);
					break;
				}
				enemy.GetComponent<EnemyMovement> ().LoadData (saveData.ShiftData<EnemyMovementData> ());
				enemy.GetComponent<EnemyHealth> ().LoadData (saveData.ShiftData<EnemyHealthData> ());
				enemy.GetComponent<EnemyAttack> ().LoadData (saveData.ShiftData<EnemyAttackData> ());
				EnemyManager.Add (enemy);
			}
				
			Time.timeScale = 1;
			return loadString;
		}

		void Update () {
			if (Input.GetKeyUp (saveKey)) {
				var saveString = Save (saveName);
				print ("saved\n" + saveString);
			} else if (Input.GetKeyUp (loadKey) && PlayerPrefs.HasKey (saveName)) {
				var loadString = Load (saveName);
				print ("loaded\n" + loadString);
			}
		}
	}
}