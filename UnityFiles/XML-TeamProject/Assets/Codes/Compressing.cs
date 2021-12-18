using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Compressing : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame

    void Update()
    {
        if (PlayerPrefs.GetInt("json") == 1)
        {
            gameObject.GetComponent<UnityEngine.UI.Button>().interactable = false;
        }
        else
        {
            gameObject.GetComponent<UnityEngine.UI.Button>().interactable = true;

        }
    }
}
