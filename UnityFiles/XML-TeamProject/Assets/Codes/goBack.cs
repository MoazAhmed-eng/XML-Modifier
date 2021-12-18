using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class goBack : MonoBehaviour
{

    public void ChangeScene()
    {
        SceneManager.LoadScene("Start");
    }

}
