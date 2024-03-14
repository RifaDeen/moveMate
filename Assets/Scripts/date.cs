using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class date : MonoBehaviour
{
    public TextMeshProUGUI DateText;
    void Start()
    {
        string time = System.DateTime.Now.ToString("ddd d MMM");
        print(time);
        DateText.text = time;
    }


}
