using TMPro;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    [Header("UI Elements")]
    public UnityEngine.UI.Button hissButton;
    public UnityEngine.UI.Button pawnButton;
    public UnityEngine.UI.Button stanceButton;
    public UnityEngine.UI.Button healButton;
    private UnityEngine.UI.Button[] buttons;

    [Header("Text fields")]
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI playerStats;
    public TextMeshProUGUI enemyStats;

    [Header("Game Manager")]
    public GameManager gameManager;

    [Header("Characters")]
    public Player player;
    public Enemy enemy;
    // Start is called before the first frame update
    void Start()
    {
        // Assign button click listeners
        InitializeButtons();
    }

    private void Update()
    {
        if (gameManager.inputEnabled)
        {
            ToggleButtons(true);
        }
        else
        {
            ToggleButtons(false);
        }
        DisplayStats();
    }

    public void InitializeButtons()
    {
        buttons = new UnityEngine.UI.Button[] { hissButton, pawnButton, stanceButton, healButton };
        foreach (var button in buttons)
        {
            button.onClick.AddListener(() => gameManager.SelectAction((Action)System.Array.IndexOf(buttons, button)));
        }
    }

    public void DisplayStats()
    {
        playerStats.text = player.health.ToString();
        enemyStats.text = enemy.health.ToString();
    }
    public void DisplayComment(string comment)
    {
        //StartCoroutine(MovePanel(showPosition.anchoredPosition));
        dialogueText.text = comment;
        //StartCoroutine(HideCommentAfterDelay());
    }
    private void ToggleButtons(bool state)
    {
        foreach (var button in buttons)
        {
            button.interactable = state;
        }
    }
}
