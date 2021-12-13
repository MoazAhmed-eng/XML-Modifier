using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddPlaceHolder : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<UnityEngine.UI.Text>().text = System.IO.File.ReadAllText(@"Assets/placeHolder.txt");
    }
}
