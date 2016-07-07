using System;

namespace InvertedTomato {
    public static class TimerExtensions {
        /// <summary>
        /// Stop the timer (if running) and start it.
        /// </summary>
        /// <param name="target"></param>
        public static void Reset(this System.Timers.Timer target) {
            if (null == target) {
                throw new ArgumentNullException("target");
            }

            target.Stop();
            target.Start(); // NullReferenceException can occur here. http://stackoverflow.com/questions/14630591/random-nullreferenceexception-on-timer-stop
        }

        /// <summary>
        /// Stop and start the timer only if running.
        /// </summary>
        /// <param name="target"></param>
        public static void Restart(this System.Timers.Timer target) {
            if (null == target) {
                throw new ArgumentNullException("target");
            }

            if (target.Enabled) {
                target.Reset();
            }
        }

        /// <summary>
        /// Stop timer if not null.
        /// </summary>
        /// <param name="target"></param>
        public static void StopIfNotNull(this System.Timers.Timer target) {
            if (null == target) {
                return;
            }

            target.Stop();
        }
    }
}
