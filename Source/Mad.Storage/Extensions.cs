using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Mad.Common.Extensions;

namespace Mad.Storage
{
    public static class Extensions
    {
        public static Task WriteAsJsonAsync(this FileInfo file, object? value, JsonSerializerOptions? options = null, CancellationToken cancellationToken = default)
        {
            if (file is null)
                throw new ArgumentNullException(nameof(file));

            using var fileStream = file.OpenWrite();
            return Json.WriteAsync(fileStream, value, options, cancellationToken);
        }

        public static async Task<FileInfo> WriteAsJsonAsync(this DirectoryInfo directory, string name, object? value, JsonSerializerOptions? options = null, CancellationToken cancellationToken = default)
        {
            if (directory is null)
                throw new ArgumentNullException(nameof(directory));

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException($"'{nameof(name)}' cannot be null or whitespace", nameof(name));

            var file = new FileInfo(Path.Combine(directory.FullName, name));
            await file.WriteAsJsonAsync(value, options, cancellationToken).Configure();
            return file;
        }

        public static Task<T?> ReadFromJsonAsync<T>(this FileInfo file, JsonSerializerOptions? options = null, CancellationToken cancellationToken = default)
        {
            if (file is null)
                throw new ArgumentNullException(nameof(file));

            using var fileStream = file.OpenRead();
            return Json.ReadAsync<T>(fileStream, options, cancellationToken);
        }

        public static Task<T?> ReadFromJsonAsync<T>(this DirectoryInfo directory, string name, JsonSerializerOptions? options = null, CancellationToken cancellationToken = default)
        {
            if (directory is null)
                throw new ArgumentNullException(nameof(directory));

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException($"'{nameof(name)}' cannot be null or whitespace", nameof(name));

            var file = directory.EnumerateFiles("*", SearchOption.TopDirectoryOnly).SingleOrDefault(f => f.Name == name);

            if (file is null)
                throw new FileNotFoundException();

            return file.ReadFromJsonAsync<T>(options, cancellationToken);
        }
    }
}
