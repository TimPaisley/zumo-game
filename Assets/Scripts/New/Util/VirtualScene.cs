using UnityEngine;
using UnityEngine.SceneManagement;

namespace Zumo {
    abstract class VirtualScene : MonoBehaviour {
        [Header("Base Scene")]
        public string sceneFilename;
        public Camera sceneCamera;

        protected GameManager gameManager { get; private set; }
        protected CameraManager cameraManager { get; private set; }
        protected MusicManager musicManager { get; private set; }

        void Start() {
            Setup();
        }

        public virtual void Load() {
            SceneManager.LoadScene(sceneFilename, LoadSceneMode.Additive);
            cameraManager.Use(sceneCamera);
        }

        public virtual void Unload() {
            SceneManager.UnloadScene(sceneFilename);
        }

        // Call this function if you override Start() in a subclass
        protected virtual void Setup() {
            gameManager = FindObjectOfType<GameManager>();
            cameraManager = gameManager.cameraManager;
            musicManager = gameManager.musicManager;

            sceneCamera.enabled = false;
        }
    }
}
