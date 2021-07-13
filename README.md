# MS Teams Clone
In this project an android app was built to do group video call and chat. 
## Features
*  **Register and Login**
*  **Permission to allow microphone and camera**
*   **Side Menu**
    *  Users can see their profile information on the top
    *  Meet : Here users can create or join new groups
    *  Chat : Here users can view their previously created groups and chat messages
    *  Logout : Users can logout of their accounts
*  **Create Meeting** (Create a new group)
    *  Enter group chat after creating meeting
    *  Enter video call after creating meeting
    *  Copy the auto generated meeting id to be shared among users
*  **Join Meeting** (Join a new group using meeting id)
    *  Join and enter group chat
    *  Join and enter video call

*  **Video Call**
    *  Multiple users can join the video call and chat
    *  Mute/Unmute audio
    *  Turn on/off video
    *  In meeting chat (users can continue this chat later also)
    *  Leave meeting
    
*  **View Groups and Chats**
    *  See previously created groups
    *  Share (Copy) the meeting id of the group so that new users can join the group
    *  Chat within the group (chat without joining the video call) : Users can chat before and after the meeting.
    *  Video call from the group
    *  Exit the group
    *  View groups based on 3 different categories namely:
        *  General
        *  Social
        *  Personal
*  **Notifications**
    *  Passwords don't match while registering, wrong password, account doesn't exist etc (during register/login)
    *  When the meeting id entered by user doesn't exist (while joining the group)
    *  On copying the meeting id
    *  When meeting agenda/meeting id /send message field is empty
*  **Confirmation**
    *  When user is wants to exit the group
    *  When user wants to proceed to video call from the group

## Guide on how to use the app
Download and install the app from releases in the master branch     

[Link for presentation](https://docs.google.com/presentation/d/1k9WMyYePS_0XulikQxLRjx8XyfsEsFPsNZZhHQEuRHk/edit?usp=sharing)

[How I used Agile](https://docs.google.com/presentation/d/189jRZ_OsqO5eqCRaRYoL12MdK4-YzNerBIeE5x7HM3g/edit?usp=sharing)

#### Demo Videos:
[Register and login](https://youtu.be/iIkjG7ZzMn4)

[Create Meeting](https://youtu.be/A-QE4rc4Rck)

[View previously created groups](https://youtu.be/SFbDc241jK0)

[Create and Join group chat](https://youtu.be/MCZr8VVPzeM)

[Multiple users joining group chat](https://youtu.be/6qZQPqvy51g)

[Meet even after meeting ends](https://youtu.be/XUXsZBrvZMs)

[Join an existing group chat](https://youtu.be/0CWSO0c7Rxc)


## Frameworks/ SDKs/ Dependencies used to create app

* Unity
* Visual Studio (C#)
* Agora video call SDK
* Firebase authentication and realtime database

## Note
* When entering the meeting id don't enter any extra character including space.
* Scroll down in the chat while viewing the new messages
* Although unlimited number of users can join the call, agora documentation recommends that number of users joining the call should be less than 7 for better performance.
* The username given will be displayed to other users while chatting.






