using UnityEngine;
using System.Collections;

public class PowerUpCreater : MonoBehaviour {
    public DeathmatchScene inGameScene;
    public Transform[] SpawnPostion;
    public GameObject PowerUpObj;
    public float SpawnTime = 4f;
    private float TimeTicker;
	private GameObject[] existedPowerUp;
	public GameObject speedUp;
	public GameObject massUp;
	public GameObject dashUp;
	public string[] PuTypes;

	void Awake () {
		PuTypes= new string[]{"mass", "speed", "dashCD"};
		TimeTicker = SpawnTime;
		existedPowerUp = new GameObject[SpawnPostion.Length];
	}

	void Update () {
        if (!inGameScene.inProgress) {
            return;
        }

        TimeTicker -= Time.deltaTime;
		if(TimeTicker < 0.0f && checkTotalPowerup() != 2)
        {
            //TimeTicker = SpawnTime;
			TimeTicker = Random.Range(5,11);
//			Debug.LogWarning (TimeTicker);

			int randomType = Random.Range(0,PuTypes.Length);
			string t = PuTypes[randomType];
			int randomPower = findEmptySpawnPostion();

			if(t.Equals("mass")){
				existedPowerUp[randomPower] = (GameObject)Instantiate(massUp, 
					SpawnPostion[randomPower].position, SpawnPostion[randomPower].rotation);
			}else if(t.Equals("speed")){
				existedPowerUp[randomPower] = (GameObject)Instantiate(speedUp, 
					SpawnPostion[randomPower].position, SpawnPostion[randomPower].rotation);
			}else if(t.Equals("dashCD")){
				existedPowerUp[randomPower] = (GameObject)Instantiate(dashUp, 
					SpawnPostion[randomPower].position, SpawnPostion[randomPower].rotation);
			}
			//PowerUp curPU = existedPowerUp[randomPower].GetComponent<PowerUp> ();
			//curPU.PuType = t;
        }
       
    }

	private int checkTotalPowerup(){
		int count = 0;
		foreach (GameObject g in existedPowerUp) {
			if (g != null) {
				count++;
			} 
		}
		return count;
	}

	private int findEmptySpawnPostion(){
		ArrayList index = new ArrayList ();
		for (int i = 0; i < existedPowerUp.Length; i++) {
			if (existedPowerUp[i] == null) {
				index.Add (i);
			}
		}
		int randomPower = Random.Range(0, index.Count);
		return (int)(index[randomPower]);
	}

}
