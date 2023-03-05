namespace AzerSelling.Helpers;
public class FileManager
{
    public static string SaveFile(string rootPath, string folderName, IFormFile imageFile)
    {
        string filename = imageFile.FileName;
        if (filename.Length > 64) filename = filename.Substring(filename.Length - 64, 64);

        filename = Guid.NewGuid() + filename;

        string path = Path.Combine(rootPath, folderName, filename);

        using (FileStream fileStream = new FileStream(path, FileMode.Create))
        {
            imageFile.CopyTo(fileStream);
        }
        return filename;
    }
    public static void DeleteFile(string rootPath, string folderName, string filename)
    {
        string path = Path.Combine(rootPath, folderName, filename);
        if (File.Exists(path)) File.Delete(path);
    }
}