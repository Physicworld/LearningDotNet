namespace MinimalAPIPeliculas.Services;

public class LocalFileStorage : IFileStorage
{
    private readonly IWebHostEnvironment _env;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public LocalFileStorage(IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
    {
        _env = env;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<string> Store(string container, IFormFile file)
    {
        var extension = Path.GetExtension(file.FileName);
        var filename = $"{Guid.NewGuid()}{extension}";
        string folder = Path.Combine(_env.WebRootPath, container);

        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }

        string path = Path.Combine(folder, filename);
        using (var ms = new MemoryStream())
        {
            await file.CopyToAsync(ms);
            var content = ms.ToArray();
            await File.WriteAllBytesAsync(path, content);
        }

        var url =
            $"{_httpContextAccessor.HttpContext!.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}/{container}/{filename}";
        return url;
    }

    public Task Delete(string? path, string container)
    {
        if (string.IsNullOrEmpty(path))
        {
            return Task.CompletedTask;
        }

        var filename = Path.GetFileName(path);
        var folderFile = Path.Combine(_env.WebRootPath, container, filename);
        if (File.Exists(folderFile))
        {
            File.Delete(folderFile);
        }

        return Task.CompletedTask;
    }
}