using UnityEngine;
using System.Collections;

[RequireComponent(typeof(DashController), typeof(PowerUpController))]
public class AnimalController : MonoBehaviour {
	private const float ALMOST_ZERO = 0.15f;

	// Global References
	private GameManager gm;

	// Local References
	public Rigidbody rb;
	public Animator anim;
	private DashController dashController;
	private PowerUpController powerupController;
	private BombController bombController;

	// Control Variables
	public Renderer board; //TODO remove; no longer used

	[Header("Movement")]
	public float minSpeed = 1.0f;
	public float maxSpeed = 6.0f;
	public float minTurnSpeed = 6.0f;
	public float acceleration = 0.01f;
	public float decceleration = 0.1f;
	[Header("Turning")]
	public float slowAngle;
	public float turnRate = 5.0f;
	[Header("Other")]
	public float knockBackDelay = 0.2f;
	private float backLash = 1.0f;
	public AudioSource hitSound;
	public AudioSource stopSound;
	public AudioSource speedSound;
	public AudioSource massSound;
	private AudioSource[] animalSound;

	// Management Variables
	private float baseMass;
	public bool knockedBack;
	private float knockBackTimer;
	private int stationaryDelay = 0;
	private int index = 0;
	private bool fakeGrounded;
    private bool inBounds = true;

	// Raycast Variables
	private RaycastHit hit;
	private Ray downRay;

	//Ability related fields
	//If panda ability is active, this is used due to how mass is handled currently
	public bool pandaAbility = false;
    public bool foxAbility = false;
    public bool tigerAbility = false;
	public bool lionAbility = false;
    public bool stopPowerup = false;
    public bool disableControl = false;
    public float elephantSpeedMultiplier = 1;// used to change the elephant's speed

	public bool isDashing {
		get { return dashController.isDashing; }
	}
	public bool isDashCharging {
		get { return dashController.dashIsCharging; }
	}
	public float currentMaxSpeed {
		get { return maxSpeed * powerupController.speedMultiplier* elephantSpeedMultiplier; }
	}
	public float currentMass {
		get { return baseMass * powerupController.massMultiplier + dashController.massIncrease; }
	}


	public float speed { get; private set; }

	public bool isInBounds {
		get {
            return inBounds;
			// Check whether a raycast straight down hits the ground
			//var raycastHit = raycast(new Vector3 (transform.position.x, transform.position.y + 0.5f, transform.position.z), Vector3.down);

			//return raycastHit.HasValue && Tags.HasAnyTag(raycastHit.Value.collider.gameObject, Tags.BoardObjects);
		}
	}

	void Awake() {
		// Initialize Global References
		gm = FindObjectOfType<GameManager>();

		// Initialize Local References
		rb = GetComponent<Rigidbody>();
		anim = GetComponentInChildren<Animator>(); // GetComponent<Animator>() when new fox is imported

		dashController = GetComponent<DashController>();
		powerupController = GetComponent<PowerUpController>();
		animalSound = GetComponents<AudioSource> ();

		baseMass = rb.mass;
		speed = minSpeed;

		//set up hitSound
		hitSound.ignoreListenerVolume = true;
	}

	void Update() {

        // Make sure mass is kept up to date
        if (pandaAbility) {
			//The panda's ability is implemented here because the current 
			//implementation updates mass every frame
			//TODO Move the ability out // might require to change how mass is handled
			rb.mass = 1000;
		} else {
			rb.mass = currentMass;
		}

		// Update animator
		if (rb.velocity.magnitude > ALMOST_ZERO || dashController.dashIsCharging) {
			anim.SetBool("isMoving", true);
			anim.speed = Mathf.Max(speed / 10, 0.7f);
		} else {
			anim.SetBool("isMoving", false);
		}

		// Process knockback time
		if (knockedBack) {
			knockBackTimer -= Time.deltaTime;

			// Reset knockback
			if (knockBackTimer <= 0.0f) {
				knockedBack = false;
				knockBackTimer = knockBackDelay;
			}
		}
	}

	void OnTriggerExit(Collider other){
		if(other.transform.tag=="Environment"){
			fakeGrounded = false;
		}
	}

