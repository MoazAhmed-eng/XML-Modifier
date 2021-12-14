using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class getXML : MonoBehaviour
{
    public void OpenExplorer()
    {
       string path=EditorUtility.OpenFilePanel("Select a XML file", "", "xml");
        
        if (path.Length>0)
        {
            if((path[path.Length - 1] == 'l' & path[path.Length - 2] == 'm' & path[path.Length - 3] == 'x'))
            {
                PlayerPrefs.SetInt("didChoose", 1);
                PlayerPrefs.SetString("path", path);
            }
        }
        else
        {
            // didn't choose a file 
            PlayerPrefs.SetString("path", "");
        }
    }
}
