#pragma warning disable IDE0073

namespace osu.Framework.BellaFiora
{
    public class Globals
    {
        public static readonly bool SHOW_WINDOW = true;
        public static readonly bool USE_CUSTOM_GAME_HOST = true;
        public static readonly bool NO_INPUT_HANDLERS = false;
        public static readonly double DRAW_THREADS_HZ = 60f;
        public static readonly double UPDATE_THREADS_HZ = 60f; // no impact on Auto gameplay (takes longer to start a map tho)
        public static readonly double INPUT_THREADS_HZ = 60f; // no impact on Auto gameplay
        public static readonly double AUDIO_THREADS_HZ = 60f;
        public static readonly bool MUTE_MUSIC = false;
        public static readonly bool MUTE_EFFECTS = false;
        public static readonly uint MUSIC_PLAYBACK_RATE = 1;
        public static readonly bool THROW_IF_CANCELLED = false;
    }
}
