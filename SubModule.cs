using System;
using System.IO;
using TaleWorlds.MountAndBlade;
using TaleWorlds.InputSystem;
using HarmonyLib;
using MountandShardblade.Util;
using MountandShardblade.Shardblade;
using MountandShardblade.Shardplate;
using TaleWorlds.Core;
using TaleWorlds.ScreenSystem;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using Logger = MountandShardblade.Util.Logger;
using Path = System.IO.Path;
using TaleWorlds.CampaignSystem;

namespace MountandShardblade
{
    public class SubModule : MBSubModuleBase
    {
        private static string shardbladeID;
        private static InputKey shardbladeSummonKey;
        private static InputKey shardplateDashKey;
        private static double summonTimeSeconds;
        private static float playerShardplateHealthLimit;
        private static float aiShardplateHealthLimit;
        private static bool testingMode;
        private static bool testingModeOneSide;
        private static Harmony harmonyInstance;

        // Config fields mapped to match config.txt
        private const string ShardbladeWeaponIDField = "shardBladeWeaponID";
        private const string SummonShardbladeHotkeyField = "summonShardbladeHotkey";
        private const string ShardplateDashHotkeyField = "shardplateDashHotkey";
        private const string TimeToSummonShardbladeField = "timeToSummonShardblade";
        private const string PlayerShardplateHealthLimitField = "playerShardplateHealthLimit";
        private const string AIShardplateHealthLimitField = "aiShardplateHealthLimit";
        private const string TestingModeField = "testingMode";
        private const string TestingModeOneSideField = "testingModeOneSide";

        // Default values
        private const string DefaultSummonShardbladeKeyChar = "q";
        private const string DefaultShardplateDashKeyStr = "X1MouseButton";
        private const float DefaultPlateHealthLimit = 500f;
        private const float DefaultAIPlateHealthLimit = 750f;
        private const string DefaultShardbladeWeaponID = "vlandia_2hsword_1_t5";
        private const InputKey DefaultSummonShardbladeInputKey = InputKey.Q;
        private const InputKey DefaultShardplateDashInputKey = InputKey.X1MouseButton;
        private const double DefaultSummonBladeTimeSeconds = 1.5;
        private const float DefaultPlayerShardplateHealthLimit = 500f;
        private const float DefaultAIShardplateHealthLimit = 750f;
        private const bool DefaultTestingMode = false;
        private const bool DefaultTestingModeOneSide = false;

        // Path for log and config
        private static readonly string BasePath = Path.Combine(Environment.CurrentDirectory, "..", "..", "Modules", "MountandShardblade");
        private static readonly string LogFilePath = Path.Combine(BasePath, "log.txt");
        private static readonly string ConfigFilePath = Path.Combine(BasePath, "config.txt");
        public static Harmony Harmony = new Harmony("MountandShardblade");

        // Properties to expose important fields
        public static string ShardbladeID => shardbladeID;
        public static InputKey ShardbladeSummonKey => shardbladeSummonKey;
        public static InputKey ShardplateDashKey => shardplateDashKey;
        public static double SummonTimeSeconds => summonTimeSeconds;
        public static bool TestingMode => testingMode;

        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            harmonyInstance = new Harmony("com.mountandshardblade");

            try
            {
                // Ensure the Logs directory exists and set the log file path
                Logger.SetLogFilePath("log.txt");  // Set custom log file path if needed
                Logger.Instance().Log("MountandShardblade initialized.", LogSeverity.Info);
                if (!Directory.Exists(BasePath))
                {
                    Directory.CreateDirectory(BasePath);
                }

                Harmony.DEBUG = true;
                harmonyInstance.PatchAll();  // Apply Harmony patches
                ParseAndLoadConfig();        // Load configuration
                ShardSoundContainer.Initialize(); // Initialize shard sounds
                Logger.Instance().Log("MountandShardblade initialized with Harmony patches applied.", LogSeverity.Info);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
                Console.WriteLine("Stack Trace: " + ex.StackTrace);
                Logger.Instance().Log($"Error applying Harmony patches: {ex.Message}", LogSeverity.Error);
            }
        }

        /// <summary>
        /// Clean up patches when the module is unloaded.
        /// </summary>
        protected override void OnSubModuleUnloaded()
        {
            base.OnSubModuleUnloaded();
            try
            {
                harmonyInstance.UnpatchAll();  // Remove all Harmony patches
                Logger.Instance().Log("Harmony patches successfully removed.", LogSeverity.Info);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
                Console.WriteLine("Stack Trace: " + ex.StackTrace);
                Logger.Instance().Log($"Error unpatching Harmony patches: {ex.Message}", LogSeverity.Error);
            }
        }

