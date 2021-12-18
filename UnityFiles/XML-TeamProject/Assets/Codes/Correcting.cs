using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

public class Correcting : MonoBehaviour
{

    public  void Correct()
    {
        List<string> correctedLines = new List<string>();
        List<string> faultyTags = new List<string>();
        Stack<string> stack = new Stack<string>();
        string xmlFile = GameObject.FindGameObjectWithTag("mainText").GetComponent<UnityEngine.UI.InputField>().text; ;

        for (int j = 0; j < xmlFile.Length; j++)
        {
            if (xmlFile[j] == '<')
            {
                if (xmlFile[j + 1] == '!' || xmlFile[j + 1] == '?')
                {
                    continue;
                }
                else if (xmlFile[j + 1] != '/') 
                {
                    j++;
                    string temp = "";

                    while (xmlFile[j] != '>')
                    {
                        if (xmlFile[j] == ' ')
                        {
                            break;
                        }
                        temp += xmlFile[j];
                        j++;
                    }
                    stack.Push(temp);
                }
                else if (xmlFile[j + 1] == '/')
                {
                    if (stack.Count == 0)
                    {
                        string tempp = "";
                        StringBuilder sb = new StringBuilder(xmlFile);
                        while (xmlFile[j] != '>' && j < xmlFile.Length)
                        {
                            tempp += sb[j];
                            sb[j] = '*';
                            j++;
                        }
                        tempp += sb[j];
                        faultyTags.Add("There is no corresponding tag for the following  " + tempp + ", we removed it");
                        sb[j] = '*';
                        xmlFile = sb.ToString();
                        continue;
                    }
                    j += 2;
                    string temp = "";
                    int startIndex = j; //start of the potentially faulty tag
                    while (xmlFile[j] != '>' & j < xmlFile.Length) // read the name of the tag
                    {
                        temp += xmlFile[j];
                        j++;
                    }
                    int lastIndex = j; //ending of the potentially faulty tag '>'

                    string top = (string)stack.Peek();
                    if (top.Equals(temp))
                    {
                        stack.Pop();
                        continue;
                    }
                    else
                    {
                        string correctPart = xmlFile.Substring(0, startIndex - 2);
                        int countChar = 0;
                        while (!top.Equals(temp))
                        {
                            string add = "</" + (string)stack.Peek() + ">";
                            correctPart += add;
                            faultyTags.Add("Inidentical closing tags, Stack Top: " + add + "\n" + "Closing tag in the file: " + temp);
                            countChar += add.Length;
                            stack.Pop();
                            if (stack.Count == 0)
                            {
                                correctPart += '\n';
                                StringBuilder sb = new StringBuilder(xmlFile);
                                startIndex -= 2;
                                while (xmlFile[startIndex] != '>')
                                {
                                    sb[startIndex] = '*';
                                    startIndex++;
                                }
                                sb[startIndex++] = '*';
                                xmlFile = sb.ToString();
                                break;

                            }
                            top = (string)stack.Peek();
                        }
                        stack.Pop();
                        xmlFile = correctPart + xmlFile.Substring(startIndex - 2);
                        j += countChar;


                    }
                }
            }
        }

        while (stack.Count != 0)
        {
            xmlFile += '\n' + "</" + (string)stack.Peek() + ">";
            faultyTags.Add("</" + (string)stack.Peek() + ">");
            stack.Pop();
        }
        xmlFile += '\n';
        xmlFile = xmlFile.Replace("*", "");


        GameObject.FindGameObjectWithTag("mainText").GetComponent<UnityEngine.UI.InputField>().text = xmlFile;
        GameObject.FindGameObjectWithTag("instr").GetComponent<UnityEngine.UI.Text>().color = Color.green;
        GameObject.FindGameObjectWithTag("instr").GetComponent<UnityEngine.UI.Text>().text = " Correction Done";

        StreamWriter writer = new StreamWriter(@"correct.xml");
        writer.AutoFlush = true; // to flush the buffer (common error to not print all the text if set to false)
        writer.Write(xmlFile);
        writer.Close();
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

        if (PlayerPrefs.GetInt("isValid")!=1 && PlayerPrefs.GetInt("checked")==1)
        {
            gameObject.GetComponent<UnityEngine.UI.Button>().interactable = true;
        }
        else
        {
            gameObject.GetComponent<UnityEngine.UI.Button>().interactable = false;

        }
    }
}
