using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

namespace CompleteProject {
	[System.Serializable]
	public class PlayerHealthData: IData {
		public int currentHealth;
		public bool isDead;
	}

	public class PlayerHealth : Savable {
		// The amount of health the player starts the game with.
		public int startingHealth = 100;
		// The current health the player has.
		public int currentHealth {
			get {
				return (data as PlayerHealthData).currentHealth;
			}
			set {
				(data as PlayerHealthData).currentHealth = value;
			}
		}

		// Reference to the UI's health bar.
		public Slider healthSlider;
		// Reference to an image to flash on the screen on being hurt.
		public Image damageImage;
		// The audio clip to play when the player dies.
		public AudioClip deathClip;
		AudioClip hurtClip;
		// The speed the damageImage will fade at.
		public float flashSpeed = 5f;
		// The colour the damageImage is set to, to flash.
		public Color flashColour = new Color (1f, 0f, 0f, 0.1f);


		// Reference to the Animator component.
		Animator anim;
		// Reference to the AudioSource component.
		AudioSource playerAudio;
		// Reference to the player's movement.
		PlayerMovement playerMovement;
		// Reference to the PlayerShooting script.
		PlayerShooting playerShooting;
		// Whether the player is dead.
		public bool isDead {
			get {
				return (data as PlayerHealthData).isDead;
			}
			set {
				(data as PlayerHealthData).isDead = value;
			}
		}

		// True when the player gets damaged.
		bool damaged;

		void Awake () {
			data = new PlayerHealthData ();

			// Setting up the references.
			anim = GetComponent <Animator> ();
			playerAudio = GetComponent <AudioSource> ();
			hurtClip = playerAudio.clip;
			playerMovement = GetComponent <PlayerMovement> ();
			playerShooting = GetComponentInChildren <PlayerShooting> ();

			// Set the initial health of the player.
			currentHealth = startingHealth;
		}

		#region implemented abstract members of Savable

		public override void LoadData (IData d) {
			PlayerHealthData oldData = d as PlayerHealthData;

			if ((data as PlayerHealthData).isDead && !oldData.isDead) {
				// revive
				anim.SetTrigger ("Revive");

				// restore default sound
				if (playerAudio.isPlaying)
					playerAudio.Stop ();
				playerAudio.clip = hurtClip;

				playerMovement.enabled = true;
				playerShooting.enabled = true;
			}

			data = oldData;
		}

		#endregion

		void Update () {
			// If the player has just been damaged...
			if (damaged) {
				// ... set the colour of the damageImage to the flash colour.
				damageImage.color = flashColour;
			}
            // Otherwise...
            else {
				// ... transition the colour back to clear.
				damageImage.color = Color.Lerp (damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
			}

			// Reset the damaged flag.
			damaged = false;

			// Set the health bar's value to the current health.
			healthSlider.value = currentHealth;
		}


		public void TakeDamage (int amount) {
			// Set the damaged flag so the screen will flash.
			damaged = true;

			// Reduce the current health by the damage amount.
			currentHealth -= amount;

			// Play the hurt sound effect.
			playerAudio.Play ();

			// If the player has lost all it's health and the death flag hasn't been set yet...
			if (currentHealth <= 0 && !isDead) {
				// ... it should die.
				Death ();
			}
		}


		void Death () {
			// Set the death flag so this function won't be called again.
			isDead = true;

			// Turn off any remaining shooting effects.
			playerShooting.DisableEffects ();

			// Tell the animator that the player is dead.
			anim.SetTrigger ("Die");

			// Set the audiosource to play the death clip and play it (this will stop the hurt sound from playing).
			playerAudio.clip = deathClip;
			playerAudio.Play ();

			// Turn off the movement and shooting scripts.
			playerMovement.enabled = false;
			playerShooting.enabled = false;
		}


		public void RestartLevel () {
			SaveGameManager.instance.Restart ();
		}
	}
}