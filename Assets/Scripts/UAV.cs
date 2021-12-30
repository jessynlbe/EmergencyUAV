using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEditor;

public class UAV : MonoBehaviour
{
    protected GameObject communication;
    protected GameObject controller;
    protected Vector3 startPos;
    protected Boolean altitudeOk;
    protected Boolean startOk;
    protected Boolean finished;
    protected Boolean returnOk;
    protected List<Vector3> wayPoints = new List<Vector3>();
    protected List<GameObject> detectedPlayers = new List<GameObject>();
    protected List<GameObject> onHold = new List<GameObject>();
    public int segments;
    public float rad;
    public float altitude;
    LineRenderer line;

    public int manual;
    public int controlManual;
    public GameObject panelCamera;
    public GameObject progressBar;

    // Start is called before the first frame update
    void Start()
    {
        CreatePoints();
    }

    void CreatePoints()
    {

        line = gameObject.GetComponent<LineRenderer>();
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.endColor = Color.white;
        line.startColor = Color.white;
        line.widthMultiplier = 0.2f;
        line.positionCount = segments + 1;
        line.useWorldSpace = false;

        float x;
        float z;

        float angle = 20f;


        for (int i = 0; i < (segments + 1); i++)
        {
            x = Mathf.Sin(Mathf.Deg2Rad * angle) * rad;
            z = Mathf.Cos(Mathf.Deg2Rad * angle) * rad;

            line.SetPosition(i, new Vector3(x, 0f, z));

            angle += (380f / segments);
        }
    }

    void Awake(){
        //////////// All initialisations ///////////////
        controller = GameObject.Find("Controller");
        communication = GameObject.Find("Communication");
        startPos = this.transform.position;
        altitude = startPos.y + 20f;
        returnOk = false;
        altitudeOk = false;
        segments = 50;
        rad = 3f;
        manual = 0;        
        controlManual = 0;

        if(this.name == "UAV0"){
            panelCamera = GameObject.Find("PanelCamera");
            panelCamera.SetActive(false);
        }

        progressBar = GameObject.Find("Slider");
    }

    void sendPlayerDetectedToUAV(GameObject player){
        detectedPlayers.Add(player);
        if(this.name != "UAV0"){
            communication.GetComponent<Communication>().addText(this.name + " to UAV0 comes to the position " + player.transform.position.ToString(), Color.red );
        }
        
        for(int i = 0 ; i < controller.GetComponent<Controller>().getNbUav() ; i++ ){
            GameObject uav = GameObject.Find("UAV" + i.ToString());
            if(uav != this.gameObject){
                uav.GetComponent<UAV>().receivePlayerDetected(player);
            }

            if(uav.name == "UAV0" && this.name != "UAV0"){
                uav.GetComponent<UAV>().addOnHold(player);
            }
        }
    }

    void receivePlayerDetected(GameObject player){
        detectedPlayers.Add(player);
    }

    // Update is called once per frame
    void Update()
    {
        
        
        if(finished == true){
            if(this.name == "UAV0"){
                if(progressBar.GetComponent<ProgressBar>().getValue() == 1f ){
                    transform.LookAt( new Vector3(startPos.x , this.transform.position.y, startPos.z) );
                    move( startPos, 30f );
                }
            }
            else{
                transform.LookAt( new Vector3(startPos.x , this.transform.position.y, startPos.z) );
                move( startPos, 30f );
            }
        }
        

        if(this.name == "UAV0" && manual == 1 && Input.GetKeyUp(KeyCode.Space)){
            communication.GetComponent<Communication>().addText(this.name + " to All :" + "Person in the position "+ onHold[0].transform.position.ToString() +" is saved" , Color.green);
            manual = 0;
            controlManual = 0;
            onHold[0].GetComponent<Renderer>().material.color = Color.green;
            onHold.RemoveAt(0);
            controller.GetComponent<Controller>().updateUAV(0 , 4);
            panelCamera.SetActive(false);
        }

        if(this.name == "UAV0" && onHold.Count > 0){
            if(manual == 0){
                controller.GetComponent<Controller>().updateUAV(1 , 3);
                manual = 1;
            }
            
            Vector3 pos = new Vector3(onHold[0].transform.position.x , altitude - 2f , onHold[0].transform.position.z);
            float dist = Vector3.Distance(this.transform.position , pos);
            
            if(dist < 0.05){
                controlManual = 1;
                panelCamera.SetActive(true);
            }
            else if(controlManual == 0){
                move(pos , 20f);
            }

            if( Input.GetKey(KeyCode.UpArrow) && controlManual == 1){
                this.transform.position += Vector3.forward * Time.deltaTime * 10f;
            }
            else if( Input.GetKey(KeyCode.DownArrow) && controlManual == 1 ){
                this.transform.position += Vector3.back * Time.deltaTime * 10f;
            }
            else if( Input.GetKey(KeyCode.LeftArrow) && controlManual == 1 ){
                this.transform.position += Vector3.left * Time.deltaTime * 10f;
            }
            else if( Input.GetKey(KeyCode.RightArrow) && controlManual == 1 ){
                this.transform.position += Vector3.right * Time.deltaTime * 10f;
            }
        }
        else{



            ////////// Movement UAV //////////

            float dY = Math.Abs(this.transform.position.y - altitude);
            float dist = Math.Abs(Vector3.Distance(startPos , this.transform.position));

            if(altitudeOk == false && dY > 0.1){
                Vector3 target = new Vector3(transform.position.x , altitude , transform.position.z);
                move(target , 10f);
            }
            else if(finished == false){
                if(controller.GetComponent<Controller>().getDonePoints().Count > 0){
                    detectectionPlayer();
                }
                startOk = true;
                followPath();
            }

        }

    }

