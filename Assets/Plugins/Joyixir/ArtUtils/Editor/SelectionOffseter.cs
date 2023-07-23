using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;

public class SelectionOffseter : EditorWindow
{
    private List<Object> objects;
    private string offsetAmount;
    private float offsetAmountfloat;
    private int directionInt;
    private Vector3 direction = Vector3.right;

    [MenuItem("Art Tools/Selection Offseter")]
    static void CreateOffseter()
    {
        GetWindow<SelectionOffseter>();
    }

    public void Offset(int dir)
    {
        HandleDirection(dir);
        var activeObjects = Selection.objects.OfType<GameObject>().ToArray();
        float offsetVal = 0;
        foreach (var activeObject in activeObjects)
        {
            var tf = activeObject.transform;
            activeObject.transform.Translate(direction * offsetVal);
            offsetVal += offsetAmountfloat;
        }
    }

    private void HandleDirection(int dir)
    {
        switch (dir)
        {
            case 0:
                direction = Vector3.right;
                break;
            case 1:
                direction = Vector3.up;
                break;
            case 2:
                direction = Vector3.forward;
                break;
            case 3:
                direction = Vector3.left;
                break;
            case 4:
                direction = Vector3.down;
                break;
            case 5:
                direction = Vector3.back;
                break;

        }
    }

    private void OnGUI()
    {
        offsetAmount = EditorGUILayout.TextField("Offset Amount", offsetAmount);
        directionInt = (int)(CursorType)EditorGUILayout.EnumPopup("Cursor:", (CursorType)directionInt);
        offsetAmountfloat = ConvertToFloat(offsetAmount);
        if (GUILayout.Button("Do Offset"))
        {
            Offset(directionInt);
            OnGUI();
            GUI.enabled = false;
        }
    }
    
    public enum CursorType
    {
        X = 0,
        Y = 1,
        Z = 2,
        XMinus = 3,
        YMinus = 4,
        ZMinus = 5,
    }
    
    
    public static float ConvertToFloat(string inputString)
    {
        float result;
        //string input;
        if (float.TryParse(inputString, out result))
        {
            return result;
        }
        else
        {
            Debug.Log("Not a valid int");
            return 0;
        }
    }
}