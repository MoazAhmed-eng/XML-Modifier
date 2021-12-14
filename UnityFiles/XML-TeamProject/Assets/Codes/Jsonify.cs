using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;




class TreeNode
{
    string name;
    string value;
    int depth;
    List<string> attributes;
    List<TreeNode> children;

    TreeNode(string name , string value)
    {
        this.depth = 0;
        this.name = name;
        this.value = value;
        this.attributes = null;
        this.children = null;
    }

    TreeNode()
    {
        this.depth = 0;
        this.name = null;
        this.value = null;
        this.attributes = null;
        this.children = null;
    }

    List<TreeNode> getChildren()
    {
        return children;
    }

    string getName()
    {
        return name;
    }

    string getValue()
    {
        return value;
    }

    void setDepth( int depth)
    {
        this.depth = depth;
    }

    void setName(string name)
    {
        this.name = name;
    }

    void setValue(string value)
    {
        this.value = value;
    }

    void setAttributes(List<string> attributes)
    {
        this.attributes = attributes;
    }
}

class Tree
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

    TreeNode getRoot()
    {
        return root;
    }
}


public class Jsonify : MonoBehaviour
{

    public void ScanThroughXML()
        
    {
        string str = GameObject.FindGameObjectWithTag("mainText").GetComponent<UnityEngine.UI.InputField>().text;

        List<string> tockenized = new List<string>();

        for (int i = 0; i < str.Length; i++)
        {
            if (str[i] == '<' && (str[i + 1] == '!' || str[i + 1] == '?' || str[i + 1] == '/')) continue;

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
                    attTemp.Append('*');
                    attTemp.Append(element);
                    tockenized.Add(attTemp.ToString());
                    attTemp.Clear();
                }
            }

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
            }

        }

        foreach (string st in tockenized)
        {
            Debug.Log(st);
        }
    }
}

