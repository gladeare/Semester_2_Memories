using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof(FOV))]
public class FOV_visualizer : Editor {

    private void OnSceneGUI(){
        FOV fov = (FOV)target;
        Handles.color = Color.white;
        Handles.DrawWireArc(fov.transform.position, Vector3.forward, -Vector3.up, 360, fov.rad);
        Vector3 AngleL = fov.DirFromAngle(-fov.viewAngle / 2, false);
        Vector3 AngleR = fov.DirFromAngle(fov.viewAngle / 2, false);

        Handles.DrawLine(fov.transform.position, fov.transform.position + AngleL * fov.rad);
        Handles.DrawLine(fov.transform.position, fov.transform.position + AngleR * fov.rad);
    }

}
