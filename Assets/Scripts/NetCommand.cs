using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class NetCommand
{
    public string id;

    public NetCommand(string i)
    {
        this.id = i;
    }

    public string ConvToString()
    {
        string s = "$NC";
        s += "&" + id;
        return s;
    }

    public static NetCommand ConvToCommand(string s)
    {
        if(s.Length < 3) { return null; }

        string commandType = s.Substring(1, 2);

        switch (commandType)
        {
            case ("NC"):
                NetCommand n = new NetCommand(s.Substring(4));
                return n;

            default:
                return null;
        }
    }
}


