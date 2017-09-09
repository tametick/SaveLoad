using UnityEngine;
using System.Collections;

namespace CompleteProject {
	[System.Serializable]
	public class EnemyMovementData: IData {

	}

	public class EnemyMovement : Savable {
		// Reference to the player's position.
		Transform player;
		// Reference to the player's health.
		PlayerHealth playerHealth;
		// Reference to this enemy's health.
		EnemyHealth enemyHealth;
		// Reference to the nav mesh agent.
		UnityEngine.AI.NavMeshAgent nav;


		void Awake () {
			// Set up the references.
			player = GameObject.FindGameObjectWithTag ("Player").transform;
			playerHealth = player.GetComponent <PlayerHealth> ();
			enemyHealth = GetComponent <EnemyHealth> ();
			nav = GetComponent <UnityEngine.AI.NavMeshAgent> ();
		}

		#region implemented abstract members of Savable

		public override void LoadData (IData d) {
			throw new System.NotImplementedException ();
		}

		#endregion

		void Update () {
			// If the enemy and the player have health left...
			if (enemyHealth.currentHealth > 0 && playerHealth.currentHealth > 0) {
				// ... set the destination of the nav mesh agent to the player.
				nav.SetDestination (player.position);
			}
            // Otherwise...
            else {
				// ... disable the nav mesh agent.
				nav.enabled = false;
			}
		}
	}
}