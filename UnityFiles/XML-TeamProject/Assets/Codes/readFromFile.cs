using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class readFromFile : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
       string str=  System.IO.File.ReadAllText($"{PlayerPrefs.GetString("path")}");
       gameObject.GetComponent<UnityEngine.UI.InputField>().text = str;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
