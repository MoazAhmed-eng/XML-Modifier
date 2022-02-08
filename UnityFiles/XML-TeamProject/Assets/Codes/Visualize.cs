using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Diagnostics;



public class Visualize : MonoBehaviour
{

    void Visual()
    {
        Jsonify.ScanThroughXML();
        Process.Start(@"visualize.exe");
        GameObject.FindGameObjectWithTag("instr").GetComponent<UnityEngine.UI.Text>().text = "Visualization Done";
    }

    public void Update()
    {
        if (PlayerPrefs.GetInt("isValid") == 1 &&  PlayerPrefs.GetInt("Corrected") != 1 & PlayerPrefs.GetInt("json") != 1)
        {
            gameObject.GetComponent<UnityEngine.UI.Button>().interactable = true;
        }
        else
        {
            gameObject.GetComponent<UnityEngine.UI.Button>().interactable = false;
        }
    }

}
