using UnityEngine;
using UnityEditor;

public static class CopyAssetPathContextMenu
{

    [MenuItem("Assets/Copy Asset Path &#c")]
    public static void CopyAssetPath()
    {
        if (Selection.activeObject != null)
        {
            string assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
            var c = Application.dataPath;
            c = c.Remove(c.Length - 6, 6);

            
            c += assetPath;
            
            int lastSlashIndex = 0;
            for (int i = c.Length; i > 0; i--)
            {
                if (c[i - 1].ToString() == "/")
                {
                    lastSlashIndex = i;
                    break;
                }
            }

            c = c.Remove(lastSlashIndex);
            EditorGUIUtility.systemCopyBuffer = c;
            Debug.Log("Copied to Buffer:" + c);
        }
        else
        {
            Debug.Log("Nothing selected.");
        }

    }
}