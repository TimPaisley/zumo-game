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
	public bool dashIsCharging { get; private set; }
	public float dashCharger; //{ get; private set;}
	public bool isDashing { get; private set; }
	public float dashLengthRemaining { get; private set; }
	public float dashCooldownRemaining { get; private set; }

    //Power up Variables
    private float originalMass;
    private float originalMaxSpeed;
    private float originalMinSpeed;
	private float originalDashCD;

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

        //Store original mass and speed and dash cooldown
        originalMass = rb.mass;
        originalMaxSpeed = maxSpeed;
        originalMinSpeed = minSpeed;
		originalDashCD = dashCooldown;
       
    }

	void FixedUpdate () {
		if(dashIsCharging){
			anim.SetBool ("isMoving", true);
			dashCharger += Time.deltaTime;
			float cap = Mathf.Min(dashCharger, 4.0f);
			//set animationspeed to moving speed
			anim.speed = Mathf.Max(cap, 0.7f);
			if(dashCharger>5.0){
				dashCharger = 5;
			}
		}
		if (isDashing) {
			dashLengthRemaining -= Time.deltaTime;
			if (dashLengthRemaining <= 0) {
				dashLengthRemaining = 0;
				isDashing = false;
				speed = minSpeed;
				dashCooldownRemaining = dashCooldown;
				puDisplay.displayPowerUp ("dash");
			} 
		} else {
			dashCooldownRemaining -= Time.deltaTime;
			puDisplay.updateDashTimer(dashCooldownRemaining);

			if (dashCooldownRemaining < 0.0f) {
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
      
    }

	public void Move (float v, float h) {



		if (!knockedBack && isGrounded) {
			// Create movement vector
			var movement = new Vector3 (h, 0, v);
            float currentAcceleration = acceleration;

            // If moving, look in the direction of movement
			if (h != 0 || v != 0||isDashing)
            {

				if(isDashing){
					rb.mass = Mathf.Max(originalMass, dashMass);
					movement = transform.forward;
					// Adjust Y movement to account for gravity
					movement.y = rb.velocity.y - dashSpeed;
				}
				else{
                	//rotate speed is dependent on current speed
                	transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(new Vector3(movement.x, 0.0f, movement.z)), Time.deltaTime * (turnRate/speed));

                	//deccelerate if turning
                	if (Vector3.Angle(movement,transform.forward)>slowAngle&&speed>minTurnSpeed) {
                     	speed = Mathf.Max(speed - 2, minSpeed);
                      	currentAcceleration = decceleration;
                 	}

				
               		//set movement to new rotation
                	movement = transform.forward;
					rb.mass = originalMass;
					// Adjust Y movement to account for gravity
					movement.y = rb.velocity.y / speed;
				}

                anim.SetBool("isMoving", true);

                //set animationspeed to moving speed
                float speedIncrem = 1 / maxSpeed*1.5f;
                anim.speed = Mathf.Max(speed*speedIncrem, 0.7f);
            }
            else
            {
                anim.SetBool("isMoving", false);
            }

            

			// Adjust Y movement to account for gravity
			movement.y = rb.velocity.y / speed;

			// Set moving speed
			if((h != 0 || v != 0)&&!isDashing){
				speed = Mathf.Min(speed + currentAcceleration, maxSpeed);
                if (speed<minSpeed) { speed = minSpeed; }
			}

			// Set stationary speed
			else if(!isDashing){
				speed = Mathf.Max(speed - acceleration, minSpeed);
			}
				
            // Apply changes in velocity
			if(isDashing){
                movement.y = Mathf.Clamp(movement.y, 0, 0.01f);

				rb.velocity = movement * dashSpeed;
			}
			else{
            	rb.velocity = movement * speed;
			}

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

	public void Rotate(float v, float h){

		if(h!=0&&v!=0){
		var movement = new Vector3 (h, 0, v);
		// Adjust Y movement to account for gravity
		movement.y = rb.velocity.y / speed;
		if (!knockedBack && isGrounded) {
			
			transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(new Vector3(movement.x, 0.0f, movement.z)), Time.deltaTime * (turnRate/speed));
		}
		}
	}


    public void Dash () {
		if (!knockedBack && isGrounded) {
			dashIsCharging = false;
			dashMass = dashCharger;
			dashCharger = 0;
			if (!isDashing && dashCooldownRemaining == 0) {
				isDashing = true;
				dashLengthRemaining = dashLength;

				dashSound.PlayOneShot (dashSound.clip);
			}
		}
    }

	public void dashCharge(){
		if(dashCooldownRemaining == 0){
		    dashIsCharging = true;
            rb.velocity = Vector3.zero;
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
		// If this collides with another animal, bounce away and display particle
		if (collision.transform.tag == "Animal") {
			dashLengthRemaining = 0.0f;
			dashIsCharging = false;
			dashCharger = 0;

			StartCoroutine(gm.ShowCollisionParticle (collision.contacts [0].point));

			BounceAway (collision.transform.position);
		}
		//if it collides with terrain
		if (collision.transform.tag == "Environment") {
			dashLengthRemaining = 0.0f;
		}

		//if it collides with powerup is done in the powerup script
    }

    public void removePowerUp(string currentPower)
    {
        //Debug.LogWarning("Remove: "+ currentPower +" change");
        if (currentPower.Equals("mass"))
        {
            rb.mass = originalMass;
			this.transform.localScale = (this.transform.localScale / 1.5f);

        }
        else if (currentPower.Equals("speed"))
        {
            minSpeed = originalMinSpeed;
            maxSpeed = originalMaxSpeed;
			speed = minSpeed;
        }
		else if (currentPower.Equals("dashCD"))
		{
			dashCooldown = originalDashCD;
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
}
