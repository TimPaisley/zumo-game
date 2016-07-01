using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System;

[InitializeOnLoad]
static class SceneAutoLoader {
	[Serializable]
	struct SceneSetupWrapper {
		public bool isLoaded, isActive;
		public string path;

		public SceneSetupWrapper(SceneSetup setup) {
			isLoaded = setup.isLoaded;
			isActive = setup.isActive;
			path = setup.path;
		}

		public SceneSetup Raw {
			get { return new SceneSetup { isActive = isActive, isLoaded = isLoaded, path = path }; }
		}
	}

	[Serializable]
	struct SceneManagerSetup {
		public SceneSetupWrapper[] setups;
	}

	// Static constructor binds a playmode-changed callback.
	// [InitializeOnLoad] above makes sure this gets executed.
	static SceneAutoLoader() {
		EditorApplication.playmodeStateChanged += OnPlayModeChanged;
	}

	// Menu items to select the "master" scene and control whether or not to load it.
	[MenuItem("Assets/Scene Autoload/Select Master Scene...")]
	static void SelectMasterScene () {
		string masterScene = EditorUtility.OpenFilePanel("Select Master Scene", Application.dataPath, "unity");
		if (!string.IsNullOrEmpty(masterScene)) {
			MasterScene = masterScene;
			LoadMasterOnPlay = true;
		}
	}

	[MenuItem("Assets/Scene Autoload/Load Master On Play", true)]
	static bool ShowLoadMasterOnPlay () {
		return !LoadMasterOnPlay;
	}

	[MenuItem("Assets/Scene Autoload/Load Master On Play")]
	static void EnableLoadMasterOnPlay () {
		LoadMasterOnPlay = true;
	}

	[MenuItem("Assets/Scene Autoload/Don't Load Master On Play", true)]
	static bool ShowDontLoadMasterOnPlay () {
		return LoadMasterOnPlay;
	}

	[MenuItem("Assets/Scene Autoload/Don't Load Master On Play")]
	static void DisableLoadMasterOnPlay () {
		LoadMasterOnPlay = false;
	}

	// Play mode change callback handles the scene load/reload.
	static void OnPlayModeChanged () {
		Debug.Log("Play mode state changed: " + EditorApplication.isPlaying + "; " + EditorApplication.isPlayingOrWillChangePlaymode);

		if (!LoadMasterOnPlay) {
			return;
		}

		if (!EditorApplication.isPlaying && EditorApplication.isPlayingOrWillChangePlaymode) {
			// User pressed play -- autoload master scene.
			PreviousSetup = EditorSceneManager.GetSceneManagerSetup();

			if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) {
				EditorSceneManager.OpenScene(MasterScene);

				for (var i = 0; i < SceneManager.sceneCount; i++) {
					if (SceneManager.GetSceneAt(i).path != MasterScene) {
						EditorSceneManager.CloseScene(SceneManager.GetSceneAt(i), true);
					}
				}
			} else {
				// User cancelled the save operation -- cancel play as well.
				EditorApplication.isPlaying = false;
			}
		}
		if (!EditorApplication.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode) {
			EditorSceneManager.RestoreSceneManagerSetup(PreviousSetup);
		}
	}

	// Properties are remembered as editor preferences.
	const string cEditorPrefLoadMasterOnPlay = "SceneAutoLoader.LoadMasterOnPlay";
	const string cEditorPrefMasterScene = "SceneAutoLoader.MasterScene";
	const string cEditorPrefPreviousScenes = "SceneAutoLoader.PreviousScenes";

	static bool LoadMasterOnPlay {
		get { return EditorPrefs.GetBool(cEditorPrefLoadMasterOnPlay, false); }
		set { EditorPrefs.SetBool(cEditorPrefLoadMasterOnPlay, value); }
	}

	static string MasterScene {
		get { return EditorPrefs.GetString(cEditorPrefMasterScene, "Master.unity"); }
		set { EditorPrefs.SetString(cEditorPrefMasterScene, value); }
	}

	static SceneSetup[] PreviousSetup {
		get {
			Debug.Log("Restoring setup: " + EditorPrefs.GetString(cEditorPrefPreviousScenes, ""));
			return parseSceneSetups(EditorPrefs.GetString(cEditorPrefPreviousScenes, ""));
		}
		set {
			Debug.Log("Saving setup: " + value.Length + dumpSceneSetups(value));
			EditorPrefs.SetString(cEditorPrefPreviousScenes, dumpSceneSetups(value));
		}
	}

	static string dumpSceneSetups(SceneSetup[] setups) {
		var wrappedSetups = setups.Select(setup => new SceneSetupWrapper(setup));

		return JsonUtility.ToJson(new SceneManagerSetup { setups = wrappedSetups.ToArray() });
	}

	static SceneSetup[] parseSceneSetups(string json) {
		var wrappedSetups = JsonUtility.FromJson<SceneManagerSetup>(json).setups;

		return wrappedSetups.Select(setup => setup.Raw).ToArray();
	}
}
