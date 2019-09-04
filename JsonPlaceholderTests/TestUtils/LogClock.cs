using System;

namespace JsonPlaceholderTests.TestUtils
{
    public class LogClock
    {
        private DateTime _now = DateTime.Parse("2017-06-11 11:09:21");
        public DateTime Now => _now;

        public void Advance(TimeSpan amount) { _now += amount; }

        public void AddSec(int n = 1) { Advance(new TimeSpan(0,0,0,n)); }
        public void AddMin(int n = 1) { Advance(new TimeSpan(0,0,n,0)); }
        public void AddHour(int n = 1) { Advance(new TimeSpan(0,n,0,0)); }
        public void AddDay(int n = 1) { Advance(new TimeSpan(n,0,0,0)); }
    }
}