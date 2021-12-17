using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 class Validating : MonoBehaviour
{
    public void Validate()
    {
        int status = 1;
        Stack stack = new Stack();
        List<string> errorLines = new List<string>();
        string str = GameObject.FindGameObjectWithTag("mainText").GetComponent<UnityEngine.UI.InputField>().text;
        int length = str.Length;
        for (int index = 0; index < length; index++)
        {
            if (str[index] == '<')
            {
                if (str[index + 1] == '!' | str[index + 1] == '?') // if it's a comment or a processing statement , ignore it
                {
                    continue;
                }

                else if (str[index + 1] == '/') // if it's a closing tag  read the name ,and then pop the top element of the stack
                {
                    if (stack.Count == 0) // if there's opening tag and the stack is empty then it's faulty
                    {
                        status = 0;
                        PlayerPrefs.SetInt("isValid", status);
                        return;
                    }

                    index += 2;
                    string temp = "";
                    int startIndex = index; //start of the potentially faulty tag

                    while (str[index] != '>' & index < length) // read the name of the tag
                    {
                        temp += str[index];
                        index++;
                    }
                    int lastIndex = index; //ending of the potentially faulty tag

                    string top = (string)stack.Peek();


                    if (top.Equals(temp))
                    {
                        stack.Pop();
                        continue;
                    }
                    else
                    {
                        Debug.Log($"ERROR , Expected: {top} , but found {temp}");
                        status = 0;
                        PlayerPrefs.SetString("Error", $"ERROR , Expected: {top} , but found {temp}");
                        PlayerPrefs.SetInt("isValid", status);
                        PlayerPrefs.SetInt("startIndex", startIndex);
                        PlayerPrefs.SetInt("lastIndex", lastIndex);
                        errorLines.Add("Inidentical tags, found closing tag: "+ temp + "expected closing tag: " + top);
                    }

                }

                else
                {
                    string temp = ""; // a temporary string to store the name of the tag
                    index++; // to advance to the next character after the '<'

                    /*
                    * if it's an opening tag , store the tag name while ignoring the attributes , and also check it the starting tag was never closed 
                    * and also check for the self closing tags
                    */

                    while (str[index] != '>' & index < length & str[index] != ' ')
                    {
                        // the opening tag wasn't closed and another was opened
                        if (str[index] == '<')
                        {
                            status = 0;
                            PlayerPrefs.SetInt("isValid", status);
                            errorLines.Add("Found two consecutive <, without >");
                        }

                        if (str[index] == '/' & str[index + 1] == '>') // check for self-closing tags
                        {
                            index++; // because we already checked the next character 
                            temp = "SELF-CLOSING-TAG"; // so it doesn't get pushed to the stack
                            break; // the tag ends
                        }

                        temp += str[index];
                        index++;
                    }
                    if (temp.Length == 0) // the tag's name is empty
                    {
                        status = 0;
                        PlayerPrefs.SetInt("isValid", status);
                        errorLines.Add("Found empty tag <>");
                    }

                    // if it's self closing
                    if (temp.Equals("SELF-CLOSING-TAG"))
                    {
                        continue; 
                    }

                    stack.Push(temp);
                }
            }
        }

        while (stack.Count!=0)
        {
            stack.Pop();
            errorLines.Add("Found unclosed opening tags, popping them");
            Debug.Log("the stack wasn't empty at the end soo, BAD XML !!");
            status = 0;
            PlayerPrefs.SetInt("isValid", status);
        }

        PlayerPrefs.SetInt("isValid", status);
        return;
    }
}
