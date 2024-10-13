#pragma warning disable IDE0073

using osu.Framework.Logging;

namespace osu.Framework.BellaFiora
{
    public class Triggers
    {
        public static void OnDrawTrigger()
        {
            Logger.Log("OnDraw called");
        }
    }
}
