using UnityEngine;
using System.IO;

public class FileIO
{
    public static void WriteFile(string FilePath, string FileContent)
    {
        string path = Path.Combine(Application.persistentDataPath, FilePath);
        File.WriteAllText(path, FileContent);
    }

    // confirm if file exists before attempting to load or write
    public static bool VerifyFileExists(string FilePath)
    {
        if (File.Exists(Path.Combine(Application.persistentDataPath, FilePath)))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static string ReadFile(string FilePath)
    {
        string path = Path.Combine(Application.persistentDataPath, FilePath);
        try
        {
            return File.ReadAllText(path);
        }
        catch 
        {
            Debug.LogError("ReadFile called on '" + path + "'file not found");
            return null;
        }
    }

}
