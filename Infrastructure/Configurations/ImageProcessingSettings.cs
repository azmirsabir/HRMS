
namespace Infrastructure.Configurations;

public abstract class ImageProcessingSettings
{
    public int MaxImageWidth { get; set; } = 1920;
    public int MaxImageHeight { get; set; } = 1080;
    public int JpegQuality { get; set; } = 85;
    public string[] SupportedImageExtensions { get; set; } = { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
}