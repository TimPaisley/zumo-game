using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zumo.InputHelper;

namespace Zumo {
    class GameOptions {
        public SceneLoader gameScene;

        public void Reset() {
            gameScene = null;
        }
    }
}
