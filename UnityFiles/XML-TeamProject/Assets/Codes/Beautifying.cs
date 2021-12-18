using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;



public class Beautifying : MonoBehaviour
{

    public string addIndentation(int n )
    {
        StringBuilder str = new StringBuilder();

        for(int i = 0; i<n; i++)
        {
            str.Append("    ");
        }
        return str.ToString();
    }

    public void Beautify()
    {
        Stack s = new Stack();
        List<string> arr = Jsonify.ScanThroughXML();
        StringBuilder str = new StringBuilder();
        StringBuilder lastOpenedTag = new StringBuilder();
        int level = -1;


        foreach (string tocken in arr)
        {
            if(tocken[0]=='_' || tocken[0]=='*')
            {
                string name = tocken.Substring(1);
                s.Push(name);
                level++;
                str.Append(addIndentation(level));
                str.Append($"<{name}>\n");
            }

            else if(tocken[0]=='/')
            {
                str.Append(addIndentation(level));
                str.Append($"</{s.Peek()}>\n");
                s.Pop();
                level--;
            }

            else
            {
                str.Append(addIndentation(level));
                str.Append($"  {tocken}\n");
            }
        }

        GameObject.FindGameObjectWithTag("mainText").GetComponent<UnityEngine.UI.InputField>().text = str.ToString();
        GameObject.FindGameObjectWithTag("instr").GetComponent<UnityEngine.UI.Text>().color = Color.green;
        GameObject.FindGameObjectWithTag("instr").GetComponent<UnityEngine.UI.Text>().text = " Beautify Done";


        StreamWriter writer = new StreamWriter("beauty.xml");
        writer.AutoFlush = true;
        writer.Write(str);
        writer.Close();
    }

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
