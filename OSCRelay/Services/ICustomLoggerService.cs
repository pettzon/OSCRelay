using System;
using System.Threading.Tasks;

namespace OSCRelay.Services;

public interface ICustomLoggerService
{
    public static event Action<string> OnLogged;
    public void LogMessage(string message);
}