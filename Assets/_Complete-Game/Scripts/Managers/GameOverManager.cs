using UnityEngine;

namespace CompleteProject {
	public class GameOverManager : MonoBehaviour {
		public PlayerHealth playerHealth;
		// Reference to the player's health.
		bool isOver;

		Animator anim;
		// Reference to the animator component.


		void Awake () {
			// Set up the reference.
			anim = GetComponent <Animator> ();
		}

		void Update () {
			// If the player has run out of health...
			if (playerHealth.currentHealth <= 0) {
				if (!isOver) {
					// ... tell the animator the game is over.
					anim.SetTrigger ("GameOver");
					isOver = true;
				}
			} else if (isOver) {
				// this happens when loading after death
				isOver = false;
				anim.SetTrigger ("BackToGame");
			}
		}
	}
}