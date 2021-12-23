using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
public class UAV : MonoBehaviour
{
    protected Vector3 targetArea;
    // Start is called before the first frame update
    void Start()
    {
    }

    void Awake(){
    }

    // Update is called once per frame
    void Update()
    {
        move(new Vector3(transform.position.x , 10f , transform.position.z));
    }

    public void move(Vector3 target){
        float dY = Math.Abs(this.transform.position.y - 10f);

        if(dY > 0.1){
            this.transform.position = Vector3.MoveTowards(transform.position, target, 2f * Time.deltaTime);
        }
        else{
            this.transform.position = Vector3.MoveTowards(transform.position , targetArea , 10f* Time.deltaTime);
        }

        
    }

    public Vector3 getTargetArea(){
        return targetArea;
    }

    public void setTargetArea(Vector3 target){
        targetArea = target;
    }
}
