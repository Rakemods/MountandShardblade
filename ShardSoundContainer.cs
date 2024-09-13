using System.Collections.Generic;
using TaleWorlds.Engine;

using MountandShardblade.Core;
using MountandShardblade.Util;

namespace MountandShardblade
{
    static class ShardSoundContainer
    {
        private const string ShardplateBreakSound = "event:/mission/combat/shield/metal_broken";
        private static int ShardplateBreakSoundIndex;
        private const string ShardplateHitByShardbladeSound = "event:/mission/combat/impact/metal_weapon/metal_shield";
        private static int ShardplateHitByShardbladeSoundIndex;
        private const string ShardplateHitByHammerSound = "event:/mission/combat/impact/stone/metal_shield";
        private static int ShardplateHitByHammerSoundIndex;
        private const string ShardbladeSummonSound = "MountandShardblade/combat/shardblade/summon";
        private static int ShardbladeSummonSoundIndex;
        private static string[] ShardbladeSummonFearSounds = {
            "event:/voice/combat/male/02/fear",
            "event:/voice/combat/male/04/fear",
            "event:/voice/combat/male/05/fear",
            "event:/voice/combat/male/06/commands/retreat"
        };
        private static List<int> ShardbladeSummonFearSoundsIndex;

        public static void Initialize()
        {
            // Initialize shardplate break sound
            ShardplateBreakSoundIndex = LoadSoundEvent(ShardplateBreakSound, "ShardplateBreakSound");

            // Initialize shardplate hit by shardblade sound
            ShardplateHitByShardbladeSoundIndex = LoadSoundEvent(ShardplateHitByShardbladeSound, "ShardplateHitByShardbladeSound");

            // Initialize shardplate hit by hammer sound
            ShardplateHitByHammerSoundIndex = LoadSoundEvent(ShardplateHitByHammerSound, "ShardplateHitByHammerSound");

            // Initialize shardblade summon sound
            ShardbladeSummonSoundIndex = LoadSoundEvent(ShardbladeSummonSound, "ShardbladeSummonSound");

            // Initialize fear sounds when a Shardblade is summoned
            ShardbladeSummonFearSoundsIndex = new List<int>();
            for (int i = 0; i < ShardbladeSummonFearSounds.Length; i++)
            {
                int soundIndex = LoadSoundEvent(ShardbladeSummonFearSounds[i], $"ShardbladeSummonFearSound {i}");
                ShardbladeSummonFearSoundsIndex.Add(soundIndex);
            }
        }

        private static int LoadSoundEvent(string soundPath, string soundName)
        {
            int soundIndex = SoundEvent.GetEventIdFromString(soundPath);
            if (soundIndex == -1)
            {
                Logger.Instance().Log($"Couldn't load {soundName} from path: {soundPath}", LogSeverity.Info);
            }
            return soundIndex;
        }

        // Sound accessor methods
        public static int SoundCodeShardPlateBreak() => ShardplateBreakSoundIndex;

        public static int SoundCodeShardPlateHitByShardBlade() => ShardplateHitByShardbladeSoundIndex;

        public static int SoundCodeShardPlateHitByHammer() => ShardplateHitByHammerSoundIndex;

        public static int SoundCodeShardBladeSummon() => ShardbladeSummonSoundIndex;

        public static int SoundCodeShardBladeSummonFear(int i)
        {
            if (i >= ShardbladeSummonFearSoundsIndex.Count)
            {
                i %= ShardbladeSummonFearSoundsIndex.Count;
            }

            return ShardbladeSummonFearSoundsIndex[i];
        }
    }
}
