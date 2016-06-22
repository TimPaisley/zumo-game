using System.Collections;
using System.Linq;
using UnityEngine;

namespace Zumo {
    class SplashScene : VirtualScene {
        [Header("Splash Scene")]
        public Camera greyFlashCamera;
        public VirtualScene nextScene;

        void Update() {
            if (gameManager.players.Any(player => player.input.confirm.isPressed)) {
                Unload();
                nextScene.Load();
            }
        }

        public override void Load() {
            base.Load();
            cameraManager.Use(greyFlashCamera, 0);

            StartCoroutine(useProperCamera());
        }

        public override void Unload() {
            base.Unload();
            cameraManager.StopShaking();
        }

        private IEnumerator useProperCamera() {
            yield return new WaitForSeconds(0.2f);

            cameraManager.Use(sceneCamera);
            cameraManager.Shake(1, 5);
        }
    }
}
