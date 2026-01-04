#if !UNITY_ANDROID && !UNITY_IOS
using Discord;
#endif
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DiscordController : MonoBehaviour
{
    public long applicationID;
    [Space]
    public string largeImage = "";
    public string largeText = "Dreamwave";

#if !UNITY_ANDROID && !UNITY_IOS
    private Discord.Discord discord;
#endif
    private double songStartTime;

    private static bool instanceExists;
    public static DiscordController instance;

    private void Awake()
    {
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            // If running on mobile platforms, disable this GameObject
            this.gameObject.SetActive(false);
        }

        if (!instanceExists)
        {
            instanceExists = true;
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (FindObjectsOfType(GetType()).Length > 1)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
#if !UNITY_ANDROID && !UNITY_IOS
        discord = new Discord.Discord(applicationID, (ulong)Discord.CreateFlags.NoRequireDiscord);
        SceneManager.sceneLoaded += OnSceneLoaded;
#endif
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

#if !UNITY_ANDROID && !UNITY_IOS
        if (discord != null)
        {
            discord.Dispose();
        }
#endif
    }

    void Update()
    {
#if !UNITY_ANDROID && !UNITY_IOS
        try
        {
            discord.RunCallbacks();
        }
        catch
        {
            Destroy(gameObject);
        }
#endif
    }

    public void UpdateState(string details, string state)
    {
        try
        {
            var activityManager = discord.GetActivityManager();
            var activity = new Discord.Activity
            {
                Details = state,
                State = details,

                Assets =
                {
                    LargeImage = largeImage,
                    LargeText = largeText
                },

                Instance = false
            };

            activity.Timestamps = new ActivityTimestamps();

            activityManager = discord.GetActivityManager();
            activityManager.ClearActivity(_ =>
            {
                activityManager.UpdateActivity(activity, _ => { });
            });

            activityManager.UpdateActivity(activity, (res) =>
            {
                if (res != Discord.Result.Ok)
                    Debug.LogWarning("Failed to update Discord activity: " + res);
            });
        }
        catch (Exception ex)
        {
            Debug.LogError("Discord update failed: " + ex.Message);
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        
    }
}
