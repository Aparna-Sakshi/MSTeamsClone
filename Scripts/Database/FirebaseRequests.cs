using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using SimpleJSON;
using UnityEngine;
using UnityEngine.SceneManagement;
//using Firebase.Unity.Editor;


public class FirebaseRequests : GenericSingletonClass<FirebaseRequests>
{
    private DatabaseReference reference;
    private EventHandler<ChildChangedEventArgs> newMessageListener;
    private EventHandler<ChildChangedEventArgs> editedMessageListener;
    private EventHandler<ChildChangedEventArgs> deletedMessageListener;
    Firebase.Auth.FirebaseUser user;


    private void Start()
    {
        
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        user = FirebaseAuth.DefaultInstance.CurrentUser;


    }

    public void GetUserMeetings(string uid, Action<Dictionary<string,UserChannel> > callback, Action<AggregateException> fallback)
    {
        reference.Child("Users").Child(uid).GetValueAsync().ContinueWithOnMainThread(task => {
          if (task.IsFaulted)
          {
                // Handle the error...
                fallback(task.Exception);
          }
          else if (task.IsCompleted)
          {
              DataSnapshot snapshot = task.Result;
                // Do something with snapshot...
                
                var userChannels = new Dictionary<string, UserChannel>();
                foreach (DataSnapshot child in snapshot.Children)
                {
                    //Process child here.
                    string key = child.Key;
                    string jsonValue = child.GetRawJsonValue();
                    var userChannel = new UserChannel();
                    userChannel.Deserialize(JSON.Parse(jsonValue));
                    userChannels.Add(key, userChannel);                    
                }               
                Debug.Log("getting user channels");                
                callback(userChannels);
          }
      });
    }
    public void JoinChannel(string channelId, string category, Action successCallback, Action failureCallback, Action<AggregateException> fallback)
    {
        reference.Child("Channels").Child(channelId).GetValueAsync().ContinueWithOnMainThread(task => {
            if (task.IsFaulted)
            {
                // Handle the error...
                fallback(task.Exception);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                // Do something with snapshot...

                
                if(snapshot.Exists)//Channel exists
                {
                    var userChannel = new UserChannel();
                    userChannel.channelName = snapshot.Child("channelName").Value as string;
                    userChannel.channelDate = snapshot.Child("channelDate").Value as string;
                    userChannel.category = category;
                    var jsonValue = userChannel.Serialize().ToString(3);
                    var path = $"Users/{user.UserId}/{snapshot.Key}";

                    //Post the data in Users/Uid/Channel_id
                    Put(jsonValue, path);
                    successCallback();
                }
                else//channel doesn't exist
                {
                    failureCallback();
                }
            }
        });
    }
    public void Post(string jsonValue, string path, Action<string> callback = null, Action<AggregateException> fallback = null)
    {
        
        DatabaseReference newChildRef = reference.Child(path).Push();
        newChildRef.SetRawJsonValueAsync(jsonValue).ContinueWithOnMainThread(task =>
        {
            
            if (task.IsCanceled || task.IsFaulted)
            {
                
                fallback(task.Exception);
            }                
            else
            {
                
                callback(newChildRef.Key);
            }
                
        });
    }

    public void Put(string jsonValue, string path, Action callback = null, Action<AggregateException> fallback = null)
    {        
        
        reference.Child(path).SetRawJsonValueAsync(jsonValue).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                fallback(task.Exception);
            }
            else
            {
                callback();
            }
                
            
        });
    }

    public void Delete(string path, Action callback=null, Action<AggregateException> fallback=null)
    {
        reference.Child(path).SetRawJsonValueAsync(null).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                fallback(task.Exception);
            }                
            else
            {
                callback();
            }
                
        });
    }

    public void ListenForNewMessages(string channelId, Action<string,Message> callback, Action<AggregateException> fallback)
    {
        Debug.Log("Added listener");
        void CurrentListener(object o, ChildChangedEventArgs args)
        {
            if (args.DatabaseError != null)
            {
                fallback(new AggregateException(new Exception(args.DatabaseError.Message)));
            }                
            else
            {
                Debug.Log("Call back called");
                var jsonValue = args.Snapshot.GetRawJsonValue();
                var message = new Message();
                message.Deserialize(JSON.Parse(jsonValue));
                var key = args.Snapshot.Key;
                callback(key,message);
            }
                
        }

        newMessageListener = CurrentListener;
        Debug.Log(channelId);
        reference.Child("Channels").Child(channelId).Child("messages").ChildAdded += newMessageListener;
    }

    public void ListenForEditedMessages(string channelId, Action<string, string> callback, Action<AggregateException> fallback)
    {
        void CurrentListener(object o, ChildChangedEventArgs args)
        {
            if (args.DatabaseError != null)
            {
                fallback(new AggregateException(new Exception(args.DatabaseError.Message)));
            }                
            else
            {
                var jsonValue = args.Snapshot.Child("content").GetRawJsonValue();
                
                var key = args.Snapshot.Key;
                callback(jsonValue, key);

            }
                
        }

        editedMessageListener = CurrentListener;

        reference.Child(channelId).Child("messages").ChildChanged += editedMessageListener;
    }

    public void ListenForDeletedMessages(string channelId, Action<string> callback, Action<AggregateException> fallback)
    {
        void CurrentListener(object o, ChildChangedEventArgs args)
        {
            if (args.DatabaseError != null)
            {
                fallback(new AggregateException(new Exception(args.DatabaseError.Message)));
            }
            else
            {
                callback(args.Snapshot.Key);
            }
            
        }

        deletedMessageListener = CurrentListener;

        reference.Child(channelId).Child("messages").ChildRemoved += deletedMessageListener;
    }

    public void StopListeningForMessages()
    {
        reference.Child("messages").ChildAdded -= newMessageListener;
        reference.Child("messages").ChildChanged -= editedMessageListener;
        reference.Child("messages").ChildRemoved -= deletedMessageListener;
    }

    public void Logout()
    {
        StopListeningForMessages();
        FirebaseAuth.DefaultInstance.SignOut();
        SceneManager.LoadScene("Intro");
    }

    

}
