using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelEndTrigger : MonoBehaviour
{
    private const float DELAY_BEFORE_SCENE_LOAD = 1f;
    private const string PLAYER_TAG = "Player";

    [Header("Lighting Settings")]
    [SerializeField] private Light targetLight;
    [SerializeField] private float fadeDuration = 3f;
    [SerializeField] private float targetIntensity = 0f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(PLAYER_TAG))
        {
            StartCoroutine(FadeLightIntensity(targetLight.intensity, targetIntensity, fadeDuration));
        }
    }

    private IEnumerator FadeLightIntensity(float startIntensity, float endIntensity, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            targetLight.intensity = Mathf.Lerp(startIntensity, endIntensity, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        targetLight.intensity = endIntensity;

        yield return new WaitForSeconds(DELAY_BEFORE_SCENE_LOAD);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
