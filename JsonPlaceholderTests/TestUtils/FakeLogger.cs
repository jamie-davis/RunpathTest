using System;
using Microsoft.Extensions.Logging;

namespace JsonPlaceholderTests.TestUtils
{
    public class FakeLogger : ILogger
    {
        private readonly FakeLog _log;
        private readonly string _categoryName;

        public FakeLogger(FakeLog log, string categoryName)
        {
            _log = log;
            _categoryName = categoryName;
        }

        #region Implementation of ILogger
        
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            _log.Log(logLevel, eventId, state, exception, formatter, _categoryName);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return _log.IsEnabled(logLevel);
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return _log.BeginScope<TState>(state);
        }

        #endregion
    }

    public class FakeLogger<T> : ILogger<T>
    {
        private readonly FakeLog _log;
        private readonly string _categoryName;

        public FakeLogger(FakeLog log, string categoryName)
        {
            _log = log;
            _categoryName = categoryName;
        }

        #region Implementation of ILogger

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            _log.Log(logLevel, eventId, state, exception, formatter, _categoryName);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return _log.IsEnabled(logLevel);
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return _log.BeginScope<TState>(state);
        }

        #endregion
    }
}
