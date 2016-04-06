#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: TimerToConsole.cs
// Date Created: 2016/04/06
// 
// Under the MIT License (MIT)
// 
// Written by Jon Smith : GitHub JonPSmith, www.thereformedprogrammer.net
// =====================================================
#endregion

using System;
using System.Diagnostics;
using System.Text;

namespace Tests.Helpers
{
    class TimerToConsole : IDisposable
    {
        private readonly string _message;
        private readonly int _numTimes;

        private readonly Stopwatch _timer;

        public TimerToConsole(string message, int numTimes = 0)
        {
            _message = message;
            _numTimes = numTimes;
            _timer = new Stopwatch();
            _timer.Start();
        }

        public void Dispose()
        {
            var timeInMs = (1000.0*_timer.ElapsedTicks)/Stopwatch.Frequency;
            var sb = new StringBuilder();
            sb.AppendFormat("{0} took {1:f2} ms to run {2}", _message, timeInMs, _numTimes);
            if (_numTimes > 0)
                sb.AppendFormat(". Each took {0:f3} ms ({1:#,#}/second)", timeInMs/_numTimes, (int)( _numTimes * 1000.0/timeInMs));
            Console.WriteLine(sb);
        }
    }
}
