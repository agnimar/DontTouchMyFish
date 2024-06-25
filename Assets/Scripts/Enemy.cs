using UnityEngine;

public class Enemy : Character 
{
    public void ChooseAction()
    {
        if (health <= 40 && !healingUsed)
        {
            PerformAction_HEAL();
            healingUsed = true;
            Debug.Log("Enemy Healed");
        }
        PerformAttack();
    }

    private void PerformAttack()
    {
        lastUsedAction = (Action)Random.Range(0, 3);
        Debug.Log("Enemy action: " + lastUsedAction);
        RegisterAction(lastUsedAction);
    }
}

