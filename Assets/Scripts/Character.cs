using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class Character : MonoBehaviour
{
    // Public fields
    public int health;
    public bool healingUsed;
    public Queue<Action> usedAttackQueue = new Queue<Action>();
    public Action lastUsedAction;
    
    // UI elements
    public SpriteRenderer spriteRenderer;  
    public Sprite[] actionSprites;
    public Sprite roundWinSprite;
    public Sprite roundLoseSprite;

    // Constants
    private const int InitialHealth = 100;
    private const int MinHealingPoints = 15;
    private const int MaxHealingPoints = 25;

    void Start()
    {
        health = InitialHealth;
        healingUsed = false;
        SetSprite(Action.IDLE);
    }

    /*void Update()
    {

    }*/

    public void RegisterAction(Action action)
    {
        lastUsedAction = action;
        usedAttackQueue.Enqueue(action);
        SetSprite(action); 
        LogQueue();
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
    public void PerformHealAction()
    {
        int healingPoints = Random.Range(MinHealingPoints, MaxHealingPoints);
        health += healingPoints;
        Debug.Log($"Health after healing: {health}");
    }
    public void ResetToIdle()
    {
        SetSprite(Action.IDLE);
    }

    public void SetRoundWinSprite()
    {
        SetSprite(roundWinSprite);
    }

    public void SetRoundLoseSprite()
    {
        SetSprite(roundLoseSprite);
    }

    private void SetSprite(Sprite sprite)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = sprite;
        }
        else
        {
            Debug.LogError("SpriteRenderer is not assigned!");
        }
    }

    public void SetHealingActionSprite()
    {
        SetSprite(Action.HEAL);
    }

    // Overloaded private method to set action sprites
    private void SetSprite(Action action)
    {
        spriteRenderer.sprite = actionSprites[(int)action];
    }
    private void LogQueue()
    {
        string queueContents = "Current Attack Queue: ";
        foreach (var act in usedAttackQueue)
        {
            queueContents += $"{act} ";
        }
        Debug.Log(queueContents);
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