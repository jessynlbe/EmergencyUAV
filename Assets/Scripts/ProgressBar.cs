using UnityEngine;
using System.Collections;
using UnityEngine.UI; // Required when Using UI elements.

public class ProgressBar : MonoBehaviour
{
    public Slider slider;
    public Text displayText;
    // Start is called before the first frame update
    void Start()
    {
        slider = this.GetComponent<Slider>();
        displayText = GameObject.Find("Percentage").GetComponent<Text>();
        slider.value = 0;
    }
    public void setValue(float value){
        slider.value = value;
        if(slider.value != 1){
            displayText.text = (slider.value * 100).ToString("0.00")+"%";
        }
        else{
            displayText.text = "100";
        }
    }
}
