using UnityEngine;

namespace CompleteProject {
	public abstract class Savable : MonoBehaviour {
		protected IData data;

		public IData GetData () {
			return data;
		}

		public abstract void LoadData (IData d);
	}
}
