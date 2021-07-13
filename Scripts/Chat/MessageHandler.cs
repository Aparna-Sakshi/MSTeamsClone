using Firebase.Auth;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MessageHandler : MonoBehaviour
{

    //Singleton
    private static MessageHandler instance = null;
    public static MessageHandler Instance
    {
        get
        {
            if (instance == null)
                instance = GameObject.FindObjectOfType(typeof(MessageHandler)) as MessageHandler;
            return instance;
        }
    }

    //Chat Container
    public Transform container;

    //Input field
    public TMP_InputField sendMessageInput;

    //Chat Prefabs
    public GameObject remoteChatPrefab;
    public GameObject localChatPrefab;

    private Dictionary<string, GameObject> messageGameObjects = new Dictionary<string, GameObject>();
    private string channelname;
    Firebase.Auth.FirebaseUser user;

    public void StartMessageListener()
    {
        channelname = PlayerPrefs.GetString("channelname");
        FirebaseRequests.Instance.ListenForNewMessages(channelname, this.AddMessage, this.ListenerFailed);
        user = FirebaseAuth.DefaultInstance.CurrentUser;
    }

    public void ListenerFailed(AggregateException exception)
    {
        Notifier.Instance.Notify("Error", "Adding Message Listener Failed");
    }
    public void PostMessage()
    {
        if(sendMessageInput.text=="")
        {
            Notifier.Instance.Notify("No Text", "Please type some text");
            return;
        }
        //new message object
        var message = new Message();
        message.content = sendMessageInput.text;
        message.username = user.DisplayName;
        DateTime date = DateTime.Now;        
        message.timestamp = date.ToString("dd/MM/yyyy HH:mm");
        message.uid = user.UserId;

        var jsonValue = message.Serialize().ToString(3);
        FirebaseRequests.Instance.Post(jsonValue, $"Channels/{channelname}/messages");
        sendMessageInput.text = "";
    }

    public void AddMessage(string key, Message message)
    {
        Debug.Log("Adding new message");
        if(message.uid==user.UserId)//if local user
        {
            var messageGO = Instantiate(localChatPrefab, container);
            var messageScript = messageGO.GetComponent<ChatPrefabScript>();
            messageScript.usernameText.text = "You";
            messageScript.contentText.text = message.content;
            messageScript.timestampText.text = message.timestamp;
            messageGameObjects.Add(key, messageGO);

        }
        else
        {
            var messageGO = Instantiate(remoteChatPrefab, container);
            var messageScript = messageGO.GetComponent<ChatPrefabScript>();
            messageScript.usernameText.text = message.username;
            messageScript.contentText.text = message.content;
            messageScript.timestampText.text = message.timestamp;
            messageGameObjects.Add(key, messageGO);
        }
        

    }

    public void ClearPanel()
    {
        foreach (Transform child in container.transform)
        {
            Destroy(child.gameObject);
            
        }
        messageGameObjects.Clear();
    }
    // Start is called before the first frame update
    void Start()
    {
        if(SceneManager.GetActiveScene().name=="Video")
        {
            StartMessageListener();
        }
        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
