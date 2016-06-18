using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Zumo {
    abstract class VirtualScene : MonoBehaviour {
        public delegate void SceneEventHandler();

        public GameManager gameManager { get; private set; }
        public CameraManager cameraManager { get; private set; }

        void Start() {
            gameManager = FindObjectOfType<GameManager>();
            cameraManager = gameManager.cameraManager;
        }
        
        public event SceneEventHandler Activated;

        public void Activate() {
            if (Activated != null) Activated();
        }
    }
}
