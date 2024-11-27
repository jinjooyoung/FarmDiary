using System.IO;
using UnityEngine;

public static class SaveSystem
{
    private static readonly string SAVE_FOLDER = Application.dataPath + "/Saves/";
    private const string SAVE_EXTENSION = "txt";

    public static void Init()
    {
        if (!Directory.Exists(SAVE_FOLDER))
        {
            Directory.CreateDirectory(SAVE_FOLDER);
        }
    }

    public static void Save(string saveString, string fileName)
    {
        string savePath = SAVE_FOLDER + fileName + "." + SAVE_EXTENSION;
        File.WriteAllText(savePath, saveString);
    }

    public static string Load(string fileName)
    {
        string savePath = SAVE_FOLDER + fileName + "." + SAVE_EXTENSION;
        if (File.Exists(savePath))
        {
            return File.ReadAllText(savePath);
        }
        else
        {
            Debug.LogWarning($"Save file {fileName} not found!");
            return null;
        }
    }
}
