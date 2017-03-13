using System;
#if WINDOWS_UWP || WINDOWS_WSA
using Windows.Foundation.Diagnostics;
using MetroLog;
using MetroLog.Targets;
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
#elif !MONO && !UNITY
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

#if MONO
            Console.WriteLine($"{DateTime.Now:HH:mm:ss} {typeof(GALogger).FullName} {formattedMessage}");
#else
			switch(type)
			{
				case EGALoggerMessageType.Error:
					{
#if UNITY
                        UnityEngine.Debug.LogError(formattedMessage);
#elif WINDOWS_UWP || WINDOWS_WSA
                        logger.LogMessage(formattedMessage, LoggingLevel.Error);
                        log.Error(formattedMessage);
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
#else
                        logger.LogInformation(formattedMessage);
#endif
                    }
                    break;
			}
#endif
		}

#endregion // Private methods
	}
}

