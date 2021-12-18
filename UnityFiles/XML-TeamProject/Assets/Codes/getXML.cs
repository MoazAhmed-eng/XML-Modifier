using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using SimpleFileBrowser;


public class getXML : MonoBehaviour
{

    public void doSomeChecking(string path)
    {


        if (path.Length > 0)
        {
            if ((path[path.Length - 1] == 'l' & path[path.Length - 2] == 'm' & path[path.Length - 3] == 'x'))
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

    public void OpenExplorer()
    {
        FileBrowser.SetFilters(false, new FileBrowser.Filter( "XML file", ".xml"));
        FileBrowser.SetDefaultFilter(".xml");
        FileBrowser.ShowLoadDialog((path)=> { doSomeChecking(path[0]); },()=> { }, FileBrowser.PickMode.Files);

    }
}
