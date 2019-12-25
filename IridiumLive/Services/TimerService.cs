/*
 * Code from: https://wellsb.com/csharp/aspnet/blazor-timer-navigate-programmatically/
 * 
 * */

using System;
using System.Timers;

namespace IridiumLive.Services
{
    public class TimerService
    {
        private Timer _timer;

        public void SetTimer(double interval)
        {
            _timer = new Timer(interval);
            _timer.Elapsed += NotifyTimerElapsed;
            _timer.Enabled = true;
            //Debug.WriteLine("Timer Started. {0}", Thread.CurrentThread.ManagedThreadId);
        }

        public event Action OnElapsed;

        private void NotifyTimerElapsed(Object source, ElapsedEventArgs e)
        {
            //Debug.WriteLine("Timer Elapsed. {0}", Thread.CurrentThread.ManagedThreadId);
            OnElapsed?.Invoke();
            _timer.Dispose();
        }

        public void KillTimer()
        {
            _timer.Stop();
            _timer.Dispose();
        }
    }
}
