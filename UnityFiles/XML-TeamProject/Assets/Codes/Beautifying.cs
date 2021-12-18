using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beautifying : MonoBehaviour
{
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
