using OpenAC.Net.DFe.Core.Http;

namespace OpenAC.Net.NFSe.Commom.Client;

internal sealed class DesktopNFSeHttpTransport : DFeHttpTransport
{
    internal static DesktopNFSeHttpTransport Instancia { get; } = new();
    
    protected override bool ManageRetries => false;
}