using UnityEngine;
using System.Collections.Generic;

namespace CompleteProject {
	[System.Serializable]
	public class EnemyManagerData : IData {
		public int liveEnemies;
		public float timer;
	}

	public enum EnemyType {
		Hellephant,
		ZomBear,
		ZomBunny
	}

	[System.Serializable]
	public class EnemyTypeData : IData {
		public EnemyType enemyType;
	}

	public class EnemyManager : Savable {
		internal static List<GameObject> enemies;

		internal static void Clear () {
			while (enemies.Count > 0) {
				var e = enemies [0];
				enemies.RemoveAt (0);
				GameObject.Destroy (e);
			}
		}

		// Reference to the player's heatlh.
		public PlayerHealth playerHealth;
		// The enemy prefab to be spawned.
		public GameObject enemy;
		// How long between each spawn.
		public float spawnTime = 3f;

		public float timer {
			get {
				return (data as EnemyManagerData).timer;
			}
			set {
				(data as EnemyManagerData).timer = value;
			}
		}

		// An array of the spawn points this enemy can spawn from.
		public Transform[] spawnPoints;

		void Awake () {
			enemies = new List<GameObject> ();
			data = new EnemyManagerData ();
		}

		public new IData GetData () {
			(data as EnemyManagerData).liveEnemies = 0;
			foreach (GameObject enemy in enemies) {
				if (enemy.GetComponent<EnemyHealth> ().currentHealth > 0) {
					(data as EnemyManagerData).liveEnemies++;
				}
			}
			return data;
		}

		#region implemented abstract members of Savable

		public override void LoadData (IData d) {
			data = d;
		}

		#endregion

		void Update () {
			// Add the time since Update was last called to the timer.
			timer += Time.deltaTime;

			// If the timer exceeds the time between attacks, the player is in range and this enemy is alive...
			if (timer >= spawnTime) {
				timer = 0;
				Spawn ();
			}
		}

		void Spawn () {
			// If the player has no health left...
			if (playerHealth.currentHealth <= 0f) {
				// ... exit the function.
				return;
			}

			// Find a random index between zero and one less than the number of spawn points.
			int spawnPointIndex = Random.Range (0, spawnPoints.Length);

			// Create an instance of the enemy prefab at the randomly selected spawn point's position and rotation.
			enemies.Add (Instantiate (enemy, spawnPoints [spawnPointIndex].position, spawnPoints [spawnPointIndex].rotation));
		}
	}
}