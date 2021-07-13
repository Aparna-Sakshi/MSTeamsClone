using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;


[System.Serializable]
public class Channel
{
    public string channelName;
    public string channelDate;
    public IDictionary<string, Message> messages = new Dictionary<string, Message>();
    public JSONNode Serialize()
    {

        var obj = new JSONObject();
        obj["channelName"] = channelName;
        obj["channelDate"] = channelDate;
        var dict = obj["messages"].AsObject;
        foreach (var msg in messages)
            dict[msg.Key] = msg.Value.Serialize();
        return obj;
    }
    public void Deserialize(JSONNode aNode)
    {

        channelName = aNode["channelName"];
        channelDate = aNode["channelDate"];
        messages = new Dictionary<string, Message>();
        foreach (var msg in aNode["messages"])
        {

            var message = new Message();
            message.Deserialize(msg.Value);
            messages[msg.Key] = message;
        }

    }



}

public class Message
{
    public string username;
    public string uid;
    public string content;
    public string timestamp;
    public JSONNode Serialize()
    {

        var obj = new JSONObject();
        obj["username"] = username;
        obj["uid"] = uid;
        obj["content"] = content;
        obj["timestamp"] = timestamp;
        
        return obj;
    }
    public void Deserialize(JSONNode aNode)
    {

        username = aNode["username"];
        uid = aNode["uid"];
        content = aNode["content"];
        timestamp = aNode["timestamp"];
        

    }

}

public class UserChannel
{
    public string channelName;
    public string channelDate;
    public string category;


    public JSONNode Serialize()
    {

        var obj = new JSONObject();
        obj["channelName"] = channelName;
        obj["channelDate"] = channelDate;
        obj["category"] = category;
        
        return obj;
    }
    public void Deserialize(JSONNode aNode)
    {

        channelName = aNode["channelName"];
        channelDate = aNode["channelDate"];
        category = aNode["category"];
        

    }
}
