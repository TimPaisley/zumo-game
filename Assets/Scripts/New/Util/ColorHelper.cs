using UnityEngine;

namespace Zumo {
	public static class ColorHelper {
		public static Color fromHex(string hexColor) {
			Color color = Color.black;
			ColorUtility.TryParseHtmlString(hexColor, out color);
			return color;
		}
	}
}

