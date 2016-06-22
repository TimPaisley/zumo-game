using System;
using UnityEngine;

namespace Zumo {
	class ColorHelper {
		public static Color fromHex(string hexColor) {
			Color color = Color.black;
			ColorUtility.TryParseHtmlString(hexColor, out color);
			return color;
		}
	}
}

