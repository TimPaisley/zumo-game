using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

/// <summary>
/// A helper editor script for finding missing references to objects.
/// </summary>
public class MissingReferencesFinder : MonoBehaviour {
	const string MENU_ROOT = "Assets/Missing References/";

	/// <summary>
	/// Finds all missing references to objects in the currently loaded scene.
	/// </summary>
	[MenuItem(MENU_ROOT + "Search in scene", false, 50)]
	public static void FindMissingReferencesInCurrentScene () {
		FindMissingReferences(GetSceneObjects());
	}

	/// <summary>
	/// Finds all missing references to objects in all enabled scenes in the project.
	/// This works by loading the scenes one by one and checking for missing object references.
	/// </summary>
	[MenuItem(MENU_ROOT + "Search in all scenes", false, 51)]
	public static void MissingSpritesInAllScenes () {
		var extraScenes = new List<Scene>();
		var openScenes = FindLoadedScenes();

		foreach (var scene in EditorBuildSettings.scenes.Where(s => s.enabled)) {
			if (!openScenes.ContainsKey(scene.path)) {
				var openedScene = EditorSceneManager.OpenScene(scene.path, OpenSceneMode.Additive);
				extraScenes.Add(openedScene);
			}
		}

		FindMissingReferences(GetSceneObjects());

		foreach (var scene in extraScenes) {
			EditorSceneManager.CloseScene(scene, true);
		}
	}

	/// <summary>
	/// Finds all missing references to objects in assets (objects from the project window).
	/// </summary>
	[MenuItem(MENU_ROOT + "Search in assets", false, 52)]
	public static void MissingSpritesInAssets () {
		var allAssets = AssetDatabase.GetAllAssetPaths().Where(path => path.StartsWith("Assets/")).ToArray();
		var objs = allAssets.Select(a => AssetDatabase.LoadAssetAtPath(a, typeof(GameObject)) as GameObject).Where(a => a != null).ToArray();

		FindMissingReferences(objs);
	}

	static void FindMissingReferences (GameObject[] objects) {
		foreach (var go in objects) {
			var components = go.GetComponents<Component>();

			foreach (var c in components) {
				// Missing components will be null, we can't find their type, etc.
				if (!c) {
					Debug.LogError("Missing Component in GO: " + GetFullPath(go), go);
					continue;
				}

				SerializedObject so = new SerializedObject(c);
				var sp = so.GetIterator();

				// Iterate over the components' properties.
				while (sp.NextVisible(true)) {
					if (sp.propertyType == SerializedPropertyType.ObjectReference) {
						if (sp.objectReferenceValue == null
						    && sp.objectReferenceInstanceIDValue != 0) {
							var context = go.scene.name ?? "Project";

							ShowError(context, go, c.GetType().Name, ObjectNames.NicifyVariableName(sp.name));
						}
					}
				}
			}
		}
	}

	static GameObject[] GetSceneObjects () {
		// Use this method since GameObject.FindObjectsOfType will not return disabled objects.
		return Resources.FindObjectsOfTypeAll<GameObject>()
			.Where(go => string.IsNullOrEmpty(AssetDatabase.GetAssetPath(go))
		&& go.hideFlags == HideFlags.None).ToArray();
	}

	static void ShowError (string context, GameObject go, string componentName, string propertyName) {
		const string ERROR_TEMPLATE = "Missing reference in {3}: {0}. Component: {1}, Property: {2}";

		Debug.LogError(string.Format(ERROR_TEMPLATE, GetFullPath(go), componentName, propertyName, context), go);
	}

	static string GetFullPath (GameObject go) {
		return go.transform.parent == null
			? go.name
				: GetFullPath(go.transform.parent.gameObject) + "/" + go.name;
	}

	static Dictionary<string, Scene> FindLoadedScenes () {
		var scenes = new Dictionary<string, Scene>();

		for (var i = 0; i < EditorSceneManager.loadedSceneCount; i++) {
			scenes.Add(SceneManager.GetSceneAt(i).path, SceneManager.GetSceneAt(i));
		}

		return scenes;
	}
}
