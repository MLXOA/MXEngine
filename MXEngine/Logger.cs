namespace MXEngine;

public class Logger(string identifier)
{
    public void Info(string message)
    {
        Console.WriteLine($"[INFO - {identifier}] {message}");
    }

    public void Warn(string message)
    {
        Console.WriteLine($"[WARN - {identifier}] {message}");
    }
}