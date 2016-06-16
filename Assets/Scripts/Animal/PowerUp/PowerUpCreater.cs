using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PowerUpCreater : MonoBehaviour {
    public DeathmatchScene inGameScene;
    private Transform[] SpawnPostion;
    public float SpawnTime = 4f;
    private float TimeTicker;
	private GameObject[] existedPowerUp;
	public GameObject speedUp;
	public GameObject massUp;
	public GameObject dashUp;
	public GameObject detonator;
	public string[] PuTypes;
    private GameManager gm;
    private string currentBoard;

	void Awake () {
        gm = FindObjectOfType<GameManager>();
		PuTypes= new string[]{"mass", "speed","stop","bomb"};//
		TimeTicker = SpawnTime;

        speedUp.gameObject.SetActive(false);
        massUp.gameObject.SetActive(false);
        dashUp.gameObject.SetActive(false);
        detonator.gameObject.SetActive(false);
    }

    void Update () {
		if (!inGameScene.inProgress && existedPowerUp != null) {
			for (var i = 0; i < existedPowerUp.Length; i++) {
				if (existedPowerUp[i] != null) {
					Destroy (existedPowerUp[i]);
					existedPowerUp[i] = null;
				}
			}

			SpawnPostion = null;
		}

		if (!inGameScene.inProgress) {
			return;
		}

        if(SpawnPostion == null)
        {
            SpawnPostion = gm.currentBoard.puPoints;
            existedPowerUp = new GameObject[SpawnPostion.Length];
        }

        TimeTicker -= Time.deltaTime;
		if(TimeTicker < 0.0f && checkTotalPowerup() != 2)
        {
            //TimeTicker = SpawnTime;
			TimeTicker = Random.Range(5,11);

//			Debug.LogWarning (TimeTicker);

			int randomType = Random.Range(0,PuTypes.Length);
			if (randomType == 3 && Random.Range(0, 2) < -1) {
				randomType = Random.Range (0, 2);
			}
			string t = PuTypes[randomType];
			int randomPower = findEmptySpawnPostion();

			if (t.Equals ("mass")) {
				existedPowerUp [randomPower] = (GameObject)Instantiate (massUp, 
					SpawnPostion [randomPower].position - Vector3.up, SpawnPostion [randomPower].rotation);
			} else if (t.Equals ("speed")) {
				existedPowerUp [randomPower] = (GameObject)Instantiate (speedUp, 
					SpawnPostion [randomPower].position - Vector3.up, SpawnPostion [randomPower].rotation);
			} else if (t.Equals ("stop")) {
				existedPowerUp [randomPower] = (GameObject)Instantiate (dashUp, 
					SpawnPostion [randomPower].position, SpawnPostion [randomPower].rotation);
			} else if (t.Equals ("bomb")) {
				existedPowerUp [randomPower] = (GameObject)Instantiate (detonator, 
					SpawnPostion [randomPower].position, SpawnPostion [randomPower].rotation);
			}

			existedPowerUp [randomPower].gameObject.SetActive (true);

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
