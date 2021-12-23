using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

public class Correcting : MonoBehaviour
{

    public void Correct()

    {
        /* !Comment: Get the file path from the user*/
        List<string> faultyTags = new List<string>();
        List<int> indices = new List<int>();
        List<string> corrections = new List<string>();
        Stack<string> stack = new Stack<string>();
        /* !Comment: openingTags is mainly used to know whether the root opening tag exists or no*/
        List<string> openingTags = new List<string>();
        int startindex = 0;
        string xmlFile = GameObject.FindGameObjectWithTag("mainText").GetComponent<UnityEngine.UI.InputField>().text;
        bool foundComment = false, addedRoot = false;
        int endIndexOfComment = 0;
        /* Part 2 checking for stack problems after syntax errors*/
        for (int j = 0; j < xmlFile.Length; j++)
        {
            if (xmlFile[j] == '<')
            {
                /* !Comment: if it is a comment, discard it*/
                if (xmlFile[j + 1] == '!' || xmlFile[j + 1] == '?')
                {
                    while (xmlFile[j] != '>')
                    {
                        j++;
                    }
                    j++;
                    endIndexOfComment = j;
                    foundComment = true;
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
                        //status = 0;
                        //PlayerPrefs.SetInt("isValid", status);
                        StringBuilder sb = new StringBuilder(xmlFile);
                        while (xmlFile[j] != '>' && j < xmlFile.Length)
                        {
                            tempp += sb[j];
                            sb[j] = '*';
                            j++;
                        }
                        string root = tempp.Substring(2);
                        tempp += sb[j];
                        /* !Comment: If it is the closing tag of the root, and there is no opening tag for it, we add it 
                        to the file, then break*/
                        if (!openingTags.Contains(root) && !addedRoot && openingTags.Count != 0)
                        {
                            faultyTags.Add("The root was missing");
                            root = "<" + root + ">";
                            corrections.Add(root);
                            addedRoot = true;
                            if (!foundComment)
                            {
                                indices.Add(0);
                                break;
                            }
                            else if (foundComment)
                            {
                                indices.Add(endIndexOfComment);
                                break;
                            }

                        }
                        /* !Comment: if it is a closing tag, but not the root, we remove it from the file*/
                        faultyTags.Add("There is no corresponding tag for the following  " + tempp + ", we removed it");
                        sb[j] = '*';
                        xmlFile = sb.ToString();
                        continue;
                    }
                    j += 2;
                    string temp = "";
                    int startIndex = j; //start of the potentially faulty tag
                    int z = j;
                    while (xmlFile[j] != '>' & j < xmlFile.Length) // read the name of the tag
                    {
                        temp += xmlFile[j];
                        j++;
                    }

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
                        /*As long as we didn't reach the root, we pop all the tags until we reach to the requried
                        tag*/
                        while (!top.Equals(temp))
                        {
                            /* !Comment: If we reached the root, and didn't find the tag, we remove it*/
                            if (stack.Count == 1)
                            {
                                if (!openingTags.Contains(temp) && !addedRoot && openingTags.Count == 1)
                                {
                                    string fakeRoot = (string)stack.Pop();
                                    stack.Push(temp);
                                    stack.Push(fakeRoot);
                                    faultyTags.Add("The root was missing");
                                    temp = "<" + temp + ">";
                                    corrections.Add(temp);
                                    addedRoot = true;
                                    if (!foundComment)
                                    {
                                        openingTags.Add(temp);
                                        indices.Add(0);
                                        break;
                                    }
                                    else if (foundComment)
                                    {
                                        openingTags.Add(temp);
                                        indices.Add(endIndexOfComment);
                                        break;
                                    }
                                }

                                faultyTags.Add("The following closing tag: " + temp + " wasn't found in the stack, so we removed it");
                                StringBuilder sb = new StringBuilder(xmlFile);
                                startIndex -= 2;
                                startindex = startIndex;
                                while (xmlFile[startIndex] != '>')
                                {
                                    sb[startIndex] = '*';
                                    startIndex++;
                                }
                                sb[startIndex] = '*';
                                xmlFile = sb.ToString();
                                break;

                            }
                            string add = "</" + (string)stack.Peek() + ">";
                            corrections.Add(add);
                            indices.Add(z - 2);
                            //z =add.Length;
                            faultyTags.Add("Inidentical closing tags, the expected tag is: " + top + ", found: " + temp);
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

        while (stack.Count != 0 && !addedRoot)
        {

            xmlFile += '\n' + "</" + (string)stack.Peek() + ">";
            faultyTags.Add("The Following tag wasn't closed: " + (string)stack.Peek());
            stack.Pop();
        }
        if (!addedRoot)
        {
            for (int i = 0; i < corrections.Count; i++)
            {

                xmlFile = xmlFile.Insert(indices[corrections.Count - 1 - i], corrections[corrections.Count - 1 - i]);
            }
        }
        if (addedRoot)
        {
            for (int i = 1; i < corrections.Count; i++)
            {

                xmlFile = xmlFile.Insert(indices[corrections.Count - 1 - i], corrections[corrections.Count - 1 - i]);
            }
            xmlFile = xmlFile.Insert(indices[corrections.Count - 1], corrections[corrections.Count - 1]);
        }
        xmlFile = xmlFile.Replace("*", "");

        ///
        GameObject.FindGameObjectWithTag("mainText").GetComponent<UnityEngine.UI.InputField>().text = xmlFile;
        GameObject.FindGameObjectWithTag("instr").GetComponent<UnityEngine.UI.Text>().color = Color.green;
        GameObject.FindGameObjectWithTag("instr").GetComponent<UnityEngine.UI.Text>().text = " Correction Done";

        StreamWriter writer = new StreamWriter(@"correct.xml");
        writer.AutoFlush = true; // to flush the buffer (common error to not print all the text if set to false)
        writer.Write(xmlFile);
        writer.Close();

        PlayerPrefs.SetInt("Corrected", 1);


    }






    // Update is called once per frame
    void Update()
    {
        if (PlayerPrefs.GetInt("json") == 1 || PlayerPrefs.GetInt("correct")==1)
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
