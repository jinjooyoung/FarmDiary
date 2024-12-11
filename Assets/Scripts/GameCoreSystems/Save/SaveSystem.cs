using System.IO;
using System.Threading.Tasks;
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

    public static void DeleteSaveFolder()
    {
        // 폴더가 존재하면
        if (Directory.Exists(SAVE_FOLDER))
        {
            Directory.Delete(SAVE_FOLDER, true); // 하위 파일 및 폴더 삭제
            Debug.Log("세이브 폴더 삭제 완료!");
        }
    }

    public static void Save(string saveString, string fileName)
    {
        string savePath = SAVE_FOLDER + fileName + "." + SAVE_EXTENSION;
        File.WriteAllText(savePath, saveString);
    }

    // 비동기 파일 저장
    public static async Task SaveAsync(string saveString, string fileName)
    {
        string savePath = SAVE_FOLDER + fileName + "." + SAVE_EXTENSION;

        // 비동기적으로 파일 저장
        await Task.Run(() => File.WriteAllText(savePath, saveString));
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

    // 비동기적으로 파일을 로드하는 메서드
    public static async Task<string> LoadAsync(string fileName)
    {
        string savePath = SAVE_FOLDER + fileName + "." + SAVE_EXTENSION;

        // 파일이 존재하는지 확인
        if (File.Exists(savePath))
        {
            // 비동기적으로 파일 읽기
            return await File.ReadAllTextAsync(savePath);
        }
        else
        {
            Debug.LogWarning($"Save file {fileName} not found!");
            return null;
        }
    }
}
