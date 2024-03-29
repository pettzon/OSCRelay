﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Global.Data.Data;
using Global.Data.Data.VRChat;

namespace OSCRelay.Services.Implementations;

public class AvatarInfoService : IAvatarInfoService
{
    private readonly string dataPath;
    private readonly ISettingsManagerService settingsManagerService;
    private readonly ICustomLoggerService customLoggerService;
    private VRCData? currentAvatarParameters;

    public static event Action<VRCData, List<VRCMessage>>? OnAvatarParametersLoaded;

    public AvatarInfoService(ISettingsManagerService settingsManagerService, ICustomLoggerService customLoggerService)
    {
        this.settingsManagerService = settingsManagerService;
        this.customLoggerService = customLoggerService;
        
        dataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).Replace("Roaming","LocalLow"), "VRChat/VRChat/OSC");

        SettingsManagerService.OnSettingsLoaded += OnSettingsLoaded;
    }

    private void OnSettingsLoaded(UserSettings userSettings)
    {
        FetchLocalOSCData(new VRCMessage(VRCMessageType.AVATAR_CHANGE ,"", settingsManagerService.GetUserSettings().LastActiveAvatarID));
    }

    public void SwapAvatar(VRCMessage vrcMessage, List<VRCMessage> enabledParameters)
    {
        SwapAvatarAsync(vrcMessage, enabledParameters);
    }

    private async Task SwapAvatarAsync(VRCMessage vrcMessage, List<VRCMessage> vrcMessageList)
    {
        VRCData vrcData = await FetchLocalOSCDataAsync(vrcMessage);
        OnAvatarParametersLoaded?.Invoke(vrcData!, vrcMessageList);
    }
    public void FetchLocalOSCData(VRCMessage vrcMessage)
    {
        FetchLocalOSCDataAsync(vrcMessage);
    }

    public VRCData? GetCurrentAvatarParameters()
    {
        return currentAvatarParameters;
    }

    private async Task<VRCData> FetchLocalOSCDataAsync(VRCMessage vrcMessage)
    {
        string path = Path.Combine(dataPath, settingsManagerService.GetUserSettings().AccountID, "Avatars", $"{vrcMessage.value}.json");
        
        string data = await File.ReadAllTextAsync(path);
        VRCData? vrcData = JsonSerializer.Deserialize<VRCData>(data);
        currentAvatarParameters = vrcData;

        customLoggerService.LogMessage($"Loaded data for avatar : {vrcData!.name} with parameter count : {vrcData.parameters!.Count}");

        return vrcData;
    }
}