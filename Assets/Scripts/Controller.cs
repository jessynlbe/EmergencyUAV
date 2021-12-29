using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEditor;
using UnityEngine.UI; // Required when Using UI elements.

public class Controller : MonoBehaviour
{
    public GameObject communication;
    public List<GameObject> objects;
    public List<Vector3> mapPoints = new List<Vector3>();
    public List<Vector3> donePoints = new List<Vector3>();
    public GameObject slider;
    public int nb_uav;
    public int idxUav;
    public GameObject ground;
    public float topLeftGround;
    public float sizeGround;
    public float xPos;
    public float zPos;

    public float xSize;
    public float zSize;
    // Start is called before the first frame update
    void Start()
    {
        initUAV();
        initMapPoints(nb_uav);
    }

    // Update is called once per frame
    void Update()
    {
        if( Input.GetKeyUp(KeyCode.R) ){
            // communication.GetComponent<Communication>().addText("adezdez");
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
        xSize = 100f;
        zSize = 100f;
        xPos = 0f;
        zPos = 0f;

        slider = GameObject.Find("Slider");
        communication = GameObject.Find("Communication");
        ground = GameObject.Find("Ground");
        nb_uav = 4;
        sizeGround = xSize;
        topLeftGround = xPos - (sizeGround / 2);
        idxUav = 0;

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
        int heightMapInVision = (int) (zSize / ( objects[0].GetComponent<UAV>().getRad() * 2) );

        float centerX = xSize/2; // X center of the ground (area to check)
        float centerZ = zSize/2; // Z center of the ground (area to check)
        float sizeSector = sizeGround / nbUav; // width of the area to be monitored by each drone
        float widthVision = objects[0].GetComponent<UAV>().getRad() * 2; // Width of the "vision" of camera on the uav
        float sizeSectorInUav = (sizeSector / widthVision)/2; // Number of return trips of the uav so that it is over the whole area that has been assigned to it, according to the width of vision of its camera. Ex: For a zone of 100 meters wide with a camera that has a vision of 1 meter in diameter, it is necessary to make 50 return trips to have checked the whole zone.
        
        float startX = topLeftGround + (widthVision /2) ;
        float startZ = zPos + (zSize/2);

        for(int j = 0 ; j < widthMapInVision ; j++){
            
            if( j % 2 == 0){
                startZ = zPos + (zSize / 2) - objects[0].GetComponent<UAV>().getRad();

                mapPoints.Add( new Vector3(startX , objects[0].GetComponent<UAV>().getAltitude() , startZ) );
                int idx = 1;

                while(idx < heightMapInVision ){
                    mapPoints.Add( new Vector3(startX , objects[0].GetComponent<UAV>().getAltitude() , startZ - (idx*objects[0].GetComponent<UAV>().getRad()*2 ) ) ) ;
                    idx++;
                }

                mapPoints.Add( new Vector3(startX , objects[0].GetComponent<UAV>().getAltitude() , zPos - (zSize / 2) + objects[0].GetComponent<UAV>().getRad() ) );
                
            }
            else{
            
                startX += widthVision;

                mapPoints.Add( new Vector3(startX , objects[0].GetComponent<UAV>().getAltitude() , zPos - (zSize / 2) + objects[0].GetComponent<UAV>().getRad() ) );

                int idx2 = heightMapInVision-1 ;

                while(idx2 > 0 ){
                    mapPoints.Add( new Vector3(startX , objects[0].GetComponent<UAV>().getAltitude() , startZ - (idx2 * objects[0].GetComponent<UAV>().getRad()*2 )) );
                    idx2--;
                }

                mapPoints.Add( new Vector3(startX , objects[0].GetComponent<UAV>().getAltitude() , startZ) );

                startX += widthVision;

            }

        }
        assignWayPoints(widthVision , sizeSector , widthMapInVision, heightMapInVision+1 ,nbUav);

    }


    void assignWayPoints(float widthVision , float sizeSector , int widthMap , int heightMap, int nbUav ){
            float nbPerUavF = (float) widthMap / (float) nbUav;
            int nbPerUav = (int) (nbPerUavF + 0.5f);

            List<int> tabVal = new List<int>(objects.Count);
            for(int i = 0 ; i < objects.Count ; i++){
                tabVal.Add(nbPerUav);

                if(i == objects.Count - 1 ){
                    tabVal[i] += widthMap % nbPerUav;
                }
            } 

            List<Vector3> clonedList = new List<Vector3>(mapPoints);
            
            for(int i = idxUav ; i < objects.Count ; i++){
                for(int j = 0 ; j < tabVal[i]*heightMap ; j++){
                    if(donePoints.Contains(clonedList[0]) == false){
                        objects[i].GetComponent<UAV>().addWayPoints(clonedList[0]);
                    }

                    clonedList.RemoveAt(0);
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
