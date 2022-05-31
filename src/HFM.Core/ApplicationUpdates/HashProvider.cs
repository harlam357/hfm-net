namespace HFM.Core.ApplicationUpdates;

/// <summary>
/// Specifies the type of hash.
/// </summary>
public enum HashProvider
{
    // ReSharper disable InconsistentNaming

    /// <summary>
    /// Secure Hashing Algorithm provider, SHA-1 variant, 160-bit.
    /// </summary>
    SHA1,
    /// <summary>
    /// Secure Hashing Algorithm provider, SHA-2 variant, 256-bit.
    /// </summary>
    SHA256,
    /// <summary>
    /// Secure Hashing Algorithm provider, SHA-2 variant, 384-bit.
    /// </summary>
    SHA384,
    /// <summary>
    /// Secure Hashing Algorithm provider, SHA-2 variant, 512-bit.
    /// </summary>
    SHA512,
    /// <summary>
    /// Message Digest algorithm 5, 128-bit.
    /// </summary>
    MD5

    // ReSharper restore InconsistentNaming
}
