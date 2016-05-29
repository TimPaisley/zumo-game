using UnityEngine;
using System.Collections;

public class PowerUpCreater : MonoBehaviour {
    public DeathmatchScene inGameScene;
    public Transform[] SpawnPostion;
    public GameObject PowerUpObj;
    public float SpawnTime = 4f;
    private float TimeTicker;
	private GameObject[] existedPowerUp;

    void Start()
    {
        TimeTicker = SpawnTime;
    }
	void Awake () {
		existedPowerUp = new GameObject[SpawnPostion.Length];
	}

	void Update () {
        if (!inGameScene.inProgress) {
            return;
        }

        TimeTicker -= Time.deltaTime;
		if(TimeTicker < 0.0f && checkTotalPowerup() != 2)
        {
            TimeTicker = SpawnTime;
			int randomPower = findEmptySpawnPostion();
			existedPowerUp[randomPower] = (GameObject)Instantiate(PowerUpObj, 
			SpawnPostion[randomPower].position, SpawnPostion[randomPower].rotation);
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
