#pragma warning disable IDE0073

namespace osu.Framework.BellaFiora
{
    public class Globals
    {
        public static readonly bool SHOW_WINDOW = true;
        public static readonly bool USE_FAST_GAME_HOST = true;
        public static readonly bool NO_INPUT_HANDLERS = false;
        public static readonly double CLOCK_RATE = 1f; // updates per second, does not take effect in gameplay
        public static readonly double FPS = 5f;
        public static readonly bool MUTE_MUSIC = false;
        public static readonly bool MUTE_EFFECTS = false;
        public static readonly uint MUSIC_PLAYBACK_RATE = 1;
        public static readonly bool THROW_IF_CANCELLED = false;
    }
}
