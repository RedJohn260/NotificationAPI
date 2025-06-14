using MSCLoader;
using UnityEngine;

namespace NotificationAPI
{
    /// <summary>
    /// Provides static methods for loading and unloading assets from an AssetBundle, specifically for the NotificationAPI.
    /// This class handles loading the notification prefab and icons from a pre-built AssetBundle.
    /// </summary>
    public static class AssetBundleLoad
    {
        /// <summary>
        /// The path to the AssetBundle file within the mod's resources.
        /// </summary>
        public static string bundlePath = "NotificationAPI.Assets.notificationapi.unity3d";

        /// <summary>
        /// The loaded AssetBundle object.
        /// </summary>
        public static AssetBundle Bundle { get; set; }

        /// <summary>
        /// The notification prefab loaded from the AssetBundle.  This prefab should contain the UI elements for the notification popup.
        /// </summary>
        public static GameObject NotificationPrefab { get; set; }

        /// <summary>
        /// A temporary GameObject containing the notification icons loaded from the AssetBundle.
        /// This is used to extract the individual icon sprites.
        /// </summary>
        public static GameObject NotificationIcons { get; set; }

        /// <summary>
        /// The sprite for the informational notification icon.
        /// </summary>
        public static Sprite InfoIcon;

        /// <summary>
        /// The sprite for the warning notification icon.
        /// </summary>
        public static Sprite WarningIcon;

        /// <summary>
        /// The sprite for the error notification icon.
        /// </summary>
        public static Sprite ErrorIcon;

        /// <summary>
        /// Loads the AssetBundle, instantiates the notification prefab and icons, and extracts the individual icon sprites.
        /// Call this method during mod initialization to load the necessary assets.
        /// </summary>
        public static void LoadAssetBundle()
        {
            // Load the AssetBundle from the specified path.
            Bundle = LoadAssets.LoadBundle(bundlePath);

            // Load the NotificationPopup prefab from the AssetBundle.
            GameObject gameObject1 = Bundle.LoadAsset("NotificationPopup.prefab") as GameObject;

            // Load the NotificationIcons prefab from the AssetBundle.
            GameObject gameObject2 = Bundle.LoadAsset("NotificationIcons.prefab") as GameObject;

            // Instantiate the NotificationPopup prefab in the scene.
            NotificationPrefab = UnityEngine.Object.Instantiate(gameObject1);

            // Instantiate the NotificationIcons prefab in the scene.
            NotificationIcons = UnityEngine.Object.Instantiate(gameObject2);

            // Extract the individual icon sprites from the NotificationIcons GameObject.
            InfoIcon = NotificationIcons.transform.Find("info").GetComponent<SpriteRenderer>().sprite;
            WarningIcon = NotificationIcons.transform.Find("warning").GetComponent<SpriteRenderer>().sprite;
            ErrorIcon = NotificationIcons.transform.Find("error").GetComponent<SpriteRenderer>().sprite;

            // Destroy the temporary GameObjects to clean up the scene.
            UnityEngine.Object.Destroy(gameObject1); // Destroy the original prefab (it was already instantiated)
            UnityEngine.Object.Destroy(gameObject2); // Destroy the original prefab (it was already instantiated)

        }

        /// <summary>
        /// Unloads the AssetBundle from memory.  Call this method when the mod is unloaded or disabled to release resources.
        /// </summary>
        public static void UnloadAssetBundle()
        {
            // Unload the AssetBundle, releasing all loaded assets.
            Bundle.Unload(false);
        }
    }
}