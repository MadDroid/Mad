using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using Mad.Common.Extensions;

namespace Mad.Storage.UWP
{
    public static class Extension
    {
        public static async Task WriteAsJsonAsync(this StorageFile file, object value, JsonSerializerOptions options = null, CancellationToken cancellationToken = default)
        {
            if (file is null)
                throw new ArgumentNullException(nameof(file));

            using (var randomStream = await file.OpenAsync(FileAccessMode.ReadWrite, StorageOpenOptions.None).AsTask().Configure())
            {
                await Json.WriteAsync(randomStream.AsStream(), value, options, cancellationToken).Configure();
            }
        }

        public static async Task<T> ReadFromJsonAsync<T>(this StorageFile file, JsonSerializerOptions options = null, CancellationToken cancellationToken = default)
        {
            if (file is null)
                throw new ArgumentNullException(nameof(file));

            using (var randomStream = await file.OpenReadAsync().AsTask().Configure())
            {
                return await Json.ReadAsync<T>(randomStream.AsStream(), options, cancellationToken).Configure();
            }
        }
    }
}
