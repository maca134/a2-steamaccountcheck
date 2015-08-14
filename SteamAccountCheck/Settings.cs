using System;
using System.IO;
using System.Reflection;

namespace SteamAccountCheck
{
    public class Settings
    {
        private readonly IniParser _ini;
        public enum LogModes
        {
            Text,
            Http
        }

        public Settings()
        {
            const string filename = "settings.ini";
            var settingsFile = Path.Combine(BasePath, filename);
            if (!File.Exists(settingsFile))
            {
                Logger.Log(String.Format("Settings file {0} does not exist. Creating default file.", settingsFile),
                    Logger.LogType.Warn, typeof(Settings));
                using (File.Create(settingsFile))
                {
                }
            }
            Logger.Log(String.Format("Loading settings from {0}", settingsFile), typeof(Settings));
            _ini = new IniParser(settingsFile);
            LoadSettings();
        }
        
        ~Settings()
        {
            SaveSettings();
        }

        public string BasePath
        {
            get { return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location); }
        }

        #region Properties
        private bool _showconsole;

        public bool ShowConsole
        {
            get { return _showconsole; }
        }

        private string _steamKey = "";

        public string SteamKey
        {
            get { return _steamKey; }
        }

        private string _serverName = "";

        public string ServerName
        {
            get { return _serverName; }
        }

        private string _steamAppId = "";

        public string SteamAppId
        {
            get { return _steamAppId; }
        }

        private string _httpurl = "";

        public string HttpUrl
        {
            get { return _httpurl; }
        }

        private bool _kickPlayer;

        public bool KickPlayer
        {
            get { return _kickPlayer; }
        }

        private string _kickMessage = "";

        public string KickMessage
        {
            get { return _kickMessage; }
        }

        private string _rconhost = "";

        public string RconHost
        {
            get { return _rconhost; }
        }

        private Int32 _rconport = 2302;

        public Int32 RconPort
        {
            get { return _rconport; }
        }

        private string _rconpass = "";

        public string RconPass
        {
            get { return _rconpass; }
        }
        #endregion

        private void LoadSettings()
        {
            try
            {
                _showconsole = _ini.GetBoolSetting("General", "ShowConsole");
            }
            catch
            {
                _showconsole = false;
            }

            try
            {
                _steamAppId = _ini.GetSetting("General", "SteamAppId");
            }
            catch
            {
                _steamAppId = "";
            }

            try
            {
                _steamKey = _ini.GetSetting("General", "SteamKey");
            }
            catch
            {
                _steamKey = "";
            }

            try
            {
                _serverName = _ini.GetSetting("General", "ServerName");
            }
            catch
            {
                _serverName = "";
            }

            try
            {
                _httpurl = _ini.GetSetting("General", "HttpLogUrl");
            }
            catch
            {
                _httpurl = "";
            }

            try
            {
                _kickPlayer = _ini.GetBoolSetting("BattlEye", "KickPlayer");
            }
            catch
            {
                _kickPlayer = false;
            }

            try
            {
                _kickMessage = _ini.GetSetting("BattlEye", "KickMessage");
            }
            catch
            {
                _kickMessage = "";
            }

            try
            {
                _rconhost = _ini.GetSetting("BattlEye", "Host");
            }
            catch
            {
                _rconhost = "";
            }

            try
            {
                _rconport = Convert.ToInt32(_ini.GetSetting("BattlEye", "Port"));
            }
            catch
            {
                _rconport = 2302;
            }

            try
            {
                _rconpass = _ini.GetSetting("BattlEye", "Password");
            }
            catch
            {
                _rconpass = "";
            }
        }

        private void SaveSettings()
        {
            _ini.SetSetting("General", "ShowConsole", _showconsole.ToString());
            _ini.SetSetting("General", "ServerName", _serverName);
            _ini.SetSetting("General", "SteamKey", _steamKey);
            _ini.SetSetting("General", "SteamAppId", _steamAppId);
            _ini.SetSetting("General", "HttpLogUrl", _httpurl);
            _ini.SetSetting("BattlEye", "KickPlayer", _kickPlayer.ToString());
            _ini.SetSetting("BattlEye", "KickMessage", _kickMessage);
            _ini.SetSetting("BattlEye", "Host", _rconhost);
            _ini.SetSetting("BattlEye", "Port", _rconport.ToString());
            _ini.SetSetting("BattlEye", "Password", _rconpass);
            _ini.Save();
            Logger.Log("Settings Saved", Logger.LogType.Info, typeof(Settings));
        }

    }
}
