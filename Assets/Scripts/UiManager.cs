using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{

    public static UiManager Instance { get; private set; }

    public Player playerMovement => Player.Instance;

    public Slider lpSlider;
    public Color selectedTextColor;

    GameObject gameUi;
    GameObject endUi;
    GameObject menuUi;
    GameObject menuPanel;
    GameObject levelSelectionPanel;
    Text fullScreenText;

    [Header("Movement")]
    [SerializeField] private EventTrigger moveLeft;
    [SerializeField] private EventTrigger moveRight;
    [SerializeField] private EventTrigger jumpButton;

    [Header("Extra Actions")]
    [SerializeField] private EventTrigger buttonE;
    [SerializeField] private EventTrigger buttonF;
    [SerializeField] private EventTrigger buttonR;

    void Start()
    {
        Instance = this;

        gameUi = transform.Find("GameUi").gameObject;
        menuUi = transform.Find("MenuUi").gameObject;
        endUi = transform.Find("EndUi").gameObject;
        lpSlider = transform.Find("GameUi/LpSlider").GetComponent<Slider>();

        menuPanel = transform.Find("MenuUi/MenuPanel").gameObject;
        levelSelectionPanel = transform.Find("MenuUi/LevelSelectionPanel").gameObject;
        fullScreenText = transform.Find("MenuUi/MenuPanel/FullScreen/Text").GetComponent<Text>();
        Button[] levelButtons = levelSelectionPanel.GetComponentsInChildren<Button>();
        var curScene = SceneManager.GetActiveScene().buildIndex;
        for (int i = 0; i < levelButtons.Length; i++)
        {
            var button = levelButtons[i];
            if (i == curScene)
                button.GetComponentInChildren<Text>().color = selectedTextColor;
            int index = i;
            button.onClick.AddListener(() =>
            {
                Time.timeScale = 1;
                GameManager.Instance.LoadLevel(index);
            });
        }

        ShowGame();
        UpdateFullScreenText(Screen.fullScreen);

        AddEvent(moveLeft, EventTriggerType.PointerDown, (data) => { playerMovement.MoveLeftDown(); });
        AddEvent(moveLeft, EventTriggerType.PointerUp, (data) => { playerMovement.MoveLeftUp(); });

        AddEvent(moveRight, EventTriggerType.PointerDown, (data) => { playerMovement.MoveRightDown(); });
        AddEvent(moveRight, EventTriggerType.PointerUp, (data) => { playerMovement.MoveRightUp(); });

        AddEvent(jumpButton, EventTriggerType.PointerDown, (data) => { playerMovement.JumpButtonDown(); });
        AddEvent(jumpButton, EventTriggerType.PointerUp, (data) => { playerMovement.JumpButtonUp(); });

        // --- Extra Buttons (E, F, R) ---
        AddEvent(buttonE, EventTriggerType.PointerDown, (data) => { playerMovement.OnEPressed(); });
        AddEvent(buttonF, EventTriggerType.PointerDown, (data) => { playerMovement.OnFPressed(); });
        AddEvent(buttonR, EventTriggerType.PointerDown, (data) => { playerMovement.OnRPressed(); });


    }

    private void AddEvent(EventTrigger trigger, EventTriggerType type, System.Action<BaseEventData> callback)
    {
        var entry = new EventTrigger.Entry { eventID = type };
        entry.callback.AddListener((data) => callback(data));
        trigger.triggers.Add(entry);
    }

    public void ShowGame()
    {
        gameUi.SetActive(true);
        endUi.SetActive(false);
    }

    public void ShowEnd()
    {
        gameUi.SetActive(false);
        endUi.SetActive(true);
    }

    public void EnableAbility(bool lightON, bool canTravel)
    {
        buttonE.GetComponent<Image>().enabled = canTravel;
        buttonF.GetComponent<Image>().enabled = lightON;
    }

    public void ToggleMenu()
    {
        if (levelSelectionPanel.activeInHierarchy)
        {
            levelSelectionPanel.SetActive(false);
            menuPanel.SetActive(true);
        }
        else if (menuPanel.activeInHierarchy)
        {
            menuPanel.SetActive(false);
            Time.timeScale = 1;
        }
    }

    public void OnMenuButton()
    {
        if (levelSelectionPanel.activeInHierarchy)
        {
            levelSelectionPanel.SetActive(false);
        }
        else if (menuPanel.activeInHierarchy)
        {
            menuPanel.SetActive(false);
            Time.timeScale = 1;
        }
        else
        {
            menuPanel.SetActive(true);
            Time.timeScale = 0;
        }
    }

    public void OpenLevelSelection()
    {
        levelSelectionPanel.SetActive(true);
        menuPanel.SetActive(false);
    }

    public void RestartLevel()
    {
        Time.timeScale = 1;
        GameManager.Instance.RestartCurrent();
    }

    public void ToggleFullScreen()
    {
        GameManager.Instance.ToggleFullScreen();
    }

    public void UpdateFullScreenText(bool isFullScreen)
    {
        if (isFullScreen)
            fullScreenText.text = "Exit Full Screen";
        else
            fullScreenText.text = "Enter Full Screen";
    }

}