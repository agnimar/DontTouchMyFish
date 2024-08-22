using System;
using System.Collections;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class GameManager : MonoBehaviour
{
    public bool inputEnabled { get; private set; }
    public GameState currentState { get; private set; }
    public UIManager uiManager;
    [Header("Characters")]
    public Player player;
    public Enemy enemy;
    private FightOutcome fightOutcome;

    #region Unity Lifecycle Methods
    void Start()
    {
        currentState = GameState.ChoosingAction;
        StartCoroutine(ChoosingActionSequence());
    }

    void Update()
    {

    }
    #endregion

    #region Action Handling

    public void SelectAction(Action selectedAction)
    {
        if (inputEnabled && selectedAction == Action.HEAL)
        {
            player.PerformAction_HEAL();
            Debug.Log("health after healing in GM" + player.health);
            StartCoroutine(ProcessHealingAnimation(player));
            StartCoroutine(ChoosingActionSequence());
        }
        else if (inputEnabled && (selectedAction == Action.PAW || selectedAction == Action.HISS || selectedAction == Action.STANCE))
        {
            player.RegisterAction(selectedAction);
            StartFight();
        }
    }

    private void HandleEnemyActionSelection()
    {
        enemy.ChooseAction();
    }
    #endregion

    #region Fight Handling
    private void StartFight()
    {
        currentState = GameState.Fighting;
        HandleEnemyActionSelection();
        StartCoroutine(ProcessFightSequence());
    }

    public void CheckIfFightEnds()
    {
        if (enemy.health <= 0)
        {
            currentState = GameState.Victory;
            StartCoroutine(ProcessVictorySequence());
        }
        else if (player.health <= 0)
        {
            currentState = GameState.Defeat;
            StartCoroutine(ProcessDefeatSequence());
        }
        else
        {
            currentState = GameState.ChoosingAction;
            StartCoroutine(ChoosingActionSequence());
        }
    }

    public FightOutcome ResolveFight(Action playerAction, Action enemyAction)
    {
        bool playerWins = (playerAction == Action.HISS && enemyAction == Action.STANCE) ||
                          (playerAction == Action.STANCE && enemyAction == Action.PAW) ||
                          (playerAction == Action.PAW && enemyAction == Action.HISS);

        if (playerAction == enemyAction)
        {
            StartCoroutine(HandleDrawOutcome());
            return FightOutcome.Draw;
        }
        else
        {
            if (playerWins)
            {
                HandlePlayerWinning();
                return FightOutcome.Win;
            }
            else
            {
                HandleEnemyWinning();
                return FightOutcome.Lose;
            }
        }
    }

    private void HandlePlayerWinning()
    {
        StartCoroutine(StartDamageSequence(enemy, player));
        currentState = enemy.CheckIfStillAlive(enemy.health) ? GameState.ChoosingAction : GameState.Victory;
    }

    private void HandleEnemyWinning()
    {
        StartCoroutine(StartDamageSequence(player, enemy));
        currentState = player.CheckIfStillAlive(player.health) ? GameState.ChoosingAction : GameState.Defeat;
    }

    private IEnumerator HandleDrawOutcome()
    {
        uiManager.DisplayComment("It's a draw!");
        yield return new WaitForSeconds(1f);
        StartCoroutine(ChoosingActionSequence());
    }
    #endregion

    #region Sequences
    private IEnumerator ChoosingActionSequence()
    {
        yield return new WaitForSeconds(0.3f);
        inputEnabled = true;
        uiManager.DisplayComment("Choose an action!");

        player.ResetToIdle();
        enemy.ResetToIdle();
    }

    private IEnumerator ProcessFightSequence()
    {
        inputEnabled = false;
        string text = "Your action: " + player.lastUsedAction.ToString() + "\nEnemy action: " + enemy.lastUsedAction.ToString();
        uiManager.DisplayComment(text);
        yield return new WaitForSeconds(1f);
        fightOutcome = ResolveFight(player.lastUsedAction, enemy.lastUsedAction);

    }
    public IEnumerator ProcessVictorySequence()
    {
        ProcessFightOutcomeUI("YOU WON!\nTHE ENEMY HAS BEEN DEFEATED");
        currentState = GameState.Victory;
        inputEnabled = false;

        yield return new WaitForSeconds(1f);
    }

    public IEnumerator ProcessDefeatSequence()
    {
        ProcessFightOutcomeUI("YOU LOST!\nYOU HAVE BEEN DEFEATED");
        currentState = GameState.Defeat;
        inputEnabled = false;

        yield return new WaitForSeconds(1f);
    }

    private void ProcessFightOutcomeUI(String text)
    {
        inputEnabled = false;
        uiManager.EnableFightOutcomeDisplay(true);
        uiManager.DisplayFightOutcomeComment(text);
    }
    private IEnumerator StartIntroSequence()
    {
        yield return new WaitForSeconds(1f);
    }

    private IEnumerator StartDamageSequence(Character winningCharacter, Character losingCharacter)
    {
        int damage = UnityEngine.Random.Range(10, 20);
        if (damage > winningCharacter.health)
            damage = winningCharacter.health;

        winningCharacter.SetRoundWinSprite();
        losingCharacter.SetRoundLoseSprite();
        winningCharacter.ApplyDamage(damage);
        uiManager.DisplayDamageComment(winningCharacter, damage);
        yield return new WaitForSeconds(0.5f);
        CheckIfFightEnds();
    }
    private IEnumerator ProcessHealingAnimation(Character character)
    {
        character.SetActionSprite(Action.HEAL);
        yield return new WaitForSeconds(0.3f);
    }
    #endregion

}

#region Enums

public enum GameState
{
    Intro,
    ChoosingAction,
    Fighting,
    Victory,
    Defeat,
    Healing
}

public enum FightOutcome
{
    Win,
    Lose,
    Draw
}

#endregion