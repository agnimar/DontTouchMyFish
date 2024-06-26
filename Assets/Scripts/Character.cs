using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Character : MonoBehaviour
{
    public int health ;//{ get { return health; } private set {health = value; } } 
    public string name;
    public bool healingUsed;
    public Queue<Action> usedAttackQueue = new Queue<Action>();
    public Action lastUsedAction;

    void Start()
    {
        health = 100;
    }
    public void RegisterAction(Action action)
    {
        lastUsedAction = action;
        usedAttackQueue.Enqueue(action);
        LogQueue();
    }
    private void LogQueue()
    {
        string queueContents = "Current Attack Queue: ";
        foreach (var act in usedAttackQueue)
        {
            queueContents += act.ToString() + " ";
        }
        Debug.Log(queueContents);
    }
    public void ApplyDamage(int damage)
    {
        health -= damage;
    }
    /*public void PerformAction_HISS()
    {
        lastUsedAction = Action.HISS;
        RegisterAction(lastUsedAction);
    }
    public void PerformAction_PAWN()
    {
        lastUsedAction = Action.PAWN;
        RegisterAction(lastUsedAction);
    }
    public void PerformAction_STANCE()
    {
        lastUsedAction = Action.STANCE;
        RegisterAction(lastUsedAction);
    }*/

    public bool CheckIfStillAlive(int health)
    {
        if (health <= 0)
        {
            return false;
        }
        else 
        {
            return true;
        }
    }
    public void PerformAction_HEAL()
    {
        var healingPoints = Random.Range(15, 25);
        health += healingPoints;
    }
}
    
public enum Action
{
    HISS,
    PAWN,
    STANCE,
    HEAL
}