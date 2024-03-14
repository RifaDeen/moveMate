using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class radialProgress : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] Image image;
    [SerializeField] float speed;
    float currentValue = 0f;

    void Update()
    {
        if (currentValue<100)
        {
            currentValue += speed * Time.deltaTime;
            text.text= ((int)currentValue).ToString() + "%";
        }
        else
        {
            text.text = "100%";
        }
        image.fillAmount = currentValue / 100;
    }
}

// hand movement cal