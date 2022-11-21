using System;
using System.Threading.Tasks;
using Global.Data.Data;
using Global.Data.Data.Web;

namespace OSCRelay.Services;

public interface IServiceProvider
{
    public event Action<Token> OnConnected;
    public event Action<AvatarParameterDTO> OnReceiveParameter;
    private void UpdateExposedAvatarParameters(ExposedParameters parameters) { }
    private async Task UpdateExposedAvatarParametersAsync(ExposedParameters parameters) { }
    public void StartProviderService();
    private async Task StartProviderServiceAsync() { }
}