using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEditor;
using UnityEngine.UI; // Required when Using UI elements.

public class Controller : MonoBehaviour
{
    public List<GameObject> objects;
    public List<Vector3> mapPoints = new List<Vector3>();
    public List<Vector3> donePoints = new List<Vector3>();
    public GameObject slider;
    public int nb_uav;
    public int idxUav;
    public GameObject ground;
    public float topLeftGround;
    public float sizeGround;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if( Input.GetKeyUp(KeyCode.R) ){
            for(int i = 0 ; i < objects.Count ; i++){
                objects[i].GetComponent<UAV>().getWayPoints().Clear();
            }
            idxUav = 1;
            mapPoints.Clear();
            initMapPoints(3);
        }

        float min = 0;
        float max = mapPoints.Count-1;
        float normalizedValue = ( (float) donePoints.Count - min) / (max - min);
        slider.GetComponent<ProgressBar>().setValue(normalizedValue);
    }

    public void updateUAV(int value , int nb){
        idxUav = value;
        
        for(int i = 0 ; i < objects.Count ; i++){
                objects[i].GetComponent<UAV>().getWayPoints().Clear();
        }

        mapPoints.Clear();
        initMapPoints(nb);
    }

    void Awake(){
        slider = GameObject.Find("Slider");

        ground = GameObject.Find("Ground");
        nb_uav = 4;
        sizeGround = ground.transform.localScale.x;
        topLeftGround = ground.transform.position.x - (sizeGround / 2);
        idxUav = 0;
        
        initUAV();
        initMapPoints(nb_uav);
    }

    void initUAV(){
        GameObject uav;

        for(int i = 0 ; i < nb_uav ; i++){
            string name = "UAV" + i.ToString();
            uav = GameObject.Find(name);
            objects.Add(uav);
        }

    }

    void initMapPoints(int nbUav){
        int widthMapInVision = (int) ( (sizeGround / ( objects[0].GetComponent<UAV>().getRad() * 2) ) );
        int heightMapInVision = (int) (ground.transform.localScale.z / ( objects[0].GetComponent<UAV>().getRad() * 2) );

        float centerX = ground.transform.localScale.x/2; // X center of the ground (area to check)
        float centerZ = ground.transform.localScale.z/2; // Z center of the ground (area to check)
        float sizeSector = sizeGround / nbUav; // width of the area to be monitored by each drone
        float widthVision = objects[0].GetComponent<UAV>().getRad() * 2; // Width of the "vision" of camera on the uav
        float sizeSectorInUav = (sizeSector / widthVision)/2; // Number of return trips of the uav so that it is over the whole area that has been assigned to it, according to the width of vision of its camera. Ex: For a zone of 100 meters wide with a camera that has a vision of 1 meter in diameter, it is necessary to make 50 return trips to have checked the whole zone.
        
        float startX = topLeftGround + (widthVision /2) ;
        float startZ = ground.transform.position.z + (ground.transform.localScale.z/2);

        for(int j = 0 ; j < (int) widthMapInVision / 2 ; j++){
            
            startZ = ground.transform.position.z + (ground.transform.localScale.z / 2) - objects[0].GetComponent<UAV>().getRad();

            mapPoints.Add( new Vector3(startX , 10f , startZ) );
            int idx = 1;

            while(idx < heightMapInVision ){
                mapPoints.Add( new Vector3(startX , 10f , startZ - (idx*objects[0].GetComponent<UAV>().getRad()*2 ) ) ) ;
                idx++;
            }

            mapPoints.Add( new Vector3(startX , 10f , ground.transform.position.z - (ground.transform.localScale.z / 2) + objects[0].GetComponent<UAV>().getRad() ) );
            
            
            
            startX += widthVision;

            mapPoints.Add( new Vector3(startX , 10f , ground.transform.position.z - (ground.transform.localScale.z / 2) + objects[0].GetComponent<UAV>().getRad() ) );

            int idx2 = heightMapInVision-1 ;

            while(idx2 > 0 ){
                mapPoints.Add( new Vector3(startX , 10f , startZ - (idx2 * objects[0].GetComponent<UAV>().getRad()*2 )) );
                idx2--;
            }

            mapPoints.Add( new Vector3(startX , 10f , startZ) );

            startX += widthVision;

        }

        assignWayPoints(widthVision , sizeSector , widthMapInVision, heightMapInVision + 1,nbUav);

    }


    void assignWayPoints(float widthVision , float sizeSector , int widthMap , int heightMap, int nbUav ){
            float nbPerUavF = (float) widthMap / (float) nbUav;
            int nbPerUav = (int) (nbPerUavF + 0.5f);

            List<int> tabVal = new List<int>(objects.Count);
            for(int i = 0 ; i < objects.Count ; i++){
                if(i == objects.Count - 1 && widthMap % nbPerUav != 0){
                    nbPerUav = widthMap % nbUav;
                }
                tabVal.Add(nbPerUav);
            } 

            int idx = 0;
            for(int i=idxUav ; i < objects.Count ; i++ ){
                for(int j  = 0 ; j < tabVal[i]*heightMap ; j++){
                    if(donePoints.Contains(mapPoints[idx]) == false){
                        objects[i].GetComponent<UAV>().addWayPoints(mapPoints[idx]);
                    }
                    idx++;
                }
            }
    }

    public List<Vector3> getDonePoints(){
        return donePoints;
    }


    public int getNbUav(){
        return nb_uav;
    }
    
}
