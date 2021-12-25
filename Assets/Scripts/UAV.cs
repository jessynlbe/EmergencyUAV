using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEditor;
public class UAV : MonoBehaviour
{
    protected Vector3 targetArea;
    protected Boolean altitudeOk;
    protected Boolean startOk;
    protected Boolean finished;
    protected List<Vector3> wayPoints = new List<Vector3>();
    // Start is called before the first frame update

    public int segments;
    public float rad;
    LineRenderer line;

    void Start()
    {
        segments = 50;
        rad = 3f;

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
    }

    // Update is called once per frame
    void Update()
    {
        float dY = Math.Abs(this.transform.position.y - 10f);
        float dist = Vector3.Distance(transform.position , targetArea);

        if(altitudeOk == false && dY > 0.1){
            move(new Vector3(transform.position.x , 10f , transform.position.z) , 3f);
        }
        else if(startOk == false && dist > 0.1){
            altitudeOk = true;
            move(targetArea , 10f);
        }
        else if(finished == false){
            startOk = true;
            followPath();
        }

    }

    public void move(Vector3 target , float speed){
        this.transform.position = Vector3.MoveTowards(transform.position , target , speed * Time.deltaTime);
    }

    public Vector3 getTargetArea(){
        return targetArea;
    }

    public void setTargetArea(Vector3 target){
        targetArea = target;
    }

    public List<Vector3> getWayPoints(){
        return wayPoints;
    }

    public void addWayPoints(Vector3 value){
        wayPoints.Add(value);
    }

    public void printWayPoints(){
        int size = wayPoints.Count;
        for(int i = 0 ; i < size ; i++){
            Debug.Log("Value " + i + " : " + wayPoints[i] + ", ");
        }
        Debug.Log("\n");
    }

    public void followPath(){

        if(wayPoints.Count >= 1){
            float distance = Vector3.Distance(transform.position , wayPoints[0]);

            if(distance > 0.1){
                move(wayPoints[0] , 20f);
            }
            else{
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
