using UnityEngine;
using System.IO;

public class FileIO
{
    public static void WriteFile(string FilePath, string FileContent)
    {
        string path = Application.persistentDataPath + "/" + FilePath;
        File.WriteAllText(path, FileContent);
    }

    public static string ReadFile(string FilePath)
    {
        string path = Application.persistentDataPath + "/" + FilePath;
        return File.ReadAllText(path);
    }

}
