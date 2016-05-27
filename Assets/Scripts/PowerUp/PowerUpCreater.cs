using UnityEngine;
using System.Collections;

public class PowerUpCreater : MonoBehaviour {
    public DeathmatchScene inGameScene;
    public Transform[] SpawnPostion;
    public GameObject PowerUpObj;
    public float SpawnTime = 4f;
    private float TimeTicker;

    void start()
    {
        TimeTicker = SpawnTime;
    }
	void Update () {
        if (!inGameScene.inProgress) {
            return;
        }
        TimeTicker -= Time.deltaTime;
        if(TimeTicker < 0.0f)
        {
            TimeTicker = SpawnTime;
            int randomPower = Random.Range(0, SpawnPostion.Length);
            Instantiate(PowerUpObj, SpawnPostion[randomPower].position, SpawnPostion[randomPower].rotation);
        }
        /*foreach (Transform t in SpawnPostion)
        {
            Instantiate(PowerUpObj, t.position, t.rotation);
        }*/
    }
}
