using Microsoft.AspNetCore.Http;

namespace Odasoft.XBOL.Commons.Extensions
{
    public static class FileExtensions
    {
        public static byte[]? ConvertIFormFileToByteArray(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return null;
            }

            using (var memoryStream = new MemoryStream())
            {
                file.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
}