    int getNumberUAV(){
        return (int) Char.GetNumericValue(this.name[3]);
    }

    void detectectionPlayer(){
        RaycastHit hit;
        int layerMask = 1 << 7;

        for(int j =(int) getRad() ; j >= 0 ; j--){
            for(int i = 0 ; i < 360 ; i+=45){
                float x = j * Mathf.Cos(Mathf.Deg2Rad * i);
                float z = j * Mathf.Sin(Mathf.Deg2Rad * i);

                Vector3 point = new Vector3(this.transform.position.x + x , 0f , this.transform.position.z + z);
                Vector3 dir = point - transform.position;

                if(Physics.Raycast(transform.position , dir , out hit , this.transform.position.y , layerMask)){
                    GameObject player = hit.transform.gameObject;
                    if(detectedPlayers.Contains(player) == false){
                        communication.GetComponent<Communication>().addText(this.name + " to All : " + "injured person detected at " + player.transform.position.ToString() , Color.white);
                        if(this.name == "UAV0"){
                            onHold.Add(player);
                        }
                        sendPlayerDetectedToUAV(player);
                    }
                }
                Debug.DrawRay(transform.position , dir , Color.green);
            }
        }

        // Vector3 center = new Vector3(this.transform.position.x , startPos.y , this.transform.position.z);
        // Vector3 dir2 = center - transform.position;
        // if(Physics.Raycast(transform.position , dir2 , out hit , 100f , layerMask)){
        //         GameObject player = hit.transform.gameObject;
        //         if(detectedPlayers.Contains(player) == false){
        //             communication.GetComponent<Communication>().addText(this.name + " to All : " + "injured person detected at " + player.transform.position.ToString() , Color.black);
        //             if(this.name == "UAV0"){
        //                 onHold.Add(player);
        //             }
        //             sendPlayerDetectedToUAV(player);
        //         }
        //     }
        // Debug.DrawRay(transform.position , dir2 , Color.green);
    }

    public void move(Vector3 target , float speed){
        this.transform.position = Vector3.MoveTowards(transform.position , target , speed * Time.deltaTime);
    }
    public List<Vector3> getWayPoints(){
        return wayPoints;
    }

    public void addWayPoints(Vector3 value){
        wayPoints.Add(value);
    }

    public void followPath(){

        if(wayPoints.Count >= 1){
            float distance = Vector3.Distance(transform.position , wayPoints[0]);

            if(distance > 0.1){
                transform.LookAt(wayPoints[0]);
                move(wayPoints[0] , 20f);
            }
            else{
                GameObject.Find("Controller").GetComponent<Controller>().getDonePoints().Add(wayPoints[0]);
                wayPoints.RemoveAt(0);
            }
        }
        else{
            finished = true;
            wayPoints.Clear();
        }
    }

    public float getRad(){
        return rad;
    }


    public void addOnHold(GameObject player){
        onHold.Add(player);
    }

    public float getAltitude(){
        return altitude;
    }

    public Boolean getFinished(){
        return finished;
    }
}
