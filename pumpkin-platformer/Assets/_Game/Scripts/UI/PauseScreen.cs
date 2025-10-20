using UnityEngine;

public class PauseScreen : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI;

    private bool isPaused = false;

    private void Awake()
    {
        pauseMenuUI.SetActive(false);
    }

    private void OnEnable()
    {
        PlayerInputHandler.Instance.OnPausePerformed += TogglePause;
        PlayerInputHandler.Instance.OnSubmitPerformed += TogglePauseBySubmit;
    }

    private void OnDisable()
    {
        PlayerInputHandler.Instance.OnPausePerformed -= TogglePause;
        PlayerInputHandler.Instance.OnSubmitPerformed -= TogglePauseBySubmit;
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        pauseMenuUI.SetActive(isPaused);

        if (isPaused)
        {
            Time.timeScale = 0f; // Pause the game
        }
        else
        {
            Time.timeScale = 1f; // Resume the game
        }
    }

    private void TogglePauseBySubmit()
    {
        if (!isPaused) return;

        TogglePause();
    }
}
