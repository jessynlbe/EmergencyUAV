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

            float centerX = ground.transform.localScale.x/2; // X center of the ground (area to check)
            float centerZ = ground.transform.localScale.z/2; // Z center of the ground (area to check)
            float sizeSector = sizeGround / nb_uav; // width of the area to be monitored by each drone

            float widthVision = uav.GetComponent<UAV>().getRad() * 2; // Width of the "vision" of camera on the uav
            float sizeSectorInUav = (sizeSector / widthVision)/2; // Number of return trips of the uav so that it is over the whole area that has been assigned to it, according to the width of vision of its camera. 
                                                                 //Ex: For a zone of 100 meters wide with a camera that has a vision of 1 meter in diameter, it is necessary to make 50 return trips to have checked the whole zone.
            
            Vector3 start = new Vector3( topLeftGround + ((sizeSector * i) + (widthVision/2) ) , 
                uav.transform.position.y , 
                ground.transform.position.z + (ground.transform.localScale.z / 2) ); // Start position of the uav to begin monitoring (top left of his area)
            
            uav.GetComponent<UAV>().setTargetArea(start); // The UAV moves to the top left of its area before starting its surveillance
            float startX = start.x;
            for(int j = 0 ; j < (int) sizeSectorInUav ; j++){

                if( j > 0){
                    startX += widthVision;
                    uav.GetComponent<UAV>().addWayPoints( new Vector3(startX , 10f , start.z ) );
                }

                uav.GetComponent<UAV>().addWayPoints( new Vector3(startX , 10f , start.z - centerZ ) );
                uav.GetComponent<UAV>().addWayPoints( new Vector3(startX , 10f , start.z - centerZ*2) );

                uav.GetComponent<UAV>().addWayPoints( new Vector3(startX + widthVision , 10f , start.z - centerZ*2) );

                uav.GetComponent<UAV>().addWayPoints( new Vector3(startX + widthVision , 10f , start.z - centerZ) );
                uav.GetComponent<UAV>().addWayPoints( new Vector3(startX + widthVision , 10f , start.z) );

                startX += widthVision;
            }

            Vector3 nextStart = new Vector3( topLeftGround + ((sizeSector * (i+1) ) + (widthVision/2) ) , uav.transform.position.y , ground.transform.position.z + (ground.transform.localScale.z / 2) );
            uav.GetComponent<UAV>().addWayPoints( new Vector3(nextStart.x  , 10f , nextStart.z  ) );
            uav.GetComponent<UAV>().addWayPoints( new Vector3(nextStart.x  , 10f , nextStart.z - centerZ ) );
            uav.GetComponent<UAV>().addWayPoints( new Vector3(nextStart.x  , 10f , nextStart.z - centerZ*2 ) );

            objects.Add(uav);
        }

        
    }


}
