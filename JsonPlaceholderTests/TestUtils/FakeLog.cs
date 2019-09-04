using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using TestConsole.OutputFormatting;
using TestConsoleLib;

namespace JsonPlaceholderTests.TestUtils
{
    public class FakeLog
    {
        private object _lock = new object();
        private List<LoggedEntry> _logs = new List<LoggedEntry>();

        public LogClock Clock => _logClock;

        #region Inner classes

        public interface ILoggedEntry
        {
            DateTime Time { get; }
            LogLevel LogLevel { get; }
            string Message { get; }
            Exception Exception { get; }
            string ExceptionText { get; }
            string FormatMessage();
        }

        private class LoggedEntry : ILoggedEntry
        {
            public EventId EventId { get; }
            public LogLevel LogLevel { get; }
            public string Message { get; }
            public string ExceptionText { get; }
            public string FormatMessage()
            {
                var sb = new StringBuilder();

                sb.AppendFormat("[{0}] ", EventId);

                if (_currentScope != null)
                {
                    sb.AppendLine(_currentScope);
                }

                sb.AppendLine(Message);

                return sb.ToString();
            }

            public Exception Exception { get; }
            public DateTime Time { get; }
            public string CategoryName { get; }

            private readonly string _currentScope;

            internal LoggedEntry(EventId eventId, Scope currentScope, LogLevel logLevel, string message, string exceptionText, Exception exception, DateTime time, string categoryName)
            {
                EventId = eventId;
                LogLevel = logLevel;
                Message = message;
                ExceptionText = exceptionText;
                Exception = exception;
                Time = time;
                CategoryName = categoryName;
                _currentScope = currentScope == null ? null : currentScope.Describe();
            }
        }


        private class Scope : IDisposable
        {
            public object State { get; }
            private FakeLog _logger;
            private Scope _oldScope;

            public Scope(FakeLog logger, object state)
            {
                State = state;
                _logger = logger;

                _oldScope = _logger.SetScope(this);
            }

            #region IDisposable

            public void Dispose()
            {
                if (ReferenceEquals(_logger.CurrentScope, this))
                    _logger.SetScope(_oldScope);
            }

            #endregion

            public string Describe()
            {
                if (_oldScope == null) return State.ToString();

                return $"{_oldScope.Describe()} => {State}";
            }
        }

        private Scope CurrentScope { get { lock (_lock) return _currentScope; } }

        #endregion

        private Scope SetScope(Scope scope)
        {
            lock (_lock)
            {
                var oldScope = _currentScope;
                _currentScope = scope;
                return oldScope;
            }
        }

        private Scope _currentScope;
        private Func<LogLevel, bool> _logLevelFilter;
        private LogClock _logClock = new LogClock();

        public FakeLog(Func<LogLevel, bool> logLevelFilter)
        {
            _logLevelFilter = logLevelFilter;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter, string categoryName)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            var exceptionText = exception == null
                ? null
                : exception.ToString();
            var message = formatter(state, exception);

            lock (_lock)
                _logs.Add(new LoggedEntry(eventId, _currentScope, logLevel, message, exceptionText, exception, _logClock.Now, categoryName));
        }

        /// <summary>
        /// Get the logs recorded
        /// </summary>
        public IEnumerable<ILoggedEntry> Logs
        {
            get
            {
                List<ILoggedEntry> logs;
                lock (_lock)
                    logs = new List<ILoggedEntry>(_logs);

                return logs;
            }
        }

        public string Report
        {
            get
            {
                var output = new Output();

                var report = Logs.AsReport(rep => rep
                    .AddColumn(l => l.Time, cc => { })
                    .AddColumn(l => l.LogLevel, cc => { })
                    .AddColumn(l => l.FormatMessage(), cc => cc.Heading("Message"))
                    .AddColumn(l => l.ExceptionText, cc => { })
                );

                output.FormatTable(report);
                return output.Report;
            }
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return _logLevelFilter(logLevel);
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            var scope = new Scope(this, state);
            _currentScope = scope;
            return scope;
        }
    }
}