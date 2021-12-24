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

            float centerX = ground.transform.localScale.x/2;
            float centerZ = ground.transform.localScale.z/2;
            float sizeSector = sizeGround / nb_uav;
            float widthUav = uav.transform.localScale.x;
            float sizeSectorInUav = sizeSector / widthUav;
            Vector3 start = new Vector3( topLeftGround + ((sizeSector * i)) , uav.transform.position.y , ground.transform.position.z + (ground.transform.localScale.z / 2) );
            
            uav.GetComponent<UAV>().setTargetArea(start);

            for(int j = 0 ; j < (int) sizeSectorInUav ; j++){
                uav.GetComponent<UAV>().addWayPoints( new Vector3(start.x + (widthUav * j) , 10f , start.z - centerZ ) );
                uav.GetComponent<UAV>().addWayPoints( new Vector3(start.x + (widthUav * j) , 10f , start.z - centerZ*2) );

                uav.GetComponent<UAV>().addWayPoints( new Vector3(start.x + (widthUav * (j+1) ) , 10f , start.z - centerZ*2) );

                uav.GetComponent<UAV>().addWayPoints( new Vector3(start.x + (widthUav * (j+1) ) , 10f , start.z - centerZ) );
                uav.GetComponent<UAV>().addWayPoints( new Vector3(start.x + (widthUav * (j+1) ) , 10f , start.z) );
            }

            objects.Add(uav);
        }

        
    }
}
