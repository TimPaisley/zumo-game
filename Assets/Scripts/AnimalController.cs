using UnityEngine;
using System.Collections;

public class AnimalController : MonoBehaviour {

	// Global References
	private GameManager gm;

	// Local References
	private Rigidbody rb;
	private Animator anim;
	private AudioSource dashSound;
	private PowerUpDisplay puDisplay;

    // Control Variables
    public Renderer board;
	public float maxSpeed = 6.0f;
	public float minSpeed = 1.0f;
    public float minTurnSpeed = 6.0f;
	public float acceleration = 0.01f;
    public float decceleration = -0.01f;
	public float dashSpeed = 2.0f;
    public float dashMass = 5.0f;
	public float dashCooldown = 1.0f;
	public float dashLength = 0.5f;
	public float knockBackDelay = 0.2f;
    public float slowAngle;
    public float turnRate = 5.0f;

    // Management Variables
    public float speed;
	private bool knockedBack;
	private float knockBackTimer;
    private int stationaryDelay = 0;
    

	// Raycast Variables
	private RaycastHit hit;
	private Ray downRay;

	// State properties
	public bool isDashing { get; private set; }
	public float dashLengthRemaining { get; private set; }
	public float dashCooldownRemaining { get; private set; }

    //Power up Variables
    private float originalMass;
    private float originalMaxSpeed;
    private float originalMinSpeed;
    private ArrayList powerUpQueue;

    void Start () {
		// Initialize Global References
		gm = FindObjectOfType<GameManager>();

		// Initialize Local References
		rb = GetComponent<Rigidbody> ();
		anim = GetComponentInChildren<Animator> (); // GetComponent<Animator>() when new fox is imported
		dashSound = GetComponent<AudioSource> ();
		puDisplay = GetComponent<PowerUpDisplay> ();

		// Set initial variables
		speed = minSpeed;

        //Store original mass and speed
        originalMass = rb.mass;
        originalMaxSpeed = maxSpeed;
        originalMinSpeed = minSpeed;
        powerUpQueue = new ArrayList();
    }

	void FixedUpdate () {
		if (isDashing) {
			dashLengthRemaining -= Time.deltaTime;

			if (dashLengthRemaining <= 0) {
				dashLengthRemaining = 0;
				isDashing = false;
				dashCooldownRemaining = dashCooldown;
			}
		} else {
			dashCooldownRemaining -= Time.deltaTime;

			if (dashCooldownRemaining < 0) {
				dashCooldownRemaining = 0;
			}
		}
	}

	void Update () {
		// Process knockback time
		if (knockedBack) {
			knockBackTimer -= Time.deltaTime;

			// Reset knockback
			if (knockBackTimer <= 0.0f) {
				knockedBack = false;
				knockBackTimer = knockBackDelay;
			}
		}
        int index = -1; 
        // The index of the power up that needs to be removed
        for (int i = 0; i < powerUpQueue.Count; i++)
        //Check if powerup run out of time
        {
            PowerUpHistory ph = (PowerUpHistory)powerUpQueue[i];
			float currTicker = ph.getTicker();
			if (currTicker < 0.0f)
            {
                removePowerUp(ph.getPuType());
                index = i;     
            }
			puDisplay.updateTimer(ph.getPuType(),currTicker);
        }
        if(index != -1)
        {
            powerUpQueue.RemoveAt(index);
        }
    }

