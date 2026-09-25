namespace UserManagerApi.Helpers;

// Работа с картинками в wwwroot/images: проверка, сохранение, удаление.
public class ImageStorage
{
    public const long MaxFileSize = 10 * 1024 * 1024;

    private static readonly HashSet<string> AllowedExtensions =
        new(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".png", ".webp", ".gif" };

    private readonly string _imagesRoot;

    public ImageStorage(IWebHostEnvironment environment)
    {
        var webRoot = environment.WebRootPath
            ?? Path.Combine(environment.ContentRootPath, "wwwroot");

        _imagesRoot = Path.Combine(webRoot, "images");
    }

    public string ProductFolder(int productId) =>
        Path.Combine(_imagesRoot, "products", productId.ToString());

    public string AvatarFolder() => Path.Combine(_imagesRoot, "avatars");

    // Возвращает текст ошибки или null, если файл подходит
    public static string? Validate(IFormFile? file)
    {
        if (file == null || file.Length == 0)
            return "Файл пустой.";

        if (file.Length > MaxFileSize)
            return "Файл больше 10 МБ.";

        if (!AllowedExtensions.Contains(Path.GetExtension(file.FileName)))
            return "Допустимы только изображения: jpg, jpeg, png, webp, gif.";

        if (!file.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
            return "Файл не является изображением.";

        return null;
    }

    // Сохраняет файл в папку и возвращает имя файла
    public async Task<string> SaveAsync(IFormFile file, string folder, string fileNameWithoutExtension)
    {
        Directory.CreateDirectory(folder);

        var fileName = fileNameWithoutExtension + Path.GetExtension(file.FileName).ToLowerInvariant();

        await using var stream = new FileStream(Path.Combine(folder, fileName), FileMode.Create);
        await file.CopyToAsync(stream);

        return fileName;
    }

    public void DeleteFile(string folder, string? fileName)
    {
        if (string.IsNullOrEmpty(fileName))
            return;

        var path = Path.Combine(folder, Path.GetFileName(fileName));

        if (File.Exists(path))
            File.Delete(path);
    }

    public void DeleteFolder(string folder)
    {
        if (Directory.Exists(folder))
            Directory.Delete(folder, true);
    }
}
