using UnityEngine;
using System.Collections;

public class PowerUp : MonoBehaviour {
    private string PuType = "";
    public float speedMultiplier = 1.5f;
    public float massMultiplier = 2f;
    public string[] PuTypes = { "mass", "speed" };

	void Start () {
        int randomPower = Random.Range(0,PuTypes.Length);
        PuType = PuTypes[randomPower];
	}
    public string getPowerUpType()
    {
        return PuType;
    }
    public float getSpeedMulti()
    {
        return speedMultiplier;
    }
    public float getMassMultiplie()
    {
        return massMultiplier;
    }
}
