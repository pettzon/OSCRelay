using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Global.Data.Data;
using Global.Data.Data.VRChat;
using OSCRelay.Services;

namespace OSCRelay;

public partial class AvatarParametersWindow : Window
{
    private readonly IAvatarInfoService avatarInfoService;
    private readonly ISettingsManagerService settingsManagerService;
    public ObservableCollection<AvatarParameter> avatarParameterViewList = new ObservableCollection<AvatarParameter>();
    
    public AvatarParametersWindow(IAvatarInfoService avatarInfoService, ISettingsManagerService settingsManagerService)
    {
        this.avatarInfoService = avatarInfoService;
        this.settingsManagerService = settingsManagerService;
        
        InitializeComponent();
        Closing += AvatarParameterWindow_Closing;

        avatarParameterViewList = GenerateParameterList();
        AvatarParameterList.ItemsSource = avatarParameterViewList;
    }

    private ObservableCollection<AvatarParameter> GenerateParameterList()
    {
        ObservableCollection<AvatarParameter> avatarParameters = new ObservableCollection<AvatarParameter>();
        VRCData? vrcData = avatarInfoService.GetCurrentAvatarParameters();
        
        if (vrcData is null)
            return new ObservableCollection<AvatarParameter>();
        
        
        Debug.WriteLine($"CURRENT AVATAR NAME {vrcData.name} {vrcData.id}");
        
        foreach (Parameter parameter in vrcData.parameters)
        {
            if (parameter.input != null)
            {
                ParameterValueType valueType = ParameterValueType.Bool;
                
                switch (parameter.input.type.ToLower())
                {
                    case "int":
                    {
                        valueType = ParameterValueType.Int;
                        break;
                    }
                    case "float":
                    {
                        valueType = ParameterValueType.Float;
                        break;
                    }
                }
                
                avatarParameters.Add(new AvatarParameter(false, parameter.name!, valueType));
            }
        }

        ExposedParameters exposedAvatarParameters = settingsManagerService.GetExposedAvatarParameters();
        Debug.WriteLine($"CURRENT EXPOSED AVATAR NAME {exposedAvatarParameters.avatarName}");

        if (exposedAvatarParameters != null)
        {
            foreach (AvatarParameter parameter in exposedAvatarParameters.avatarParameters)
            {
                foreach (AvatarParameter targetParameter in avatarParameters)
                {
                    if (parameter.name == targetParameter.name)
                    {
                        targetParameter.enabled = true;
                        break;
                    }
                }
            }
        }

        return avatarParameters;
    }

    private void AvatarParameterWindow_Closing(object sender, CancelEventArgs e)
    {
        VRCData vrcData = avatarInfoService.GetCurrentAvatarParameters();
        
        if (vrcData is null)
            return;
        
        List<AvatarParameter> parameterList = new List<AvatarParameter>();
        
        foreach(AvatarParameter parameter in avatarParameterViewList)
        {
            if (!parameter.enabled) continue;
            
            parameter.enabled = false;
            parameterList.Add(parameter);
        }
        
        ExposedParameters exposedParameters = new ExposedParameters(avatarInfoService.GetCurrentAvatarParameters().id, avatarInfoService.GetCurrentAvatarParameters().name, parameterList);
        settingsManagerService.SetUserEnabledParameters(exposedParameters);
    }
}