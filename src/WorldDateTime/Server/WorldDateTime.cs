using System;
using GTANetworkAPI;
using RageMpBase;
using RageMpBase.Attributes;
using GTAN.Extensions;

namespace WorldTimeServer
{
    [Logger]
    public class WorldDateTime : RageScript
    {
        public WorldDateTime() { }        

        #region Server Events
        /// <summary>
        /// Starts the timer for the games World time
        /// </summary>
        /// <param name="resourceName"></param>
        [ServerEvent(Event.ResourceStartEx)]
        public void ResourceStart(string resourceName)
        {            
            //Load the settings for WorldTimeServer
            if (resourceName == ResourceName)
            {
                if (NAPI.Resource.HasSetting(this, "time_multiplier"))
                {
                    LoadSettings();
                }
                else
                    throw new NullReferenceException("Can't find timer multiplier setting");
            }
        }

        /// <summary>
        /// Dispose of the timer when resource stopped
        /// </summary>
        [ServerEvent(Event.ResourceStop)]
        public void ResourceStop()
        {
            NAPI.World.DisposeWorldTimer();            
        }
        #endregion

        /// <summary>
        /// Set the time with extension
        /// </summary>
        /// <param name="client"></param>
        /// <param name="hours"></param>
        /// <param name="minutes"></param>
        [Command(ACLRequired = true, Alias = "settime")]
        public void SetTimeCommand(Client client, int hours, int minutes)
        {
            var name = client?.Name ?? string.Empty;
            Log("Time set to {hours}:{minutes} by {name}", args: new object[] { hours, minutes, name });
            NAPI.World.SetTime(hours, minutes);
            client.SendChatMessage($"Time set on next tick: {hours}:{minutes}");
        }

        #region Private Methods

        /// <summary>
        /// Parse settings from meta.xml
        /// </summary>
        private void LoadSettings()
        {
            var settingMultiplier = NAPI.Resource.GetSetting<double>(this, "time_multiplier");
            var tickRate = NAPI.World.StartWorldTimer(settingMultiplier);
            var time = NAPI.Resource.GetSetting<string>(this, "time").Split(':');
            if (NAPI.Resource.GetSetting<bool>(this, "enable_start_time"))
            {
                var strTime = NAPI.Resource.GetSetting<string>(this, "time").Split(':');
                int.TryParse(strTime[0], out var hrs);
                int.TryParse(strTime[1], out var mins);
                NAPI.World.SetTime(hrs, mins);
            }

            Log("Time multiplier: {settingMultiplier}, Seconds per game min: {tickRate}", args:new object[] { settingMultiplier, tickRate });
        }
        #endregion
    }
}