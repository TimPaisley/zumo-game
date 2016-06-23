using UnityEngine;
using UnityEngine.SceneManagement;

namespace Zumo {
    class SceneLoader : MonoBehaviour {
        public string sceneFilename;

        public void Load() {
            SceneManager.LoadScene(sceneFilename, LoadSceneMode.Additive);
        }

        public virtual void Unload() {
            SceneManager.UnloadScene(sceneFilename);
        }
    }
}
