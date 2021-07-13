using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VideoPrefabHandler : MonoBehaviour
{
    public GameObject videoScreen;
    public Text userName;

    public GameObject audioOff;
    public GameObject audioOn;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnAudioStateChange(bool value)
    {
        if(value)
        {
            audioOn.SetActive(true);
            audioOff.SetActive(false);
        }
        else
        {
            audioOn.SetActive(false);
            audioOff.SetActive(true);

        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
