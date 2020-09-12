﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;

public class Export : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    [MenuItem("Export to HTML5/Export")]
    public static void ExportToHTML()
    {
        string path = Application.dataPath + "/../Export/index.html";
        string fileText = "";
        //Start of file
        fileText += "<html><head> <title>Sam Eller Test</title> <script src='https://code.playcanvas.com/playcanvas-stable.min.js'></script></head><body> <canvas id='application'></canvas> <script>const canvas=document.getElementById('application'); const app=new pc.Application(canvas); app.setCanvasFillMode(pc.FILLMODE_FILL_WINDOW); app.setCanvasResolution(pc.RESOLUTION_AUTO); const light=new pc.Entity('light'); light.addComponent('light'); app.root.addChild(light); light.setEulerAngles(45, 0, 0);";

        //Camera HMTL
        fileText += "const camera=new pc.Entity('camera'); camera.addComponent('camera',{clearColor: new pc.Color(0.1, 0.1, 0.1)}); app.root.addChild(camera);";

        //Camera position needs Z inverted and 2x
        Vector3 pos = Camera.main.transform.position;
        fileText += "camera.setPosition("+ (pos.x) + "," + (pos.y)+ "," + pos.z * -2 +");";
        //Camera rotation needs x and y rotation inverted... not sure what Playcanvas settings invert this
        Vector3 rot = Camera.main.transform.eulerAngles;
        fileText += "camera.setEulerAngles(" +rot.x *-1 + ","+ rot.y * -1 + ","+(rot.z)+");";

        //Cubes are GameObjects with tag Untagged
        GameObject[] allObjects = FindObjectsOfType<GameObject>();

        //Loop through to find Parents
        foreach (GameObject go in allObjects)
        {
            if (go.tag == "Untagged")
            {
                string root = "app.root";
                string box = go.name;
                string cubeString = "";
                //Make sure no parent
                if (!go.transform.parent)
                {
                    cubeString = "const " + box + " =new pc.Entity('cube');" + box + ".addComponent('model',{type: 'box'}); " + root + ".addChild(" + box + ");";
                }
                fileText += cubeString;
                //Cube postion
                string cubePosition = box + ".setPosition" + go.transform.position + ";";
                fileText += cubePosition;
            }
        }
        //Loop through to find Children
        foreach (GameObject go in allObjects)
        {
            if (go.tag == "Untagged")
            {
                string root = "app.root";
                string box = go.name;
                string cubeString = "";
                //Make sure it has a parent
                if (go.transform.parent)
                {
                    root = go.transform.parent.name;
                    cubeString = "const " + box + " =new pc.Entity('cube');" + box + ".addComponent('model',{type: 'box'}); " + root + ".addChild(" + box + ");";
                }
                fileText += cubeString;
                //Cube postion
                string cubePosition = box + ".setPosition" + go.transform.position + ";";
                fileText += cubePosition;
            }
        }

        //Final stuff
        fileText += " app.start(); </script></body></html>";

        File.WriteAllText(path, fileText);
        Debug.Log("Exporting... "+ path);
    }
}