	public void Move (float v, float h) {
		if (!knockedBack && isGrounded) {
			// Create movement vector
			var movement = new Vector3 (h, 0, v);
            float currentAcceleration = acceleration;

            // If moving, look in the direction of movement
            if (h != 0 || v != 0)
            {
                //rotate speed is dependent on current speed
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(new Vector3(movement.x, 0.0f, movement.z)), Time.deltaTime * (turnRate/speed));

                //deccelerate if turning
                if (Vector3.Angle(movement,transform.forward)>slowAngle&&speed>minTurnSpeed) {
                     speed = Mathf.Max(speed - 2, minSpeed);
                      currentAcceleration = decceleration;
                 }


               //set movement to new rotation
                movement = transform.forward;
                anim.SetBool("isMoving", true);

                //set animationspeed to moving speed
                float speedIncrem = 1 / maxSpeed*1.5f;
                anim.speed = Mathf.Max(speed*speedIncrem, 0.7f);
            }
            else
            {
                anim.SetBool("isMoving", false);
            }

            

            // Apply dashing speed
            if (isDashing) {
				movement *= dashSpeed;
                rb.mass = dashMass;
			} else {
                rb.mass = originalMass;
            }

			// Adjust Y movement to account for gravity
			movement.y = rb.velocity.y / speed;

			// Set moving speed
			if(h != 0 || v != 0){
				speed = Mathf.Min(speed + currentAcceleration, maxSpeed);
                if (speed<minSpeed) { speed = minSpeed; }
			}

			// Set stationary speed
			else {
				speed = Mathf.Max(speed - acceleration, minSpeed);
			}

            // Apply changes in velocity
            rb.velocity = movement * speed;

            //remove speed if player is stationary
            if (rb.velocity == new Vector3(0.0f, 0.0f, 0.0f))
            {
                stationaryDelay++;
                if (stationaryDelay == 5)
                {
                    speed = minSpeed;
                }
            }
            else { stationaryDelay = 0; }

		}
	}

    public void Dash () {
        if (!isDashing && dashCooldownRemaining == 0) {
            isDashing = true;
            dashLengthRemaining = dashLength;

            dashSound.PlayOneShot(dashSound.clip);
        }
    }

    public void Kill () {
        anim.Stop();
        throwOutOfBounds();
    }

    public bool isInBounds {
        get {
            // Check whether a raycast straight down hits the ground
            var raycastHit = raycast(new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z), Vector3.down);

			return raycastHit.HasValue && Tags.HasAnyTag(raycastHit.Value.collider.gameObject, Tags.BoardObjects);
        }
    }

    void OnCollisionEnter (Collision collision) {
		// If this collides with another animal, bounce away
		if (collision.transform.tag == "Animal") {
			BounceAway (collision.transform.position);
		}
        // If the collides with power up item
        else if (collision.transform.tag == "PowerUp")
        {
            PowerUp pu = collision.gameObject.GetComponent<PowerUp>();
            string currentPower = pu.getPowerUpType();
            PowerUpHistory nph = new PowerUpHistory(currentPower);
            for(int i = 0; i<powerUpQueue.Count; i++)
            // if the power up is already active on the animal restart the timer
            {
                PowerUpHistory ph = (PowerUpHistory)powerUpQueue[i];
                if (ph.getPuType().Equals(currentPower))
                {
                    Debug.LogWarning("Extended: " + currentPower + " timer");
                    ph.restTicker();
                    Destroy(collision.gameObject);
					puDisplay.displayPowerUp(currentPower);
                    return;
                }
            }
            powerUpQueue.Add(nph);
            Debug.LogWarning("Apply: " + currentPower + " powerup");
            if (currentPower.Equals("mass"))
            {
                rb.mass = rb.mass * pu.getMassMultiplie();
            }
            else if (currentPower.Equals("speed"))
            {
                minSpeed *= pu.getSpeedMulti();
                maxSpeed *= pu.getSpeedMulti();
				speed = maxSpeed;
            }
            Destroy(collision.gameObject);
			puDisplay.displayPowerUp(currentPower);
        }

    }

    public void removePowerUp(string currentPower)
    {
        Debug.LogWarning("Remove: "+ currentPower +" change");
        if (currentPower.Equals("mass"))
        {
            rb.mass = originalMass;
        }
        else if (currentPower.Equals("speed"))
        {
            minSpeed = originalMinSpeed;
            maxSpeed = originalMaxSpeed;
			speed = minSpeed;
        }
    }

	public void BounceAway (Vector3 otherPos) {
		// Calculate vector away from collision object
		Vector3 awayDir = (transform.position - otherPos);

		// Calculate vector between direction and Y-axis (upwards)
		Vector3 dir = new Vector3 (awayDir.x, 0.0f, awayDir.z).normalized + new Vector3 (0, 1, 0);

		// Add impulse force in that direction
		rb.AddForce (dir * gm.bounceForce, ForceMode.Impulse);

		// Allow player to leave the ground
		knockedBack = true;
	}

	private bool isGrounded {
        get {
            var raycastHit = raycast(new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z), Vector3.down);

            return raycastHit.HasValue && raycastHit.Value.distance < 1.0f;
        }
	}

    private RaycastHit? raycast (Vector3 origin, Vector3 direction) {
        // Set up raycast variables
        downRay = new Ray(origin, direction);

        // Draw raycast ray [DEBUG]
        Debug.DrawRay(origin, direction);
		
		if (Physics.Raycast(downRay, out hit)) {
			return hit;
		} else {
			return null;
		}
    }

    private void throwOutOfBounds () {
        var boardPos = board.transform.position;

        rb.freezeRotation = false;
        rb.mass = 10;
        rb.AddForce(Vector3.Normalize(transform.position - boardPos) * 100 + Vector3.up * 200);
        rb.AddTorque(1000, 500, 1000);
    }

    public class PowerUpHistory{
        private string pu = "";
        private float ticker = 10.0f;

        public PowerUpHistory(string p)
        {
            pu = p;
        }
        public float getTicker()
        {
            ticker -= Time.deltaTime;
            return ticker;
        }
        public void restTicker()
        {
            ticker = 10.0f;
        }
        public string getPuType()
        {
            return pu;
        }
    }
}
