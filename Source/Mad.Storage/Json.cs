using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Mad.Storage
{
    /// <summary>
    /// Provides functionality to read/write objects from/to a file/stream. 
    /// </summary>
    public static class Json
    {
        /// <summary>
        /// Write an object to a file.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <param name="value">The object to write to the file.</param>
        /// <param name="options">The options to be used when serializing the json.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <exception cref="UnauthorizedAccessException"/>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="PathTooLongException"/>
        /// <exception cref="DirectoryNotFoundException"/>
        /// <exception cref="NotSupportedException"/>
        /// <returns></returns>
        public static Task WriteAsync(string path, object? value, JsonSerializerOptions? options = null, CancellationToken cancellationToken = default)
        {
            using var fileStream = File.OpenWrite(path);
            return WriteAsync(fileStream, value, options, cancellationToken);
        }

        /// <summary>
        /// Write an object to a stream.
        /// </summary>
        /// <param name="stream">The stream to write the object to.</param>
        /// <param name="value">The object to write to the stream.</param>
        /// <param name="options">The options to be used when serializing the json.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="NotSupportedException"/>
        /// <returns></returns>
        public static Task WriteAsync(Stream stream, object? value, JsonSerializerOptions? options = null, CancellationToken cancellationToken = default)
        {
            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            return JsonSerializer.SerializeAsync(stream, value, options, cancellationToken);
        }

        /// <summary>
        /// Read an object from a file.
        /// </summary>
        /// <typeparam name="T">The type of the object the be read.</typeparam>
        /// <param name="path">The path of the file to be read.</param>
        /// <param name="options">The options to be used when deserializing the json.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="PathTooLongException"/>
        /// <exception cref="DirectoryNotFoundException"/>
        /// <exception cref="UnauthorizedAccessException"/>
        /// <exception cref="FileNotFoundException"/>
        /// <exception cref="NotSupportedException"/>
        /// <exception cref="IOException"/>
        /// <exception cref="JsonException"/>
        /// <returns></returns>
        public static Task<T?> ReadAsync<T>(string path, JsonSerializerOptions? options = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException($"'{nameof(path)}' cannot be null or whitespace", nameof(path));
            }

            using var fileStream = File.OpenRead(path);
            return ReadAsync<T>(fileStream, options, cancellationToken);
        }

        /// <summary>
        /// Read an object from a stream.
        /// </summary>
        /// <typeparam name="T">The type of the object the be read.</typeparam>
        /// <param name="stream">The stream to read from.</param>
        /// <param name="options">The options to be used when deserializing the json.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <exception cref="JsonException"/>
        /// <exception cref="NotSupportedException"/>
        /// <exception cref="ArgumentNullException"/>
        /// <returns></returns>
        public static Task<T?> ReadAsync<T>(Stream stream, JsonSerializerOptions? options = null, CancellationToken cancellationToken = default)
        {
            if (stream is null)
                throw new ArgumentNullException(nameof(stream));

            return JsonSerializer.DeserializeAsync<T>(stream, options, cancellationToken).AsTask();
        }
    }
}
