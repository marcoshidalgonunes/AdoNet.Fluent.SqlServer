using AdoNet.Fluent.SqlServer.Demo.Services;
using Microsoft.Extensions.Logging;

namespace AdoNet.Fluent.SqlServer.Demo.Commands;

internal class ImageCommand(IImageService service, ILogger<ImageCommand> logger) : Command(logger)
{
    private readonly IImageService _service = service;

    private static void DeleteImageFromDisk(string imageFile)
    {
        if (File.Exists(imageFile))
        {
            File.Delete(imageFile);
        }
    }

    private static void SaveImageToDisk(string imageFile, byte[] imageData)
    {
        using FileStream fs = new(imageFile, FileMode.OpenOrCreate, FileAccess.Write);
        using BinaryWriter bw = new(fs);
        bw.Write(imageData);
        bw.Flush();
    }

    #region IDisposable member

    #endregion

    #region ICommand members

    public override void Execute()
    {
        int productId = 70;
        string imageFile = Environment.ExpandEnvironmentVariables($"%TEMP%\\ThumbnailProduct{productId}");

        try
        {
            Logger.LogInformation("Reading image from Product Id {id}", productId);
            byte[]? imageData = _service.Get(productId);

            if (imageData != null) 
            { 
                Logger.LogInformation("Creating file '{imageFile}'...", imageFile);
                SaveImageToDisk(imageFile, imageData);

                Logger.LogInformation("Reading file '{imageFile}'...", imageFile);
                byte[] imageSaved = File.ReadAllBytes(imageFile);

                Logger.LogInformation("Updating image from Product Id {id}", productId);
                _service.Save(productId, imageSaved);
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "ImageCommand.Execute() exception");
            throw;
        }
        finally
        {
            DeleteImageFromDisk(imageFile);
        }
    }

    public override async Task ExecuteAsync()
    {
        int productId = 80;
        string imageFile = Environment.ExpandEnvironmentVariables($"%TEMP%\\ThumbnailProduct{productId}");

        try
        {
            Logger.LogInformation("Reading image from Product Id {id}", productId);
            byte[]? imageData = await _service.GetAsync(productId);

            if (imageData != null)
            {
                Logger.LogInformation("Creating file '{imageFile}'...", imageFile);
                SaveImageToDisk(imageFile, imageData);

                Logger.LogInformation("Reading file '{imageFile}'...", imageFile);
                byte[] imageSaved = File.ReadAllBytes(imageFile);

                Logger.LogInformation("Updating image from Product Id {id}\r\n", productId);
                await _service.SaveAsync(productId, imageSaved);
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "ImageCommand.ExecuteAsync() exception");
            throw;
        }
        finally 
        {
            DeleteImageFromDisk(imageFile);
        }
    }

    #endregion
}
