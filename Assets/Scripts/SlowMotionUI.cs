using UnityEngine;
using UnityEngine.UI;

public class SlowMotionUI : MonoBehaviour
{
    [SerializeField] private Button slowMotionButton; // Assign your UI button here
    [SerializeField] private float slowMotionFactor = 0.3f;   // How slow the game runs
    [SerializeField] private float slowMotionDuration = 10f;  // Duration in seconds

    private float originalFixedDeltaTime;
    private bool isSlowMotionActive = false;
    private float slowMotionTimer = 0f;

    private void Start()
    {
        // Store original physics step
        originalFixedDeltaTime = Time.fixedDeltaTime;

        // Add button listener
        if (slowMotionButton != null)
            slowMotionButton.onClick.AddListener(ActivateSlowMotion);
    }

    private void Update()
    {
        if (isSlowMotionActive)
        {
            slowMotionTimer += Time.unscaledDeltaTime;

            if (slowMotionTimer >= slowMotionDuration)
            {
                ResetTime();
            }
        }
    }

    private void ActivateSlowMotion()
    {
        Time.timeScale = slowMotionFactor;
        Time.fixedDeltaTime = originalFixedDeltaTime * Time.timeScale;

        isSlowMotionActive = true;
        slowMotionTimer = 0f;
    }

    private void ResetTime()
    {
        Time.timeScale = 1f;
        Time.fixedDeltaTime = originalFixedDeltaTime;

        isSlowMotionActive = false;
    }
}
