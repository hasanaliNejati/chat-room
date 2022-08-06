using System;
using System.Collections.Generic;
using SocketIOClient;
using UnityEngine;

public class ServerManager : MonoBehaviour
{
    public SocketIOUnity socket;

    // Start is called before the first frame update
    void Start()
    {
        var uri = new Uri("http://127.0.0.1:3000");
        socket = new SocketIOUnity(uri, new SocketIOOptions
        {
            Query = new Dictionary<string, string>
            {
                { "token", "UNITY" }
            },
            Transport = SocketIOClient.Transport.TransportProtocol.WebSocket
        });
        socket.Connect();

        var a = new Newtonsoft.Json.JsonSerializer();

        socket.OnConnected += (sender, e) => { print("Conected To Server"); };
    }

    // Update is called once per frame
}