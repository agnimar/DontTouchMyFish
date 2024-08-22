using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class Character : MonoBehaviour
{
    public int health;
    public bool healingUsed;
    public Queue<Action> usedAttackQueue = new Queue<Action>();
    public Action lastUsedAction;

    public SpriteRenderer spriteRenderer;  
    public Sprite[] actionSprites;
    public Sprite roundWinSprite;
    public Sprite roundLoseSprite;

    void Start()
    {
        health = 100;
        healingUsed = false;
        SetActionSprite(Action.IDLE);
    }

    void Update()
    {

    }

    public void RegisterAction(Action action)
    {
        lastUsedAction = action;
        usedAttackQueue.Enqueue(action);
        SetActionSprite(action); 
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
        return health > 0;
    }
    public void PerformAction_HEAL()
    {
        var healingPoints = Random.Range(15, 25);
        health += healingPoints;
        Debug.Log("health after healing" + health);
    }
    public void SetActionSprite(Action action)
    {
        spriteRenderer.sprite = actionSprites[(int)action];
    }
    public void ResetToIdle()
    {
        SetActionSprite(Action.IDLE);
    }
    public void SetRoundWinSprite()
    {
        spriteRenderer.sprite = roundWinSprite;
    }

    public void SetRoundLoseSprite()
    {
        spriteRenderer.sprite = roundLoseSprite;
    }

}

public enum Action
{
    HISS,
    PAW,
    STANCE,
    HEAL,
    IDLE
}