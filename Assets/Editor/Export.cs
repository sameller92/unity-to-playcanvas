using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;

public class Export : MonoBehaviour
{
    [MenuItem("Export to HTML5/Export")]
    public static void ExportToHTML()
    {
        string path = Application.dataPath + "/../Export/index.html";
        
        string fileText = "";
        //Start of file
        fileText += "<html><head> <title>Sam Eller Test</title> <script src='https://code.playcanvas.com/playcanvas-stable.min.js'></script></head><body> <canvas id='application'></canvas> <script>const canvas=document.getElementById('application'); const app=new pc.Application(canvas); app.setCanvasFillMode(pc.FILLMODE_FILL_WINDOW); app.setCanvasResolution(pc.RESOLUTION_AUTO); const light=new pc.Entity('light'); light.addComponent('light'); app.root.addChild(light); light.setEulerAngles(45, 135, 0);";

        //Camera HMTL
        fileText += "const camera=new pc.Entity('camera'); camera.addComponent('camera',{clearColor: new pc.Color(0.1, 0.1, 0.1),nearClip: 1, farClip: 1000,fov: 60}); app.root.addChild(camera);";

        //Camera position
        Vector3 pos = Camera.main.transform.position;
        fileText += "camera.setPosition("+ (pos.x) + "," + (pos.y)+ "," + pos.z +");";

        //Camera rotation needs y rotation 180... not sure why Playcanvas settings invert this... everything is mirrored Horizontally
        Vector3 rot = Camera.main.transform.eulerAngles;
        double flippedY = -1*rot.y + 180;
        fileText += "camera.setEulerAngles(" +rot.x + ","+ flippedY + ","+rot.z+");";

        GameObject[] allObjects = FindObjectsOfType<GameObject>();

        //do objects with no parents first
        foreach (GameObject go in allObjects)
        {
            //Cubes are GameObjects with tag Untagged
            if (go.tag == "Untagged")
            {
                //only GameObjects with no parents are instantiated first
                if (!go.transform.parent)
                {
                    string root = "app.root";
                    string box = go.name;
                    Debug.Log(box);
                    string cubeString = "const " + box + " =new pc.Entity('cube');" + box + ".addComponent('model',{type: 'box'}); " + root + ".addChild(" + box + ");";
                    fileText += cubeString;
                    //Cube postion
                    string cubePosition = box + ".setPosition" + go.transform.position + ";";
                    fileText += cubePosition;
                }
            }
        }

        //Keyvalue pair List to preserve hierarchy
        List<KeyValuePair<GameObject, int>> orderedList = new List<KeyValuePair<GameObject, int>>();
        foreach (GameObject go in allObjects)
        {
            if (go.tag == "Untagged")
            {
                if (go.transform.parent)
                {
                    int parentCount = GetParentCount(go);
                    KeyValuePair<GameObject, int> pair = new KeyValuePair<GameObject, int>(go, parentCount);
                    orderedList.Add(pair);
                }
            }
        }

        //Sort List by parent values
        orderedList.Sort((x, y) => (x.Value.CompareTo(y.Value)));

        //Iterate through sorted Pairs and instantiate gameobjects in HTML5 as child of parent
        foreach (KeyValuePair<GameObject, int> pair in orderedList)
        {
            string root = pair.Key.transform.parent.name;
            string box = pair.Key.name;
            Debug.Log(box);
            string cubeString = "const " + box + " =new pc.Entity('cube');" + box + ".addComponent('model',{type: 'box'}); " + root + ".addChild(" + box + ");";
            fileText += cubeString;
            //Cube postion
            string cubePosition = box + ".setPosition" + pair.Key.transform.position + ";";
            fileText += cubePosition;

        }

        //Final HTML tags and Playcanvas start
        fileText += " app.start(); </script></body></html>";

        //See if Export folder exists
        try
        {
            File.WriteAllText(path, fileText);
        }
        catch
        {
            var folder = Directory.CreateDirectory(Application.dataPath + "/../Export");

            File.WriteAllText(path, fileText);
        }
        Debug.Log("Exporting... "+ path);
    }

    //Recursively get Parent count for orderedList
    static int GetParentCount(GameObject go)
    {
        GameObject subject = go;
        int counter = 0;
        while (subject.transform.parent)
        {
            subject = subject.transform.parent.gameObject;
            counter++;
        }
        return counter;
    }
}
