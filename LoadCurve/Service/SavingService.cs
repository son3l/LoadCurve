using LoadCurve.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace LoadCurve.Service;

public class SavingService
{
    public List<Server> LoadServers()
    {
        string CacheDirectory = Path.Combine(Environment.CurrentDirectory, "cache");
        if (!Directory.Exists(CacheDirectory)) return [];
        List<string> files = [.. Directory.GetFiles(CacheDirectory)];
        List<Server> Servers = new();
        foreach (var file in files)
        {
            Server server = JsonSerializer.Deserialize<Server>(File.ReadAllText(Path.Combine(CacheDirectory, file)));
            if (server is not null)
                Servers.Add(server);
        }
        return Servers;
    }
    public bool SaveServer(List<Server> SavingServers)
    {
        if (SavingServers is null) return false;
        foreach (var file in Directory.GetFiles("cache"))
        {
            File.Delete(file);
        }
        if (Directory.Exists("cache"))
        {
            foreach (var SavingServer in SavingServers)
            {
                string data = JsonSerializer.Serialize(SavingServer);
                File.WriteAllTextAsync(Path.Combine(Environment.CurrentDirectory, "cache", $"{SavingServer.Name}.server"), data);
            }
            return true;
        }
        else
        {
            Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, "cache"));
            foreach (var SavingServer in SavingServers)
            {
                string data = JsonSerializer.Serialize(SavingServer);
                File.WriteAllTextAsync(Path.Combine(Environment.CurrentDirectory, "cache", $"{SavingServer.Name}.server"), data);
            }
            return true;
        }
    }
}