        /// <summary>
        /// Initialize the mission behaviors for our custom logic.
        /// </summary>
        public override void OnMissionBehaviorInitialize(Mission mission)
        {
            base.OnMissionBehaviorInitialize(mission);

            // 🔥 Assign Player Character as a Shardbearer (Restoring Shardblade Logic)
            var agent = Agent.Main;
            if (agent != null && agent.GetComponent<ShardbladeAgentComponent>() == null)
            {
                // Adding ShardbladeAgentComponent to player character
                CharacterObject character = agent.Character as CharacterObject;
                float agentWealth = character?.HeroObject?.Gold ?? 10000;  // If the character is a hero, get their gold, otherwise default to 0
                agent.AddComponent(new ShardbladeAgentComponent(agent, agentWealth));
                Logger.Instance().Log("ShardbladeAgentComponent added to the player's agent at mission start.", LogSeverity.Info);
            }

            mission.AddMissionBehavior(new UnifiedMissionBehavior());
        }

        /// <summary>
        /// Parses the configuration file to load shardblade and shardplate settings.
        /// </summary>
        private void ParseAndLoadConfig()
        {
            var submoduleConfig = new Config(ConfigFilePath);

            shardbladeID = submoduleConfig.GetField("shardBladeWeaponID") ?? DefaultShardbladeWeaponID;
            string shardbladeKey = submoduleConfig.GetField("summonShardbladeHotkey");
            shardbladeSummonKey = ValidateHotkey(shardbladeKey, DefaultSummonShardbladeInputKey);

            string shardplateDashKeyField = submoduleConfig.GetField("shardplateDashHotkey");
            shardplateDashKey = ValidateHotkey(shardplateDashKeyField, DefaultShardplateDashInputKey);

            testingMode = bool.TryParse(submoduleConfig.GetField("testingMode"), out testingMode) && testingMode;
            testingModeOneSide = bool.TryParse(submoduleConfig.GetField("testingModeOneSide"), out testingModeOneSide) && testingModeOneSide;

            summonTimeSeconds = double.TryParse(submoduleConfig.GetField("timeToSummonShardblade"), out summonTimeSeconds) ? summonTimeSeconds : DefaultSummonBladeTimeSeconds;
            playerShardplateHealthLimit = float.TryParse(submoduleConfig.GetField("playerShardplateHealthLimit"), out playerShardplateHealthLimit) ? playerShardplateHealthLimit : DefaultPlayerShardplateHealthLimit;
            aiShardplateHealthLimit = float.TryParse(submoduleConfig.GetField("aiShardplateHealthLimit"), out aiShardplateHealthLimit) ? aiShardplateHealthLimit : DefaultAIPlateHealthLimit;

            ApplyTestingModeSettings();
        }

        /// <summary>
        /// Applies the settings for testing mode from the config file or defaults.
        /// </summary>
        private void ApplyTestingModeSettings()
        {
            if (bool.TryParse(new Config(ConfigFilePath).GetField("testingMode"), out bool testMode))
            {
                testingMode = testMode;
                Logger.Instance().Log($"Testing mode set to: {testingMode}", LogSeverity.Info);
            }
            else
            {
                testingMode = DefaultTestingMode;
                Logger.Instance().Log($"Invalid testingMode value. Using default: {DefaultTestingMode}", LogSeverity.Warning);
            }

            if (bool.TryParse(new Config(ConfigFilePath).GetField("testingModeOneSide"), out bool testModeOneSide))
            {
                testingModeOneSide = testModeOneSide;
                Logger.Instance().Log($"Testing mode one-side set to: {testingModeOneSide}", LogSeverity.Info);
            }
            else
            {
                testingModeOneSide = DefaultTestingModeOneSide;
                Logger.Instance().Log($"Invalid testingModeOneSide value. Using default: {DefaultTestingModeOneSide}", LogSeverity.Warning);
            }
        }

        /// <summary>
        /// Validates and parses a hotkey string from the config file.
        /// </summary>
        private InputKey ValidateHotkey(string keyString, InputKey defaultKey)
        {
            if (Enum.TryParse(keyString, out InputKey key) && !IsKeyInConflict(key))
            {
                return key;
            }
            Logger.Instance().Log($"Hotkey {keyString} is invalid or conflicts with other controls. Using default key: {defaultKey}", LogSeverity.Warning);
            return defaultKey;
        }

        /// <summary>
        /// Checks if the given key conflicts with other game or mod controls.
        /// </summary>
        private bool IsKeyInConflict(InputKey key)
        {
            // Example logic: Check if the key conflicts with another game or mod function
            return key == InputKey.X || key == InputKey.Z;  // Example conflict keys
        }
    }
}
