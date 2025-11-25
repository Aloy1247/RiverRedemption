using UnityEngine;
using TMPro;
using UnityEngine.UI; 
using System.Collections;
using UnityEngine.SceneManagement; // Needed for button logic

public class UIManager : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text healthText;
    public TMP_Text coinText;

    [Header("Popups")]
    [Tooltip("Red panel for damage.")]
    public CanvasGroup damageFlashGroup;
    
    [Tooltip("Black panel for death.")]
    public CanvasGroup deathScreenGroup;

    // --- NEW: Win Screen ---
    [Tooltip("The panel that appears when all trash is sold.")]
    public CanvasGroup winScreenGroup;
    // ---

    private CanvasGroup hudCanvasGroup;
    private RectTransform containerRect;

    [Header("Player References")]
    public PlayerHealth playerHealth;

    [Header("Display Settings")]
    public float displayDuration = 3.0f;
    public KeyCode showHudKey = KeyCode.H;

    [Header("Animation Settings")]
    public float slideOffset = 100f;
    public float slideSpeed = 5f;

    // State variables
    private Coroutine hideTimerCoroutine;
    private Coroutine animationCoroutine;
    private Vector2 defaultContainerPos;
    private Vector2 hiddenContainerPos;
    private bool isVisible = false;
    
    private int lastHealth;

    void Start()
    {
        hudCanvasGroup = GetComponent<CanvasGroup>();
        containerRect = GetComponent<RectTransform>();

        if (hudCanvasGroup == null || containerRect == null)
        {
            Debug.LogError("UIManager: Missing CanvasGroup or RectTransform!");
            return;
        }

        // UI Position Setup
        defaultContainerPos = containerRect.anchoredPosition;
        hiddenContainerPos = defaultContainerPos + new Vector2(0, slideOffset);
        hudCanvasGroup.alpha = 0;
        containerRect.anchoredPosition = hiddenContainerPos;

        // Initialize screens to be invisible
        if (damageFlashGroup != null) damageFlashGroup.alpha = 0;
        if (deathScreenGroup != null) deathScreenGroup.alpha = 0;
        
        // --- NEW: Hide Win Screen initially ---
        if (winScreenGroup != null) 
        {
            winScreenGroup.alpha = 0;
            winScreenGroup.interactable = false;
            winScreenGroup.blocksRaycasts = false;
        }
        // ---

        // Event Listeners
        if (playerHealth != null)
        {
            lastHealth = playerHealth.maxHealth; 

            playerHealth.OnHealthChanged += (current, max) => {
                CheckHealthChange(current, max);
                UpdateHealthUI(current, max);
            };
            
            healthText.text = $"Health {playerHealth.currentHealth}";
        }

        if (DropOffZone.Instance != null)
        {
            DropOffZone.Instance.OnScoreChanged += (score) => {
                UpdateCoinUI(score);
            };
            coinText.text = $"Coins {DropOffZone.Instance.GetScore()}";

            // --- NEW: Listen for Win Event ---
            DropOffZone.Instance.OnLevelComplete += ShowWinScreen;
            // ---
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(showHudKey)) ShowUI();
    }

    // --- NEW: WIN SCREEN FUNCTIONS ---
    void ShowWinScreen()
    {
        if (winScreenGroup == null) return;

        // Show the screen
        winScreenGroup.alpha = 1;
        winScreenGroup.interactable = true;
        winScreenGroup.blocksRaycasts = true;

        // Pause the game
        Time.timeScale = 0f;
        PauseMenu.GameIsPaused = true; // Tell camera to stop moving

        // Show Cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Connect this to your "Continue Free Roaming" button
    public void ContinueFreeRoaming()
    {
        if (winScreenGroup == null) return;

        // Hide the screen
        winScreenGroup.alpha = 0;
        winScreenGroup.interactable = false;
        winScreenGroup.blocksRaycasts = false;

        // Resume game
        Time.timeScale = 1f;
        PauseMenu.GameIsPaused = false; // Tell camera to move again

        // Hide Cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Connect this to your "Main Menu" button
    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        PauseMenu.GameIsPaused = false;
        SceneManager.LoadScene("MainMenu"); // Make sure this matches your scene name
    }
    // --------------------------------

    void CheckHealthChange(int current, int max)
    {
        if (current < lastHealth)
        {
            if (current <= 0) StartCoroutine(ShowDeathScreen());
            else StartCoroutine(FlashDamage());
        }
        lastHealth = current;
    }

    IEnumerator FlashDamage()
    {
        if (damageFlashGroup == null) yield break;
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * 10f; 
            damageFlashGroup.alpha = Mathf.Lerp(0f, 0.5f, t); 
            yield return null;
        }
        t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * 2f; 
            damageFlashGroup.alpha = Mathf.Lerp(0.5f, 0f, t);
            yield return null;
        }
        damageFlashGroup.alpha = 0;
    }

    IEnumerator ShowDeathScreen()
    {
        if (deathScreenGroup == null) yield break;
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * 2f; 
            deathScreenGroup.alpha = Mathf.Lerp(0f, 1f, t);
            yield return null;
        }
        deathScreenGroup.alpha = 1;
        yield return new WaitForSeconds(2.0f);
        t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * 1f; 
            deathScreenGroup.alpha = Mathf.Lerp(1f, 0f, t);
            yield return null;
        }
        deathScreenGroup.alpha = 0;
    }

    void UpdateHealthUI(int current, int max)
    {
        healthText.text = $"Health {current}";
        ShowUI();
    }

    void UpdateCoinUI(int current)
    {
        coinText.text = $"Coins {current}";
        ShowUI();
    }

    void ShowUI()
    {
        if (hideTimerCoroutine != null) StopCoroutine(hideTimerCoroutine);
        hideTimerCoroutine = StartCoroutine(HideUIAfterDelay());

        if (!isVisible)
        {
            if (animationCoroutine != null) StopCoroutine(animationCoroutine);
            animationCoroutine = StartCoroutine(AnimateSlide(true));
        }
    }

    IEnumerator HideUIAfterDelay()
    {
        yield return new WaitForSeconds(displayDuration);
        if (animationCoroutine != null) StopCoroutine(animationCoroutine);
        animationCoroutine = StartCoroutine(AnimateSlide(false));
    }

    IEnumerator AnimateSlide(bool show)
    {
        isVisible = show;
        float t = 0;
        Vector2 startPos = containerRect.anchoredPosition;
        Vector2 targetPos = show ? defaultContainerPos : hiddenContainerPos;
        float startAlpha = hudCanvasGroup.alpha;
        float targetAlpha = show ? 1f : 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * slideSpeed;
            containerRect.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            hudCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            yield return null;
        }
    }
    
    void OnDestroy() { } 
}