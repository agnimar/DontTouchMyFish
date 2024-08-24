using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Managers")]
    public UIManager uiManager;

    [Header("Characters")]
    public Player player;
    public Enemy enemy;

    public bool inputEnabled { get; private set; }
    public GameState currentState { get; private set; }
    
    // Private fields
    private FightOutcome fightOutcome;
    private const float WAIT_TIME = 0.8f;
    private const float REDUCTION = 0.5f;

    #region Unity Lifecycle Methods
    void Start()
    {
        currentState = GameState.ChoosingAction;
        ReturnToChoosingActionState();
    }

    void Update()
    {

    }
    #endregion

    #region Action Handling

    public void SelectAction(Action selectedAction)
    {
        if (!inputEnabled) return;

        switch (selectedAction)
        {
            case Action.HEAL:
                PerformHealing();
                break;

            case Action.PAW:
            case Action.HISS:
            case Action.STANCE:
                PerformAttackAction(selectedAction);
                break;

            default:
                Debug.LogWarning("Selected action is not recognized.");
                break;
        }
    }
    private void PerformHealing()
    {
        player.PerformHealAction();
        Debug.Log("Health after healing in GM: " + player.health);
        StartCoroutine(ProcessHealingAnimation(player));
    }
    private void PerformAttackAction(Action selectedAction)
    {
        player.RegisterAction(selectedAction);
        StartFight();
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
            HandleVictory();
        }
        else if (player.health <= 0)
        {
            HandleDefeat();
        }
        else
        {
            HandleContinueFight();
        }
    }

    private void HandleVictory()
    {
        currentState = GameState.Victory;
        StartCoroutine(ProcessVictorySequence());
    }

    private void HandleDefeat()
    {
        currentState = GameState.Defeat;
        StartCoroutine(ProcessDefeatSequence());
    }
    private void HandleContinueFight()
    {
        currentState = GameState.ChoosingAction;
        ReturnToChoosingActionState();
    }
    public FightOutcome ResolveFight(Action playerAction, Action enemyAction)
    {
        if (playerAction == enemyAction)
        {
            StartCoroutine(HandleDrawOutcome());
            return FightOutcome.Draw;
        }

        if (IsPlayerWinner(playerAction, enemyAction))
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

    private bool IsPlayerWinner(Action playerAction, Action enemyAction)
    {
        return (playerAction == Action.HISS && enemyAction == Action.STANCE) ||
               (playerAction == Action.STANCE && enemyAction == Action.PAW) ||
               (playerAction == Action.PAW && enemyAction == Action.HISS);
    }

    private void HandlePlayerWinning()
    {
        StartCoroutine(StartDamageSequence(player, enemy));
        currentState = enemy.CheckIfStillAlive(enemy.health) ? GameState.ChoosingAction : GameState.Victory;
    }

    private void HandleEnemyWinning()
    {
        StartCoroutine(StartDamageSequence(enemy, player));
        currentState = player.CheckIfStillAlive(player.health) ? GameState.ChoosingAction : GameState.Defeat;
    }

    private IEnumerator HandleDrawOutcome()
    {
        uiManager.DisplayComment("It's a draw!");
        yield return new WaitForSeconds(WAIT_TIME);
        ReturnToChoosingActionState();
    }
    #endregion

    #region Sequences
    private void ReturnToChoosingActionState()
    {
        inputEnabled = true;
        uiManager.DisplayComment("Choose an action!");
        ResetCharactersToIdle();
    }

    private void ResetCharactersToIdle()
    {
        player.ResetToIdle();
        enemy.ResetToIdle();
    }

    private IEnumerator ProcessFightSequence()
    {
        inputEnabled = false;
        string text = "Your action: " + player.lastUsedAction.ToString() + "\nEnemy action: " + enemy.lastUsedAction.ToString();
        uiManager.DisplayComment(text);
        yield return new WaitForSeconds(WAIT_TIME);
        fightOutcome = ResolveFight(player.lastUsedAction, enemy.lastUsedAction);

    }
    public IEnumerator ProcessVictorySequence()
    {
        ProcessFightOutcomeUI("YOU WON!\nTHE ENEMY HAS BEEN DEFEATED");
        currentState = GameState.Victory;
        inputEnabled = false;

        yield return new WaitForSeconds(WAIT_TIME);
    }

    public IEnumerator ProcessDefeatSequence()
    {
        ProcessFightOutcomeUI("YOU LOST!\nYOU HAVE BEEN DEFEATED");
        currentState = GameState.Defeat;
        inputEnabled = false;

        yield return new WaitForSeconds(WAIT_TIME);
    }

    private void ProcessFightOutcomeUI(String text)
    {
        inputEnabled = false;
        uiManager.EnableFightOutcomeDisplay(true);
        uiManager.DisplayFightOutcomeComment(text);
    }
    private IEnumerator StartIntroSequence()
    {
        yield return new WaitForSeconds(WAIT_TIME);
    }

    private IEnumerator StartDamageSequence(Character winningCharacter, Character losingCharacter)
    {
        int damage = CalculateDamage(losingCharacter);
        SetRoundOutcomeSprites(winningCharacter, losingCharacter);
        losingCharacter.ApplyDamage(damage);
        uiManager.DisplayDamageComment(losingCharacter, damage);
        yield return new WaitForSeconds(WAIT_TIME);
        CheckIfFightEnds();
    }
    private void SetRoundOutcomeSprites(Character winningCharacter, Character losingCharacter)
    {
        winningCharacter.SetRoundWinSprite();
        losingCharacter.SetRoundLoseSprite();
    }
    private int CalculateDamage(Character character)
    {
        int maxDamage = Mathf.Min(UnityEngine.Random.Range(10, 20), character.health);
        return maxDamage;
    }
    private IEnumerator ProcessHealingAnimation(Character character)
    {
        character.SetHealingActionSprite();
        yield return new WaitForSeconds(WAIT_TIME*REDUCTION);
        ReturnToChoosingActionState();
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