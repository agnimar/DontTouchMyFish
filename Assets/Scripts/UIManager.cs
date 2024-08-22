using System.Collections;
using TMPro;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("UI Elements")]
    public UnityEngine.UI.Button hissButton;
    public UnityEngine.UI.Button pawButton;
    public UnityEngine.UI.Button stanceButton;
    public UnityEngine.UI.Button healButton;
    private UnityEngine.UI.Button[] buttons;
    public UnityEngine.UI.Button playAgainButton;
    public UnityEngine.UI.Button quitToMenuButton;
    public UnityEngine.UI.Button exitGameButton;

    [Header("Text fields")]
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI playerStats;
    public TextMeshProUGUI enemyStats;
    public TextMeshProUGUI fightOutcomeText;

    [Header("Panels")]
    public GameObject fightOutcomePanel;
    public GameObject actionChoicePanel;
    public GameObject enemyStatsPanel;
    public GameObject playerStatsPanel;

    [Header("Game Manager")]
    public GameManager gameManager;

    [Header("Characters")]
    public Player player;
    public Enemy enemy;

    void Start()
    {
        InitializeButtons();
        EnableFightOutcomeDisplay(false);
        EnableFightDisplay(true);
    }
    private void Update()
    {
        UpdateButtonInteractivity();
        UpdateStatsDisplay();
    }
    public void InitializeButtons()
    {
        buttons = new UnityEngine.UI.Button[] { hissButton, pawButton, stanceButton, healButton };
        foreach (var button in buttons)
        {
            button.onClick.AddListener(() => gameManager.SelectAction((Action)System.Array.IndexOf(buttons, button)));
        }
        // Add listeners for end game buttons
        playAgainButton.onClick.AddListener(PlayAgain);
        quitToMenuButton.onClick.AddListener(QuitToMenu);
        exitGameButton.onClick.AddListener(ExitGame);
    }
    private void UpdateButtonInteractivity()
    {
        bool enableButtons = gameManager.inputEnabled;
        ToggleButtons(enableButtons);
    }

    private void ToggleButtons(bool state)
    {
        foreach (var button in buttons)
        {
            button.interactable = state;
        }
    }

    private void UpdateStatsDisplay()
    {
        playerStats.text = player.health.ToString();
        enemyStats.text = enemy.health.ToString();
    }
    public void EnableFightOutcomeDisplay(bool isActive)
    {
        fightOutcomePanel.SetActive(isActive);
    }
    public void EnableFightDisplay(bool isActive)
    {
        playerStatsPanel.SetActive(isActive);
        actionChoicePanel.SetActive(isActive);
        enemyStatsPanel.SetActive(isActive);
    }
    public void DisplayComment(string comment)
    {
        dialogueText.text = comment;
    }
    public void DisplayFightOutcomeComment(string comment)
    {
        fightOutcomeText.text = comment;
    }
    public void DisplayDamageComment(Character character, int damage)
    {
        if (character == enemy)
        {
            DisplayComment($"Good job!\nDamage dealt to the enemy: {damage}");
        }
        else
        {
            DisplayComment($"Oh no!\nDamage dealt to you: {damage}");
        }
    }
    public IEnumerator DisplayEnemyHealingComment(string comment)
    {
        dialogueText.text = comment;
        enemy.SetActionSprite(Action.HEAL);
        yield return new WaitForSeconds(1.5f);
    }
    // End game button methods
    private void PlayAgain()
    {
        // Reset game logic, could involve reloading the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void QuitToMenu()
    {
        // Load main menu scene, replace "MainMenu" with the actual main menu scene name
        SceneManager.LoadScene("MainMenuScene");
    }

    private void ExitGame()
    {
        // Quit the application
        Application.Quit();
    }
}