using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class Character : MonoBehaviour
{
    public int health;
    public string name;
    public bool healingUsed;
    public Queue<Action> usedAttackQueue = new Queue<Action>();
    public Action lastUsedAction;

    void Start()
    {
        health = 100;
        healingUsed = false;
    }

    void Update()
    {
        
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
        Debug.Log($"{GetType().Name} has {health} health remaining after taking {damage} damage.");
    }
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
        Debug.Log("health after healing" + health);
    }
}
    
public enum Action
{
    HISS,
    PAWN,
    STANCE,
    HEAL
}