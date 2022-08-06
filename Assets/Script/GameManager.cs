using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UI;
using Task = System.Threading.Tasks.Task;

public class GameManager : MonoBehaviour
{
    public GameObject RegisterPanel;

    public TextMeshProUGUI RegisterText;

    public GameObject Loading;

    public bool InLoading = false;

    bool _registerd;

    async void Start()
    {
        InLoading = true;
        await FindObjectOfType<ServerManager>().ConnectToServer("http://127.0.0.1:3000");
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
        bool result = await FindObjectOfType<ServerManager>().DoRegister(PlayerPrefs.GetString("username"));
        while (!result)
        {
            result = await FindObjectOfType<ServerManager>().DoRegister(PlayerPrefs.GetString("username"));
            Task.Yield();
        }

        _registerd = true;
    }

    // Update is called once per frame
    void Update()
    {
        Loading.SetActive(InLoading);
    }
}