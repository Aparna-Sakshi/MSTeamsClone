using agora_gaming_rtc;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using agora_utilities;
using UnityEngine.SceneManagement;
using Tymski;

public class AgoraManager : GenericSingletonClass<AgoraManager>
{
    public GameObject videoPrefab;
    

    

    [SerializeField]
    private string APP_ID;    
    public IRtcEngine mRtcEngine = null;
    
    private static string tokenBase = "https://microsoftteamsclone.herokuapp.com";
    private CONNECTION_STATE_TYPE state = CONNECTION_STATE_TYPE.CONNECTION_STATE_DISCONNECTED;

    private string channelName;
    private string userName;
    private string channelToken="";
    public uint userid;
    
    public string GetChannelName()
    {
        return channelName;
    }
    void InitEngine()
    {
        mRtcEngine = IRtcEngine.GetEngine(APP_ID);
       
        //Adding listeners
        mRtcEngine.EnableAudio();
        mRtcEngine.EnableVideo();
        mRtcEngine.EnableVideoObserver();
        mRtcEngine.OnJoinChannelSuccess += OnJoinChannelSuccessHandler;
        mRtcEngine.OnLeaveChannel += OnLeaveChannelHandler;
        mRtcEngine.OnWarning += OnSDKWarningHandler;
        mRtcEngine.OnError += OnSDKErrorHandler;
        mRtcEngine.OnConnectionLost += OnConnectionLostHandler;
        mRtcEngine.OnUserJoined += OnUserJoinedHandler;
        mRtcEngine.OnUserOffline += OnUserOfflineHandler;
        mRtcEngine.OnTokenPrivilegeWillExpire += OnTokenPrivilegeWillExpireHandler;
        //mRtcEngine.OnUserMuteVideo += OnUserMuteVideoHandler;
        mRtcEngine.OnRemoteVideoStateChanged += onRemoteVideoStateChangedHandler;
        mRtcEngine.OnRemoteAudioStateChanged += onRemoteAudioStateChangedHandler;


    }

    private void onRemoteAudioStateChangedHandler(uint uid, REMOTE_AUDIO_STATE state, REMOTE_AUDIO_STATE_REASON reason, int elapsed)
    {
        
        bool unmuted = state.Equals(REMOTE_AUDIO_STATE.REMOTE_AUDIO_STATE_DECODING);
        TurnOnLocalAudioStream(uid, unmuted);
    }

    private void onRemoteVideoStateChangedHandler(uint uid, REMOTE_VIDEO_STATE state, REMOTE_VIDEO_STATE_REASON reason, int elapsed)
    {
        //throw new NotImplementedException();
        bool unmuted = state.Equals(REMOTE_VIDEO_STATE.REMOTE_VIDEO_STATE_DECODING);
        TurnOnLocalVideoStream(uid, unmuted);
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("agora manager scene start");
        // It is save to remove listeners even if they
        // didn't exist so far.
        // This makes sure it is added only once
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;

        // Add the listener to be called when a scene is loaded
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;

        DontDestroyOnLoad(gameObject);
        

        
        // Store the creating scene as the scene to trigger start
        //var currscene = SceneManager.GetActiveScene();
    }

    // Listener for sceneLoaded
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // return if not the start calling scene
        Debug.Log("Adding video prefs");
        if (scene.name!="Video")
        {
            return;
        }
            

