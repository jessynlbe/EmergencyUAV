using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public List<GameObject> objects;
    public int nb_uav;
    public GameObject ground;
    public float topLeftGround;
    public float sizeGround;
    // Start is called before the first frame update
    void Start()
    {
        ///// init /////
        ground = GameObject.Find("Ground");
        nb_uav = 4;
        ///////////////

        // Debug.Log("Ground : " + ground.transform.localScale);
        sizeGround = ground.transform.localScale.x;
        topLeftGround = ground.transform.position.x - (sizeGround / 2);
        
        initUAV();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void initUAV(){
        GameObject uav;

        for(int i = 0 ; i < nb_uav ; i++){
            string name = "UAV" + i.ToString();
            uav = GameObject.Find(name);
            float sizeSector = sizeGround / nb_uav;
            Vector3 targetVal = new Vector3( topLeftGround + ( (sizeSector * (i+1)) - sizeSector/2) , uav.transform.position.y , ground.transform.position.z);
            Debug.Log(sizeSector + " " + sizeGround + " " + topLeftGround + " " + targetVal);

            uav.GetComponent<UAV>().setTargetArea(targetVal);
            objects.Add(uav);
        }

        
    }
}
