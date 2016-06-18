using UnityEngine;
using UnityEngine.SceneManagement;

namespace Zumo {
    abstract class VirtualScene : MonoBehaviour {
        public string sceneName;

        protected GameManager gameManager { get; private set; }
        protected CameraManager cameraManager { get; private set; }
        protected MusicManager musicManager { get; private set; }

        void Start() {
            SetupReferences();
        }

        public virtual void Load() {
            SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
        }

        public virtual void Unload() {
            SceneManager.UnloadScene(sceneName);
        }

        // Call this function if you override Start() in a subclass
        protected virtual void SetupReferences() {
            gameManager = FindObjectOfType<GameManager>();
            cameraManager = gameManager.cameraManager;
            musicManager = gameManager.musicManager;
        }
    }
}
