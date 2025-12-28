using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using System;

namespace Final_solo_project
{
    internal static class AudioManager
    {
        // ======== SFX ========
        private static SoundEffect sfxJump;
        private static SoundEffect sfxIperJump;
        private static SoundEffect sfxTrap;
        private static SoundEffect sfxEnemyKillMelee;
        private static SoundEffect sfxEnemyKillRanged;
        private static SoundEffect sfxBreakingPlatform;
        private static SoundEffect sfxGameOver;

        // ======== SETTINGS ========
        public static bool SfxEnabled { get; set; } = true;

        /// <summary> Volume globale SFX (0..1) </summary>
        public static float MasterSfx { get; set; } = 0.8f;

        // ======== PARAMETRI PER OGNI SUONO ========
        // Jump
        public static float JumpVolume = 0.25f;
        public static float JumpPitch = 0f;
        public static float JumpPan = 0f;

        // Hyper jump
        public static float IperJumpVolume = 0.55f;
        public static float IperJumpPitch = 0f; // più “acuto”
        public static float IperJumpPan = 0f;

        // Trap
        public static float TrapVolume = 0.7f;
        public static float TrapPitch = -0.1f; // più “grave”
        public static float TrapPan = 0f;

        // Enemy melee
        public static float EnemyMeleeVolume = 0.5f;
        public static float EnemyMeleePitch = 0f;
        public static float EnemyMeleePan = 0f;

        // Enemy ranged
        public static float EnemyRangedVolume = 0.4f;
        public static float EnemyRangedPitch = 0.05f;
        public static float EnemyRangedPan = 0f;

        // Break platform
        public static float BreakPlatformVolume = 0.5f;
        public static float BreakPlatformPitch = 0f;
        public static float BreakPlatformPan = 0f;

        // Game over
        public static float GameOverVolume = 0.7f;
        public static float GameOverPitch = 0f;
        public static float GameOverPan = 0f;

        // ======== LOAD ========
        public static void LoadContent(ContentManager content)
        {
            sfxJump = content.Load<SoundEffect>("Audio/Sound_jump");
            sfxIperJump = content.Load<SoundEffect>("Audio/Sound_ipersalto");
            sfxTrap = content.Load<SoundEffect>("Audio/Sound_Trap");
            sfxEnemyKillMelee = content.Load<SoundEffect>("Audio/Sound_kill_enemy_melee");
            sfxEnemyKillRanged = content.Load<SoundEffect>("Audio/Sound_kill_enemy_ranged");
            sfxBreakingPlatform = content.Load<SoundEffect>("Audio/Sound_breaking_platfprm2");
            sfxGameOver = content.Load<SoundEffect>("Audio/Sound_game_over");
        }

        // ======== CORE ========
        private static void PlaySfx(
            SoundEffect sfx,
            float volume,
            float pitch,
            float pan)
        {
            if (!SfxEnabled || sfx == null) return;

            float finalVolume = Clamp01(volume) * Clamp01(MasterSfx);
            sfx.Play(finalVolume, Clamp(pitch, -1f, 1f), Clamp(pan, -1f, 1f));
        }

        private static float Clamp01(float v)
            => MathF.Max(0f, MathF.Min(1f, v));

        private static float Clamp(float v, float min, float max)
            => MathF.Max(min, MathF.Min(max, v));

        // ======== PLAY METHODS ========
        public static void PlayJump()
            => PlaySfx(sfxJump, JumpVolume, JumpPitch, JumpPan);

        public static void PlayIperJump()
            => PlaySfx(sfxIperJump, IperJumpVolume, IperJumpPitch, IperJumpPan);

        public static void PlayTrap()
            => PlaySfx(sfxTrap, TrapVolume, TrapPitch, TrapPan);

        public static void PlayEnemyKillTop()
            => PlaySfx(sfxEnemyKillMelee, EnemyMeleeVolume, EnemyMeleePitch, EnemyMeleePan);

        public static void PlayEnemyKillBullet()
            => PlaySfx(sfxEnemyKillRanged, EnemyRangedVolume, EnemyRangedPitch, EnemyRangedPan);

        public static void PlayBreakingPlatform()
            => PlaySfx(sfxBreakingPlatform, BreakPlatformVolume, BreakPlatformPitch, BreakPlatformPan);

        public static void PlayGameOver()
            => PlaySfx(sfxGameOver, GameOverVolume, GameOverPitch, GameOverPan);
    }
}
