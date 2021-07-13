using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VideoCallUI : MonoBehaviour
{
    [SerializeField] private GameObject videoOn;
    [SerializeField] private GameObject videoOff;

    [SerializeField] private GameObject audioOn;
    [SerializeField] private GameObject audioOff;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void VideoOffClick()
    {
        //AgoraManager.Instance.mRtcEngine.MuteLocalVideoStream(true);
        //AgoraManager.Instance.mRtcEngine.DisableVideo();
        AgoraManager.Instance.mRtcEngine.EnableLocalVideo(false);
        AgoraManager.Instance.TurnOnLocalVideoStream(AgoraManager.Instance.userid,false);

        videoOff.SetActive(false);
        videoOn.SetActive(true);
    }
    
    public void VideoOnClick()
    {

        //AgoraManager.Instance.mRtcEngine.MuteLocalVideoStream(false);
        //AgoraManager.Instance.mRtcEngine.EnableVideo();
        AgoraManager.Instance.mRtcEngine.EnableLocalVideo(true);
        AgoraManager.Instance.TurnOnLocalVideoStream(AgoraManager.Instance.userid,true);
        videoOff.SetActive(true);
        videoOn.SetActive(false);
    }

    public void AudioOffClick()
    {
        AgoraManager.Instance.mRtcEngine.EnableLocalAudio(false);
        AgoraManager.Instance.TurnOnLocalAudioStream(AgoraManager.Instance.userid, false);
        //AgoraManager.Instance.mRtcEngine.MuteLocalAudioStream(true);
        audioOff.SetActive(false);
        audioOn.SetActive(true);

        
    }
    public void AudioOnClick()
    {
        AgoraManager.Instance.mRtcEngine.EnableLocalAudio(true);
        AgoraManager.Instance.TurnOnLocalAudioStream(AgoraManager.Instance.userid, true);
        //AgoraManager.Instance.mRtcEngine.MuteLocalAudioStream(false);
        audioOff.SetActive(true);
        audioOn.SetActive(false);
    }

    public void LeaveCallClick()
    {
        AgoraManager.Instance.OnChannelLeave();
        
    }
}
