using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : Character
{
    public UIManager uIManager;
    public GameManager gameManager;
    public void Update()
    {
        if (health <= 40 && !healingUsed && gameManager.currentState == GameState.ChoosingAction)
        {
            PerformAction_HEAL();
            healingUsed = true;
            StartCoroutine(uIManager.DisplayEnemyHealingComment("Enemy healed"));
        }
        Debug.Log("Enemy health " + health);
    }

    public void ChooseAction()
    {
        lastUsedAction = (Action)Random.Range(0, 3);
        Debug.Log("Enemy action: " + lastUsedAction);
        RegisterAction(lastUsedAction);
    }
}