	void OnTriggerEnter(Collider other) {
        if (other.transform.tag=="outOfBounds") {
            inBounds = false;
        }
		if(other.transform.tag == "Environment"){
			fakeGrounded=true;
		}
		if (other.transform.tag == "PowerUp") {
			if (other.gameObject.GetComponent<PowerUp> ().PuType == "bomb") {
				FindObjectOfType<BombController> ().Deploy ();
			} else {
				powerupController.Apply(other.gameObject.GetComponent<PowerUp>());
				string puType = other.gameObject.GetComponent<PowerUp> ().PuType;
				if(puType=="speed"){
					speedSound.PlayOneShot(speedSound.clip);
				}
				else if(puType=="mass"){
					massSound.PlayOneShot(massSound.clip);
				}
				else if(puType=="stop"){
					stopSound.PlayOneShot(stopSound.clip);
				}
			}

			Destroy(other.gameObject);
		}
    	// If this collides with another animal, bounce away and display particle
    	if (!knockedBack && other.transform.tag == "AnimalHead" && !foxAbility) {
			//get the animalobject from the collision
			AnimalController otherAnimal = other.GetComponentInParent<AnimalController>();
		
			// Calculate vector away from collision object
			Vector3 awayDir = (rb.velocity.normalized + otherAnimal.rb.velocity.normalized);
           // Vector3 awayDir = (transform.position - otherAnimal.transform.position);

            // Calculate vector between direction and Y-axis (upwards)
            Vector3 dir = new Vector3 (awayDir.x, 0.0f, awayDir.z).normalized + new Vector3 (0, 1, 0);


			float oppSpeed = Mathf.Max(otherAnimal.speed/2,5);
			float oppMass = otherAnimal.currentMass;


			float sizeOfHit = (dir * oppSpeed* oppMass * gm.bounceForce).magnitude;
			float volume = 1.0f;
			Debug.Log ("size of hit: "+sizeOfHit);

			//playhitting sound
			volume = sizeOfHit/50;

			hitSound.volume = volume;
			hitSound.PlayOneShot(hitSound.clip);

            // Add impulse force in that direction
            if (!tigerAbility)// in tiger ability don't perform impulse
            {
                rb.velocity = Vector3.zero;
                rb.AddForce(dir * oppSpeed * oppMass * gm.bounceForce, ForceMode.Impulse);
            }

            // Allow player to leave the ground
            knockedBack = true;
			Debug.Log ("size of hit: "+sizeOfHit);
			Debug.Log ("upwardMomentum when hit:"+(rb.velocity).y);


			Debug.Log ("is dashing?: "+ isDashing);

				//make other animal bounce back
				//otherAnimal.GetComponent<Rigidbody>().velocity = Vector3.zero;
				//Vector3 otherAwayDir = (otherAnimal.transform.position - transform.position);
				//Vector3 otherDir = new Vector3 (otherAwayDir.x, 0.0f, otherAwayDir.z).normalized + new Vector3 (0, 1, 0);
				//otherAnimal.GetComponent<Rigidbody>().AddForce(otherDir*oppSpeed*otherAnimal.backLash* gm.bounceForce, ForceMode.Impulse);
				//otherAnimal.knockedBack = true;

				//Debug.Log ("upwardMomentum when hitting:"+(otherAnimal.GetComponent<Rigidbody>().velocity).y);
				//Debug.Log ("size of recoil: "+(otherDir*oppSpeed*backLash* gm.bounceForce).magnitude);
				makeRandomNoise();
                // if (!otherAnimal.tigerAbility)// if the other animal is using tigerability ignore
                otherAnimal.recoil(transform.position, oppSpeed);

             if (isDashing) {
				dashController.Stop();
			}
			if(otherAnimal.isDashing){
				otherAnimal.dashController.Stop();
			}
		}
	}

	public void recoil(Vector3 otherAnimal,float oppSpeed){
		//make other animal bounce back
		rb.velocity = Vector3.zero;
        Vector3 awayDir = -transform.forward;
        Vector3 otherDir = new Vector3(awayDir.x, 0.0f, awayDir.z).normalized + new Vector3(0, 1, 0);

        //Vector3 otherDir = (-rb.velocity.normalized * backLash) + Vector3.up;

        rb.AddForce(otherDir * oppSpeed * currentMass * gm.bounceForce, ForceMode.Impulse);
        knockedBack = true;

		Debug.Log ("upwardMomentum when hitting:"+(rb.velocity).y);
		Debug.Log ("size of recoil: "+(otherDir*oppSpeed*backLash* gm.bounceForce).magnitude);

	}

	public void makeRandomNoise(){
		//animalSound [index].Stop ();
		index = (int)(3*Random.value);
		Debug.Log (index);
		animalSound[index].PlayOneShot(animalSound[index].clip);
	}


	private bool isGrounded {
		get {
			Vector3 extents = GetComponent<BoxCollider> ().bounds.extents;

			RaycastHit centerHit;
			Physics.Raycast(new Vector3 (transform.position.x, transform.position.y + 0.5f, transform.position.z), Vector3.down, out centerHit);

			RaycastHit centerLeftHit;
			Physics.Raycast(new Vector3 (transform.position.x - extents.x, transform.position.y + 0.5f, transform.position.z), Vector3.down, out centerLeftHit);

			RaycastHit centerRightHit;
			Physics.Raycast(new Vector3 (transform.position.x + extents.x, transform.position.y + 0.5f, transform.position.z), Vector3.down, out centerRightHit);
           // Debug.DrawRay(new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z+extents.z), -transform.forward);

            RaycastHit topLeftHit;
			Physics.Raycast(new Vector3 (transform.position.x - extents.x, transform.position.y + 0.5f, transform.position.z + extents.z), Vector3.down, out topLeftHit);

			RaycastHit topRightHit;
			Physics.Raycast(new Vector3 (transform.position.x + extents.x, transform.position.y + 0.5f, transform.position.z + extents.z), Vector3.down, out topRightHit);

			RaycastHit bottomLeftHit;
			Physics.Raycast(new Vector3 (transform.position.x - extents.x, transform.position.y + 0.5f, transform.position.z - extents.z), Vector3.down, out bottomLeftHit);

			RaycastHit bottomRightHit;
			Physics.Raycast(new Vector3 (transform.position.x + extents.x, transform.position.y + 0.5f, transform.position.z - extents.z), Vector3.down, out bottomRightHit);

            // return topLeftHit.HasValue && topLeftHit.Value.distance < 1.0f;
           // if (centerRightHit.collider.tag == "Environment") {
              //  return true;
           // };
			return 	(centerHit.distance < 1.0f || centerLeftHit.distance < 1.0f || centerRightHit.distance < 1.0f || topLeftHit.distance < 1.0f || topRightHit.distance < 1.0f || bottomLeftHit.distance < 1.0f || bottomRightHit.distance < 1.0f);
		}
	}

