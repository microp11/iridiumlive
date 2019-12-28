/*
 * Code from: https://wellsb.com/csharp/aspnet/blazor-timer-navigate-programmatically/
 * 
 * */

using System;
using System.Diagnostics;
using System.Timers;

namespace IridiumLive.Services
{
    public class TimerG : Timer
    {
        //public Guid ElGuid { get; private set; } = Guid.NewGuid();

        public TimerG(double interval) : base (interval)
        {
        }
    }
    public class TimerService
    {
        private TimerG _timer;

        public void SetTimer(double interval)
        {
            _timer = new TimerG(interval);
            _timer.Elapsed += NotifyTimerElapsed;
            _timer.Enabled = true;
            //Debug.WriteLine("Timer Started. {0}", Thread.CurrentThread.ManagedThreadId);
            //Debug.WriteLine("creating timer {0}", _timer.ElGuid.ToString());
        }

        public event Action OnElapsed;

        private void NotifyTimerElapsed(Object source, ElapsedEventArgs e)
        {
            //Debug.WriteLine("Timer Elapsed. {0}", Thread.CurrentThread.ManagedThreadId);
            //Debug.WriteLine("desposing timer {0}", _timer.ElGuid.ToString());
            OnElapsed?.Invoke();
            _timer.Dispose();
        }
    }
}
