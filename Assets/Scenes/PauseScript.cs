using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; // Needed for Coroutines

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;

    [Header("UI References")]
    [Tooltip("The GameObject parent of your menu.")]
    public GameObject pauseMenuUI;
    
    [Tooltip("The CanvasGroup component for fading.")]
    public CanvasGroup menuCanvasGroup;

    [Header("Settings")]
    public string mainMenuSceneName = "MainMenu";
    public float fadeSpeed = 10f; // Higher = Faster

    void Start()
    {
        // Ensure everything is reset at start
        if (pauseMenuUI != null) 
        {
            pauseMenuUI.SetActive(false);
        }
        if (menuCanvasGroup != null)
        {
            menuCanvasGroup.alpha = 0f;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        // 1. Unfreeze the game immediately so it feels responsive
        Time.timeScale = 1f;
        GameIsPaused = false;

        // 2. Hide cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // 3. Start fading OUT
        StopAllCoroutines(); // Stop any existing fade in
        StartCoroutine(FadeMenu(false));
    }

    void Pause()
    {
        // 1. Activate the object so we can see it
        pauseMenuUI.SetActive(true);

        // 2. Start fading IN
        StopAllCoroutines();
        StartCoroutine(FadeMenu(true));

        // 3. Freeze time
        Time.timeScale = 0f;
        GameIsPaused = true;

        // 4. Show cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    /// <summary>
    /// This special loop handles the animation even when time is frozen.
    /// </summary>
    IEnumerator FadeMenu(bool fadeIn)
    {
        if (menuCanvasGroup == null) yield break;

        float targetAlpha = fadeIn ? 1f : 0f;
        float startAlpha = menuCanvasGroup.alpha;
        float t = 0f;

        while (t < 1f)
        {
            // IMPORTANT: We use unscaledDeltaTime because timeScale is 0!
            t += Time.unscaledDeltaTime * fadeSpeed;
            
            menuCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            yield return null;
        }

        // Ensure we hit the exact target
        menuCanvasGroup.alpha = targetAlpha;

        // If we finished fading out, turn off the game object to save performance
        if (!fadeIn)
        {
            pauseMenuUI.SetActive(false);
        }
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        GameIsPaused = false;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void OpenOptions()
    {
        Debug.Log("Options button clicked.");
    }
}