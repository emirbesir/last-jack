using TMPro;
using UnityEngine;

public class UIPopUp : MonoBehaviour
{
    [SerializeField] private TMP_Text popUpText;

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        PlayerInputHandler.Instance.OnSubmitPerformed += HandleSubmit;
    }

    private void OnDisable()
    {
        PlayerInputHandler.Instance.OnSubmitPerformed -= HandleSubmit;
    }

    private void HandleSubmit()
    {
        if (!gameObject.activeSelf) return;
        
        ToggleVisibility();
        Time.timeScale = 1f; // Resume the game
    }

    public void ToggleVisibility()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }

    public void SetText(string message)
    {
        popUpText.text = message;
    }
}
