using UnityEngine;
using System.Collections;

public class PowerUp : MonoBehaviour {
    private string PuType = "";
    public float speedMultiplier = 1.5f;
    public float massMultiplier = 2f;
	public float reduceDashCD = 2f;
	public string[] PuTypes;
	void Awake(){
		PuTypes= new string[]{"mass", "speed", "dashCD"};
		int randomPower = Random.Range(0,PuTypes.Length);
//		Debug.LogWarning (PuTypes[2]);
		PuType = PuTypes[randomPower];
	}
	void Start () {
		
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
	public float getNewDashCD()
	{
		return reduceDashCD;
	}
		
}
