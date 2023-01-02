using System;
using System.Threading;
using System.Threading.Tasks;
using Global.Data.Data;
using Global.Data.Data.Web;

namespace OSCRelay.Services;

public interface IServiceProvider
{
    public event Action<Token> OnConnected;
    public event Action<AvatarParameterDTO> OnReceiveParameter;
    public void UpdateExposedAvatarParameters(ExposedParameters parameters);
    private async Task UpdateExposedAvatarParametersAsync(ExposedParameters parameters) { }
    public void StartProviderService(Token token, string host);
    private async Task StartProviderServiceAsync(CancellationToken cancellationToken, Token token, string host) { }
}