using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace CompleteProject {
	[System.Serializable]
	public class ScoreManagerData : IData {
		public int score;
	}

	public class ScoreManager : Savable {
		internal static ScoreManager instance;

		// The player's score.
		public int score {
			get {
				return (data as ScoreManagerData).score;
			}
			set {
				(data as ScoreManagerData).score = value;
			}
		}

		// Reference to the Text component.
		Text text;


		void Awake () {
			instance = this;
			data = new ScoreManagerData ();
			// Set up the reference.
			text = GetComponent <Text> ();

			// Reset the score.
			score = 0;
		}

		#region implemented abstract members of Savable

		public override void LoadData (IData d) {
			data = d;
		}

		#endregion

		void Update () {
			// Set the displayed text to be the word "Score" followed by the score value.
			text.text = "Score: " + score;
		}
	}
}