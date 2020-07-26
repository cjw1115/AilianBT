using Serilog;
using System;
using System.Runtime.CompilerServices;

namespace AilianBT.Services
{
    public class LogService
    {
        private LoggerConfiguration _loggerConfiguration = new LoggerConfiguration();
        private ILogger _logger;
        private AppCenterService _appCenterService;

        public LogService(string logFolder, AppCenterService appCenterService)
        {
            _appCenterService = appCenterService;

            var template = "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff}][{Level:u3}]{Message:lj}{NewLine}{Exception}";
#if DEBUG
            _loggerConfiguration.MinimumLevel.Debug();
            _loggerConfiguration.WriteTo.Debug(outputTemplate: template);
#else
            _loggerConfiguration.MinimumLevel.Information();
#endif
            var logFile = System.IO.Path.Combine(logFolder, "AilianBT_.log");
            _loggerConfiguration.WriteTo.File(logFile, outputTemplate: template, rollingInterval: RollingInterval.Day);
            _logger = _loggerConfiguration.CreateLogger();
        }

        private string _stackInfo(string filePath, string function)
        {
            return $"{System.IO.Path.GetFileName(filePath)}+<{function}>";
        }

        public void Debug(string messageTemplate, [CallerFilePath] string filePath = null, [CallerMemberName] string functionName = null)
        {
            _logger.Debug($"[{_stackInfo(filePath, functionName)}]::{messageTemplate}");
        }

        public void Warning(string messageTemplate, [CallerFilePath] string filePath = null, [CallerMemberName] string functionName = null)
        {
            _logger.Warning($"[{_stackInfo(filePath, functionName)}]::{messageTemplate}");
        }

        public void Error(string messageTemplate, Exception exception = null, [CallerFilePath] string filePath = null, [CallerMemberName] string functionName = null)
        {
            _logger.Error(exception, $"[{_stackInfo(filePath, functionName)}]::{messageTemplate}");
            if (exception != null)
            {
                _appCenterService.TrackError(new Exception(messageTemplate));
            }
        }

        public void Information(string messageTemplate, [CallerFilePath] string filePath = null, [CallerMemberName] string functionName = null)
        {
            _logger.Information($"[{_stackInfo(filePath, functionName)}]::{messageTemplate}");
        }
    }
}
