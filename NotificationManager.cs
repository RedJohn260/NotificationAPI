using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using MSCLoader;

/// <summary>
/// **NotificationManager**
/// </summary>
/// <remarks>
///  A robust system for creating and displaying in-game notifications within a Unity environment.
///  It offers the ability to generate notifications of varying types (Info, Warning, Error) with customizable durations and persistence.
///  The system employs a UI prefab for visual representation and manages their placement, animation, and lifecycle.
/// </remarks>
public class NotificationManager : MonoBehaviour
{
    #region Public Fields

    /// <summary>
    /// The UI prefab used as a template for creating new notifications.
    /// </summary>
    /// <remarks>
    ///  **Important:** The prefab should adhere to a specific structure:
    ///   - A root GameObject with a `RectTransform` component.
    ///   - Child GameObjects named "Icon", "Title", "Message", and "CloseButton".
    ///   - "Icon" must have an `Image` component to display the notification icon.
    ///   - "Title" and "Message" must have `Text` components to display the notification text.
    ///   - "CloseButton" must have a `Button` component to allow the user to dismiss the notification.
    /// </remarks>
    public GameObject notificationPrefab;

    /// <summary>
    /// The `Transform` component of the UI element that will act as the parent for all instantiated notifications.
    /// </summary>
    public Transform notificationParent;

    /// <summary>
    ///  The vertical spacing between individual notifications in the notification stack.
    /// </summary>
    public float notificationSpacing = 10f;

    /// <summary>
    /// The duration of the slide-in and slide-out animation of notifications, measured in seconds.
    /// </summary>
    public float notificationSpeed = 0.5f;

    /// <summary>
    /// The time (in seconds) for which a non-persistent notification will remain visible before automatically disappearing.
    /// </summary>
    public float notificationDuration = 3f;

    /// <summary>
    /// The `Sprite` used to visually represent informational notifications.
    /// </summary>
    public Sprite InfoIcon;

    /// <summary>
    /// The `Sprite` used to visually represent warning notifications.
    /// </summary>
    public Sprite WarningIcon;

    /// <summary>
    /// The `Sprite` used to visually represent error notifications.
    /// </summary>
    public Sprite ErrorIcon;

    /// <summary>
    /// The maximum number of notifications that can be displayed simultaneously.
    /// </summary>
    public int maxNotifications = 8; // Set a reasonable default value

    /// <summary>
    /// AudioSource for the notification sound.
    /// </summary>
    public AudioSource NotificationAudioSource;
    /// <summary>
    /// Settings to play or not to play audio alert sound.
    /// </summary>
    public bool PlaySoundAlert;

    #endregion

    #region Private Fields

    /// <summary>
    /// A list of `GameObject` representing the notifications currently being displayed.
    /// </summary>
    private List<GameObject> activeNotifications = new List<GameObject>();

    #endregion

    #region Enums

    /// <summary>
    /// Defines the possible types of notifications that can be displayed.
    /// </summary>
    public enum NotificationType
    {
        /// <summary>
        /// Represents an informational notification.
        /// </summary>
        Info,
        /// <summary>
        /// Represents a warning notification.
        /// </summary>
        Warning,
        /// <summary>
        /// Represents an error notification.
        /// </summary>
        Error
    }

    #endregion

    #region Structs

    /// <summary>
    /// A data structure that encapsulates all the information required to create a notification.
    /// </summary>
    public struct NotificationData
    {
        /// <summary>
        /// The `NotificationType` of the notification (Info, Warning, Error).
        /// </summary>
        public NotificationType type;

        /// <summary>
        /// The title text displayed on the notification.
        /// </summary>
        public string title;

        /// <summary>
        /// The main message content displayed on the notification.
        /// </summary>
        public string message;

        /// <summary>
        /// A boolean flag indicating whether the notification should persist on the screen until manually dismissed by the user.
        /// </summary>
        public bool isPersistent;
    }

    #endregion

    #region Unity Methods

