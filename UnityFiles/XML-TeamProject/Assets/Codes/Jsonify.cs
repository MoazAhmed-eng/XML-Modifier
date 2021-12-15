using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;




public class TreeNode
{
    string name;
    string value;
    int depth;
    List<string> attributes;
    List<TreeNode> children;
    List<List<TreeNode>> childrenCollected;

    public TreeNode(string name , string value)
    {
        this.depth = 0;
        this.name = name;
        this.value = value;
        this.attributes = null;
        this.children = null;
    }

    public TreeNode()
    {
        this.depth = 0;
        this.name = null;
        this.value = null;
        this.attributes = null;
        this.children = null;
        this.childrenCollected = null;
    }

    public List<TreeNode> getChildren()
    {
        return children;
    }

    public List<List<TreeNode>> getChildrenCollected()
    {
        return childrenCollected;
    }

    public int getDepth()
    {
        return depth;
    }
    public string getName()
    {
        return name;
    }

    public void addChildren(TreeNode node)
    {
        if(this.children==null)
        {
            this.children = new List<TreeNode>();
        }
        this.children.Add(node);
    }
     public string getValue()
    {
        return value;
    }

     public void setDepth( int depth)
    {
        this.depth = depth;
    }

     public void setName(string name)
    {
        this.name = name;
    }

    public void setValue(string value)
    {
        this.value = value;
    }

    public void setChildrenCollected(List<List<TreeNode>> list)
    {
        this.childrenCollected = list;
    }

    public void setAttributes(List<string> attributes)
    {
        this.attributes = attributes;
    }
}

public class Tree
{
    TreeNode root;

    Tree()
    {
        this.root = null;
    }

    Tree(TreeNode root)
    {
        this.root = root;
    }

    public TreeNode getRoot()
    {
        return root;
    }
}


public class Jsonify : MonoBehaviour
{
    /* this function parses the xml and gives back a list containing a tockenized version of the xml file ,
     * where the tag names is saved as "_tagName"
     * and the values are saved as is
     * and the attributes are saved as "*attributeName"
    */
    public List<string> ScanThroughXML() 
        // works with minified text
    {
        GameObject.FindGameObjectWithTag("minify").GetComponent<Minifying>().Minify(); // call the minfy function

        string str = GameObject.FindGameObjectWithTag("mainText").GetComponent<UnityEngine.UI.InputField>().text;

        List<string> tockenized = new List<string>();

        for (int i = 0; i < str.Length; i++)

        {
            if (str[i] == '<' && str[i + 1] == '/') tockenized.Add("/");

            if (str[i] == '<' && (str[i + 1] == '!' || str[i + 1] == '?')) continue;

            if (str[i] == '<' && str[i + 1] != '/')
            {
                i++;
                StringBuilder temp = new StringBuilder();
                StringBuilder attTemp = new StringBuilder();

                temp.Append("_");

                while (str[i] != '>') // read the name of the tag
                {
                    temp.Append(str[i]);
                    i++;
                }
                tockenized.Add(temp.ToString().Split(' ')[0]);
                i--;

                foreach (string element in temp.ToString().Split(' '))
                {
                    if (element.Equals(temp.ToString().Split(' ')[0]))
                    {
                        continue;
                    }
                    attTemp.Append('*'); // add the * to the temp
                    attTemp.Append(element.Split('=')[0]); // add the name of the attribute
                    tockenized.Add(attTemp.ToString()); // add it to the list
                    tockenized.Add(element.Split('=')[1].Replace("\"","")); // add its value 
                    tockenized.Add("/"); // add '/' because we deal with it as a tag
                    attTemp.Clear();
                }
            } // Append tag names with _ in the start and attributes with *

            else if (str[i] == '>')
            {
                i++;
                StringBuilder temp = new StringBuilder();
                while (i < str.Length)
                {
                    if (str[i] != '<')
                    {
                            temp.Append(str[i]);
                            i++;
                    }
                    else
                    {
                        break;
                    } 
                }
                string t = temp.ToString();
                i--;
                if (t.Length > 0)
                {
                    tockenized.Add(t);
                }
            } // Append the values (data)

        }
        return tockenized;
    }




    // the one that prints the json string and saves it
    public void  RecursivePrint(TreeNode node)
    {

        if (node.getChildren() == null) return;

        List<List<TreeNode>> lists = new List<List<TreeNode>>();

        foreach(TreeNode child in node.getChildren())
        {
            RecursivePrint(child);
            bool flag = false;
            foreach(List<TreeNode> list in lists)
            {
                if(child.getName().Equals(list[0].getName()))
                {
                    list.Add(child);
                    flag = true;
                    break;
                }
            }

            if(!flag)
            {
                List<TreeNode> temp = new List<TreeNode>();
                temp.Add(child);
                lists.Add(temp);
            }
        }

        node.setChildrenCollected(lists);
    } 


    public void recursionAgain(TreeNode node, StreamWriter writer,bool isRepeated)
    {
        if(!isRepeated && node.getChildren()!=null)
        {
            writer.Write($"{{\"{node.getName()}\" : ");
        }

        if (node.getChildrenCollected() == null)
        {
            writer.Write($"\"{node.getName()}\" : ");
            writer.Write($"\"{node.getValue()}\",");
            writer.WriteLine("");
            return;
        }

        foreach(List<TreeNode> list in node.getChildrenCollected())
        {
            if(list.Count==1)
            { 
                recursionAgain(list[0], writer,false);
            }
            else
            {
                writer.Write($"[{{{list[0].getName()} : ");
                foreach(TreeNode t in list)
                {
                    writer.Write("{");
                    recursionAgain(t,writer,true);
                    writer.Write(",");
;


                }
                writer.Write("]");

            }
        }

    }

    public void  ShowTree() // the one used with the button
    {
        StreamWriter writer = new StreamWriter(@"Assets/JSON.txt");
        writer.AutoFlush = true;
        List<string> arr = ScanThroughXML();
        TreeNode node = new TreeNode();
        int i = 0;
        fillTree(node, arr, ref i);
        RecursivePrint(node);
        recursionAgain(node,writer,false);



    }

    /*
     *  This function will recursively fill the tree , as follows : 
     *  the first element is certainly an open tag so save the name and the attributes to the tree
     *  
     *  The two base cases : 
     *  1- if it's a '/' then it's a closing tag so return
     *  2- if it's a data then save it to the current node and return (Here we are making the assumption that each tag either
     *  have a data or children)
     *  
     *  
     *  the Recursive case :
     *  if the following element is starting with '_'  or '*' then it's an opening tag or attribure (which is treated as an openingTag)  so create a node
     *  and append it as a child to the current node and then call the function recursively passing to it the 
     *  last created Node.
     *  
     * 
     * 
     */
    public void fillTree(TreeNode node , List<string> arr , ref int i) 
    {
        for(; i<arr.Count;i++)
        {
            if (i == 0) // special case to the save the first element
            {
                node.setName(arr[0].Substring(1));
                continue;
            }
            if (arr[i][0] == ('/')) return; // Base case number one
            if (arr[i][0] != '_' && arr[i][0] != '*')
            {
                node.setValue(arr[i]);
                i++;
                return;
            }
            if(arr[i][0]=='*' || arr[i][0] == '_') // then it must be an opening tag or an attribute
            {
                TreeNode child = new TreeNode();
                child.setName(arr[i].Substring(1));
                child.setDepth(node.getDepth() + 1);
                node.addChildren(child);
                i++;
                fillTree(child,arr,ref i);
            }
        }
    }
}

