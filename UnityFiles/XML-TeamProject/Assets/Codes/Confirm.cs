using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Confirm : MonoBehaviour
{
    // Start is called before the first frame update

    public void ChangeScene()
    {
        SceneManager.LoadScene("SampleScene");
    }

    // Update is called once per frame
    void Update()
    {
        if(PlayerPrefs.GetString("path").Length<=0 & PlayerPrefs.GetInt("didChoose")==0)
        {
            gameObject.GetComponent<UnityEngine.UI.Button>().interactable = false;
        }
        else if(PlayerPrefs.GetString("path").Length > 0 & PlayerPrefs.GetInt("didChoose") == 1)
        {
            gameObject.GetComponent<UnityEngine.UI.Button>().interactable = true;
        }
    }
}
