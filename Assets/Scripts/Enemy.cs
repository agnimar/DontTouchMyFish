using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : Character
{
    public UIManager uIManager;
    public GameManager gameManager;
    public void Update()
    {
        if (ShouldHeal())
        {
            PerformHealAction();
            healingUsed = true;
            StartCoroutine(uIManager.DisplayEnemyHealingComment("Enemy healed"));
        }
        LogHealth();
    }

    private bool ShouldHeal()
    {
        return health <= 40 && !healingUsed && gameManager.currentState == GameState.ChoosingAction;
    }

    private void LogHealth()
    {
        Debug.Log($"Enemy health: {health}");
    }

    public void ChooseAction()
    {
        lastUsedAction = GetRandomAction();
        Debug.Log($"Enemy action: {lastUsedAction}");
        RegisterAction(lastUsedAction);
    }

    private Action GetRandomAction()
    {
        return (Action)Random.Range(0, 3);
    }

}