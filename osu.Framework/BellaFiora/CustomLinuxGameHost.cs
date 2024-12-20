#pragma warning disable IDE0073

using System.Runtime.Versioning;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using osu.Framework.Configuration;
using osu.Framework.Logging;
using osu.Framework.Threading;
using osu.Framework.Timing;
using System;
using osu.Framework.Input.Handlers;
using osu.Framework.Platform.Linux;

namespace osu.Framework.BellaFiora
{
    [SupportedOSPlatform("linux")]
    public class CustomLinuxGameHost : LinuxGameHost
    {
        public CustomLinuxGameHost(string gameName, HostOptions hostOptions) : base(gameName, hostOptions) { }

        private IFrameBasedClock? customClock;

        protected override IFrameBasedClock SceneGraphClock => customClock ?? base.SceneGraphClock;

        public override bool OpenFileExternally(string filename)
        {
            Logger.Log($"Application has requested file \"{filename}\" to be opened.");
            return true;
        }

        public override bool PresentFileExternally(string filename)
        {
            Logger.Log($"Application has requested file \"{filename}\" to be shown.");
            return true;
        }

        public override void OpenUrlExternally(string url) => Logger.Log($"Application has requested URL \"{url}\" to be opened.");

        // protected override Clipboard CreateClipboard() => new HeadlessClipboard();

        protected override void SetupConfig(IDictionary<FrameworkSetting, object> defaultOverrides)
        {
            defaultOverrides[FrameworkSetting.AudioDevice] = "No sound";

            base.SetupConfig(defaultOverrides);

            if (FrameworkEnvironment.StartupExecutionMode != null)
            {
                Config.SetValue(FrameworkSetting.ExecutionMode, FrameworkEnvironment.StartupExecutionMode.Value);
                Logger.Log($"Startup execution mode set to {FrameworkEnvironment.StartupExecutionMode} from envvar");
            }
        }

        protected override void SetupForRun()
        {
            // We want the draw thread to run, but it doesn't matter how fast it runs.
            // This limiting is mostly to reduce CPU overhead.
            // MaximumDrawHz = 60;

            // if (!realtime)
            // {
            customClock = new FramedClock(new FastClock(Globals.UPDATE_THREADS_HZ, Threads.ToArray()));

                // time is incremented per frame, rather than based on the real-world time.
                // therefore our goal is to run frames as fast as possible.
                // MaximumUpdateHz = MaximumInactiveHz = 0;
            // }
            // else
            // {
                // in realtime runs, set a sane upper limit to avoid cpu overhead from spinning.
                // MaximumUpdateHz = MaximumInactiveHz = 1000;
            // }

            base.SetupForRun();
        }

        protected override void UpdateFrame()
        {
            customClock?.ProcessFrame();

            base.UpdateFrame();
        }

        protected override IEnumerable<InputHandler> CreateAvailableInputHandlers() =>
            Globals.NO_INPUT_HANDLERS ? Array.Empty<InputHandler>() : base.CreateAvailableInputHandlers();

        private class FastClock : IClock
        {
            private readonly double increment;

            private readonly GameThread[] gameThreads;
            private readonly ulong[] gameThreadLastFrames;

            private readonly Stopwatch stopwatch = new Stopwatch();

            private double time;

            /// <summary>
            /// A clock which increments each time <see cref="CurrentTime"/> is requested.
            /// Run fast. Run consistent.
            /// </summary>
            /// <param name="increment">Milliseconds we should increment the clock by each time the time is requested.</param>
            /// <param name="gameThreads">The game threads.</param>
            public FastClock(double increment, GameThread[] gameThreads)
            {
                this.increment = increment;
                this.gameThreads = gameThreads;
                gameThreadLastFrames = new ulong[gameThreads.Length];
            }

            public double CurrentTime
            {
                get
                {
                    double realElapsedTime = stopwatch.Elapsed.TotalMilliseconds;
                    stopwatch.Restart();

                    if (allThreadsHaveProgressed)
                    {
                        for (int i = 0; i < gameThreads.Length; i++)
                            gameThreadLastFrames[i] = gameThreads[i].FrameIndex;

                        // Increment time at the expedited rate.
                        return time += increment;
                    }

                    // Fall back to real time to ensure we don't break random tests that expect threads to be running.
                    return time += realElapsedTime;
                }
            }

            private bool allThreadsHaveProgressed
            {
                get
                {
                    for (int i = 0; i < gameThreads.Length; i++)
                    {
                        if (gameThreads[i].FrameIndex == gameThreadLastFrames[i])
                            return false;
                    }

                    return true;
                }
            }

            public double Rate => 1;
            public bool IsRunning => true;
        }
    }
}
