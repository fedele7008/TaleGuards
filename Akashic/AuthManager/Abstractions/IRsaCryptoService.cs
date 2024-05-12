namespace AuthManager.Abstractions;

public interface IRsaCryptoService
{
    public string Encrypt(string data, string? publicKey = null);
    public string Decrypt(string data, string? privateKey = null);
}