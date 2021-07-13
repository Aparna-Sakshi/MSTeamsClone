using Firebase.Auth;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MeetingPrefabScript : MonoBehaviour
{
    public Text meetingAgenda;
    public Text date;
    public string category;
    public string channelId;

    Firebase.Auth.FirebaseUser user;
    private string username;
    public void ShareChannelId()
    {
        UniClipboard.SetText(channelId);
        Notifier.Instance.Notify("Copied!", "The meeting id has been copied");
    }


    public void JoinChat()
    {
        PlayerPrefs.SetString("channelname", channelId);
        ChatUIManager.Instance.chatPanel.SetActive(true);
        ChatUIManager.Instance.meetingHeading.text = meetingAgenda.text;
        MessageHandler.Instance.StartMessageListener();
        ChatUIManager.Instance.sideMenuPanel.SetActive(false);
    }

    

    public void DeleteConfirmation()
    {
        PlayerPrefs.SetString("channelname", channelId);
        ChatUIManager.Instance.deleteConfirmation.OpenWindow();
    }
    
    public void MeetConfirmation()
    {
        PlayerPrefs.SetString("channelname", channelId);
        ChatUIManager.Instance.videoConfirmation.OpenWindow();
    }
    // Start is called before the first frame update
    void Start()
    {
        user = FirebaseAuth.DefaultInstance.CurrentUser;
        username = user.DisplayName;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
