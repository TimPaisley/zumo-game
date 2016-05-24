using System;
using UnityEngine;

public class Tags {
	public const string Animal = "Animal";
	public const string Ground = "Ground";
	public const string Environment = "Environment";
	public const string PowerUp = "PowerUp";

	public static readonly string[] BoardObjects = {
		Animal, Ground, Environment, PowerUp
	};

	public static bool HasAnyTag(GameObject obj, params string[] tags) {
		foreach (var tag in tags) {
			if (obj.CompareTag(tag)) {
				return true;
			}
		}

		return false;
	}
}
