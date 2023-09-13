using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;
    
    private const string saveFileName = "userData.json";

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }
    
    public void SaveUserData(UserData data)
    {
        string filePath = Path.Combine(Application.persistentDataPath, saveFileName);
        string jsonData = JsonUtility.ToJson(data);
        File.WriteAllText(filePath, jsonData);
    }

    public UserData LoadUserData()
    {
        string filePath = Path.Combine(Application.persistentDataPath, saveFileName);

        if (File.Exists(filePath))
        {
            string jsonData = File.ReadAllText(filePath);
            return JsonUtility.FromJson<UserData>(jsonData);
        }
        else
        {
            Debug.LogWarning("Save file not found.");
            return null;
        }
    }
    
    #if UNITY_EDITOR
    [MenuItem("EditorTools/Reset User Data")]
    public static void ResetUserData()
    {
        File.Delete(Path.Combine(Application.persistentDataPath, saveFileName));
    }
    
    [MenuItem("EditorTools/Select Monster")]
    public static void SelectMonster()
    {
        Selection.activeTransform = FindObjectOfType<Monster>().transform;
    }
    #endif
}
