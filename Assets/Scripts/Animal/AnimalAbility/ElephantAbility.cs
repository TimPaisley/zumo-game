using UnityEngine;
using System.Collections;

public class ElephantAbility : MonoBehaviour,AnimalAbility {

	private bool isAvailable = true;
	private bool isActive = false;
	public float ticker = 5.0f;
    private AnimalController animal;

    public bool shake = false;
	private float originalPostion;
	private SkinnedMeshRenderer rend;
	public Color[] colors;
	private CameraManager cm;
    public ParticleSystem ragePS;
    private ParticleSystem.EmissionModule rageEM;

    void Awake(){
        animal = GetComponent<AnimalController>();
        rend = GetComponentInChildren<SkinnedMeshRenderer>();  
		colors = new Color[rend.materials.Length];	
		for (int i = 0; i < rend.materials.Length; i++) {
			colors [i] = rend.materials [i].color;
		}
        rageEM = ragePS.emission;

    }

    void Update () {
		Vector3 camPostion;
		if (isActive) {
			ticker -= Time.deltaTime;
			camPostion = cm.mainCamera.transform.localPosition;
			float x = (originalPostion + Mathf.Sin(ticker * 8f));
			camPostion = new Vector3 (x, camPostion.y, camPostion.z);
			cm.mainCamera.transform.localPosition = camPostion;

			if (ticker < 0.0f) {
				isActive = false;
				camPostion = cm.mainCamera.transform.localPosition;
				camPostion.x = originalPostion;
				cm.mainCamera.transform.localPosition = camPostion;
				for (int i = 0; i < rend.materials.Length; i++) {
					rend.materials [i].color = colors [i];
				}
                rageEM.enabled = false;
                ragePS.Stop();
                ragePS.Clear();
                animal.elephantSpeedMultiplier = 1.0f;
                print("Disabled Elephant ability");
			} 
		}
	}

	public void applyAbility (){
		cm = FindObjectOfType<CameraManager> ();
		originalPostion = cm.mainCamera.transform.localPosition.x;
		if (isAvailable) {
			print ("Elephant Ability");
			isActive = true;
			foreach(Material m in rend.materials){
				m.color = Color.red;
			}
            ragePS.Simulate(0.0f, true, true);
            rageEM.enabled = true;
            ragePS.Play();
            animal.elephantSpeedMultiplier = 1.5f;
            
    }
		isAvailable = false;
	}
}
