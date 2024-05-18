namespace StudentHelper.Services.Interfaces;

public interface IGigachatService
{
    Task<string?> SendMessage(string promptContent);
    Task<byte[]> GetImageBytes(string fileId);
}