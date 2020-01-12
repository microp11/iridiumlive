/*
 * Code from: https://wellsb.com/csharp/aspnet/blazor-timer-navigate-programmatically/
 * 
 * */

using System;
using System.Diagnostics;
using System.Timers;

namespace IridiumLive.Services
{
    public class TimerService
    {
        private Timer _timer;

        public void SetTimer(double interval)
        {
            try
            {
                _timer = new Timer(interval);
                _timer.Elapsed += NotifyTimerElapsed;
                _timer.Enabled = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public event Action OnElapsed;

        private void NotifyTimerElapsed(Object source, ElapsedEventArgs e)
        {
            try
            {
                OnElapsed?.Invoke();
                _timer.Dispose();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}
