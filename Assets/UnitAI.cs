using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAI
{
    public Entity381 entity; //public only for ease of debugging
    // Start is called before the first frame update
    public UnitAI(Entity381 ent)
    {
        entity = ent;
        commands = new List<Command>();
        intercepts = new List<Intercept>();
        moves = new List<Move>();
    }

    public List<Move> moves;
    public List<Command> commands;
    public List<Intercept> intercepts;

    public void OnUpdate(float dt)
    {
        if (commands.Count > 0)
        {
            if (commands[0].IsDone())
            {
                StopAndRemoveCommand(0);
            }
            else
            {
                commands[0].Tick();
                commands[0].isRunning = true;
            }
        }
    }

    void StopAndRemoveCommand(int index)
    {
        commands[index].Stop();
        commands.RemoveAt(index);
    }
    
    public void StopAndRemoveAllCommands()
    {
        for(int i = commands.Count - 1; i >= 0; i--) {
            StopAndRemoveCommand(i);
        }
    }

    public void AddCommand(Command c)
    {
        //print("Adding command; " + c.ToString());
        c.Init();
        commands.Add(c);
        if (c is Intercept)
            intercepts.Add(c as Intercept);
        else if (c is Follow)
            ;
        else
            moves.Add(c as Move);
    }

    public void SetCommand(Command c)
    {
        //print("Setting command: " + c.ToString());
        StopAndRemoveAllCommands();
        commands.Clear();
        moves.Clear();
        intercepts.Clear();
        AddCommand(c);

    }
    //---------------------------------

}
