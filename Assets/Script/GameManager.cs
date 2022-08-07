using System;
using TMPro;
using UnityEngine;
using Task = System.Threading.Tasks.Task;

public class GameManager : MonoBehaviour
{
    public GameObject RegisterPanel;

    public GameObject MessageText;

    public Transform MessagePanel;

    public TextMeshProUGUI RegisterText;

    public GameObject Loading;

    public bool InLoading = false;

    private bool HasOnDisconnect = false;

    bool _registerd;

    ServerManager ServerManager;

    public TextMeshProUGUI textInput;

    async void Start()
    {
        ServerManager = FindObjectOfType<ServerManager>();
        InLoading = true;
        await ServerManager.ConnectToServer("http://185.110.190.182:3000");
        if (PlayerPrefs.GetString("username", String.Empty) == String.Empty)
        {
            RegisterPanel.SetActive(true);
        }
        else
        {
            SetUserName();
        }

        InLoading = false;
        ServerManager.GetMessageEvent += GetNewMessage;
        ServerManager.WellcomeEvent += sayWellcome;
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

    public async void SendMessageOptions()
    {
        string m = textInput.text;
        textInput.text = String.Empty;
        doAddText("You: " + m);
        ServerManager.SendMessage(m);
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


    void sayWellcome(object sender, string userName)
    {
        doAddText("Say Wellcome To " + userName);
    }

    void GetNewMessage(object sender, MessageBag messageBag)
    {
        doAddText(messageBag.sender + ": " + messageBag.message);
    }

    void doAddText(string text)
    {
        GameObject result = Instantiate(MessageText, MessagePanel);
        result.GetComponent<TextMeshProUGUI>().text = text;
    }
}