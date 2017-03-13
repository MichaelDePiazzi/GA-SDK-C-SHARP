using System;
#if WINDOWS_UWP || WINDOWS_WSA
using Windows.Foundation.Diagnostics;
using MetroLog;
using MetroLog.Targets;
#elif MONO
using NLog;
using NLog.Config;
using NLog.Targets;
using GameAnalyticsSDK.Net.Device;
using System.IO;
#endif

namespace GameAnalyticsSDK.Net.Logging
{
	internal class GALogger
	{
#region Fields and properties

		private static readonly GALogger _instance = new GALogger();
		private bool infoLogEnabled;
		private bool infoLogVerboseEnabled;
#pragma warning disable 0649
		private static bool debugEnabled;
#pragma warning restore 0649
		private const string Tag = "GameAnalytics";

#if WINDOWS_UWP || WINDOWS_WSA
        private IFileLoggingSession session;
        private ILoggingChannel logger;
        private ILogger log;
#elif MONO
		private static ILogger logger;
#elif !UNITY
        private ILogger logger;
#endif

        private static GALogger Instance
		{
			get 
			{
				return _instance;
			}
		}

		public static bool InfoLog 
		{
			set 
			{
				Instance.infoLogEnabled = value;
			}
		}

		public static bool VerboseLog
		{
			set
			{
				Instance.infoLogVerboseEnabled = value;
			}
		}

#endregion // Fields and properties

		private GALogger()
		{
#if DEBUG
			debugEnabled = true;
#endif
#if WINDOWS_UWP || WINDOWS_WSA
            session = new FileLoggingSession("ga-session");
#if WINDOWS_UWP
            var options = new LoggingChannelOptions();
            logger = new LoggingChannel("ga-channel", options);
#else
            logger = new LoggingChannel("ga-channel");
#endif
            session.AddLoggingChannel(logger);

            LogManagerFactory.DefaultConfiguration.AddTarget(LogLevel.Trace, LogLevel.Fatal, new StreamingFileTarget());
            log = LogManagerFactory.DefaultLogManager.GetLogger<GALogger>();
#elif MONO
			logger = LogManager.GetCurrentClassLogger();
			var config = new LoggingConfiguration();

			var consoleTarget = new ColoredConsoleTarget();
			config.AddTarget("console", consoleTarget);

			var fileTarget = new FileTarget();
			config.AddTarget("file", fileTarget);

			consoleTarget.Layout = @"${date:format=HH\:mm\:ss} ${logger} ${message}";
			fileTarget.FileName = GADevice.WritablePath + Path.DirectorySeparatorChar + "ga_log.txt";
			fileTarget.Layout = "${message}";

			var rule1 = new LoggingRule("*", LogLevel.Debug, consoleTarget);
			config.LoggingRules.Add(rule1);

			var rule2 = new LoggingRule("*", LogLevel.Debug, fileTarget);
			config.LoggingRules.Add(rule2);

			LogManager.Configuration = config;
#endif
        }

        #region Public methods

        public static void I(String message)
		{
			if(!Instance.infoLogEnabled)
			{
				return;
			}

			string formattedMessage = "Info/" + Tag + ": " + message;
			Instance.SendNotificationMessage(message, formattedMessage, EGALoggerMessageType.Info);
		}

		public static void W(String message)
		{
			string formattedMessage = "Warning/" + Tag + ": " + message;
			Instance.SendNotificationMessage(message, formattedMessage, EGALoggerMessageType.Warning);
		}

		public static void E(String message)
		{
			string formattedMessage = "Error/" + Tag + ": " + message;
			Instance.SendNotificationMessage(message, formattedMessage, EGALoggerMessageType.Error);
		}

		public static void II(String message)
		{
			if(!Instance.infoLogVerboseEnabled)
			{
				return;
			}

			string formattedMessage = "Verbose/" + Tag + ": " + message;
			Instance.SendNotificationMessage(message, formattedMessage, EGALoggerMessageType.Info);
		}

		public static void D(String message)
		{
			if(!debugEnabled)
			{
				return;
			}

			string formattedMessage = "Debug/" + Tag + ": " + message;
			Instance.SendNotificationMessage(message, formattedMessage, EGALoggerMessageType.Debug);
		}

#endregion // Public methods

#region Private methods

		private void SendNotificationMessage(string message, string formattedMessage, EGALoggerMessageType type)
		{
            GameAnalytics.MessageLogged(message, type);

			switch(type)
			{
				case EGALoggerMessageType.Error:
					{
#if UNITY
                        UnityEngine.Debug.LogError(formattedMessage);
#elif WINDOWS_UWP || WINDOWS_WSA
                        logger.LogMessage(formattedMessage, LoggingLevel.Error);
                        log.Error(formattedMessage);
#elif MONO
						logger.Error(formattedMessage);
#else
                        logger.LogError(formattedMessage);
#endif
                    }
                    break;

				case EGALoggerMessageType.Warning:
					{
#if UNITY
                        UnityEngine.Debug.LogWarning(formattedMessage);
#elif WINDOWS_UWP || WINDOWS_WSA
                        logger.LogMessage(formattedMessage, LoggingLevel.Warning);
                        log.Warn(formattedMessage);
#elif MONO
						logger.Warn(formattedMessage);
#else
                        logger.LogWarning(formattedMessage);
#endif
                    }
                    break;

				case EGALoggerMessageType.Debug:
					{
#if UNITY
                        UnityEngine.Debug.Log(formattedMessage);
#elif WINDOWS_UWP || WINDOWS_WSA
                        logger.LogMessage(formattedMessage, LoggingLevel.Information);
                        log.Debug(formattedMessage);
#elif MONO
						logger.Debug(formattedMessage);
#else
                        logger.LogDebug(formattedMessage);
#endif
                    }
                    break;

				case EGALoggerMessageType.Info:
					{
#if UNITY
                        UnityEngine.Debug.Log(formattedMessage);
#elif WINDOWS_UWP || WINDOWS_WSA
                        logger.LogMessage(formattedMessage, LoggingLevel.Information);
                        log.Info(formattedMessage);
#elif MONO
						logger.Info(formattedMessage);
#else
                        logger.LogInformation(formattedMessage);
#endif
                    }
                    break;
			}
		}

#endregion // Private methods
	}
}

