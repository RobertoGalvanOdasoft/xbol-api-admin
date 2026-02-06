using Microsoft.AspNetCore.Http;

namespace Odasoft.XBOL.Commons.Extensions
{
    public static class FileExtensions
    {
        // TODO: Move to configuration or settings table
        private static readonly List<string> _allowedExtensions = new List<string> { ".pdf", ".docx", ".doc", ".png", ".jpeg", ".jpg" };

        public static bool IsExtensionAllowed(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            return _allowedExtensions.Contains(extension);
        }

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
