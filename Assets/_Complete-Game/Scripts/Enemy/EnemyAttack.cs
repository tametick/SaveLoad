using UnityEngine;
using System.Collections;

namespace CompleteProject {
	[System.Serializable]
	public class EnemyAttackData: IData {
		
	}

	public class EnemyAttack : Savable {
		// The time in seconds between each attack.
		public float timeBetweenAttacks = 0.5f;
		// The amount of health taken away per attack.
		public int attackDamage = 10;


		// Reference to the animator component.
		Animator anim;
		// Reference to the player GameObject.
		GameObject player;
		// Reference to the player's health.
		PlayerHealth playerHealth;
		// Reference to this enemy's health.
		EnemyHealth enemyHealth;
		// Whether player is within the trigger collider and can be attacked.
		bool playerInRange;
		// Timer for counting up to the next attack.
		float timer;
		bool playerDead;

		void Awake () {
			// Setting up the references.
			player = GameObject.FindGameObjectWithTag ("Player");
			playerHealth = player.GetComponent <PlayerHealth> ();
			enemyHealth = GetComponent<EnemyHealth> ();
			anim = GetComponent <Animator> ();
		}

		#region ISavable implementation

		public override void LoadData (IData d) {
			throw new System.NotImplementedException ();
		}

		#endregion

		void OnTriggerEnter (Collider other) {
			// If the entering collider is the player...
			if (other.gameObject == player) {
				// ... the player is in range.
				playerInRange = true;
			}
		}


		void OnTriggerExit (Collider other) {
			// If the exiting collider is the player...
			if (other.gameObject == player) {
				// ... the player is no longer in range.
				playerInRange = false;
			}
		}


		void Update () {
			if (enemyHealth.currentHealth <= 0)
				return;

			// Add the time since Update was last called to the timer.
			timer += Time.deltaTime;

			// If the timer exceeds the time between attacks, the player is in range and this enemy is alive...
			if (timer >= timeBetweenAttacks && playerInRange) {
				// ... attack.
				Attack ();
			}

			// If the player has zero or less health...
			if (playerHealth.currentHealth <= 0) {
				if (!playerDead) {
					playerDead = true;
					// ... tell the animator the player is dead.
					anim.SetTrigger ("PlayerDead");
				}
			} else if (playerDead) {
				playerDead = false;
				anim.SetTrigger ("PlayerRevived");
			}
		}


		void Attack () {
			// Reset the timer.
			timer = 0f;

			// If the player has health to lose...
			if (playerHealth.currentHealth > 0) {
				// ... damage the player.
				playerHealth.TakeDamage (attackDamage);
			}
		}
	}
}