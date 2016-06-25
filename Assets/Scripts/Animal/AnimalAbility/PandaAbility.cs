using UnityEngine;
using System.Collections;

public class PandaAbility : MonoBehaviour, AnimalAbility {
	//private Rigidbody rb;
	private AnimalController animal;
	private bool isAvailable = true;
	private bool isActive = false;
	public float ticker = 10.0f;
	private SkinnedMeshRenderer rend;
	public Color[] colors;
	private Animator animator;

    public GameObject head;

	void Start(){
	    animator = GetComponent<Animator> ();
	    //rb = this.GetComponent<Rigidbody>();
	    animal = GetComponent<AnimalController>();
		rend = GetComponentInChildren<SkinnedMeshRenderer>();  
		colors = new Color[rend.materials.Length];	
		for (int i = 0; i < rend.materials.Length; i++) {
			colors [i] = rend.materials [i].color;
		}
	}

	void Update () {
		if (isActive) {
			ticker -= Time.deltaTime;
			if (ticker < 0.0f) {
				isActive = false;
				animal.pandaAbility = false;
				animal.disableControl = false;
				animator.SetBool("usingAbility", false);

                // Stop it being pushed back for a moment
                animal.tigerAbility = true;
                StartCoroutine(disableAbility());

//				for (int i = 0; i < rend.materials.Length; i++) {
//					rend.materials [i].color = colors [i];
//				}

				print ("Disabled Panda ability");

			}
		} else {
			return;
		}
	}

    private IEnumerator disableAbility() {
        yield return new WaitForSeconds(1);

        animal.tigerAbility = false;
    }

	public void applyAbility (){
		if (isAvailable) {
			print ("Panda ability");
			animal.pandaAbility = true;
			animal.disableControl = true;
			animator.SetBool("usingAbility", true);
//			foreach(Material m in rend.materials){
//				//m.color = Color.gray;
//				Color color = new Color(m.color.a/2, m.color.r, m.color.g, m.color.b);
//				//color.a = 0.1f;
//				m.color = color;
//			}
			isActive = true;
		}
		isAvailable = false;
	}

}
