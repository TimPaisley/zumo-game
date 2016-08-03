using UnityEngine;
using Zumo.InputHelper;

namespace Zumo {
	public class Animal : MonoBehaviour {
		// Configuration

		public float animationSpeedMultiplier = 0.1f;
		public float minAnimationSpeed = 0.7f;

		public Player player { get; set; }
		public bool isAlive { get; private set; }
		public bool isActive { get; set; }

		// Local References

		public Rigidbody rigidBody { get; private set; }

		BoxCollider boxCollider;
		Animator animator;
		float baseMass;

		AnimalMovement movement;
		AnimalBouncing bouncer;
		AnimalDash dash;
		AnimalPickups pickups;
		AnimalAbility ability;

		// State

		float currentMass {
			get { return baseMass + dash.massIncrease + pickups.massIncrease + ability.massIncrease; }
		}

        float currentMinSpeed {
            get { return movement.baseMinSpeed + dash.speedIncrease + pickups.speedIncrease; }
        }

        float currentMaxSpeed {
            get { return movement.baseMaxSpeed + dash.speedIncrease + pickups.speedIncrease; }
        }

		InputMap input {
			get { return player.input; }
		}

		bool isGrounded {
			get { return Physics.CheckBox(boxCollider.center + (Vector3.up * -0.5f), boxCollider.size / 2); }
		}

		bool isControllable {
			get { return isGrounded && !bouncer.knockedBack && !ability.disableControl && !pickups.disableControl; }
		}

		bool canRotate {
			get { return isControllable && !dash.isDashing; }
		}

		bool wantsToRotate {
			get { return input.joystick.isPressed; }
		}

		bool canMove {
			get { return isControllable && !dash.isCharging; }
		}

		bool wantsToMove {
			get { return dash.isDashing || input.joystick.isPressed; }
		}

		bool canStartDashCharging {
			get { return isControllable && dash.isInactive; }
		}

		bool canStartDashing {
			get { return dash.isCharging; }
		}

		bool canPerformAbility {
			get { return isControllable && ability.canBePerformed && !dash.isCharging && !dash.isDashing; }
		}

		bool canBounceBack { get { return !bouncer.knockedBack; } }

		bool shouldRecoil { get { return !ability.disableRecoil; }}

		// Lifecycle

		void Awake () {
			rigidBody = GetComponent<Rigidbody>();
			boxCollider = GetComponent<BoxCollider>();
			animator = GetComponentInChildren<Animator>();

			movement = GetComponent<AnimalMovement>();
			bouncer = GetComponent<AnimalBouncing>();
			dash = GetComponent<AnimalDash>();
			pickups = GetComponent<AnimalPickups>();
			ability = GetComponent<AnimalAbility>();

			baseMass = rigidBody.mass;

			isAlive = true;
		}

		void Update () {
			if (isAlive && isActive) {
				updateComponentState();

				if (canRotate && wantsToRotate) {
					movement.Rotate(new Vector3(input.joystick.xAxis, 0, input.joystick.yAxis));
				}

				if (canMove && wantsToMove) {
					var actualMaxSpeed = dash.isDashing ? currentMaxSpeed : currentMaxSpeed * Mathf.Clamp(input.joystick.magnitude, 0, 1);
                    
                    movement.Move(currentMinSpeed, actualMaxSpeed);
				} else {
					movement.RemainStationary();
				}

				if (canStartDashCharging && input.dash.isPressed) {
					dash.StartDashCharging();
				}

				if (canStartDashing && input.dash.wasReleased) {
					dash.StartDashing();
				}

				if (canPerformAbility && input.ability.isPressed) {
					ability.Perform();
				}
			}
		}

		void OnTriggerEnter (Collider other) {
			if (other.CompareTag(Tags.OutOfBounds)) {
				kill();
			} else if (other.CompareTag(Tags.Pickup)) {
				pickups.PickUp(other.GetComponent<Pickup>());
			} else if (other.CompareTag(Tags.AnimalHead) && canBounceBack) {
				var otherAnimal = other.GetComponentInParent<Animal>();

				bouncer.BounceAwayFrom(otherAnimal);
				dash.StopIfDashing();

				otherAnimal.RecoilFrom(this);
			}
		}

		void OnCollisionEnter (Collision collision) {
			var other = collision.gameObject;

			if (other.CompareTag(Tags.Environment)) {
				dash.StopIfDashing();
			}
		}

		public void RecoilFrom (Animal other) {
			if (shouldRecoil) {
				bouncer.RecoilFrom(this);
				dash.StopIfDashing();
			}
		}

		void updateComponentState () {
			rigidBody.mass = currentMass;

			if (canMove && wantsToMove) {
				animator.SetBool("isMoving", true);
				animator.speed = Mathf.Max(movement.currentSpeed * animationSpeedMultiplier, minAnimationSpeed);
			} else {
				animator.SetBool("isMoving", false);
			}
		}

		void kill () {
			isAlive = false;
			throwOutOfBounds();
		}

		void throwOutOfBounds() {
			rigidBody.freezeRotation = false;
			rigidBody.mass = 10;
			rigidBody.AddForce(transform.position.normalized * 100 + Vector3.up * 200);
			rigidBody.AddTorque(1000, 500, 1000);
		}

		RaycastHit? raycast(Vector3 origin, Vector3 direction) {
			RaycastHit hit;

			var ray = new Ray(origin, direction);

			if (Physics.Raycast(ray, out hit)) {
				return hit;
			} else {
				return null;
			}
		}

		float raycastDistance(Vector3 origin, Vector3 direction) {
			var hit = raycast(origin, direction);

			return hit.HasValue ? hit.Value.distance : float.PositiveInfinity;
		}
	}
}