	void OnCollisionEnter(Collision collision) {
		//if it collides with terrain
		if (collision.transform.tag == "Environment") {
			if(isDashing){
				dashController.Stop();
			}
		}

		//if it collides with powerup
		if (collision.transform.tag == "PowerUp") {
			
			Destroy(collision.gameObject);
		}


        if (foxAbility && collision.transform.tag == "Animal")
        {
            BoxCollider bc = this.GetComponent<BoxCollider>();
            Physics.IgnoreCollision(bc, collision.collider);
        }
    }

	public void Rotate(float v, float h) {
		if (!disableControl) { // Disable control when actived panda ability
			
			if (v == 0 && h == 0) {
				return;
			}

			if (!knockedBack && isGrounded) {
				var movement = new Vector3 (h, 0, v);

				//rotate speed is dependent on current speed
				transform.rotation = Quaternion.Lerp (transform.rotation, Quaternion.LookRotation (new Vector3 (movement.x, 0.0f, movement.z)), Time.deltaTime * (turnRate / speed));

				//deccelerate if turning
				if (Vector3.Angle (movement, transform.forward) > slowAngle && speed > minTurnSpeed) {
					speed = Mathf.Max (speed - decceleration, minSpeed);
				}
			}
		}
	}

	public void Move(float inputMagnitude) {
		if (!disableControl) { // Disable control when actived panda ability
			
			if (inputMagnitude == 0&&!isDashing) {
				return;
			}

			var maxMovementSpeed = currentMaxSpeed * inputMagnitude;
			var movement = new Vector3 (transform.forward.x, rb.velocity.y, transform.forward.z);

			if (!knockedBack && isGrounded) {
				// Adjust Y movement to account for gravity
				movement.y = rb.velocity.y / speed;

				// Apply changes in velocity
				if (isDashing) {
					// Make sure player doesn't fly upwards on slopes
					movement.y = Mathf.Clamp (movement.y, 0, 0.01f);

					rb.velocity = movement * dashController.dashSpeed;
				} else {
					speed = Mathf.Min (speed + acceleration, maxMovementSpeed);

					//cancel out weird super high bounce bug
					if(movement.y>50.0f||movement.y<-50.0f){
						movement.y = 0.0f;
					}

					rb.velocity = movement * speed;
				}

				//remove speed if player is stationary
				if (rb.velocity == Vector3.zero) {
					stationaryDelay++;
					if (stationaryDelay == 5) {
						speed = minSpeed;
					}
				} else {
					stationaryDelay = 0;
				}
			}
		}
	}

	public void StartDashCharge() {
		if (!disableControl) {
			if (dashController.StartDashCharge ()) {
				rb.velocity = Vector3.zero;
			}
		}
	}

	public void PerformDash() {
		if (!disableControl) { // Disable control when actived panda ability
			dashController.PerformDash();
		}
	}


	public void halt(){
		rb.velocity = Vector3.zero;
		knockedBack = true;
	}
	public void PerformAbility(){
		AnimalAbility ability = GetComponent<AnimalAbility>();
		ability.applyAbility ();
	}

	public void Kill() {
		anim.Stop();
		powerupController.RemoveAll();
		throwOutOfBounds();
	}

	public void removePowerUps(){
		powerupController.RemoveAll();
	}

	private RaycastHit? raycast(Vector3 origin, Vector3 direction) {
		// Set up raycast variables
		downRay = new Ray (origin, direction);

		// Draw raycast ray [DEBUG]
		Debug.DrawRay(origin, direction);
		
		if (Physics.Raycast(downRay, out hit)) {
			return hit;
		} else {
			return null;
		}
	}

	private void throwOutOfBounds() {
		var boardPos = gm.currentBoard.transform.position;

		rb.freezeRotation = false;
		rb.mass = 10;
		rb.AddForce(Vector3.Normalize(transform.position - boardPos) * 100 + Vector3.up * 200);
		rb.AddTorque(1000, 500, 1000);
	}
}
