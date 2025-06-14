using MSCLoader;
using UnityEngine;

namespace NotificationAPI
{
    /// <summary>
    /// A Mod that provides an API for displaying pop-up notifications.  This mod sets up the NotificationManager and provides access to it.
    /// </summary>
    /// <remarks>
    /// This class initializes the NotificationManager, loads assets required for the notification system, and provides settings
    /// for customizing notification appearance and behavior.  It registers the NotificationManager as a singleton instance accessible via the Notification class.
    /// This mod **does nothing on its own** but acts as a base for other mods to utilize its notification functionalities.
    /// </remarks>
    public class NotificationAPI : Mod
    {
        /// <inheritdoc/>
        public override string ID => "NotificationAPI"; // Your (unique) mod ID 

        /// <inheritdoc/>
        public override string Name => "NotificationAPI"; // Your mod name

        /// <inheritdoc/>
        public override string Author => "RedJohn260"; // Name of the Author (your name)

        /// <inheritdoc/>
        public override string Version => "1.0.0"; // Version

        /// <inheritdoc/>
        public override string Description => "Display Pop-Up notifications API. Does nothing on it's own."; // Short description of your mod

        /// <summary>
        /// The NotificationManager instance.
        /// </summary>
        private NotificationManager _manager;

        /// <summary>
        /// Header color for settings menu.
        /// </summary>
        public static Color _settingHeaderColor = new Color(0.1f, 0.1f, 0.1f, 1);

        /// <summary>
        /// Settings slider for notification speed.
        /// </summary>
        public static SettingsSlider NotificationSpeedSlider;

        /// <summary>
        /// Settings slider for notification duration.
        /// </summary>
        public static SettingsSlider NotificationDurationSlider;

        /// <summary>
        /// Settings slider for notification spacing.
        /// </summary>
        public static SettingsSlider NotificationSpacingSlider;

        /// <summary>
        /// Settings checkbox for notification sound.
        /// </summary>
        public static SettingsCheckBox NotificationSoundChk;

        /// <summary>
        /// Settings slider for volume sound.
        /// </summary>
        public static SettingsSlider NotificationVolumeSlider;

        /// <inheritdoc/>
        public override void ModSetup()
        {
            SetupFunction(Setup.PreLoad, Mod_PreLoad);
            SetupFunction(Setup.ModSettings, Mod_Settings);
            SetupFunction(Setup.OnLoad, Mod_OnLoad);
        }

        /// <summary>
        /// Adds settings to the Mod Settings menu.
        /// </summary>
        private void Mod_Settings()
        {
            Settings.AddHeader(this, "Notification Settings", _settingHeaderColor, false);
            NotificationSpeedSlider = Settings.AddSlider(this, "NotificationSpeedSlider", "Notification Speed", 0.1f, 3.0f, 0.5f, OnNotificationSpeedChange, 1);
            NotificationDurationSlider = Settings.AddSlider(this, "NotificationDurationSlider", "Notification Duration", 0.1f, 20.0f, 3.0f, OnNotificationDurationhange, 1);
            NotificationSpacingSlider = Settings.AddSlider(this, "NotificationSpacingSlider", "Notification Spacing", 0f, 100f, 5f, OnNotificationSpacingChange, 1);
            Settings.AddHeader(this, "Notification Volume Settings", _settingHeaderColor, false);
            NotificationSoundChk = Settings.AddCheckBox("NotificationSoundChk", "Play Notification Sound", true, OnNotificationSoundChange);
            NotificationVolumeSlider = Settings.AddSlider(this, "NotificationVolumeSlider", "Notification Volume", 0.0f, 1.0f, 1.0f, OnNotificationVolumeChange, 1);
        }

        /// <summary>
        /// Callback function for when the notification spacing setting changes.
        /// </summary>
        private void OnNotificationSpacingChange()
        {
            _manager.notificationSpacing = NotificationSpacingSlider.GetValue();
        }

        /// <summary>
        /// Callback function for when the notification duration setting changes.
        /// </summary>
        private void OnNotificationDurationhange()
        {
            _manager.notificationDuration = NotificationDurationSlider.GetValue();
        }

        /// <summary>
        /// Callback function for when the notification speed setting changes.
        /// </summary>
        private void OnNotificationSpeedChange()
        {
            _manager.notificationSpeed = NotificationSpeedSlider.GetValue();
        }
        /// <summary>
        /// Callback function for when the notification playsound setting changes.
        /// </summary>
        private void OnNotificationSoundChange()
        {
            _manager.PlaySoundAlert = NotificationSoundChk.GetValue();
        }
        /// <summary>
        /// Callback function for when the notification volume setting changes.
        /// </summary>
        private void OnNotificationVolumeChange()
        {
            _manager.NotificationAudioSource.volume = NotificationVolumeSlider.GetValue();
        }

        /// <summary>
        /// Called before the main game is loaded.  Loads assets and creates the NotificationManager.
        /// </summary>
        private void Mod_PreLoad()
        {
            AssetBundleLoad.LoadAssetBundle(); // Loads the asset bundle containing the notification prefab and icons

            // Find the MSCLoader Canvas to parent the notification UI elements to
            GameObject ModLoaderCanvas = GameObject.Find("MSCLoader Canvas");

            //Set prefab parent to the canvas.
            AssetBundleLoad.NotificationPrefab.transform.SetParent(ModLoaderCanvas.transform, false); // Set parent without changing world position/rotation/scale

            // Add the NotificationManager component to the canvas
            _manager = ModLoaderCanvas.AddComponent<NotificationManager>();

            // Assign the required references to the NotificationManager
            _manager.notificationParent = ModLoaderCanvas.transform; //Setting the parent to the Canvas.
            _manager.notificationPrefab = AssetBundleLoad.NotificationPrefab;
            _manager.InfoIcon = AssetBundleLoad.InfoIcon;
            _manager.WarningIcon = AssetBundleLoad.WarningIcon;
            _manager.ErrorIcon = AssetBundleLoad.ErrorIcon;
            _manager.NotificationAudioSource = AssetBundleLoad.NotificationIcons.GetComponent<AudioSource>();

            // Set the singleton instance of the NotificationManager in the Notification class
            Notification._instance = _manager;
        }

        /// <summary>
        /// Called when the mod is loaded.  Applies settings to the notification manager.
        /// </summary>
        private void Mod_OnLoad()
        {
            // Apply the initial settings to the NotificationManager
            OnNotificationSpeedChange();
            OnNotificationDurationhange();
            OnNotificationSpacingChange();
            OnNotificationSoundChange();
            OnNotificationVolumeChange();
        }
    }
}