        Debug.Log("Adding video prefs");
        makeVideoView(userid);
    }

    // Update is called once per frame
    void Update()
    {
        PermissionHelper.RequestMicrophontPermission();
        PermissionHelper.RequestCameraPermission();
    }

    void RenewOrJoinToken(string newToken)
    {
        channelToken = newToken;
        Debug.Log(newToken);
        if (state == CONNECTION_STATE_TYPE.CONNECTION_STATE_DISCONNECTED
            || state == CONNECTION_STATE_TYPE.CONNECTION_STATE_DISCONNECTED
            || state == CONNECTION_STATE_TYPE.CONNECTION_STATE_FAILED
        )
        {
            // If we are not connected yet, connect to the channel as normal
            JoinChannel();
            Debug.Log("Token fetched");
        }
        else
        {
            // If we are already connected, we should just update the token
            UpdateToken();
        }
    }

    void UpdateToken()
    {
        mRtcEngine.RenewToken(channelToken);
    }

    public void OnJoinVideoChat(string newchannelname, string newusername)
    {

        Debug.Log("Trying to join channel");
        InitEngine();
        channelName = newchannelname;
        userName = newusername;
        channelToken = "";
        JoinChannel();
    }
    void JoinChannel()
    {
        Debug.Log("Trying to join channel part2");
        if (channelToken.Length == 0)
        {
            Debug.Log("FetchingToken");
            StartCoroutine(HelperClass.FetchToken(tokenBase, channelName, 0, this.RenewOrJoinToken));
            return;
        }
        userid= Convert.ToUInt32(mRtcEngine.JoinChannelByKey(channelToken, channelName, userName, 0));
    }
    void OnJoinChannelSuccessHandler(string channelName, uint uid, int elapsed)
    {
        
        SceneManager.LoadScene("Video");
    }

    void OnLeaveChannelHandler(RtcStats stats)
    {
        
        DestroyVideoView(userid);
        SceneManager.LoadScene("Home");
    }

    void OnUserJoinedHandler(uint uid, int elapsed)
    {
        
        makeVideoView(uid);
    }

    void OnUserOfflineHandler(uint uid, USER_OFFLINE_REASON reason)
    {
        DestroyVideoView(uid);
    }

    void OnUserMuteVideoHandler(uint uid, bool muted)
    {
        Debug.Log("User muted");
        TurnOnLocalVideoStream(uid, !muted);
    }
    void OnTokenPrivilegeWillExpireHandler(string token)
    {
        StartCoroutine(HelperClass.FetchToken(tokenBase, channelName, 0, this.RenewOrJoinToken));
    }

    void OnConnectionStateChangedHandler(CONNECTION_STATE_TYPE state, CONNECTION_CHANGED_REASON_TYPE reason)
    {
        this.state = state;
    }

    
    void OnSDKWarningHandler(int warn, string msg)
    {
        Debug.Log("SDK warning");
        Debug.Log(warn);
    }
    
    
    void OnSDKErrorHandler(int error, string msg)
    {
        Debug.Log("SDK error");
        Debug.Log(error);
    }
    
    void OnConnectionLostHandler()
    {
        Debug.Log("Connection is lost");
        if(SceneManager.GetActiveScene().name == "Home")
        {
            HomeUIManager.Instance.loadingScreen.SetActive(false);
            Notifier.Instance.Notify("Lost Connection", "Please re-try");

        }
        
    }

    void OnApplicationQuit()
    {
        Debug.Log("OnApplicationQuit");
        OnChannelLeave();
        IRtcEngine.Destroy();
    }

    public void OnChannelLeave()
    {
        
        if (mRtcEngine != null)
        {
            mRtcEngine.LeaveChannel();
            mRtcEngine.DisableVideoObserver();
            
        }
    }
    private void DestroyVideoView(uint uid)
    {
        GameObject go = GameObject.Find(uid.ToString());
        if (!ReferenceEquals(go, null))
        {
            Destroy(go);
        }
    }

    public void TurnOnLocalVideoStream(uint uid, bool value)
    {
        GameObject go = GameObject.Find(uid.ToString());
        go.GetComponent<VideoPrefabHandler>().videoScreen.SetActive(value);        
    }
    public void TurnOnLocalAudioStream(uint uid, bool value)
    {
        GameObject go = GameObject.Find(uid.ToString());        
        go.GetComponent<VideoPrefabHandler>().OnAudioStateChange(value);
    }

    private void makeVideoView(uint uid)
    {
        GameObject go = GameObject.Find(uid.ToString());
        if (!ReferenceEquals(go, null))
        {
            return; // reuse
        }

        // create a GameObject and assign to this new user
        VideoSurface videoSurface = makeImageSurface(uid.ToString());
        if (!ReferenceEquals(videoSurface, null))
        {
            // configure videoSurface
            videoSurface.SetForUser(uid);
            videoSurface.SetEnable(true);
            videoSurface.SetVideoSurfaceType(AgoraVideoSurfaceType.RawImage);
            videoSurface.SetGameFps(30);
        }
    }

    
    public VideoSurface makeImageSurface(string goName)
    {
        Transform videoCanvas = GameObject.FindGameObjectWithTag("VideoCanvas").transform;
        GameObject go = GameObject.Instantiate(videoPrefab, videoCanvas);

        if (go == null)
        {
            return null;
        }

        go.name = goName;
        // to be renderered onto
        
        var videoPrefabScript = go.GetComponent<VideoPrefabHandler>();
        GameObject videoScreen = videoPrefabScript.videoScreen;




        // configure videoSurface
        VideoSurface videoSurface = videoScreen.AddComponent<VideoSurface>();
        return videoSurface;
    }
    
    
}
