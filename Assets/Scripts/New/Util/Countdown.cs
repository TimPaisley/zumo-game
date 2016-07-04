using UnityEngine;
using System.Collections;

namespace Zumo {
	class Countdown : MonoBehaviour {
		public IEnumerator Play () {
			yield return new WaitForSeconds(1f);
		}
	}
}
