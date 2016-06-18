using System.Linq;

namespace Zumo {
    class SplashScene : VirtualScene {
        public VirtualScene nextScene;

        void Update() {
            if (gameManager.players.Any(player => player.input.confirm.isPressed)) {
                Unload();
                nextScene.Load();
            }
        }

        public override void Load() {
            base.Load();
            cameraManager.Use(cameraManager.splashCamera);
            cameraManager.Shake(1, 5);
        }

        public override void Unload() {
            base.Unload();
            cameraManager.StopShaking();
        }
    }
}
