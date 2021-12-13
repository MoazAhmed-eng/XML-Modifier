using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class Minifying : MonoBehaviour
{
    public void Minify()
    {
        string str = GameObject.FindGameObjectWithTag("mainText").GetComponent<UnityEngine.UI.InputField>().text;
        string Minified = "";
        for (int i = 0; i < str.Length; i++)
        {
            if (str[i] == ' ')
            {
                if (str[i - 1] == '>' || str[i + 1] == '<' || str[i - 1] == ' ' || str[i + 1] == ' ')
                {
                    continue;
                }
                else { Minified += str[i]; }
            }
            else
            {
                Minified += str[i];
            }
        }
        Minified = Minified.Trim();
        Minified = Minified.Replace("\n", "");
        Minified = Minified.Replace("\r", "");

        using (StreamWriter writer = new StreamWriter(@"Assets/MINIFIED.txt"))
        {
            writer.Write(Minified);
        }
        GameObject.FindGameObjectWithTag("mainText").GetComponent<UnityEngine.UI.InputField>().text=Minified;

    }
}
