using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Global.Data.Data;
using Global.Data.Data.Web;
using SocketIOClient;
using SocketIOClient.Transport;

namespace OSCRelay.Services.Implementations;

public class SocketIOService : IServiceProvider
{
    private SocketIO client;
    private readonly ICustomLoggerService customLogger;
    private Token APItoken;

    private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
    private readonly CancellationToken cancellationToken;
    
    public event Action<Token>? OnConnected;
    public event Action<AvatarParameterDTO> OnReceiveParameter;
    
    public SocketIOService(ICustomLoggerService customLogger)
    {
        this.customLogger = customLogger;
        cancellationToken = cancellationTokenSource.Token;
        
        // client = new SocketIO("https://lush-successful-spring.glitch.me/", new SocketIOOptions()
        // {
        //     EIO = EngineIO.V4,
        //     Transport = TransportProtocol.WebSocket,
        //     ExtraHeaders = new Dictionary<string, string>() { {"User-agent","OSCRelay"}}
        // });
    }

    public void StartProviderService(Token token)
    {
        StartProviderServiceAsync(cancellationToken, token);
    }

    private async Task StartProviderServiceAsync(CancellationToken cancellationToken, Token token)
    {
        client = new SocketIO("ws://api-vrcosc.huks.dev/", new SocketIOOptions()
        {
            EIO = EngineIO.V4,
            Transport = TransportProtocol.WebSocket,
            Path = "/ws/socket.io",
            ExtraHeaders = new Dictionary<string, string>() { {"User-agent","OSCRelay"}}
        });

        RegisterClientEvents();

        if (string.IsNullOrEmpty(token.token))
        {
            customLogger.LogMessage("Fetching token");
            APItoken = await FetchAccessToken();
        }

        customLogger.LogMessage($"Token fetched {APItoken}");
        
        if (string.IsNullOrEmpty(APItoken.token))
        {
            customLogger.LogMessage($"Could not connect to remote server!");
            cancellationTokenSource.Cancel();
        }
        
        await client.ConnectAsync();

        SettingsManagerService.OnExposedParametersChanged += UpdateExposedAvatarParameters;
    }

    private void RegisterClientEvents()
    {
        client.OnConnected += OnConnectedAsync;
        client.On("param", OnReceiveAvatarParams);
    }

    private void OnReceiveAvatarParams(SocketIOResponse response)
    {
        OnReceiveParameter?.Invoke(response.GetValue<AvatarParameterDTO>());
    }

    private void OnConnectedAsync(object? sender, EventArgs e)
    {
        customLogger.LogMessage($"Connected to session : {client.Id} with token : {APItoken.token}");
        OnConnected?.Invoke(APItoken);
    }

    private async Task<Token> FetchAccessToken()
    {
        using (HttpClient client = new HttpClient())
        {
            HttpResponseMessage message = await client.GetAsync("https://api-vrcosc.huks.dev/my-token/");
            message.EnsureSuccessStatusCode();

            string json = await message.Content.ReadAsStringAsync();
            Token? accessToken = JsonSerializer.Deserialize<Token>(json);

            return accessToken!;
        }
    }

    public void UpdateExposedAvatarParameters(ExposedParameters parameters)
    {
        UpdateExposedAvatarParametersAsync(parameters);
    }

    private async Task UpdateExposedAvatarParametersAsync(ExposedParameters parameters)
    {
        AllAvatarParametersDTO AllParamsDTO = ConvertAllParamsToDTO(parameters);

        await client.EmitAsync("token", APItoken);
        await client.EmitAsync("params", AllParamsDTO);
    }

    private AllAvatarParametersDTO ConvertAllParamsToDTO(ExposedParameters parameters)
    {
        List<AvatarParameterDTO> DTO = new List<AvatarParameterDTO>();
        
        foreach(AvatarParameter parameter in parameters.avatarParameters)
        {
            DTO.Add(new AvatarParameterDTO(parameter.value, parameter.name, parameter.type));
            Debug.WriteLine($"Sending parameter {parameter.name} with value {parameter.value}");
        }

        return new AllAvatarParametersDTO(parameters.avatarID, DTO);
    }
}