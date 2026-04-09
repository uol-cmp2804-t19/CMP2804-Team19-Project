using UnityEngine;
using System.IO;

public class FileIO
{
    public static void WriteFile(string FilePath, string FileContent)
    {
        string path = Path.Combine(Application.persistentDataPath, FilePath);
        File.WriteAllText(path, FileContent);
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
            //TODO if this can't be found, create it
            Debug.LogError("ReadFile called on '" + path + "'file not found");
            return null;
        }
    }

}
