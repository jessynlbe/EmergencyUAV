using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Required when Using UI elements.


public class Datas : MonoBehaviour
{
    public List<Text> textState = new List<Text>();
    public List<Text> textStart = new List<Text>();
    public List<Text> textEnd = new List<Text>();
    public int sizeLists;
    // Start is called before the first frame update
    void Start()
    {
    }

    void Awake(){
        sizeLists = 4;
        initList("textState" , textState);
        initList("textStart" , textStart);
        initList("textEnd" , textEnd);
    }

    void initList(string name , List<Text> list){
        for(int i = 0 ; i < sizeLists ; i++){
            string nameElement = name + i.ToString(); 
            Text el = GameObject.Find(nameElement).GetComponent<Text>();
            list.Add(el);
        }
    }

    public void setTextState(string text , int value){
        string message = "State : " + text;
        textState[value].text = message;
    }

    public void setTextStart(string text , int value){
        string message = "Start : " + text;
        textStart[value].text = message;
    }

    public void setTextEnd(string text , int value){
        string message = "End : " + text;
        textEnd[value].text = message;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
