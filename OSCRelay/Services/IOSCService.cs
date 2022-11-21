using Global.Data.Data;
using Global.Data.Data.Web;

namespace OSCRelay.Services;

public interface IOSCService
{
    public void ConnectToOSC(int receiverPort, int senderPort);
    public void SendAvatarParameters(AvatarParameterDTO avatarParameter);
    private void ParseMessage(string? message) { }
}