using UnityEngine;
using UnityEditor;
using System.IO;

public class FixAnimationClip

{
    [MenuItem("Art Tools/Mixamo/Fix Animation Clips")]
    static void ReadString()
    {
        var activeObjects = Selection.objects;
        foreach (var activeObject in activeObjects)
        {
            string path = AssetDatabase.GetAssetPath(activeObject);

            StreamReader reader = new StreamReader(path);

            var str = reader.ReadToEnd();
            str = str.Replace("mixamorig:", "mixamorig_");

            reader.Close();
            WriteString(str, path);
        }

    }

    static void WriteString(string str, string path)

    {
        StreamWriter writer = new StreamWriter(path);
        writer.Write(str);
        writer.Close();
        AssetDatabase.ImportAsset(path);
    }
}