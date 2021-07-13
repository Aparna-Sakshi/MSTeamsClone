using Michsky.UI.ModernUIPack;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Notifier : MonoBehaviour
{
    //Singleton
    private static Notifier instance = null;
    public static Notifier Instance
    {
        get
        {
            if (instance == null)
                instance = GameObject.FindObjectOfType(typeof(Notifier)) as Notifier;
            return instance;
        }
    }



    //Notification Manager
    public NotificationManager myNotification;
    //public ModalWindowManager myModalWindow;


    public void Notify(string title, string description)
    {

        myNotification.title = title; // Change title
        myNotification.description = description; // Change desc        
        myNotification.OpenNotification(); // Open notification

    }

    /*
    void Window()
    {
        
        myModalWindow.titleText = ""; // Change title
        myModalWindow.descriptionText = “New Description”; // Change desc
        myModalWindow.UpdateUI(); // Update UI
        myModalWindow.OpenWindow(); // Open window
        myModalWindow.CloseWindow(); // Close window
        myModalWindow.AnimateWindow(); // Close/Open window automatically
    }
    */


}
