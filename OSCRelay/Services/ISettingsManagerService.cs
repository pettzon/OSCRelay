using System.Collections.Generic;
using System.Threading.Tasks;
using Global.Data.Data;
using Global.Data.Data.VRChat;

namespace OSCRelay.Services;

public interface ISettingsManagerService
{
    private void LoadSettings() {}
    private async Task LoadSettingsAsync() {}
    public UserSettings GetUserSettings();
    public ExposedParameters GetExposedAvatarParameters();
    public void SetUserSettings(UserSettings settings);
    public void SetUserEnabledParameters(ExposedParameters exposedParameters);
    private async Task SaveUserSettingsAsync(UserSettings settings) {}
    private void OnAvatarParametersLoaded(VRCData vrcData) {}
}