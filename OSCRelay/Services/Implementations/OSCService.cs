using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Global.Data.Data;
using Global.Data.Data.Web;
using Rug.Osc.Core;

namespace OSCRelay.Services.Implementations;

public class OSCService : IOSCService
{
    private OscReceiver OSCReceiver;
    private OscSender OSCSender;
    
    private Thread thread;
    private readonly IRelayService relayService;
    private readonly ICustomLoggerService customLoggerService;
    private readonly IServiceProvider serviceProvider;
    
    private int receiverPort;
    private int senderPort;
    
    public bool isConnectedOSC { get; private set; }

    public static event Action<string[], string>? OnMessageReceived;

    public OSCService(IRelayService relayService, ICustomLoggerService customLoggerService, IServiceProvider serviceProvider)
    {
        this.relayService = relayService;
        this.customLoggerService = customLoggerService;
        this.serviceProvider = serviceProvider;

        serviceProvider.OnReceiveParameter += SendAvatarParameters;
    }
    
    public void ConnectToOSC(int receiverPort, int senderPort, Token token)
    {
        this.receiverPort = receiverPort;
        this.senderPort = senderPort;
        
        customLoggerService.LogMessage($"Creating listener : {receiverPort} sender : {senderPort}");
        OSCReceiver = new OscReceiver(receiverPort);

        thread = new Thread(ListenLoop);
        
        OSCReceiver.Connect();
        thread.Start();
        
        customLoggerService.LogMessage($"Token is {token.token}");
        serviceProvider.StartProviderService(token);

        isConnectedOSC = true;
    }

    public void SendAvatarParameters(AvatarParameterDTO avatarParameter)
    {
        switch (avatarParameter.type)
        {
            case ParameterValueType.Bool:
            {
                SendAvatarParameter(new OscMessage($"/avatar/parameters/{avatarParameter.name}", avatarParameter.value > 0 ? true : false));
                break;
            }
            case ParameterValueType.Int:
            {
                SendAvatarParameter(new OscMessage($"/avatar/parameters/{avatarParameter.name}", (int)avatarParameter.value));
                break;
            }
            case ParameterValueType.Float:
            {
                SendAvatarParameter(new OscMessage($"/avatar/parameters/{avatarParameter.name}", avatarParameter.value));
                break;
            }
        }
    }
    
    private void SendAvatarParameter(OscMessage message)
    {
        using (OSCSender = new OscSender(IPAddress.Loopback, 0, senderPort))
        {
            OSCSender.Connect();
            customLoggerService.LogMessage($"Sending message {message}");
            OSCSender.Send(message);
        }
    }

    private void ListenLoop()
    {
        try
        {
            while (OSCReceiver.State != OscSocketState.Closed)
            {
                if (OSCReceiver.State != OscSocketState.Connected) continue;
                
                OscPacket packet = OSCReceiver.Receive();
                ParseMessage(packet.ToString());
            }
        }
        catch (Exception ex)
        {
            if (OSCReceiver.State == OscSocketState.Connected)
            {
                
            }
        }
    }

    private void ParseMessage(string? message)
    {
        string[] messageValueSplit = message.Split(',');
        string value = messageValueSplit.Length > 1 ? messageValueSplit[1].Replace('"', ' ').Trim() : string.Empty;
        
        string[] messageSplit = messageValueSplit[0][1..].Replace('"', ' ').Trim().Split('/');
        OnMessageReceived?.Invoke(messageSplit, value);

        // if (VRCMessageDictionary.StringTypeDictionary.TryGetValue(messageValue[0].Trim(), out VRCMessageType value))
        // {
        //     Debug.WriteLine(message);
        //     VRCMessage vrcMessage = new VRCMessage(value, messageValue[1].Replace('"', ' ').Trim());
        //     OnMessageReceived?.Invoke(vrcMessage);
        // }
    }
}