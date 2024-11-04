using Azure.Core;
using Microsoft.AspNetCore.Components.Forms;
using System.Text;

namespace ASM_PH35423.StaticClass
{
    public static class ExtensionMethod
    {
        private static Random rng = new Random();
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            for (int i = 0; i < n; i++)
            {
                var k = rng.Next(n );
                T value = list[k];
                list[k] = list[i];
                list[i] = value;
            }
        }
        public static async Task<IFormFile> ToFormFile(this IBrowserFile browserFile)
        {
            var memoryStream = new MemoryStream();
            await browserFile.OpenReadStream(maxAllowedSize: 100000000).CopyToAsync(memoryStream);
            memoryStream.Position = 0;
            return new FormFile(memoryStream, 0, browserFile.Size, browserFile.Name, browserFile.Name)
            {
                Headers = new HeaderDictionary(),
                ContentType = browserFile.ContentType
            };
        }

        public static string EncodeToBase64(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        public static string DecodeFromBase64(string base64EncodedData)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }

    }
}
