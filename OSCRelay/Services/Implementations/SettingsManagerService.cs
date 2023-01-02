using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Global.Data.Data;
using Global.Data.Data.VRChat;

namespace OSCRelay.Services.Implementations;

public class SettingsManagerService : ISettingsManagerService
{
    private readonly ICustomLoggerService customLoggerService;
    
    private UserSettings? userSettings;
    private ExposedParameters? exposedAvatarParameters;

    private readonly string? settingsPath;
    private readonly string? settingsFileName;
    private readonly string? avatarParameterStorage;

    public static event Action<UserSettings>? OnSettingsLoaded;
    public static event Action<ExposedParameters> OnExposedParametersChanged;

    public SettingsManagerService(ICustomLoggerService customLoggerService)
    {
        this.customLoggerService = customLoggerService;
        
        settingsPath = @".\config";
        settingsFileName = "settings.json";
        avatarParameterStorage = @".\config\avatar";

        AvatarInfoService.OnAvatarParametersLoaded += OnAvatarParametersLoaded;
        
        GenerateDirectories();
        LoadSettings();
    }

    private async Task LoadSettings()
    {
        userSettings = await LoadSettingsAsync();
        OnSettingsLoaded?.Invoke(userSettings!);
        
        exposedAvatarParameters = await LoadExposedAvatarParametersAsync(userSettings.LastActiveAvatarID);
    }
    
    private async Task<UserSettings> LoadSettingsAsync()
    {
        string fullPath = Path.Combine(settingsPath!, settingsFileName!);
        
        if (File.Exists(fullPath))
        {
            string json = await File.ReadAllTextAsync(fullPath);
            return JsonSerializer.Deserialize<UserSettings>(json)!;
        }
        else
        {
            return await GenerateDefaultSettingsAsync();
        }
    }

    private void LoadExposedAvatarParameters(string id)
    {
        LoadExposedAvatarParametersAsync(id);
    }

    private async Task<ExposedParameters> LoadExposedAvatarParametersAsync(string id)
    {
        string fullPath = Path.Combine(avatarParameterStorage, $"{id}.json");

        if (File.Exists(fullPath))
        {
            string json = await File.ReadAllTextAsync(fullPath);
            return JsonSerializer.Deserialize<ExposedParameters>(json);
        }

        return new ExposedParameters(id, "", new List<AvatarParameter>());
    }

    private void GenerateDirectories()
    {
        Directory.CreateDirectory(settingsPath!);
        Directory.CreateDirectory(avatarParameterStorage!);
    }
    
    private async Task<UserSettings> GenerateDefaultSettingsAsync()
    {
        string fullPath = Path.Combine(settingsPath!, settingsFileName!);

        UserSettings settings = new UserSettings()
        {
            //VRChat default send / receive ports
            PortReceive = 9001,
            PortSend = 9000
        };

        string json = JsonSerializer.Serialize(settings);
        await File.WriteAllTextAsync(fullPath, json);

        return settings;
    }

    public UserSettings GetUserSettings()
    {
        return userSettings;
    }

    public ExposedParameters GetExposedAvatarParameters()
    {
        return exposedAvatarParameters;
    }
    
    public void SetUserSettings(UserSettings settings)
    {
        userSettings = settings;
    }

    public async Task SaveUserSettings()
    {
        await SaveJson(userSettings, settingsPath!, settingsFileName!);
    }

    public void SetUserEnabledParameters(ExposedParameters exposedParameters)
    {
        exposedAvatarParameters = exposedParameters;
        SaveJson(exposedParameters, avatarParameterStorage!, $"{exposedParameters.avatarID}.json");
        
        OnExposedParametersChanged?.Invoke(exposedParameters);
    }
    
    private async Task SaveJson<T>(T type, string path, string fileName)
    {
        string fullPath = Path.Combine(path, fileName);
        string json = JsonSerializer.Serialize(type);
        await File.WriteAllTextAsync(fullPath, json);
        customLoggerService.LogMessage($"SAVING {type}");
    }

    private async void OnAvatarParametersLoaded(VRCData vrcData, List<VRCMessage> vrcMessages)
    {
        userSettings.LastActiveAvatarID = vrcData.id;
        exposedAvatarParameters = await LoadExposedAvatarParametersAsync(vrcData.id);
        await SaveJson(userSettings, settingsPath!, settingsFileName!);

        if (exposedAvatarParameters.avatarParameters.Count > 0)
        {
            await SetEnabledParameters(exposedAvatarParameters, vrcMessages);
        }
        
        OnExposedParametersChanged?.Invoke(exposedAvatarParameters);
    }

    private async Task SetEnabledParameters(ExposedParameters parameters, List<VRCMessage> vrcMessages)
    {
        await Task.Run(() =>
        {
            foreach (AvatarParameter parameter in parameters.avatarParameters)
            {
                foreach (VRCMessage message in vrcMessages)
                {
                    if (message.parameter != parameter.name)
                        continue;
                    
                    if (float.TryParse(message.value, out float value))
                    {
                        parameter.value = value;
                        continue;
                    }

                    parameter.value = message.value.ToLower() == "true" ? 1f : 0f;
                }
            }
        });
    }
}