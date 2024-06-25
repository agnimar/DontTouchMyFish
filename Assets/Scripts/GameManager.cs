using System;
using System.Collections;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class GameManager : MonoBehaviour
{
    public bool inputEnabled { get; private set; } 
    public GameState currentState {  get; private set; }
    public UIManager uiManager;
    public Player player;
    public Enemy enemy;
    private FightOutcome fightOutcome;

    // Start is called before the first frame update
    void Start()
    {
        currentState = GameState.ChoosingAction;
        StartCoroutine(ChoosingActionSequence());
    }

    // Update is called once per frame
    void Update()
    {
        if(currentState == GameState.ChoosingAction && inputEnabled)
        {
            HandleActionSelectionInput();
        }
    }
    private void HandleActionSelectionInput()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            SelectAction(player.lastUsedAction);
        }
    }

    public void SelectAction(Action selectedAction)
    {
        player.RegisterAction(selectedAction);
        StartFight();
    }
    private void HandleEnemyActionSelection()
    {
        enemy.ChooseAction();
    }
    private void StartFight()
    {
        currentState = GameState.Fighting;
        HandleEnemyActionSelection();
        StartCoroutine(ProcessFightSequence());

    }
    public void CheckIfFightEnds()
    {
        if(enemy.health <= 0)
        {
            currentState = GameState.Victory;
        }
        else if(player.health <= 0) 
        {
            currentState = GameState.Defeat;
        }
        else { }
    }
    private IEnumerator ProcessFightSequence()
    {
        inputEnabled = false;
        string text = "Your action: " + player.lastUsedAction.ToString() + "\nEnemy action: " + enemy.lastUsedAction.ToString();
        uiManager.DisplayComment(text);
        yield return new WaitForSeconds(1);
        fightOutcome = ResolveFight(player.lastUsedAction, enemy.lastUsedAction);
        /*uiManager.DisplayFightOutcome(fightOutcome);*/
    }
    private IEnumerator ChoosingActionSequence()
    {
        uiManager.DisplayComment("Choose an action!");
        yield return new WaitForSeconds(1);

        currentState = GameState.ChoosingAction;
        inputEnabled = true;
    }
    private IEnumerator StartIntroSequence()
    {
        yield return new WaitForSeconds(1);
    }
    private IEnumerator StartDamageSequence(Character character)
    {
        int damage = UnityEngine.Random.Range(10, 20);
        character.ApplyDamage(damage);
        if(character == enemy)
        {
            uiManager.DisplayComment("Good job!\nDamage dealth to the enemy: " + damage.ToString());
        }
        else
        {
            uiManager.DisplayComment("Oh no!\nDamage dealth to you: " + damage.ToString());
        }
        yield return new WaitForSeconds(1.5f);
        StartCoroutine(ChoosingActionSequence());
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
        if (enemy.CheckIfStillAlive(enemy.health))
        {
            currentState = GameState.ChoosingAction;
        }
        else
        {
            currentState = GameState.Victory;
        }
    }
    private void HandleEnemyWinning()
    {
        StartCoroutine(StartDamageSequence(player));
        if (player.CheckIfStillAlive(player.health))
        {
            currentState = GameState.ChoosingAction;
        }
        else
        {
            currentState = GameState.Defeat;
        }
    }

  
    private IEnumerator HandleDrawOutcome()
    {
        uiManager.DisplayComment("It's a draw!");
        currentState = GameState.ChoosingAction;
        StartCoroutine(ChoosingActionSequence());
        yield return new WaitForSeconds(1.5f);
        StartCoroutine(ChoosingActionSequence());
    }
}

public enum GameState
{
    Intro,
    ChoosingAction,
    Fighting,
    Victory,
    Defeat
}

public enum FightOutcome
{
    Win,
    Lose,
    Draw
}