using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class hide : MonoBehaviour
{

    public void Hide()
    {
        GameObject.FindGameObjectWithTag("hide").transform.Find("InputField (TMP)").gameObject.SetActive(false);
        PlayerPrefs.SetInt("hide", 1);
    }
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerPrefs.GetInt("isValid") != 1 && PlayerPrefs.GetInt("checked") == 1 && PlayerPrefs.GetInt("hide")!=1)
        {
            gameObject.GetComponent<UnityEngine.UI.Button>().interactable = true;
        }
        else
        {
            gameObject.GetComponent<UnityEngine.UI.Button>().interactable = false;
        }
    }
}
