using UnityEngine;
using UnityEngine.EventSystems;

public class MobileVibration : MonoBehaviour, IPointerDownHandler
{
    [Tooltip("Enable vibration on this button")]
    public bool enableVibration = true;

    [Tooltip("Milliseconds to vibrate (Android only). Leave 0 for default short buzz.")]
    public int vibrationDurationMs = 30;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!enableVibration) return;

#if UNITY_ANDROID && !UNITY_EDITOR
        // On Android we can request a custom duration if vibrator is available
        try
        {
            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                AndroidJavaObject vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");

                if (vibrator != null)
                {
                    if (vibrationDurationMs > 0)
                        vibrator.Call("vibrate", (long)vibrationDurationMs);
                    else
                        vibrator.Call("vibrate", (long)30);
                }
            }
        }
        catch
        {
            Handheld.Vibrate(); // fallback
        }
#elif UNITY_IOS && !UNITY_EDITOR
        // iOS – only short vibration available without plugin
        Handheld.Vibrate();
#else
        Debug.Log("Vibration triggered (Editor only, no haptic).");
#endif
    }
}