    /// <summary>
    /// Called when the script instance is being loaded.  Used here to perform initial setup and error checking.
    /// </summary>
    void Start()
    {
        // Error checking to prevent null reference exceptions during runtime
        if (notificationPrefab == null)
        {
            ModConsole.Error("Notification prefab is not assigned.");
            enabled = false; // Disable the script to prevent further errors
        }
        if (notificationParent == null)
        {
            ModConsole.Error("Notification parent is not assigned.");
            enabled = false; // Disable the script to prevent further errors
        }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Creates and displays a new notification with the specified data.
    /// </summary>
    /// <param name="data">A `NotificationData` struct containing the information for the notification.</param>
    public void PushNotification(NotificationData data)
    {
        //Check if we are at the limit
        if (activeNotifications.Count >= maxNotifications)
        {
            return;
        }

        CreateNotification(data);
    }

    /// <summary>
    /// Initiates the coroutine that animates a notification sliding out of view before being destroyed.
    /// </summary>
    /// <param name="notification">The `GameObject` representing the notification to slide out.</param>
    public void SlideOutNotification(GameObject notification)
    {
        StartCoroutine(SlideOutAndDestroy(notification));
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Instantiates the notification prefab, populates it with the provided data, and manages its lifecycle.
    /// </summary>
    /// <param name="data">The `NotificationData` containing the information for the notification to be created.</param>
    /// <returns>The newly created `GameObject` representing the notification, or `null` if the creation process failed.</returns>
    GameObject CreateNotification(NotificationData data)
    {
        // Instantiate the notification prefab
        GameObject newNotification = Instantiate(notificationPrefab);
        newNotification.transform.SetParent(notificationParent.transform, false); // Set the parent, preserving local scale, rotation, and position
        newNotification.transform.localScale = Vector3.one; // Ensure the scale is reset to (1, 1, 1)

        // Get the RectTransform for positioning and animation
        RectTransform notificationRect = newNotification.GetComponent<RectTransform>();
        if (notificationRect == null)
        {
            ModConsole.Error("Notification prefab does not have a RectTransform component.");
            Destroy(newNotification);
            return null;
        }

        // Find the UI elements within the notification prefab
        Image iconImage = newNotification.transform.Find("Icon").GetComponent<Image>();
        Text titleText = newNotification.transform.Find("Title").GetComponent<Text>();
        Text messageText = newNotification.transform.Find("Message").GetComponent<Text>();
        Button closeButton = newNotification.transform.Find("CloseButton").GetComponent<Button>();

        // Set the icon based on the notification type
        Sprite iconSprite = GetIconForNotificationType(data.type);
        if (iconImage != null && iconSprite != null)
            iconImage.sprite = iconSprite;

        // Set the title and message text
        if (titleText != null)
            titleText.text = data.title;

        if (messageText != null)
            messageText.text = data.message;

        // Add a listener to the close button to destroy the notification
        if (closeButton != null)
            closeButton.onClick.AddListener(() => DestroyNotification(newNotification));

        activeNotifications.Add(newNotification); // Add the notification to the list of active notifications
        UpdateNotificationPositions(); // Update the positions of all active notifications

        // Slide the notification in from off-screen
        Vector2 targetPos = notificationRect.anchoredPosition;
        Vector2 startPos = GetOffScreenPosition(notificationRect);
        notificationRect.anchoredPosition = startPos;

        //Play audio alert one time
        if (PlaySoundAlert)
        {
            StartCoroutine(PlaySound());
        }

        StartCoroutine(SlideNotification(notificationRect, startPos, targetPos, notificationSpeed));

        // Attach behavior to handle auto-hide or persistence
        var behaviour = newNotification.AddComponent<NotificationBehaviour>();
        behaviour.Setup(data.isPersistent, notificationDuration, this, newNotification);

        return newNotification;
    }

    /// <summary>
    /// Destroys a notification `GameObject`, removing it from the active notifications list and updating the positions of the remaining notifications.
    /// </summary>
    /// <param name="notification">The `GameObject` representing the notification to be destroyed.</param>
    private void DestroyNotification(GameObject notification)
    {
        if (notification != null)
        {
            activeNotifications.Remove(notification);
            Destroy(notification);
            UpdateNotificationPositions();
        }
    }

    /// <summary>
    /// Returns the appropriate `Sprite` for a given `NotificationType`.
    /// </summary>
    /// <param name="type">The `NotificationType` for which to retrieve the icon.</param>
    /// <returns>The `Sprite` associated with the specified `NotificationType`.</returns>
    Sprite GetIconForNotificationType(NotificationType type)
    {
        switch (type)
        {
            case NotificationType.Warning: return WarningIcon;
            case NotificationType.Error: return ErrorIcon;
            default: return InfoIcon;
        }
    }

    /// <summary>
    /// Updates the vertical positions of all active notifications to maintain consistent spacing between them.
    /// </summary>
    void UpdateNotificationPositions()
    {
        float currentY = 0;
        for (int i = 0; i < activeNotifications.Count; i++)
        {
            RectTransform rt = activeNotifications[i].GetComponent<RectTransform>();
            float newY = currentY - (rt.rect.height + notificationSpacing) * i;
            rt.anchoredPosition = new Vector2(0, newY);
        }
    }

    /// <summary>
    /// Calculates the off-screen position from which a notification will slide in.
    /// </summary>
    /// <param name="rect">The `RectTransform` of the notification.</param>
    /// <returns>A `Vector2` representing the off-screen position.</returns>
    Vector2 GetOffScreenPosition(RectTransform rect)
    {
        RectTransform canvasRect = notificationParent.GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        float x = canvasRect.rect.width / 2 + rect.rect.width;
        float y = rect.anchoredPosition.y;
        return new Vector2(x, y);
    }

    #endregion

    #region Coroutines

    /// <summary>
    /// A coroutine that animates a notification's position, creating a sliding effect.
    /// </summary>
    /// <param name="rect">The `RectTransform` of the notification to animate.</param>
    /// <param name="from">The starting position of the animation.</param>
    /// <param name="to">The ending position of the animation.</param>
    /// <param name="duration">The duration of the animation in seconds.</param>
    /// <returns>An `IEnumerator` to allow the coroutine to execute over multiple frames.</returns>
    IEnumerator SlideNotification(RectTransform rect, Vector2 from, Vector2 to, float duration)
    {
        float time = 0;
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / duration); // Clamp to 0-1 range to ensure t is between 0 and 1
            rect.anchoredPosition = Vector2.Lerp(from, to, t);
            yield return null; // Wait for the next frame
        }
        rect.anchoredPosition = to; // Ensure the final position is exactly 'to'
    }

    /// <summary>
    /// A coroutine that slides a notification out of view and then destroys it.
    /// </summary>
    /// <param name="notification">The notification `GameObject` to slide out and destroy.</param>
    /// <returns>An `IEnumerator` for the coroutine.</returns>
    private IEnumerator SlideOutAndDestroy(GameObject notification)
    {
        if (notification == null) yield break; // Exit if the notification is already null

        RectTransform rect = notification.GetComponent<RectTransform>();
        Vector2 startPos = rect.anchoredPosition;
        Vector2 targetPos = GetOffScreenPosition(rect);

        yield return StartCoroutine(SlideNotification(rect, startPos, targetPos, notificationSpeed)); // Wait for the slide animation to complete

        DestroyNotification(notification); // Destroy the notification after it has slid out of view
    }

    /// <summary>
    /// After the notification speed delay, plays the notification audio alert one time.
    /// </summary>
    private IEnumerator PlaySound()
    {
        if (NotificationAudioSource == null)
            yield return null;

        if (!NotificationAudioSource.isPlaying)
        {
            yield return new WaitForSeconds(notificationSpeed);
            NotificationAudioSource.PlayOneShot(NotificationAudioSource.clip);
        }
    }

    #endregion
}