using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEditor;
public class UAV : MonoBehaviour
{
    protected Boolean altitudeOk;
    protected Boolean startOk;
    protected Boolean finished;
    protected List<Vector3> wayPoints = new List<Vector3>();
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
        altitudeOk = false;
        segments = 50;
        rad = 2f;
        stop = false;
    }

    // Update is called once per frame
    void Update()
    {
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

        if(altitudeOk == false && dY > 0.1){
            move(new Vector3(transform.position.x , 10f , transform.position.z) , 3f);
        }
        else if(finished == false && stop == false){
            startOk = true;
            followPath();
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
                move(wayPoints[0] , 10f);
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
