using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RGiesecke.DllExport;

namespace SteamAccountCheck
{
    public class DllEntry
    {
        private static Settings _settings;

        [DllExport("_RVExtension@12", CallingConvention = CallingConvention.Winapi)]
        public static void RVExtension(StringBuilder output, int outputSize, string function)
        {
            outputSize--;
            if (_settings == null)
            {
                _settings = new Settings();
                if (_settings.ShowConsole)
                    ConsoleHelper.CreateConsole();
            }
            Task.Factory.StartNew(() =>
            {
                ParseRequest(function);
            });
        }

        public static void ParseRequest(string request)
        {
            if (_settings.SteamKey == "")
            {
                Logger.Log("Steam key is empty", Logger.LogType.Error);
                return;
            }

            var playerid = "";
            var name = "";

            try
            {
                var data = request.Split('\n');
                playerid = data[0];
                name = data[1];
            }
            catch (Exception ex)
            {
                Logger.Log(String.Format("Error parsing request from ARMA: {0} - {1}", request, ex.Message), Logger.LogType.Error);
                return;
            }

            if (playerid == "" || name == "")
            {
                Logger.Log(String.Format("Error parsing request from ARMA: {0}", request), Logger.LogType.Error);
                return;
            }

            var url = string.Format("http://api.steampowered.com/IPlayerService/IsPlayingSharedGame/v0001/?key={0}&steamid={1}&appid_playing={2}&format=json",
                _settings.SteamKey,
                playerid,
                _settings.SteamAppId);

            var json = HttpGet(url);

            dynamic steamData;

            try
            {
                steamData = JsonConvert.DeserializeObject(json);
            }
            catch (Exception ex)
            {
                Logger.Log(String.Format("Error parsing request from Steam: {0} - {1}", json, ex.Message), Logger.LogType.Error);
                return;
            }

            Console.WriteLine(steamData);

            var lender = "0";
            try
            {
                lender = steamData.response.lender_steamid;
            }
            catch (Exception ex)
            {
                Logger.Log(String.Format("Error parsing request from Steam: {0} - {1}", json, ex.Message), Logger.LogType.Error);
                return;
            }

            if (lender == "0")
            {

                Logger.Log(String.Format("Player {0} is not using a shared account", name), Logger.LogType.Debug);
                return;
            }

            var _log = string.Format("Player {0} ({1}) is using a shared account. Original: {2}", name, playerid, lender);
            Logger.Log(_log, Logger.LogType.Warn);

            if (_settings.HttpUrl != "")
            {
                try
                {
                    using (var client = new WebClient())
                    {
                        client.UploadValues(_settings.HttpUrl, new NameValueCollection()
                        {
                            { "current_steamid", playerid },
                            { "parent_steamid", lender },
                            { "log", _log },
                            { "server", _settings.ServerName },
                            { "type", "SteamIdCheck" }
                        });
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log(String.Format("Error sending log to http url: {0}", ex.Message), Logger.LogType.Error);
                }
            }

            /*if (_settings.KickPlayer)
            {
                // to do

            }*/
        }

        public static string HttpGet(string url)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(url);
                var response = (HttpWebResponse)request.GetResponse();
                var stream = response.GetResponseStream();
                var reader = new StreamReader(stream);
                var data = reader.ReadToEnd();
                reader.Close();
                stream.Close();
                return data;
            }
            catch (Exception ex)
            {
                Logger.Log(String.Format("Error downloading Steam data: {0}", ex.Message));
            }

            return null;
        }
    }
}
