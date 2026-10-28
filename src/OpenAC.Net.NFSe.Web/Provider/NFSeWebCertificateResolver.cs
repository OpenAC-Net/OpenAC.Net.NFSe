using System.Security.Cryptography.X509Certificates;
using OpenAC.Net.NFSe.Configuracao;

namespace OpenAC.Net.NFSe.Web.Provider;

/// <summary>Carrega certificados explicitamente configurados sem recorrer a seleção interativa.</summary>
internal static class NFSeWebCertificateResolver
{
    public static X509Certificate2 Resolve(ConfigCertificadosNFSe configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        if (configuration.CertificadoBytes is { Length: > 0 } certificateBytes)
            return Load(certificateBytes, configuration.Senha);

        if (!string.IsNullOrWhiteSpace(configuration.Certificado) && File.Exists(configuration.Certificado))
            return Load(File.ReadAllBytes(configuration.Certificado), configuration.Senha);

        throw new InvalidOperationException(
            "O certificado da NFS-e deve ser configurado explicitamente por CertificadoBytes ou por um arquivo existente. " +
            "A biblioteca Web não realiza seleção interativa de certificados.");
    }

    private static X509Certificate2 Load(byte[] certificateBytes, string? password)
    {
        const X509KeyStorageFlags flags = X509KeyStorageFlags.EphemeralKeySet | X509KeyStorageFlags.Exportable;
#if NET9_0_OR_GREATER
        return X509CertificateLoader.LoadPkcs12(certificateBytes, password, flags);
#else
        return new X509Certificate2(certificateBytes, password, flags);
#endif
    }
}
