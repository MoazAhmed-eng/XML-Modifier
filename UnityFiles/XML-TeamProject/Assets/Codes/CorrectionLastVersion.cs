using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

    public class ValidationModified
    {
    public class Correction
    {
        public static void Correctingg()
        {
            /* !Comment: Get the file path from the user*/
            string filePath = @"E:\Kolyaa\3rd Year\1st Semester\DataStructure\XML File\UnityFiles\XML-TeamProject\Assets\test.txt";
            List<string> lines = new List<string>();
            List<string> correctedLines = new List<string>();
            List<string> faultyTags = new List<string>();
            Stack<string> stack = new Stack<string>();
            /* !Comment: openingTags is mainly used to know whether the root opening tag exists or no*/
            List<string> openingTags = new List<string>();
            int startindex = 0;
            lines = File.ReadAllLines(filePath).ToList();
            string xmlFile = File.ReadAllText(filePath);
            string lineToString;
            int start = 0, end = 0;
            bool foundChar1, foundChar2, foundError;
            string correctedLine;
            int status = 1;
            foreach (string line in lines)
            {
                /* Part 1 found two consecutive < without >*/
                correctedLine = "";
                lineToString = line;
                foundChar1 = false;
                foundChar2 = false;
                foundError = false;
                //Console.WriteLine(lineToString);
                for (int i = 0; i < lineToString.Length; i++)
                {
                    if (lineToString[i] == '<')
                    {
                        if (!foundChar1 & !foundChar2)
                        {
                            start = i + 1;
                            foundChar1 = true;
                            continue;
                        }
                        else if (foundChar1 & !foundChar2)
                        {
                            foundError = true;
                            int lengthOfTag = lineToString.Length - (lineToString.LastIndexOf('<') + 3);
                            correctedLine = lineToString.Substring(0, lineToString.IndexOf('<'));
                            correctedLine += lineToString.Substring(lineToString.IndexOf('<'), lengthOfTag + 1) + '>';
                            correctedLine += lineToString.Substring(lineToString.IndexOf('<') + lengthOfTag + 1);
                            correctedLines.Add(correctedLine);
                            break;
                        }
                    }
                    else if (lineToString[i] == '>')
                    {
                        if (!foundChar2)
                        {
                            end = i;
                            foundChar2 = true;
                        }
                        else if (foundChar2)
                        {

                        }
                    }
                    if (foundChar1 && foundChar2)
                    {
                        foundChar1 = false;
                        foundChar2 = false;
                    }
                }
                if (!foundError)
                {
                    correctedLines.Add(lineToString);
                }
                //Console.WriteLine(lineToString);
            }
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
                            
                            status = 0;
                            //PlayerPrefs.SetInt("isValid", status);
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
                            if(!openingTags.Contains(root))
                            {
                                faultyTags.Add("The root was missing");
                                xmlFile = "<" + root + ">\n" + xmlFile;
                                break;
                            }
                            /* !Comment: if it is a closing tag, but not the root, we remove it from the file*/
                            faultyTags.Add("There is no corresponding tag for the following  " + tempp +", we removed it");
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
                            //Debug.Log($"ERROR , Expected: {top} , but found {temp}");
                            //status = 0;
                            //PlayerPrefs.SetString("Error", $"ERROR , Expected: {top} , but found {temp}");
                            //PlayerPrefs.SetInt("isValid", status);
                            //PlayerPrefs.SetInt("startIndex", startIndex);
                            //PlayerPrefs.SetInt("lastIndex", lastIndex);
                            string correctPart = xmlFile.Substring(0, startIndex - 2);
                            int countChar = 0;
                            /*As long as we didn't reach the root, we pop all the tags until we reach to the requried
                            tag*/
                            while (!top.Equals(temp))
                            {
                                /* !Comment: If we reached the root, and didn't find the tag, we remove it*/
                                if (stack.Count == 1)
                                {
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
                                    xmlFile = xmlFile.Replace("*", "");
                                    //Console.WriteLine(xmlFile);
                                    break;

                                }
                                string add = "</" + (string)stack.Peek() + ">";
                                correctPart += add;
                                faultyTags.Add("Inidentical closing tags, the expected tag is: " + top + ", found: " + temp);
                                countChar += add.Length;
                                stack.Pop();
                                top = (string)stack.Peek();
                            }
                            if (stack.Count > 1)
                            {
                                stack.Pop();
                            }

                            xmlFile = correctPart + xmlFile.Substring(startindex);
                            j += countChar-9;


                        }




                    }


                }
            }

            while (stack.Count != 0)
            {
                xmlFile += '\n' + "</" + (string)stack.Peek() + ">";
                faultyTags.Add("The Following tag wasn't closed: " + (string)stack.Peek());
                stack.Pop();
            }
        }

    }
}
    


