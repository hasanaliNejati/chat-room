using System;
using TMPro;
using UnityEngine;
using Task = System.Threading.Tasks.Task;

public class GameManager : MonoBehaviour
{
    public GameObject RegisterPanel;

    public TextMeshProUGUI RegisterText;

    public GameObject Loading;

    public bool InLoading = false;

    private bool HasOnDisconnect = false;

    bool _registerd;

    ServerManager ServerManager;


    async void Start()
    {
        ServerManager = FindObjectOfType<ServerManager>();
        InLoading = true;
        await ServerManager.ConnectToServer("http://127.0.0.1:3000");
        if (PlayerPrefs.GetString("username", String.Empty) == String.Empty)
        {
            RegisterPanel.SetActive(true);
        }
        else
        {
            SetUserName();
        }

        InLoading = false;
    }


    public async void SetName()
    {
        if (RegisterText.text != String.Empty)
        {
            PlayerPrefs.SetString("username", RegisterText.text);
            RegisterPanel.SetActive(false);
            SetUserName();
        }
    }


    public async Task SetUserName()
    {
        bool result = await ServerManager.DoRegister(PlayerPrefs.GetString("username"));
        while (!result)
        {
            result = await ServerManager.DoRegister(PlayerPrefs.GetString("username"));
            await Task.Yield();
        }

        _registerd = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (ServerManager.HasDisconnect)
        {
            InLoading = true;
            HasOnDisconnect = true;
        }
        else if (HasOnDisconnect)
        {
            print("reconnected.");
            HasOnDisconnect = false;
            InLoading = false;
            if (PlayerPrefs.GetString("username", String.Empty) != String.Empty)
            {
                SetUserName();
            }
        }

        Loading.SetActive(InLoading);
    }
}