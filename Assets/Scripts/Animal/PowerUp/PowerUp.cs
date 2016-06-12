using UnityEngine;
using System.Collections;

public class PowerUp : MonoBehaviour {
	public string PuType = "";
    public float speedMultiplier = 1.5f;
    public float massMultiplier = 2f;
	public float reduceDashCD = 2f;
	void Awake(){
		
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
	public float getDashCDMulti()
	{
		return 1 / reduceDashCD;
	}
		
}
