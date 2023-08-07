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

    public virtual string ConvToString()
    {
        string s = "$NC";
        s += "&" + id;
        return s;
    }

    public static NetCommand ConvToCommand(string s)
    {
        if(s.Length < 3) { return null; }

        string commandType = s.Substring(1, 2);
        string[] splitCommand = s.Split('&');

        switch (commandType)
        {
            case ("NC"): // Base

                NetCommand nc = new NetCommand(splitCommand[1]);
                return nc;

            case ("SP"): // Spawn

                Enum.TryParse<EntityType>(splitCommand[2], out EntityType sp_e);
                Vector3 sp_p = new Vector3(float.Parse(splitCommand[3]), float.Parse(splitCommand[4]), float.Parse(splitCommand[5]));
                SpawnCommand sp = new SpawnCommand(splitCommand[1], sp_e, sp_p, float.Parse(splitCommand[6]));
                return sp;

            case ("WP"): // Add Waypoint

                Vector3 wp_p = new Vector3(float.Parse(splitCommand[2]), float.Parse(splitCommand[3]), float.Parse(splitCommand[4]));
                List<int> wp_i = new List<int>();
                for(int wp_x = 6; wp_x < splitCommand.Length; wp_x++)
                {
                    wp_i.Add(int.Parse(splitCommand[wp_x]));
                }
                WaypointCommand wp = new WaypointCommand(splitCommand[1], wp_p, bool.Parse(splitCommand[5]), wp_i);
                return wp;

            case ("DH"): // Desired Heading
                DesiredHeadingCommand dh = new DesiredHeadingCommand(splitCommand[1], float.Parse(splitCommand[2]), int.Parse(splitCommand[3]));
                return dh;

            case ("DS"): // Desired Speed
                DesiredSpeedCommand ds = new DesiredSpeedCommand(splitCommand[1], float.Parse(splitCommand[2]), int.Parse(splitCommand[3]));
                return ds;

            default:
                return null;
        }
    }
}

[Serializable]
public class SpawnCommand : NetCommand
{
    public EntityType type;
    public Vector3 pos;
    public float heading;

    public SpawnCommand(string i, EntityType e, Vector3 p, float h) : base(i)
    {
        this.type = e;
        this.pos = p;
        this.heading = h;
    }

    public override string ConvToString()
    {
        string s = "$SP";
        s += "&" + id;
        s += "&" + type.ToString();
        s += "&" + pos.x.ToString() + "&" + pos.y.ToString() + "&" + pos.z.ToString();
        s += "&" + heading.ToString();
        return s;
    }
}

[Serializable]
public class WaypointCommand : NetCommand
{
    public Vector3 pos;
    public bool clear; // Whether to clear the list
    public List<int> entityIDs; // IDs of the entities to move

    public WaypointCommand(string i, Vector3 p, bool c, List<int> e) : base(i)
    {
        this.pos = p;
        this.clear = c;
        this.entityIDs = e;
    }

    public override string ConvToString()
    {
        string s = "$WP";
        s += "&" + id;
        s += "&" + pos.x.ToString() + "&" + pos.y.ToString() + "&" + pos.z.ToString();
        s += "&" + clear.ToString();

        foreach(int en in entityIDs)
        {
            s += "&" + en.ToString();
        }

        return s;
    }
}

[Serializable]
public class DesiredHeadingCommand : NetCommand
{
    public float heading;
    public int entityID;

    public DesiredHeadingCommand(string i, float h, int e) : base(i)
    {
        this.heading = h;
        this.entityID = e;
    }

    public override string ConvToString()
    {
        string s = "$DH";
        s += "&" + id;
        s += "&" + heading.ToString();
        s += "&" + entityID.ToString();
        return s;
    }
}

[Serializable]
public class DesiredSpeedCommand : NetCommand
{
    public float speed;
    public int entityID;

    public DesiredSpeedCommand(string i, float s, int e) : base(i)
    {
        this.speed = s;
        this.entityID = e;
    }

    public override string ConvToString()
    {
        string s = "$DS";
        s += "&" + id;
        s += "&" + speed.ToString();
        s += "&" + entityID.ToString();
        return s;
    }
}


