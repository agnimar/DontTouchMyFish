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
            StartCoroutine(ChoosingActionSequence());
        }
        else if(inputEnabled && (selectedAction == Action.PAWN || selectedAction == Action.HISS || selectedAction == Action.STANCE))
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
                          (playerAction == Action.STANCE && enemyAction == Action.PAWN) ||
                          (playerAction == Action.PAWN && enemyAction == Action.HISS);

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
        StartCoroutine(StartDamageSequence(enemy));
        currentState = enemy.CheckIfStillAlive(enemy.health) ? GameState.ChoosingAction : GameState.Victory;
    }

    private void HandleEnemyWinning()
    {
        StartCoroutine(StartDamageSequence(player));
        currentState = player.CheckIfStillAlive(player.health) ? GameState.ChoosingAction : GameState.Defeat;
    }

    private IEnumerator HandleDrawOutcome()
    {
        uiManager.DisplayComment("It's a draw!");
        yield return new WaitForSeconds(1.5f);
        StartCoroutine(ChoosingActionSequence());
    }
    #endregion

    #region Sequences
    private IEnumerator ChoosingActionSequence()
    {
        inputEnabled = true;
        uiManager.DisplayComment("Choose an action!");
        yield return new WaitForSeconds(1);
    }

    private IEnumerator ProcessFightSequence()
    {
        inputEnabled = false;
        string text = "Your action: " + player.lastUsedAction.ToString() + "\nEnemy action: " + enemy.lastUsedAction.ToString();
        uiManager.DisplayComment(text);
        yield return new WaitForSeconds(1);
        fightOutcome = ResolveFight(player.lastUsedAction, enemy.lastUsedAction);
    }
    public IEnumerator ProcessVictorySequence()
    {
        ProcessFightOutcomeUI("YOU WON!\nTHE ENEMY HAS BEEN DEFEATED");
        currentState = GameState.Victory;
        inputEnabled = false;

        yield return new WaitForSeconds(1);
    }

    public IEnumerator ProcessDefeatSequence()
    {
        ProcessFightOutcomeUI("YOU LOST!\nYOU HAVE BEEN DEFEATED");
        currentState = GameState.Defeat;
        inputEnabled = false;

        yield return new WaitForSeconds(1);
    }
    
    private void ProcessFightOutcomeUI(String text)
    {
        inputEnabled = false;
        uiManager.EnableFightOutcomeDisplay(true);
        uiManager.DisplayFightOutcomeComment(text);
    }
    private IEnumerator StartIntroSequence()
    {
        yield return new WaitForSeconds(1);
    }

    private IEnumerator StartDamageSequence(Character character)
    {
        int damage = UnityEngine.Random.Range(10, 20);
        if (damage > character.health)
            damage = character.health;
        character.ApplyDamage(damage);
        uiManager.DisplayDamageComment(character, damage);
        yield return new WaitForSeconds(1.5f);
        CheckIfFightEnds();
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

