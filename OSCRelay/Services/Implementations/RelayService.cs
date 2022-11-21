using System.Collections.Generic;
using System.Diagnostics;
using Global.Data.Data;

namespace OSCRelay.Services.Implementations;

public class RelayService : IRelayService
{
    private readonly IAvatarInfoService avatarInfoService;
    
    public RelayService(IAvatarInfoService avatarInfoService)
    {
        this.avatarInfoService = avatarInfoService;
        
        OSCService.OnMessageReceived += OnReceiveMessage;
    }
    
    private void OnReceiveMessage(VRCMessage vrcMessage)
    {
        Debug.WriteLine($"{vrcMessage.key} {vrcMessage.value}");

        List<VRCMessage> messages = new List<VRCMessage>();

        switch (vrcMessage.key)
        {
            case VRCMessageType.AVATAR_CHANGE:
            {
                avatarInfoService.FetchLocalOSCData(vrcMessage);
                break;
            }
            case VRCMessageType.MUTE_SELF:
            {
                break;
            }
            case VRCMessageType.ENABLED_PARAMETER:
            {
                break;
            }
            default:
            {
                break;
            }
        }
    }
}