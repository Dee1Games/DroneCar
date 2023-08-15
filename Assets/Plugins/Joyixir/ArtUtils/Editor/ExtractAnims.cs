using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ExtractAnimations : MonoBehaviour
{
    [MenuItem("Art Tools/Extract Animations")]
    static void ExtractAnims()
    {
        string s;

        foreach (Object o in Selection.objects)
        {
            // s.Remove(s.Length - n);

            s = AssetDatabase.GetAssetPath(o);

            // Print the path of the created asset
            Debug.Log("Reading " + s);

            AnimationClip orgClip = (AnimationClip)AssetDatabase.LoadAssetAtPath(s, typeof(AnimationClip));
            SerializedObject serializedClip = new SerializedObject(orgClip);
            // AnimationClipSettings clipSettings =
            //     new AnimationClipSettings(serializedClip.FindProperty("m_AnimationClipSettings"));
            //
            // clipSettings.loopTime = true;
            // serializedClip.ApplyModifiedProperties();

            //Save the clip

            s = s.Remove(s.Length - 4) + ".anim";
            Debug.Log("Writing " + s);

            if (!Resources.Load(s))
            {
                AnimationClip placeClip = new AnimationClip();
                EditorUtility.CopySerialized(orgClip, placeClip);
                AssetDatabase.CreateAsset(placeClip, s);
                AssetDatabase.Refresh();
            }
        }
    }
}