using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class AnimalControl : MonoBehaviour {
    public float speed = 6f;

    private Vector3 movement;
    Animator anim;
    Rigidbody playerRigidbody;
    public Text countGiftText;
    public Text HerbCount;
    public GameObject herbo;
    public GameObject gifto;
     List <GameObject> collectedHerb;
     List<GameObject> collectedGift;
    void Awake()
    {
        anim = GetComponent<Animator>();
        playerRigidbody = GetComponent<Rigidbody>();
        collectedHerb = new List<GameObject>();
        collectedGift = new List<GameObject>();
    }

    void FixedUpdate()
    {
        // with get axis the value will be 1 and -1 / for raw -1 0 1 
        float h = Input.GetAxis("Horizontal"); // full speed at start // no acceration
        float v = Input.GetAxis("Vertical");
        Move(h, v);
        Animating(h, v);
    }

    void Move(float h, float v)
    {
        movement.Set(h, 0f, v);
        movement = movement.normalized * speed * Time.deltaTime;
        playerRigidbody.MovePosition(transform.position + movement);
       
    }


    void Animating(float h, float v)
    {
        //bool walking = h != 0f | v != 0f;
        //anim.SetBool("IsWalking", walking);
        if(h != 0f | v != 0f)
        {
            /*Vector3 rotation = Vector3.Normalize(new Vector3(h, 0f, v));
            transform.forward = rotation;*/
            playerRigidbody.rotation = Quaternion.LookRotation(new Vector3(h, 0f, v));
            anim.SetBool("IsWalking", true);
        }
        else
        {
            anim.SetBool("IsWalking", false);
        }
        
    }

    void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.CompareTag("Gifts"))
        {
            int count = int.Parse(countGiftText.text);
            countGiftText.text = (count + 1).ToString();
            collectedGift.Add(other.gameObject);
            other.gameObject.SetActive(false);
        }
        else if (other.gameObject.CompareTag("Herb"))
        {
            int count = int.Parse(HerbCount.text);
            HerbCount.text = (count + 1).ToString();
            collectedHerb.Add(other.gameObject);
            other.gameObject.SetActive(false);
        }
    }

    public void DropGift()
    {
        if(collectedGift.Count != 0)
        {
            GameObject temp = collectedGift[0];
            collectedGift.RemoveAt(0);
            temp.SetActive(true);
            temp.transform.position = new Vector3(this.transform.position.x, (this.transform.position.y+2), this.transform.position.z) 
                + transform.forward * 2;
            int count = int.Parse(countGiftText.text);
            countGiftText.text = (count -1).ToString();
        }
    }


    public void DropHerb()
    {
        if (collectedHerb.Count != 0)
        {
            GameObject temp = collectedHerb[0];
            collectedHerb.RemoveAt(0);
            Vector3 v3 = this.transform.position;
            Vector3 nv3 = new Vector3(v3.x-2, (this.transform.position.y-1), v3.z-2) + transform.forward * 2;
            temp.transform.position = nv3;
            temp.SetActive(true);
            int count = int.Parse(HerbCount.text);
            HerbCount.text = (count - 1).ToString();
        }
    }


}
