using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEditor;

public class UAV : MonoBehaviour
{
    protected GameObject datas;
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
        datas = GameObject.Find("Datas");
        communication = GameObject.Find("Communication");
        startPos = this.transform.position;
        returnOk = false;
        altitudeOk = false;
        segments = 50;
        rad = 2f;
        manual = 0;
        altitude = 10f;

        datas.GetComponent<Datas>().setTextState("Automatic" , getNumberUAV());
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
            transform.LookAt( new Vector3(startPos.x , this.transform.position.y, startPos.z) );
            move( startPos, 10f );
        }

        if(this.name == "UAV0" && manual == 1 && Input.GetKeyUp(KeyCode.Space)){
            communication.GetComponent<Communication>().addText(this.name + " to All :" + "Person in the position "+ onHold[0].transform.position.ToString() +" is saved" , Color.green);
            manual = 0;
            onHold[0].GetComponent<Renderer>().material.color = Color.green;
            onHold.RemoveAt(0);
            datas.GetComponent<Datas>().setTextState("Automatic" , getNumberUAV());
            controller.GetComponent<Controller>().updateUAV(0 , 4);
        }

        if(this.name == "UAV0" && onHold.Count > 0){
            if(manual == 0){
                controller.GetComponent<Controller>().updateUAV(1 , 3);
                manual = 1;
                datas.GetComponent<Datas>().setTextState("Manual" , getNumberUAV());
            }
            else{
                Vector3 pos = new Vector3(onHold[0].transform.position.x , altitude - 2f , onHold[0].transform.position.z);
                move(pos , 20f);
            }
        }
        else{



            ////////// Movement UAV //////////

            float dY = Math.Abs(this.transform.position.y - altitude);
            float dist = Math.Abs(Vector3.Distance(startPos , this.transform.position));

            if(altitudeOk == false && dY > 0.1){
                Vector3 target = new Vector3(transform.position.x , altitude , transform.position.z);
                move(target , 3f);
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
        for(int i = 0 ; i < 360 ; i+=45){
            float x = getRad() * Mathf.Cos(Mathf.Deg2Rad * i);
            float z = getRad() * Mathf.Sin(Mathf.Deg2Rad * i);

            Vector3 point = new Vector3(this.transform.position.x + x , 0f , this.transform.position.z + z);
            Vector3 dir = point - transform.position;

            if(Physics.Raycast(transform.position , dir , out hit , this.transform.position.y , layerMask)){
                GameObject player = hit.transform.gameObject;
                if(detectedPlayers.Contains(player) == false){
                    communication.GetComponent<Communication>().addText(this.name + " to All : " + "injured person detected at " + player.transform.position.ToString() , Color.black);
                    if(this.name == "UAV0"){
                        onHold.Add(player);
                    }
                    sendPlayerDetectedToUAV(player);
                }
            }
            Debug.DrawRay(transform.position , dir , Color.green);
        }
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
                move(wayPoints[0] , 30f);
            }
            else{
                GameObject.Find("Controller").GetComponent<Controller>().getDonePoints().Add(wayPoints[0]);
                wayPoints.RemoveAt(0);
            }
        }
        else{
            finished = true;
            datas.GetComponent<Datas>().setTextState("Finished" , getNumberUAV());
            wayPoints.Clear();
            changeTextPoints();
        }
    }

    public float getRad(){
        return rad;
    }


    public void addOnHold(GameObject player){
        onHold.Add(player);
    }

    public void changeTextPoints(){
        if(wayPoints.Count >= 2){
            datas.GetComponent<Datas>().setTextStart(wayPoints[0].ToString() , getNumberUAV());
            datas.GetComponent<Datas>().setTextEnd(wayPoints[wayPoints.Count -1].ToString() , getNumberUAV());
        }
        else{
            datas.GetComponent<Datas>().setTextStart("" , getNumberUAV());
            datas.GetComponent<Datas>().setTextEnd("" , getNumberUAV());
        }
    }
}
