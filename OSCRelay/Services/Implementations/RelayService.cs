using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Global.Data.Data;

namespace OSCRelay.Services.Implementations;

public class RelayService : IRelayService
{
    private readonly IAvatarInfoService avatarInfoService;
    private PeriodicTimer messageTimer = new PeriodicTimer(TimeSpan.FromMilliseconds(100));
    private readonly CancellationTokenSource queueCheckCancellationTokenSource = new CancellationTokenSource();
    private List<VRCMessage> capturedMessages = new List<VRCMessage>();

    public RelayService(IAvatarInfoService avatarInfoService)
    {
        this.avatarInfoService = avatarInfoService;
        OSCService.OnMessageReceived += OnReceiveMessage;
        Task.Run(() => StartMessageQueue());
    }

    private void OnReceiveMessage(string[] messages, string value)
    {
        if (messages.Length <= 0)
            return;

        if (messages[0] != "avatar")
            return;

        switch (messages[1])
        {
            case "parameters":
            {
                ConvertToVRCMessage(messages[1], messages[2], value);
                break;
            }
            case "change":
            {
                ConvertToVRCMessage(messages[1],messages[1], value);
                break;
            }
        }
    }

    private void ConvertToVRCMessage(string type, string parameter, string value)
    {
        if (!VRCMessageDictionary.StringTypeDictionary.TryGetValue(type, out VRCMessageType messageType))
            return;

        if (VRCMessageDictionary.StringTypeDictionary.TryGetValue(parameter, out VRCMessageType valueType))
            messageType = valueType;
        
        VRCMessage vrcMessage = new VRCMessage(messageType, parameter, value);
        
        HandleMessage(vrcMessage);
    }

    private void HandleMessage(VRCMessage vrcMessage)
    {
        switch (vrcMessage.messageType)
        {
            case VRCMessageType.AVATAR_CHANGE:
            {
                avatarInfoService.SwapAvatar(vrcMessage, new List<VRCMessage>(capturedMessages));
                break;
            }
            case VRCMessageType.MUTE_SELF:
            {
                break;
            }
            case VRCMessageType.AVATAR_PARAMETER:
            {
                capturedMessages.Add(vrcMessage);
                break;
            }
            default:
            {
                break;
            }
        }
    }

    private async Task StartMessageQueue()
    {
        while (await messageTimer.WaitForNextTickAsync())
        {
            if (capturedMessages.Count == 0)
                continue;
            
            capturedMessages.Clear();
        }
    }
}