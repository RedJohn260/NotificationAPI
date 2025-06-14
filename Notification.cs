using MSCLoader;
using UnityEngine;
using static NotificationManager;

namespace NotificationAPI
{
    /// <summary>
    /// Provides a static interface for sending notifications to the game.
    /// This class is the main entry point for displaying notifications using the NotificationAPI.
    /// </summary>
    public static class Notification
    {
        /// <summary>
        ///  A static reference to the NotificationManager instance.
        ///  This is used to push notifications to the manager.
        /// </summary>
        public static NotificationManager _instance;

        /// <summary>
        /// Displays a notification in the game.
        /// </summary>
        /// <param name="type">The type of notification (e.g., Info, Warning, Error).</param>
        /// <param name="title">The title of the notification.</param>
        /// <param name="message">The message content of the notification.</param>
        /// <param name="persistent">Whether the notification should remain on screen until dismissed by the user.
        /// If `true`, the notification will stay visible until manually closed. If `false`, it will disappear after a short time.</param>
        public static void Notify(NotificationManager.NotificationType type, string title, string message, bool persistent)
        {
            // Find the NotificationManager instance in the scene.
            // This approach, while working, is inefficient if called frequently. A better approach would be to find the NotificationManager once and store it, or use Dependency Injection.
            NotificationManager notificationManager = GameObject.FindObjectOfType<NotificationManager>();

            if (_instance != null)
            {
                // Create a new NotificationData object to hold the notification parameters.
                NotificationData data = new NotificationData
                {
                    type = type,
                    title = title,
                    message = message,
                    isPersistent = persistent
                };

                // Push the notification to the NotificationManager.
                _instance.PushNotification(data);
            }
            else
            {
                // Log an error message to the ModConsole if the NotificationManager is not found.
                // This is important for debugging and informing the user that notifications cannot be displayed.
                ModConsole.Error("NotificationManager not found in the scene.  Notification cannot be displayed.");
            }
        }
    }
}