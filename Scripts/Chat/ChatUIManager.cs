using Firebase.Auth;
using Michsky.UI.ModernUIPack;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChatUIManager : MonoBehaviour
{
    //Side Menu
    [Header("Side Menu")]
    public TMP_Text usernameText;
    public TMP_Text emailText;


    public GameObject chatPanel;
    public GameObject meetingPrefab;
    public Text meetingHeading;
    public Transform meetingContainer;
    public GameObject sideMenuPanel;
    //public Transform chatContainer;
    public Image groupsBackground;

    public ModalWindowManager deleteConfirmation;
    public ModalWindowManager videoConfirmation;


    Firebase.Auth.FirebaseUser user;
    private Dictionary<string, GameObject> meetingGameObjects = new Dictionary<string, GameObject>();

    //Color
    public Color allColor;
    public Color generalColor;
    public Color socialColor;
    public Color workColor;
    



    //Singleton
    private static ChatUIManager instance = null;
    public static ChatUIManager Instance
    {
        get
        {
            if (instance == null)
                instance = GameObject.FindObjectOfType(typeof(ChatUIManager)) as ChatUIManager;
            return instance;
        }
    }

    public GameObject loadingScreen;
    // Start is called before the first frame update
    void Start()
    {
        
        user = FirebaseAuth.DefaultInstance.CurrentUser;
        usernameText.text = user.DisplayName;
        emailText.text = user.Email;
        FirebaseRequests.Instance.GetUserMeetings(user.UserId, OnGetMeetings, GetMeetingsFailed);
    }

    public void OnGetMeetings(Dictionary<string, UserChannel> meetings)
    {
        Debug.Log("Mett");
        Debug.Log(meetings.Count);
        foreach(var meeting in meetings)
        {
            InstantiateMeetingPrefab(meeting.Key, meeting.Value);
        }
        string key = PlayerPrefs.GetString("channelname");
        if (key !="None")
        {
            meetingGameObjects[key].GetComponent<MeetingPrefabScript>().JoinChat();
        }
    }

    public void InstantiateMeetingPrefab(string key, UserChannel meeting)
    {
        var meetingGO = Instantiate(meetingPrefab, meetingContainer);
        var meetingScript = meetingGO.GetComponent<MeetingPrefabScript>();
        meetingScript.meetingAgenda.text = meeting.channelName;
        meetingScript.date.text = meeting.channelDate;
        meetingScript.category = meeting.category;
        meetingScript.channelId = key;
        meetingGameObjects.Add(key, meetingGO);

    }

    public void GetMeetingsFailed(AggregateException exception)
    {
        Notifier.Instance.Notify("Failed", "Failed to fetch the meetings");
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void LeaveChat()
    {
        chatPanel.SetActive(false);
        MessageHandler.Instance.ClearPanel();
        FirebaseRequests.Instance.StopListeningForMessages();
        sideMenuPanel.SetActive(true);
    }

    public void ShowGroups(string category)
    {
        foreach(var meetingGO in meetingGameObjects)
        {
            if((meetingGO.Value.GetComponent<MeetingPrefabScript>().category == category) || (category == "ALL"))
            {
                meetingGO.Value.SetActive(true);
            }
            else
            {
                meetingGO.Value.SetActive(false);
            }
        }
        Encolor(category);

    }
    private void Encolor(string category)
    {
        switch (category)
        {
            case "ALL":
                groupsBackground.color = allColor;
                break;
            case "GENERAL":
                groupsBackground.color = generalColor;
                break;
            case "SOCIAL":
                groupsBackground.color = socialColor;
                break;
            case "WORK":
                groupsBackground.color = workColor;
                break;
            default:
                groupsBackground.color = allColor;
                break;
        }
    }


    //Delete Meeting
    public void Delete()
    {
        FirebaseRequests.Instance.Delete($"Users/{user.UserId}/{PlayerPrefs.GetString("channelname")}", this.DestroySelf);
    }
    public void DestroySelf()
    {
        var channelid = PlayerPrefs.GetString("channelname");
        GameObject.Destroy(meetingGameObjects[channelid]);
        meetingGameObjects.Remove(channelid);

    }

    //Join Meeting
    public void JoinMeet()
    {
        var channelid = PlayerPrefs.GetString("channelname");
        loadingScreen.SetActive(true);
        FirebaseRequests.Instance.StopListeningForMessages();
        AgoraManager.Instance.OnJoinVideoChat(channelid, user.DisplayName);
    }

    #region SideMenu
    public void LoadMeetScene()
    {
        PlayerPrefs.SetString("channelname", "None");
        SceneManager.LoadScene("Home");
    }

    public void LogOut()
    {
        FirebaseRequests.Instance.Logout();
    }

    #endregion


}
