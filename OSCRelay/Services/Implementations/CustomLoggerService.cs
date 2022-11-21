using System;
using System.Threading.Tasks;

namespace OSCRelay.Services.Implementations;

public class CustomLoggerService : ICustomLoggerService
{
    public static event Action<string> OnLogged;
    
    public void LogMessage(string message)
    {
        OnLogged?.Invoke(message);
    }
}