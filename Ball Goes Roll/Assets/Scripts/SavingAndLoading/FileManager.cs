using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

//[System.Serializable]     // May need this ???
public static class FileManager {
    public static bool WriteToFile(string fileName, string fileContents) {
        var fullPath = Path.Combine(Application.persistentDataPath, fileName);
        Debug.Log(fullPath);

        try {
            File.WriteAllText(fullPath, fileContents);
            return true;
        } catch (System.Exception e) {
            Debug.LogError($"Failed to write to {fullPath} with exception of {e}");
            throw;
        }
    }

    public static bool LoadFromFile(string fileName, out string result) {
        var fullPath = Path.Combine(Application.persistentDataPath, fileName);

        try {
            result = File.ReadAllText(fullPath);
            return true;
        } catch (System.Exception e) {
            Debug.LogError($"Failed to read from {fullPath} with exception of {e}");
            result = "";
            return false;
        }
    }
}
