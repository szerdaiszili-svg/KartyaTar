namespace KartyaTar2.Services;


public class PhotoService : IPhotoService
{
    public async Task<string?> TakePhotoAsync()
    {
        if (!MediaPicker.Default.IsCaptureSupported)
            throw new InvalidOperationException("Ezen a gépen nincs használható kamera.");

        var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
        if (status != PermissionStatus.Granted)
            status = await Permissions.RequestAsync<Permissions.Camera>();
        if (status != PermissionStatus.Granted)
            throw new UnauthorizedAccessException("Engedély nélkül nem tudok fotózni.");

        var photo = await MediaPicker.Default.CapturePhotoAsync();
        return photo == null ? null : await SavePhotoAsync(photo);
    }

    public async Task<string?> PickPhotoAsync()
    {
        var photo = await MediaPicker.Default.PickPhotoAsync();
        return photo == null ? null : await SavePhotoAsync(photo);
    }

    private async Task<string> SavePhotoAsync(FileResult photo)
    {
        string folder = Path.Combine(FileSystem.Current.AppDataDirectory, "photos");
        Directory.CreateDirectory(folder);

        string extension = Path.GetExtension(photo.FileName);
        if (string.IsNullOrEmpty(extension))
            extension = ".jpg";

        string fileName = $"card_{DateTime.Now:yyyyMMdd_HHmmss_fff}{extension}";
        string targetPath = Path.Combine(folder, fileName);

        using Stream sourceStream = await photo.OpenReadAsync();
        using FileStream localFileStream = File.OpenWrite(targetPath);
        await sourceStream.CopyToAsync(localFileStream);

        return targetPath;
    }

    public void DeletePhoto(string? path)
    {
        try
        {
            if (!string.IsNullOrEmpty(path) && File.Exists(path))
                File.Delete(path);
        }
        catch
        {
            /* A nagy semmi */
        }
    }
}
