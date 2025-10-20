using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;

public class UIPopUpActivator : MonoBehaviour
{
    private const string PLAYER_TAG = "Player";
    
    [Header("UI Pop-Up Reference")]
    [SerializeField] private UIPopUp popUpUI;
    [Header("Pop-Up Settings")]
    [SerializeField] private bool isOnTrigger;
    [SerializeField] private bool isOnInteract;
    [SerializeField] private float delayBeforePopUp;
    [Header("Pop-Up Message")]
    [SerializeField] private string message;

    private bool hasBeenTriggered = false;

    private void OnEnable()
    {
        PlayerInputHandler.Instance.OnInteractPerformed += HandleInteract;
    }

    private void OnDisable()
    {
        PlayerInputHandler.Instance.OnInteractPerformed -= HandleInteract;
    }

    private void HandleInteract()
    {
        if (isOnInteract && !hasBeenTriggered)
        {
            TriggerPopUp();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(PLAYER_TAG) && isOnTrigger && !hasBeenTriggered)
        {
            StartCoroutine(TriggerPopUp());
        }
    }

    private IEnumerator TriggerPopUp()
    {
        if (popUpUI.gameObject.activeSelf) yield break;

        yield return new WaitForSeconds(delayBeforePopUp);

        popUpUI.ToggleVisibility();
        popUpUI.SetText(message);
        hasBeenTriggered = true;
        Time.timeScale = 0f; // Pause the game
    }
}
