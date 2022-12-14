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

    public bool HasDisconnect = false;

    public event EventHandler<MessageBag> GetMessageEvent;

    public event EventHandler<string> WellcomeEvent;

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

        socket.On("usernameGot", (data) => { registerFull = true; });

        socket.OnDisconnected += (sender, e) =>
        {
            print("disconected");
            HasDisconnect = true;
        };

        socket.OnConnected += (sender, e) =>
        {
            print("Conected To Server");
            if (HasDisconnect)
            {
                HasDisconnect = false;
            }
        };

        socket.OnUnityThread("messageServer", GetNewMessage);

        socket.OnUnityThread("welcome", (data) =>
        {
            string result = data.GetValue<string>();
            WellcomeEvent.Invoke(this, result);
        });


        await socket.ConnectAsync();
    }

    public async Task<bool> DoRegister(string username)
    {
        registerFull = false;
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

    public async void GetNewMessage(SocketIOResponse data)
    {
        var SData = data.GetValue<MessageBag>();

        GetMessageEvent.Invoke(this, SData);
    }

    public async void SendMessage(string message)
    {
        socket.Emit("messageClient", message);
    }
}