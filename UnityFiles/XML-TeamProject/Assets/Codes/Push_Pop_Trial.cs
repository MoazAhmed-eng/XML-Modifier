using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFirstProject
{
    public class Jasonify
    {
        public void Jasonifing()
        {
            string filePath = @"E:\Embedded Diploma\Github\DataStructure-TeamProject\UnityFiles\XML-TeamProject\Assets\test.txt";
            string output = @"E:\Embedded Diploma\Github\DataStructure-TeamProject\UnityFiles\XML-TeamProject\Assets\Codes\Output.txt";
            List<string> lines = new List<string>();
            List<string> correctedLines = new List<string>();
            Stack<string> stack = new Stack<string>();
            lines = File.ReadAllLines(filePath).ToList();
            string lineToString;
            string correctedLine;
            int start = 0, end = 0;
            bool foundChar1, foundChar2, foundSpace;
            foreach (string line in lines)
            {
                lineToString = line;
                foundChar1 = false;
                foundChar2 = false;
                foundSpace = false;
                for (int i = 0; i < lineToString.Length; i++)
                {
                    if (lineToString[i] == '<')
                    {
                        if (!foundChar1)
                        {
                            start = i + 1;
                            foundChar1 = true;
                            continue;
                        }
                        else
                        {
                            correctedLine = lineToString.Substring(start - 1, i);
                        }
                    }
                    if (lineToString[i] == '>')
                    {
                        end = i;
                        foundChar2 = true;
                    }
                    if (foundChar1 && foundChar2)
                    {
                        if (lineToString[start] == '!' | lineToString[start] == '?')
                        {
                            continue;
                        }
                        if (lineToString[start] != '/')
                        {
                            for (int k = start; k < end; k++)
                            {
                                if (lineToString[k] == ' ')
                                {
                                    end = k;
                                    Console.WriteLine("I will push " + lineToString.Substring(start, end - start + 1));
                                    stack.Push(lineToString.Substring(start, end - start + 1));
                                    foundSpace = true;
                                    break;
                                }

                            }
                            if (!foundSpace)
                            {
                                Console.WriteLine("I will Push " + lineToString.Substring(start, end - start));
                                stack.Push(lineToString.Substring(start, end - start));
                            }
                        }
                        else if (lineToString[start] == '/')
                        {
                            //Console.WriteLine("I will pull " + lineToString.Substring(start+1,end-start-1));
                            if (lineToString.Substring(start + 1, end - start - 1) == (stack.Peek()))
                            {
                                Console.WriteLine("I will pull " + lineToString.Substring(start + 1, end - start - 1));
                                Console.WriteLine("Identical Closing Tag");
                                stack.Pop();
                            }
                            else
                            {
                                Console.WriteLine("Found:" + lineToString.Substring(start + 1, end - start - 1) + " ," + "Stack Top :" + stack.Peek());
                            }

                            /*
                            if(stack.Count!=0)
                            {
                                    Console.WriteLine("I need to Pull " + stack.Peek());
                                
                            }
                            */

                        }
                        foundChar1 = false;
                        foundChar2 = false;
                    }

                }





            }
        }
    }
}