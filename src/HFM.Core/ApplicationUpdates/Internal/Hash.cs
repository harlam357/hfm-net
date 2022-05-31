// A simple, string-oriented wrapper class for encryption functions, including 
// Hashing, Symmetric Encryption, and Asymmetric Encryption.
//
//  Jeff Atwood
//   http://www.codinghorror.com/
//   http://www.codeproject.com/KB/security/SimpleEncryption.aspx

using System.Security.Cryptography;

namespace HFM.Core.ApplicationUpdates.Internal;

/// <summary>
/// Provides access to factory methods for creating HashAlgorithm instances.
/// </summary>
internal static class HashAlgorithmFactory
{
    /// <summary>
    /// Creates a new instance of the HashAlgorithm class based on the specified provider.
    /// </summary>
    /// <param name="provider">Provides the type of hash algorithm to create.</param>
    /// <returns>The HashAlgorithm object.</returns>
    /// <exception cref="ArgumentException">The provider is unknown.</exception>
    public static HashAlgorithm Create(HashProvider provider)
    {
        switch (provider)
        {
            case HashProvider.SHA1:
#pragma warning disable CA5350 // Do Not Use Weak Cryptographic Algorithms
                return SHA1.Create();
#pragma warning restore CA5350 // Do Not Use Weak Cryptographic Algorithms
            case HashProvider.SHA256:
                return SHA256.Create();
            case HashProvider.SHA384:
                return SHA384.Create();
            case HashProvider.SHA512:
                return SHA512.Create();
            case HashProvider.MD5:
#pragma warning disable CA5351 // Do Not Use Broken Cryptographic Algorithms
                return MD5.Create();
#pragma warning restore CA5351 // Do Not Use Broken Cryptographic Algorithms
        }

        throw new ArgumentException("Unknown HashProvider.", nameof(provider));
    }
}

// Hash functions are fundamental to modern cryptography. These functions map binary 
// strings of an arbitrary length to small binary strings of a fixed length, known as 
// hash values. A cryptographic hash function has the property that it is computationally
// infeasible to find two distinct inputs that hash to the same value. Hash functions 
// are commonly used with digital signatures and for data integrity.

/// <summary>
/// Represents an object that performs hashing.
/// </summary>
internal class Hash : IDisposable
{
    private readonly HashAlgorithm _hash;

    /// <summary>
    /// Initializes a new instance of the Hash class with the specified hash provider.
    /// </summary>
    public Hash(HashProvider provider)
    {
        _hash = HashAlgorithmFactory.Create(provider);
    }

    /// <summary>
    /// Calculates the hash on a stream of arbitrary length.
    /// </summary>
    public byte[] Calculate(Stream stream) => _hash.ComputeHash(stream);

    /// <summary>
    /// Calculates the hash on a seekable stream while reporting progress.
    /// </summary>
    public byte[] Calculate(Stream stream, IProgress<int> progress)
    {
        if (stream == null) throw new ArgumentNullException(nameof(stream));
        if (progress == null) throw new ArgumentNullException(nameof(progress));
        if (!stream.CanSeek) throw new ArgumentException("stream must support seeking.", nameof(stream));

        const int bufferLength = 1048576;
        long totalBytesRead = 0;
        long size = stream.Length;
        var buffer = new byte[bufferLength];

        int bytesRead = stream.Read(buffer, 0, buffer.Length);
        totalBytesRead += bytesRead;

        do
        {
            int oldBytesRead = bytesRead;
            byte[] oldBuffer = buffer;

            buffer = new byte[bufferLength];
            bytesRead = stream.Read(buffer, 0, buffer.Length);

            totalBytesRead += bytesRead;

            if (bytesRead == 0)
            {
                _hash.TransformFinalBlock(oldBuffer, 0, oldBytesRead);
            }
            else
            {
                _hash.TransformBlock(oldBuffer, 0, oldBytesRead, oldBuffer, 0);
            }

            progress.Report((int)((double)totalBytesRead * 100 / size));

        } while (bytesRead != 0);

        return _hash.Hash;
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    /// <filterpriority>2</filterpriority>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
    /// <filterpriority>2</filterpriority>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // clean managed resources
                if (_hash is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
            // clean unmanaged resources
        }

        _disposed = true;
    }

    private bool _disposed;
}
