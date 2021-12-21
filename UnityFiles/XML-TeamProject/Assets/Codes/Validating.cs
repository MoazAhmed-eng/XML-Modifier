using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;
using TMPro;


class Validating : MonoBehaviour
{
    public void Validate()
    
    {
        /* !Comment: Get the file path from the user*/
        string xmlFile = GameObject.FindGameObjectWithTag("mainText").GetComponent<UnityEngine.UI.InputField>().text;
        List<int> insertIndices1 = new List<int>();
        List<int> insertIndices2 = new List<int>();


        List<string> faultyTags = new List<string>();
        Stack<string> stack = new Stack<string>();

        /* !Comment: openingTags is mainly used to know whether the root opening tag exists or no*/
        List<string> openingTags = new List<string>();
        int startindex = 0;

        /* Part 2 checking for stack problems after syntax errors*/
        for (int j = 0; j < xmlFile.Length; j++)
        {
            if (xmlFile[j] == '<')
            {
                /* !Comment: if it is a comment, discard it*/
                if (xmlFile[j + 1] == '!' || xmlFile[j + 1] == '?')
                {
                    continue;
                }

                /* !Comment: if it is an opening tag, push it to the stack*/
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
                    openingTags.Add(temp);
                    stack.Push(temp);
                }
                /* !Comment: if it is a closing tag, we will check for several errors*/
                else if (xmlFile[j + 1] == '/')
                {
                    /* !Comment: Error1: Found closing tag meanwhile the stack is empty*/
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
                        string root = tempp.Substring(2);
                        /* !Comment: If it is the closing tag of the root, and there is no opening tag for it, we add it 
                        to the file, then break*/
                        if (!openingTags.Contains(root))
                        {
                            faultyTags.Add("The root was missing");
                            break;
                        }
                        /* !Comment: if it is a closing tag, but not the root, we remove it from the file*/
                        faultyTags.Add("There is no corresponding tag for the following  " + tempp + ", we removed it");
                        sb[j] = '*';
                        continue;
                    }
                    int insertHere = j;

                    j += 2;
                    string temp = "";
                    int startIndex = j; //start of the potentially faulty tag
                    while (xmlFile[j] != '>' & j < xmlFile.Length) // read the name of the tag
                    {
                        temp += xmlFile[j];
                        j++;
                    }
                    int stopHere = j+1;





                    int lastIndex = j; //ending of the potentially faulty tag '>'
                    /* !Comment: If the stack top is the same as the closing tag we received, Pop it!*/
                    string top = (string)stack.Peek();
                    if (top.Equals(temp))
                    {
                        stack.Pop();
                        continue;
                    }
                    /* !Comment: Received different closing tag than the expected on the top of the stack*/
                    else
                    {
                        insertIndices1.Add(insertHere);
                        insertIndices2.Add(stopHere);
                        string correctPart = xmlFile.Substring(0, startIndex - 2);
                        int countChar = 0;
                        /*As long as we didn't reach the root, we pop all the tags until we reach to the requried
                        tag*/
                        while (!top.Equals(temp))
                        {
                            /* !Comment: If we reached the root, and didn't find the tag, we remove it*/
                            if (stack.Count == 1)
                            {
                                faultyTags.Add("The following closing tag: " + temp + "wasn't opened ! ");
                                StringBuilder sb = new StringBuilder(xmlFile);
                                startIndex -= 2;
                                startindex = startIndex;
                                while (xmlFile[startIndex] != '>')
                                {
                                    sb[startIndex] = '*';
                                    startIndex++;
                                }
                                sb[startIndex] = '*';
                                break;

                            }
                            string add = "</" + (string)stack.Peek() + ">";
                            correctPart += add;
                            faultyTags.Add("Inidentical closing tags !, the expected tag is: " + top + ", found: " + temp);
                            countChar += add.Length;
                            stack.Pop();
                            top = (string)stack.Peek();
                        }
                        if (stack.Count > 1)
                        {
                            stack.Pop();
                        }

                    }
                }
            }
        }


            for(int i =0; i<insertIndices1.Count;i++)
            {
            xmlFile=xmlFile.Insert(insertIndices2[insertIndices2.Count - 1 - i], "</color=#ff0000ff>");
            xmlFile=xmlFile.Insert(insertIndices1[insertIndices1.Count - 1 - i], "<color=#ff0000ff>");
            }





        while (stack.Count != 0)
        {
            faultyTags.Add("The Following tag wasn't closed: " + (string)stack.Peek());
            stack.Pop();
        }

        StreamWriter writer = new StreamWriter(@"validationLOG.txt");
        writer.AutoFlush = true; // to flush the buffer (common error to not print all the text if set to false)
        foreach (string error in faultyTags)
        {
            writer.WriteLine(error);
        }
        writer.Close();



        if (faultyTags.Count != 0)
        {
            GameObject.FindGameObjectWithTag("hide").transform.Find("InputField (TMP)").gameObject.SetActive(true);
            GameObject.FindGameObjectWithTag("hide").transform.Find("InputField (TMP)").gameObject.GetComponent<TMP_InputField>().text = xmlFile;
            GameObject.FindGameObjectWithTag("instr").GetComponent<UnityEngine.UI.Text>().color = Color.red;
            GameObject.FindGameObjectWithTag("instr").GetComponent<UnityEngine.UI.Text>().text = $"Errors Found! , The first one is : {faultyTags[0]}";
            PlayerPrefs.SetInt("isValid", 0);
        }
        else
        {
            GameObject.FindGameObjectWithTag("instr").GetComponent<UnityEngine.UI.Text>().color = Color.green;
            GameObject.FindGameObjectWithTag("instr").GetComponent<UnityEngine.UI.Text>().text = " XML is Valid ";
            PlayerPrefs.SetInt("isValid", 1);
        }

        PlayerPrefs.SetInt("checked", 1);
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