using System.Threading.Tasks;
using Global.Data.Data;
using Global.Data.Data.VRChat;

namespace OSCRelay.Services;

public interface IAvatarInfoService
{
    private async Task FetchLocalOSCDataAsync(VRCMessage vrcMessage) {}
    public void FetchLocalOSCData(VRCMessage vrcMessage);
    public VRCData? GetCurrentAvatarParameters();
}