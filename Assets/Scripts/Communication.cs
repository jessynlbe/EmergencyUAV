using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Required when Using UI elements.


public class Communication : MonoBehaviour
{
    public List<Text> textList = new List<Text>();
    public int sizeList ;
    public int idxList;
    // Start is called before the first frame update
    void Start()
    {
        idxList=0;
        sizeList = 9;
        for(int i = 0 ; i <  sizeList ; i++){
            string name = "Text" + i.ToString();
            textList.Add(GameObject.Find(name).GetComponent<Text>());
        }
    }

    void incrementIdx(){
        if(idxList >= sizeList - 1){
            idxList = 0;
            for(int i = 0 ; i < sizeList ; i++){
                textList[i].color = Color.white;
                textList[i].text = "• ";
                
            }
        }
        else{
            idxList++;
        }
    }

    public void addText(string text , Color color){
        string message = "• " + text;
        textList[idxList].text = message;
        textList[idxList].color = color;
        incrementIdx();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
