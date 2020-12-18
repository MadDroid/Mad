using System;
using System.Runtime.ExceptionServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using Mad.Common.Extensions;

namespace Mad.Storage.UWP
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

        public async Task WriteAsync(StorageFolder folder, string name, object value, CancellationToken cancellationToken = default)
        {
            if (folder is null)
                throw new ArgumentNullException(nameof(folder));

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException($"'{nameof(name)}' cannot be null or whitespace", nameof(name));

            var (file, backup) = await GetFilesAsync(folder, name).Configure();

            if (file != null && backup is null)
            {
                try
                {
                    backup = await file.CopyAsync(folder, name + BackupExtension).AsTask().Configure();
                }
                catch (Exception ex)
                {
                    backup = await folder.TryGetItemAsync(name + BackupExtension).AsTask().Configure() as StorageFile;
                    await backup?.DeleteAsync();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                }
            }

            await file.WriteAsJsonAsync(value, Options, cancellationToken).Configure();

            backup?.DeleteAsync();
        }

        public async Task<T> ReadAsync<T>(StorageFolder folder, string name, CancellationToken cancellationToken = default)
        {
            if (folder is null)
                throw new ArgumentNullException(nameof(folder));

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException($"'{nameof(name)}' cannot be null or whitespace", nameof(name));

            var (file, backup) = await GetFilesAsync(folder, name).Configure();

            if (file is null)
                return default;

            if (backup != null)
            {
                await backup.CopyAndReplaceAsync(file).AsTask().Configure();
                await backup.DeleteAsync().AsTask().Configure();
            }

            return await file.ReadFromJsonAsync<T>(Options, cancellationToken).Configure();
        }

        private async Task<(StorageFile File, StorageFile Backup)> GetFilesAsync(StorageFolder folder, string name)
        {
            var file = await folder.TryGetItemAsync(name).AsTask().Configure() as StorageFile;
            var backupFile = await folder.TryGetItemAsync(name + BackupExtension).AsTask().Configure() as StorageFile;

            return (file, backupFile);
        }
    }

    public class Test
    {
        public Test()
        {
            _ = JsonStorage.Default.WriteAsync(null, "", null);
        }
    }
}
