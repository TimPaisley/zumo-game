using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Zumo.InputHelper;

namespace Zumo {
    class PlayerController {
        public PlayerController(int index, InputMap input) {
            this.index = index;
            this.input = input;

            isReady = false;
        }

        public int index { get; private set; }
        public InputMap input { get; private set; }
        
        public bool isReady { get; set; }
    }
}
