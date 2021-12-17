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
            string filePath = @"E:\Kolyaa\3rd Year\1st Semester\DataStructure\XML File\UnityFiles\XML-TeamProject\Assets\test.txt";
            List<string> lines = new List<string>();
            List<string> correctedLines = new List<string>();
            List<string> faultyTags = new List<string>();
            Stack<string> stack = new Stack<string>();
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
                    if (xmlFile[j + 1] == '!' || xmlFile[j + 1] == '?')
                    {
                        continue;
                    }
                    else if (xmlFile[j + 1] != '/') //opening
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

                        string top = (string)stack.Peek();
                        if (top.Equals(temp))
                        {
                            stack.Pop();
                            continue;
                        }
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
                            while (!top.Equals(temp))
                            {
                                string add = "</" + (string)stack.Peek() + ">";
                                correctPart += add;
                                faultyTags.Add("Inidentical closing tags, Stack Top: " + add +"\n" +"Closing tag in the file: " + temp);
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
        }

        public static void Main()
        {
            Correctingg();
        }
    }
}
    


