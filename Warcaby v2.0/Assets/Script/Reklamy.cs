using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Reklamy : MonoBehaviour
{
    public float timeStart = 60;
    int x = 1;




    public void changepic(int x)
    {
        if (x == 0)
        {
            GameObject.Find("snake").SetActive(false);
            GameObject zdjecie1 = GameObject.Find("Reklamy").FindObject("Wimiip");
            zdjecie1.gameObject.SetActive(true);
        }

        else if (x == 1)
        {
            GameObject.Find("Wimiip").SetActive(false);
            GameObject zdjecie1 = GameObject.Find("Reklamy").FindObject("pepsi");
            zdjecie1.gameObject.SetActive(true);
        }

        else if (x == 2)
        {
            GameObject.Find("pepsi").SetActive(false);
            GameObject zdjecie1 = GameObject.Find("Reklamy").FindObject("snake");
            zdjecie1.gameObject.SetActive(true);
        }
    }
    void Update()
    {
        timeStart -= Time.deltaTime;
        if (timeStart < 0)
        {
            timeStart = 60;
            changepic(x);
            if (x == 2)
            {
                x = 0;
            }
            else
            {
                x = x + 1;
            }
        }


    }
}
