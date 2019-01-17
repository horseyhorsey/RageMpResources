using GTANetworkAPI;
using RageMpBase;
using RageMpBase.Attributes;
using GTAN.Extensions;

namespace PlayerDeath.Server
{
    [Logger]
    public class PlayerDeath : RageScript
    {
        #region Fields
        #region Meta Settings
        private bool _showNotifications = true;
        private bool _wastedEnabled;
        private string _wastedTitle;
        private string _wastedMessage;
        private int _wastedDuration;
        private float _wastedTimeScale;
        private bool _notify_death;
        #endregion

        private Vector3 DefaultSpawnLocation = null; 
        #endregion

        public PlayerDeath()
        {            
        }

        #region Server Events
        /// <summary>
        /// Init the meta settings
        /// </summary>
        /// <param name="resourceName"></param>
        [ServerEvent(Event.ResourceStartEx)]
        public void ResourceStart(string resourceName)
        {
            if (resourceName == ResourceName)
            {
                Log("PlayerDeath initializing");

                //Spawning options
                NAPI.Server.SetAutoSpawnOnConnect(NAPI.Resource.GetSetting<bool>(this, "spawn_connected"));
                NAPI.Server.SetAutoRespawnAfterDeath(NAPI.Resource.GetSetting<bool>(this, "respawn_death"));

                var vec = NAPI.Resource.GetSetting<string>(this, "spawn_location");
                if (!string.IsNullOrWhiteSpace(vec))
                {
                    DefaultSpawnLocation = (new Vector3()).ConvertToVector3(vec);
                    NAPI.Server.SetDefaultSpawnLocation(DefaultSpawnLocation);
                    Log("Default spawn location set to {}", args: vec.ToString());
                }

                _showNotifications = NAPI.Resource.GetSetting<bool>(this, "show_notifications");

                //Set wasted options
                _wastedEnabled = NAPI.Resource.GetSetting<bool>(this, "wasted_enabled");
                _wastedTitle = NAPI.Resource.GetSetting<string>(this, "wasted_title");
                _wastedMessage = NAPI.Resource.GetSetting<string>(this, "wasted_message");
                _wastedDuration = NAPI.Resource.GetSetting<int>(this, "wasted_duration");
                _wastedTimeScale = NAPI.Resource.GetSetting<float>(this, "wasted_timescale");
                _notify_death = NAPI.Resource.GetSetting<bool>(this, "notify_death");
            }
        }

        /// <summary>
        /// Gets the real reason for death and notifies all players of the kill
        /// </summary>
        /// <param name="client"></param>
        /// <param name="killer"></param>
        /// <param name="reason"></param>
        [ServerEvent(Event.PlayerDeath)]
        public void OnPlayerDeath(Client client, Client killer, uint reason)
        {
            //Get reason and if kill themselves
            string realReason = GetDeathReason(reason);
            bool killedSelf = client == killer;

            NotifyDeath(killer.Name, client.Name, realReason, killedSelf);
        }

        /// <summary>
        /// Initializes client wasted settings
        /// </summary>
        /// <param name="client"></param>
        [ServerEvent(Event.PlayerConnected)]
        public void OnPlayerConnected(Client client)
        {
            NAPI.ClientEvent.TriggerClientEvent(client, "InitSettings",
                _wastedEnabled, _wastedDuration, _wastedTitle, _wastedMessage, _wastedTimeScale);
        }
        #endregion

        #region Remote Events
        /// <summary>
        /// Respawns the player
        /// </summary>
        /// <param name="client"></param>
        [RemoteEvent("WastedScreenFinished")]
        public void WastedScreenFinished(Client client)
        {
            if (client == null)
            {
                Log("wasted screen client not found.");
            }
            else
            {
                var name = client.SocialClubName;
                Log("respawning player {name}", args: name);
                NAPI.Player.SpawnPlayer(client, DefaultSpawnLocation);
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Gets the death reason from <see cref="DeathDicts.DeathReasons"/>
        /// </summary>
        /// <param name="reason"></param>
        /// <returns></returns>
        private string GetDeathReason(uint reason)
        {
            var realReason = string.Empty;
            if (!DeathDicts.DeathReasons.ContainsKey(reason))
                Log("failed to find death reason for {reason}", loglvl: RageLogLevel.Warning, args: reason);
            else
                realReason = DeathDicts.DeathReasons[reason];

            if (string.IsNullOrWhiteSpace(realReason))
                realReason = "killed";

            return realReason;
        }

        /// <summary>
        /// Notifies all players who killed who
        /// </summary>
        /// <param name="killerName"></param>
        /// <param name="victimName"></param>
        /// <param name="reason"></param>
        /// <param name="killedSelf"></param>
        private void NotifyDeath(string killerName, string victimName, string reason, bool killedSelf = false)
        {
            if (_notify_death)
            {
                if (killedSelf)
                    NAPI.Notification.SendNotificationToAll($"<C>{victimName}</C> {reason}");
                else
                    NAPI.Notification.SendNotificationToAll($"<C>{killerName}</C> {reason} <C>{victimName}</C>");
            }
        } 
        #endregion
    }
}