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

            SetupConfig(Config, Path.Combine("BepInEx", "data", ModName));
            SetupHarmony();
        }

        private void SetupConfig(ConfigFile config, string saveFolder, bool isSaveManager = false)
        {
            var dataFolder = Path.Combine("BepInEx", "data", ModName);

            if (!isSaveManager)
            {
                ConfigEnabled = config.Bind("General",
                   "Enabled",
                   true,
                   "Enables the mod.");
            }

            ConfigScoreRankAssetFolderPath = Config.Bind("General",
                 "ScoreRankAssetFolderPath",
                 Path.Combine(dataFolder, "Assets"),
                 "The location for all the Score Rank image assets.");
        }

        private void SetupHarmony()
        {
            // Patch methods
            _harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);

            LoadPlugin(ConfigEnabled.Value);
        }

        public static void LoadPlugin(bool enabled)
        {
            if (enabled)
            {
                bool result = true;
                // If any PatchFile fails, result will become false
                result &= Instance.PatchFile(typeof(ScoreRankPatch));
                result &= Instance.PatchFile(typeof(SongSelectScoreRankpatch));
                result &= Instance.PatchFile(typeof(CourseSelectPatch));
                if (result)
                {
                    ModLogger.Log($"Plugin {MyPluginInfo.PLUGIN_NAME} is loaded!");
                }
                else
                {
                    ModLogger.Log($"Plugin {MyPluginInfo.PLUGIN_GUID} failed to load.", LogType.Error);
                    // Unload this instance of Harmony
                    // I hope this works the way I think it does
                    Instance._harmony.UnpatchSelf();
                }
            }
            else
            {
                ModLogger.Log($"Plugin {MyPluginInfo.PLUGIN_NAME} is disabled.");
            }
        }

        private bool PatchFile(Type type)
        {
            if (_harmony == null)
            {
                _harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
            }
            try
            {
                _harmony.PatchAll(type);
#if DEBUG
                ModLogger.Log("File patched: " + type.FullName);
#endif
                return true;
            }
            catch (Exception e)
            {
                ModLogger.Log("Failed to patch file: " + type.FullName);
                ModLogger.Log(e.Message);
                return false;
            }
        }

        public static void UnloadPlugin()
        {
            Instance._harmony.UnpatchSelf();
            ModLogger.Log($"Plugin {MyPluginInfo.PLUGIN_NAME} has been unpatched.");
        }

        public static void ReloadPlugin()
        {
            // Reloading will always be completely different per mod
            // You'll want to reload any config file or save data that may be specific per profile
            // If there's nothing to reload, don't put anything here, and keep it commented in AddToSaveManager
            //SwapSongLanguagesPatch.InitializeOverrideLanguages();
            //TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.MusicData.Reload();
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