using Firebase.Auth;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Michsky.UI.ModernUIPack;
using System;
using TMPro;

//Manages UI in the Home scene and also talks to Firebase and Agora
public class HomeUIManager : MonoBehaviour
{


    //UI References  
    [Header("Header")]
    public GameObject enterHeader;
    public GameObject createHeader;



    [Header("Body")]
    public GameObject enterBody;
    public GameObject createBody;
    public GameObject joinBody;
    public GameObject loadingScreen;


    //Create meeting
    [Header("Create")]
    public TMP_InputField meetingAgendaInput;
    public TMP_Text createMeetingCategory;

    //Join Meeting
    [Header("Join")]
    public TMP_InputField meetingidInput;
    public TMP_Text joinMeetingCategory;


    //Enter Meeting
    [Header("Enter")]
    public TMP_Text meetingAgendaText;
    public Text channelnameText;

    //Side Menu
    [Header("Side Menu")]
    public TMP_Text usernameText;
    public TMP_Text emailText;


    private string token;
    private string channelname;
    private string username;
    private string meetingAgenda;
    private string category;
    Firebase.Auth.FirebaseUser user;
    

    //Singleton
    private static HomeUIManager instance = null;
    public static HomeUIManager Instance
    {
        get
        {
            if (instance == null)
                instance = GameObject.FindObjectOfType(typeof(HomeUIManager)) as HomeUIManager;
            return instance;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        loadingScreen.SetActive(false);
        user = FirebaseAuth.DefaultInstance.CurrentUser;
        username = user.DisplayName;
        usernameText.text = username;
        emailText.text = user.Email;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    

    #region SideMenu
    public void LoadChatScene()
    {
        PlayerPrefs.SetString("channelname", "None");
        SceneManager.LoadScene("Chat");
    }

    public void LogOut()
    {
        FirebaseRequests.Instance.Logout();
    }

    #endregion


    #region CreateMeeting
    //attached to Create Button on create meeting panel
    public void OnCreateClick(int task)
    {
        //set username and channelname
        if(meetingAgendaInput.text=="")
        {
            Notifier.Instance.Notify("Meeting Agenda", "Please provide a meeting agenda");
        }
        else
        {
            //loadingScreen.SetActive(true);
            //Create a new channel object
            meetingAgenda = meetingAgendaInput.text;
            var newChannel = new Channel();
            newChannel.channelName = meetingAgenda;            
            DateTime date = DateTime.Now;            
            newChannel.channelDate = date.ToString("dd/MM/yyyy HH:mm");
            string jsonValue = newChannel.Serialize().ToString(3);
            Debug.Log(jsonValue);
            category = createMeetingCategory.text;

            //post the channel in firebase
            //task = 0(Create and join chat); 1(Create and join video call)
            if(task==0)
            {
                FirebaseRequests.Instance.Post(jsonValue, "Channels", this.EnterGroupChat, this.OnCreateFailed);
            }
            else
            {
                FirebaseRequests.Instance.Post(jsonValue, "Channels", this.EnterMeeting, this.OnCreateFailed);
            }
            
        }       
    }

    public void JoinInstead()
    {
        joinBody.SetActive(true);
        createBody.SetActive(false);
        enterBody.SetActive(false);
        createHeader.SetActive(true);
        enterHeader.SetActive(false);

    }
    public void EnterGroupChat(string key)
    {
        channelname = key;
        PlayerPrefs.SetString("channelname", channelname);
        FirebaseRequests.Instance.JoinChannel(channelname, category, this.OnJoinGroupChat, this.ChannelNotFound, this.OnJoinFailed);
    }
    public void OnJoinGroupChat()
    {
        SceneManager.LoadScene("Chat");
    }
    public void EnterMeeting(string key)
    {
        Debug.Log("Inside callback");
        loadingScreen.SetActive(false);
        meetingAgendaText.text = meetingAgenda;
        channelname = key;
        channelnameText.text = channelname;
        
        //Go to Enter Panel
        createBody.SetActive(false);
        enterBody.SetActive(true);
        createHeader.SetActive(false);
        enterHeader.SetActive(true);
    }

    public void OnCreateFailed(AggregateException exception)
    {
        Notifier.Instance.Notify("Unable to create meeting!", exception.Message);
        loadingScreen.SetActive(false);
    }

    #endregion

    #region EnterMeeting 
    //attched to copy button in enter meeting panel
    public void CopyMeetingID()
    {
        UniClipboard.SetText(channelname);
        Notifier.Instance.Notify("Copied!", "The meeting id has been copied");
    }

    public void OnEnterClick()
    {

        loadingScreen.SetActive(true);
        FirebaseRequests.Instance.JoinChannel(channelname, category, this.OnJoinVideoCall, this.ChannelNotFound, this.OnJoinFailed);
    }

    #endregion

    #region JoinMeeting
    //attached to Join button on JoinMeeting panel, and enter button on create meeting panel
    public void OnJoinClick(int task)
    {
        
        if(meetingidInput.text=="")
        {
            Notifier.Instance.Notify("No meeting ID!", "Meeting id is not provided");
        }
        else
        {
            channelname = meetingidInput.text;
            loadingScreen.SetActive(true);
            category = joinMeetingCategory.text;
            //task = 0(Create and join chat); 1(Create and join video call)
            if (task==0)
            {
                PlayerPrefs.SetString("channelname", channelname);
                FirebaseRequests.Instance.JoinChannel(channelname, category, this.OnJoinGroupChat, this.ChannelNotFound, this.OnJoinFailed);
            }
            else
            {
                FirebaseRequests.Instance.JoinChannel(channelname, category, this.OnJoinVideoCall, this.ChannelNotFound, this.OnJoinFailed);
            }
            
        }
        
    }

    public void CreateInstead()
    {
        joinBody.SetActive(false);
        createBody.SetActive(true);
        enterBody.SetActive(false);
        createHeader.SetActive(true);
        enterHeader.SetActive(false);

    }

    public void ChannelNotFound()
    {
        Notifier.Instance.Notify("Invalid", "Meeting ID entered doesn't exist, please verify");
        loadingScreen.SetActive(false);
    }

    public void OnJoinFailed(AggregateException exception)
    {
        Notifier.Instance.Notify("Unable to Join meeting!", exception.Message);
        loadingScreen.SetActive(false);
    }
    public void OnJoinVideoCall()
    {
        //Go to video scene
        PlayerPrefs.SetString("channelname", channelname);
        AgoraManager.Instance.OnJoinVideoChat(channelname, username);

    }

    #endregion



}
