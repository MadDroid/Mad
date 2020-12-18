using System;
using System.IO;
using System.Runtime.ExceptionServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Mad.Common.Extensions;

namespace Mad.Storage
{
    public class JsonStorage
    {
        public static JsonStorage Default { get; } = new JsonStorage(new JsonSerializerOptions());
        public JsonSerializerOptions Options { get; }
        public string BackupExtension { get; }

        public JsonStorage(JsonSerializerOptions options)
            : this(options, ".old") { }

        public JsonStorage(JsonSerializerOptions options, string backupExt)
        {
            if (string.IsNullOrWhiteSpace(backupExt))
                throw new ArgumentException($"'{nameof(backupExt)}' cannot be null or whitespace", nameof(backupExt));

            Options = options ?? throw new ArgumentNullException(nameof(options));
            BackupExtension = backupExt;
        }

        public async Task WriteAsync(string path, object? value, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException($"'{nameof(path)}' cannot be null or whitespace", nameof(path));

            var backupPath = GetBackupPath(path);

            if (File.Exists(path) && !File.Exists(backupPath))
            {
                try
                {
                    File.Copy(path, backupPath);
                }
                catch (Exception ex)
                {
                    File.Delete(backupPath);
                    ExceptionDispatchInfo.Capture(ex).Throw();
                }
            }

            await Json.WriteAsync(path, value, Options, cancellationToken).Configure();

            File.Delete(backupPath);
        }

        public Task SaveAsync(Stream stream, object? value, CancellationToken cancellationToken = default)
            => Json.WriteAsync(stream, value, Options, cancellationToken);

        public async Task<T?> ReadAsync<T>(string path, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException($"'{nameof(path)}' cannot be null or whitespace", nameof(path));

            if (!File.Exists(path))
                return default;

            var backupPath = GetBackupPath(path);

            if (File.Exists(backupPath))
            {
                File.Copy(backupPath, path, true);
                File.Delete(backupPath);
            }

            var result = await Json.ReadAsync<T>(path, Options, cancellationToken).Configure();

            return result;
        }

        public Task<T?> LoadAsync<T>(Stream stream, CancellationToken cancellationToken = default)
            => Json.ReadAsync<T>(stream, Options, cancellationToken);

        private string GetBackupPath(string path) => path + BackupExtension;
    }
}
