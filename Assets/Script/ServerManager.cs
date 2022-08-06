using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SocketIOClient;
using UnityEngine;
using Task = System.Threading.Tasks.Task;

public class ServerManager : MonoBehaviour
{
    public SocketIOUnity socket;

    private bool registerFull = false;

    // Start is called before the first frame update

    public async Task ConnectToServer(string url)
    {
        var uri = new Uri(url);
        socket = new SocketIOUnity(uri, new SocketIOOptions
        {
            Query = new Dictionary<string, string>
            {
                { "token", "UNITY" }
            },
            Transport = SocketIOClient.Transport.TransportProtocol.WebSocket
        });
        await socket.ConnectAsync();

        socket.On("usernameGot", (data) => { registerFull = true; });

        socket.OnConnected += (sender, e) => { print("Conected To Server"); };
    }

    public async Task<bool> DoRegister(string username)
    {
        long startTime = ((DateTimeOffset)DateTime.Now).ToUnixTimeMilliseconds();
        await socket.EmitAsync("register", username);

        while (registerFull)
        {
            if (((DateTimeOffset)DateTime.Now).ToUnixTimeMilliseconds() - startTime == 3000)
            {
                return false;
            }

            await Task.Yield();
        }

        return true;
    }
}