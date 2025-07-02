using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections;
using UnityEngine;
using BepInEx.Configuration;
using ScoreRanks.Patches;
using System.IO;
using DG.Tweening.Plugins.Core;

#if IL2CPP
using BepInEx.Unity.IL2CPP.Utils;
using BepInEx.Unity.IL2CPP;
#endif

namespace ScoreRanks
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, ModName, MyPluginInfo.PLUGIN_VERSION)]
#if MONO
    public class Plugin : BaseUnityPlugin
#elif IL2CPP
    public class Plugin : BasePlugin
#endif
    {
        public const string ModName = "ScoreRanks";

        public static Plugin Instance;
        private Harmony _harmony;
        public new static ManualLogSource Log;

        public ConfigEntry<bool> ConfigEnabled;

        public ConfigEntry<string> ConfigScoreRankAssetFolderPath;

#if MONO
        private void Awake()
#elif IL2CPP
        public override void Load()
#endif
        {
            Instance = this;

#if MONO
            Log = Logger;
#elif IL2CPP
            Log = base.Log;
#endif

            SetupConfig();
            SetupHarmony();
        }

        private void SetupConfig()
        {
            var dataFolder = Path.Combine("BepInEx", "data", ModName);

            ConfigEnabled = Config.Bind("General",
                "Enabled",
                true,
                "Enables the mod.");

            ConfigScoreRankAssetFolderPath = Config.Bind("General",
                 "ScoreRankAssetFolderPath",
                 Path.Combine(dataFolder, "Assets"),
                 "The location for all the Score Rank image assets.");
        }

        private void SetupHarmony()
        {
            // Patch methods
            _harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);

            if (ConfigEnabled.Value)
            {
                _harmony.PatchAll(typeof(ScoreRankPatch));
                _harmony.PatchAll(typeof(SongSelectScoreRankpatch));
                _harmony.PatchAll(typeof(CourseSelectPatch));
                Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_NAME} is loaded!");
            }
            else
            {
                Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_NAME} is disabled.");
            }

        }

        // I never used these, but they may come in handy at some point
        public static MonoBehaviour GetMonoBehaviour() => TaikoSingletonMonoBehaviour<CommonObjects>.Instance;

        public void StartCoroutine(IEnumerator enumerator)
        {
#if MONO
            GetMonoBehaviour().StartCoroutine(enumerator);
#elif IL2CPP
            GetMonoBehaviour().StartCoroutine(enumerator);
#endif
        }
    }
}