using UnityEngine;

namespace Zumo {
    public class GroundedChecker : MonoBehaviour {
        const float GROUND_LEVEL = 1.7f;
        const float FAKE_GROUNDED_VELOCITY_THRESHOLD = 0.1f;
        const float FAKE_GROUNDED_TIMEOUT = 1f;
        
        Rigidbody rigidBody;
        float fakeGroundedStartTime = 0;

        public bool isGrounded {
            get { return atGroundLevel || fakeGroundedTimeoutFinished; }
        }

        // State

        bool atGroundLevel {
            get { return transform.position.y <= GROUND_LEVEL; }
        }

        bool movingVertically {
            get { return Mathf.Abs(rigidBody.velocity.y) > FAKE_GROUNDED_VELOCITY_THRESHOLD; }
        }

        bool fakeGroundedTimeoutStarted {
            get { return fakeGroundedStartTime != 0; }
        }

        bool fakeGroundedTimeoutFinished {
            get { return fakeGroundedTimeoutStarted && (Time.fixedTime - fakeGroundedStartTime) > FAKE_GROUNDED_TIMEOUT;  }
        }

        // Lifecycle

        void Awake () {
            rigidBody = GetComponent<Rigidbody>();
        }

        void FixedUpdate () {
            if (fakeGroundedTimeoutStarted) {
                if (atGroundLevel || movingVertically) {
                    fakeGroundedStartTime = 0;
                }
            } else {
                if (!atGroundLevel && !movingVertically) {
                    fakeGroundedStartTime = Time.fixedTime;
                }
            }
        }
    }
}
