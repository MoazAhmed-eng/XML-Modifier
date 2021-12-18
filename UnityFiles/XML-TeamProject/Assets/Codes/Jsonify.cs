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
    List<TreeNode> children;
    List<List<TreeNode>> childrenCollected; // saves the  repeated children collected together in one list 

    public TreeNode(string name , string value)
    {
        this.depth = 0;
        this.name = name;
        this.value = value;
        this.children = null;
    }

    public TreeNode()
    {
        this.depth = 0;
        this.name = null;
        this.value = null;
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
    public static List<string> ScanThroughXML() 
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
                    attTemp.Append("**"); // add the * to the temp
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




    /* the function loops on the children of each node and save them into lists ,
     * where similar tags go into the same list and different tags are in lists of 
     * length one
     */
    public void  saveChildrenCollected(TreeNode node)
    {

        if (node.getChildren() == null) return; // baseCase when there's no childs for the tag

        List<List<TreeNode>> lists = new List<List<TreeNode>>(); // the list of lists that holds the children lists

        foreach(TreeNode child in node.getChildren()) // loop on the children of the tag
        {
            saveChildrenCollected(child); // call it recursively on the child
            bool flag = false; // flag to check if it was found in one of the lists
            foreach(List<TreeNode> list in lists) // loop on each list
            {
                if(child.getName().Equals(list[0].getName())) // if the name of the tag is same as the name of the elements of the list
                {
                    list.Add(child); // add it to the existing list
                    flag = true; // set the flag to true --> meaning it was found in one of the lists
                    break;
                }
            }

            if(!flag) // if not found in the lists
            {
                List<TreeNode> temp = new List<TreeNode>(); // create new list
                temp.Add(child); // append the new  child tag into it 
                lists.Add(temp); // add it to the list of lists
            }
        }

        node.setChildrenCollected(lists); // set the childrenCollected field of the note to the list of lists
    } 



    /*
     * the function loops on the childrenCollected of each node and if a list of the lists
     * has more than one element then it prints as an array , but if the list is of length one then print it
     * as an object normally
     * 
     * the boolean isRepeated indicates that the currentNode is one of a repeated tags that is a part of the array
     * the boolean isLast indicates that this tag is the last child for its parent so not to add an extra ','
     */
    public void printJson(TreeNode node, StreamWriter writer,bool isRepeated,bool isLast)
    {
        if (node.getDepth() == 0) // close the root
        {
            writer.Write('{');
        }

        if(isRepeated && node.getChildren()!=null)
        {
            writer.Write($"{{");
        }

        /* if the tag is not one of the repeated tags and it has children then we need to print its name ,
         * and add a '{'
        */

        if (!isRepeated && node.getChildren()!=null) 
        {
            writer.Write($"\"{node.getName()}\" : {{");
        }


        //while if the tag has no children then we also need to print its name only
        if (node.getChildrenCollected() == null)
        {
            if(!isRepeated)
            {
                writer.Write($"\"{node.getName()}\" : ");
            }
            if (isLast) // if it's the last tag of its parent then no need to add ','
            {
                writer.Write($"\"{node.getValue()}\"");
            }
            else if(!isLast) // otherwise add the ','
            {
                writer.Write($"\"{node.getValue()}\",");
            }
            writer.WriteLine(""); //After finishing the tag , add a new line
            return; // the tag is done so return (base case)
        }

        foreach(List<TreeNode> list in node.getChildrenCollected()) // if it has children loop on each list
        {
            if(list.Count==1) // if the length of the list is one , then there's nothing special
            { 
                if(list==node.getChildrenCollected()[node.getChildrenCollected().Count-1]) // if last tag for the parent
                {
                    printJson(list[0], writer, false, true);
                }
                else // if not the last tag for the parent
                {
                    printJson(list[0], writer, false, false);
                }
            }
            else // if the length is greater than one 
            {
                writer.Write($"\"{list[0].getName()}\" : [ "); //start the repeated tags with the name of the first one and a '['
                foreach(TreeNode t in list) // loop on each tag of the repeated ones
                {
                    if(t==list[list.Count-1]) // if it's the last item in its parent
                    {
                        printJson(t, writer, true, true);
                    }
                    else
                    {
                        printJson(t, writer, true, false);
                        //writer.Write(",");
                    }
                }
                writer.Write("]"); // close the array of the repeated items
            }
        }
        writer.Write("}"); // after processing all the children of a tag close it 

        if(!isLast) // if it was a list and not the last child
        {
            writer.Write(",");
        }

        if(node.getDepth()==0) // close the root
        {
            writer.Write('}');
        }
    }

    public void  ShowJson() // the one used with the button
    {
        StreamWriter writer = new StreamWriter(@"JSON.txt"); // open text file to write the json string in
        writer.AutoFlush = true; // to flush the buffer (common error to not print all the text if set to false)
        List<string> arr = ScanThroughXML(); // tockenize the xml string  (described above)
        TreeNode node = new TreeNode(); // create a node (root)
        int i = 0; // starting index to loop over the tockenized array 
        fillTree(node, arr, ref i); // turn the tockenized version of the xml to a tree
        saveChildrenCollected(node);
        printJson(node,writer,false,true); // print the json string (very self explanatory , hehe :))
        writer.Close(); // close the file
        string json = System.IO.File.ReadAllText(@"JSON.txt"); // read from the JSON text
        GameObject.FindGameObjectWithTag("mainText").GetComponent<UnityEngine.UI.InputField>().text = json; // show it on the screen
        GameObject.FindGameObjectWithTag("instr").GetComponent<UnityEngine.UI.Text>().color = Color.green;
        GameObject.FindGameObjectWithTag("instr").GetComponent<UnityEngine.UI.Text>().text = " Jsonify Done";
        PlayerPrefs.SetInt("json", 1);
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
    public  static void fillTree(TreeNode node , List<string> arr , ref int i) 
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

    public void Update()
    {
        if (PlayerPrefs.GetInt("isValid") == 1 & PlayerPrefs.GetInt("json")!=1)
        {
            gameObject.GetComponent<UnityEngine.UI.Button>().interactable = true;
        }
        else
        {
            gameObject.GetComponent<UnityEngine.UI.Button>().interactable = false;

        }
    }
}

