using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Export : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    [MenuItem("Export/ExportToHTML5")]
    public static void ExportToHTML()
    {
        Debug.Log("Exporting...");
    }
}
