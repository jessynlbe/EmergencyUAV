using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEditor;

public class UAV : MonoBehaviour
{
    protected GameObject controller;
    protected Vector3 startPos;
    protected Boolean altitudeOk;
    protected Boolean startOk;
    protected Boolean finished;
    protected Boolean returnOk;
    protected List<Vector3> wayPoints = new List<Vector3>();
    protected List<GameObject> detectedPlayers = new List<GameObject>();
    public int segments;
    public float rad;
    LineRenderer line;
    public Boolean stop;


    // Start is called before the first frame update
    void Start()
    {

        line = gameObject.GetComponent<LineRenderer>();
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.endColor = Color.white;
        line.startColor = Color.white;
        line.widthMultiplier = 0.2f;
        line.positionCount = segments + 1;
        line.useWorldSpace = false;

        CreatePoints();
    }

    void CreatePoints()
    {
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
        controller = GameObject.Find("Controller");
        startPos = this.transform.position;
        returnOk = false;
        altitudeOk = false;
        segments = 50;
        rad = 2f;
        stop = false;
    }

    void sendPlayerDetectedToUAV(GameObject player){
        detectedPlayers.Add(player);

        for(int i = 0 ; i < controller.GetComponent<Controller>().getNbUav() ; i++ ){
            GameObject uav = GameObject.Find("UAV" + i.ToString());
            if(uav != this.gameObject){
                uav.GetComponent<UAV>().receivePlayerDetected(player);
            }
        }
    }

    void receivePlayerDetected(GameObject player){
        detectedPlayers.Add(player);
        Debug.Log(this.name + " : " + "Player data received");
    }

    // Update is called once per frame
    void Update()
    {
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
                    sendPlayerDetectedToUAV(player);
                }
            }
            Debug.DrawRay(transform.position , dir , Color.green);
        }

        if (Input.GetKeyUp(KeyCode.Escape)){
            if(stop == false){
                stop = true;
            }
            else{
                stop = false;
            }
        }
        
        if( Input.GetKeyUp(KeyCode.Space)){
            Debug.Log(this.name + " : " + String.Join(", ", wayPoints));
        }

        float dY = Math.Abs(this.transform.position.y - 10f);
        float dist = Math.Abs(Vector3.Distance(startPos , this.transform.position));

        if(altitudeOk == false && dY > 0.1){
            move(new Vector3(transform.position.x , 10f , transform.position.z) , 3f);
        }
        else if(finished == false && stop == false){
            startOk = true;
            followPath();
        }
        else if(returnOk == false){
            // move( startPos, 10f );
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
                move(wayPoints[0] , 20f);
            }
            else{
                GameObject.Find("Controller").GetComponent<Controller>().getDonePoints().Add(wayPoints[0]);
                wayPoints.RemoveAt(0);
            }
        }
        else{
            finished = true;
            Debug.Log("Arrived\n");
        }
    }

    public float getRad(){
        return rad;
    }



}
