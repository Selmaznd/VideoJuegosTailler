using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Global pause manager that spawns its own UI and works across gameplay scenes.
/// Toggle with Escape (configurable) to resume, restart, or return to the level selector.
/// </summary>
public class PauseMenuController : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private KeyCode primaryPauseKey = KeyCode.Escape;
    [SerializeField] private KeyCode secondaryPauseKey = KeyCode.Space;

    [Header("Scene Rules")]
    [SerializeField] private List<string> pauseDisabledScenes = new() { "Main Menu", "ChooseLevel", "win" };
    [SerializeField] private string levelSelectScene = "ChooseLevel";

    private static PauseMenuController _instance;

    private Canvas _canvas;
    private GameObject _panel;
    private Button _resumeButton;
    private Button _restartButton;
    private Button _levelSelectButton;
    private bool _isPaused;
    private bool _allowPause = true;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Bootstrap()
    {
        if (_instance != null) return;

        var go = new GameObject("PauseMenuController");
        _instance = go.AddComponent<PauseMenuController>();
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        EnsureEventSystemExists();
        BuildUi();

        SceneManager.sceneLoaded += OnSceneLoaded;
        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    private void OnDestroy()
    {
        if (_instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            _instance = null;
        }
    }

    private void Update()
    {
        if (!_allowPause) return;

        if ((primaryPauseKey != KeyCode.None && Input.GetKeyDown(primaryPauseKey)) ||
            (secondaryPauseKey != KeyCode.None && Input.GetKeyDown(secondaryPauseKey)))
        {
            TogglePause();
        }
    }

    private void TogglePause()
    {
        if (_isPaused)
        {
            Resume();
        }
        else
        {
            Pause();
        }
    }

    private void Pause()
    {
        _isPaused = true;
        Time.timeScale = 0f;
        _panel.SetActive(true);

        EnsureEventSystemExists();
        EventSystem.current.SetSelectedGameObject(_resumeButton.gameObject);
    }

    private void Resume()
    {
        _isPaused = false;
        Time.timeScale = 1f;
        _panel.SetActive(false);
    }

    private void Restart()
    {
        Resume();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void GoToLevelSelect()
    {
        Resume();
        if (!string.IsNullOrEmpty(levelSelectScene))
        {
            SceneManager.LoadScene(levelSelectScene);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Resume();
        _allowPause = !pauseDisabledScenes.Contains(scene.name);
        _panel.SetActive(false);
    }

    private void BuildUi()
    {
        _canvas = new GameObject("PauseCanvas").AddComponent<Canvas>();
        _canvas.transform.SetParent(transform);
        _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        _canvas.sortingOrder = 2000;

        var scaler = _canvas.gameObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        _canvas.gameObject.AddComponent<GraphicRaycaster>();

        _panel = new GameObject("PausePanel");
        _panel.transform.SetParent(_canvas.transform, false);

        var panelImage = _panel.AddComponent<Image>();
        panelImage.color = new Color(0f, 0f, 0f, 0.6f);

        var panelRect = _panel.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0f, 0f);
        panelRect.anchorMax = new Vector2(1f, 1f);
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        var stack = new GameObject("ButtonStack");
        stack.transform.SetParent(_panel.transform, false);
        var stackRect = stack.AddComponent<RectTransform>();
        stackRect.anchorMin = new Vector2(0.5f, 0.5f);
        stackRect.anchorMax = new Vector2(0.5f, 0.5f);
        stackRect.sizeDelta = new Vector2(400f, 300f);
        stackRect.anchoredPosition = Vector2.zero;

        _resumeButton = CreateButton(stackRect, "Resume", 80f, Resume);
        _restartButton = CreateButton(stackRect, "Restart", 0f, Restart);
        _levelSelectButton = CreateButton(stackRect, "Level Select", -80f, GoToLevelSelect);

        _panel.SetActive(false);
    }

    private Button CreateButton(RectTransform parent, string label, float yOffset, UnityAction onClick)
    {
        var buttonGo = new GameObject(label + "Button");
        buttonGo.transform.SetParent(parent, false);

        var rect = buttonGo.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(280f, 60f);
        rect.anchoredPosition = new Vector2(0f, yOffset);

        var image = buttonGo.AddComponent<Image>();
        image.color = new Color(0.2f, 0.2f, 0.2f, 0.85f);

        var button = buttonGo.AddComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(onClick);

        var textGo = new GameObject("Label");
        textGo.transform.SetParent(buttonGo.transform, false);

        var text = textGo.AddComponent<Text>();
        text.alignment = TextAnchor.MiddleCenter;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 28;
        text.text = label;
        text.color = Color.white;

        var textRect = text.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        var colors = button.colors;
        colors.normalColor = new Color(0.2f, 0.2f, 0.2f, 0.85f);
        colors.highlightedColor = new Color(0.3f, 0.3f, 0.3f, 0.9f);
        colors.pressedColor = new Color(0.15f, 0.15f, 0.15f, 0.9f);
        button.colors = colors;

        return button;
    }

    private void EnsureEventSystemExists()
    {
        if (EventSystem.current != null) return;

        var es = new GameObject("EventSystem");
        es.AddComponent<EventSystem>();
        es.AddComponent<StandaloneInputModule>();
        DontDestroyOnLoad(es);
    }
}
