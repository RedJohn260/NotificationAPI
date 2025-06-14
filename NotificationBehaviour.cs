using UnityEngine;
using System.Collections;

/// <summary>
/// Handles the behavior of individual notifications, including auto-closing and interaction with the NotificationManager.
/// </summary>
/// <remarks>
/// This script is responsible for determining whether a notification should be automatically closed after a certain duration,
/// and for triggering the slide-out animation and destruction of the notification.  It is intended to be attached
/// to the notification prefab instance.
/// </remarks>
public class NotificationBehaviour : MonoBehaviour
{
    /// <summary>
    /// Indicates whether the notification should persist until manually closed.
    /// </summary>
    private bool isPersistent;

    /// <summary>
    /// The duration (in seconds) for which the notification should be displayed before auto-closing (if not persistent).
    /// </summary>
    private float duration;

    /// <summary>
    /// A reference to the NotificationManager that created this notification.
    /// </summary>
    private NotificationManager manager;

    /// <summary>
    /// A reference to the GameObject that represents the notification.
    /// </summary>
    private GameObject notificationGO;

    /// <summary>
    /// Initializes the NotificationBehaviour with the necessary data.  This method should be called immediately after instantiating the prefab.
    /// </summary>
    /// <param name="isPersistent">Whether the notification is persistent.</param>
    /// <param name="duration">The duration of the notification (if not persistent).</param>
    /// <param name="manager">The NotificationManager instance.</param>
    /// <param name="notificationGO">The notification GameObject instance.</param>
    public void Setup(bool isPersistent, float duration, NotificationManager manager, GameObject notificationGO)
    {
        this.isPersistent = isPersistent;
        this.duration = duration;
        this.manager = manager;
        this.notificationGO = notificationGO;

        // If the notification is not persistent, start the auto-close coroutine
        if (!isPersistent)
        {
            StartCoroutine(AutoClose());
        }
    }

    /// <summary>
    /// Coroutine that waits for the specified duration and then triggers the slide-out animation and destruction of the notification.
    /// </summary>
    /// <returns>An IEnumerator for the coroutine.</returns>
    private IEnumerator AutoClose()
    {
        yield return new WaitForSeconds(duration); // Wait for the specified duration

        // Check if the notification and manager still exist to prevent errors after scene changes or other potential issues
        if (notificationGO != null && manager != null)
        {
            manager.SlideOutNotification(notificationGO); // Trigger the slide-out animation and destruction
        }
    }
}