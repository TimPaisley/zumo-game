using UnityEngine;

namespace Zumo {
	public class AnimalMovement : MonoBehaviour {
		[Header("Movement")]
		public float minSpeed;
		public float maxSpeed;
		public float acceleration;

		[Header("Turning")]
		public float turnRate;
		public float slowAngle;
		public float turnDeceleration;
		public float turnMinSpeed;

		public float currentSpeed { get; private set; }

		Rigidbody rigidBody;

		void Awake () {
			rigidBody = GetComponent<Rigidbody>();
		}

		public void Rotate (Vector3 desiredDirection) {
			// Rotation speed is dependent on current speed
			transform.rotation = Quaternion.Lerp(
				transform.rotation,
				Quaternion.LookRotation(desiredDirection),
				Time.deltaTime * (turnRate / currentSpeed)
			);

			// Decelerate if turning
			if (Vector3.Angle(desiredDirection, transform.forward) > slowAngle && currentSpeed > turnMinSpeed) {
				currentSpeed = Mathf.Clamp(currentSpeed - turnDeceleration, turnMinSpeed, maxSpeed);
			}
		}

		public void Move (float minSpeed, float maxSpeed) {
			currentSpeed = Mathf.Clamp(currentSpeed + acceleration, minSpeed, maxSpeed);

			var movement = new Vector3(
				transform.forward.x * currentSpeed,
				rigidBody.velocity.y,
				transform.forward.z * currentSpeed
			);

			rigidBody.velocity = movement;
		}

		public void RemainStationary () {
			// Progressively lower the starting speed
			currentSpeed = Mathf.Clamp(currentSpeed - acceleration, minSpeed, maxSpeed);
		}
	}
}
