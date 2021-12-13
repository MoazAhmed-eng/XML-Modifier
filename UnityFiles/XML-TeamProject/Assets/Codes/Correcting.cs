using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Correcting : MonoBehaviour
{


    public void Correct()
    {
    }




    // Update is called once per frame
    void Update()
    {
        if(PlayerPrefs.GetInt("isValid")==1)
        {
            gameObject.GetComponent<UnityEngine.UI.Button>().interactable = false;
        }
        else
        {
            gameObject.GetComponent<UnityEngine.UI.Button>().interactable = true;

        }
    }
}
