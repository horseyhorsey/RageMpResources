using System;
using System.Linq;
using System.Threading;
using GTANetworkAPI;
using Newtonsoft.Json;
using RageMpBase;
using RageMpBase.Attributes;

namespace XpLevel
{
    [Logger]
    public class XpLevel : RageScript
    {
        #region Fields
        private Timer _timer;

        /// <summary>
        /// Max experience a Player can earn
        /// </summary>
        private int _maxExp;

        /// <summary>
        /// Maximum Level. (XpLevels.Length)
        /// </summary>
        private int _maxLevel;

        /// <summary>
        /// Shared data variable
        /// </summary>
        private const string PLY_LEVEL = "PLY_LEVEL";

        private bool _onConnected = true;
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes the max levels and experience
        /// </summary>
        public XpLevel()
        {
            _maxLevel = XpLevels.XpData.Length - 1;
            _maxExp = XpLevels.XpData[_maxLevel];            
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Initializes shared data for the player. Set the current XP which will update the levels
        /// </summary>
        /// <param name="client"></param>
        /// <param name="levels">The level the player is on</param>
        public void InitPlayer(Client client, int currentXp)
        {
            var name = client.SocialClubName;
            var levels = new int[] { 0, 0, 0, 0, 0 };

            SetPlayersRankData(client, levels);
            SetXp(client, currentXp);
            Log($"{PLY_LEVEL} set for {name} - {levels}", args: new object[] { name, levels });            
        }

        /// <summary>
        /// Increments XP and deals with any levelling up
        /// </summary>
        /// <param name="client"></param>
        /// <param name="xp"></param>
        public void AddXp(Client client, int xp)
        {
            var name = client.SocialClubName;
            Log("Adding Xp for {}", args: name);
            
            int[] ranks = GetSharedRankData(client);

            //Prev = currentXp
            ranks[1] = ranks[2];
            //Set current XP
            ranks[2] = Math.Clamp(ranks[1] + xp, 0, _maxExp);
             
            if (ranks[2] != ranks[1])
            {
                var calculatedLevel = GetLevelFromExp(ranks[2]);
                if (ranks[0] != calculatedLevel)
                {
                    ranks[0] = calculatedLevel;
                }

                var val = XpLevels.XpData[ranks[0] - 1] + ranks[0] > 1 ? 1 : 0;
                ranks[3] = XpLevels.XpData[val];
                ranks[4] = XpLevels.XpData[ranks[0]];
                //Log($"Limit {ranks[3]} Next Limit {ranks[4]}");
                SetPlayersRankData(client, ranks);
            }
        }

        /// <summary>
        /// Sets the CurrentXP level for the player and deals with levelling up.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="amount"></param>
        public void SetXp(Client client, int amount)
        {
            //Get previous shared data
            int[] ranks = GetSharedRankData(client);

            //prev > CurrentXp
            ranks[1] = ranks[2];
            ranks[2] = Math.Clamp(amount, 0, _maxExp);

            if (ranks[2] != ranks[1])
            {
                var calculatedLevel = GetLevelFromExp(ranks[2]);
                Log($"Calculated level {calculatedLevel}");

                //current level not calcedLevel
                if (ranks[0] != calculatedLevel)
                {
                    Log($"Levelled Up {calculatedLevel}");
                    ranks[0] = calculatedLevel;
                }


                var val = XpLevels.XpData[ranks[0] - 1] + ranks[0];
                ranks[3] = val;
                ranks[4] = XpLevels.XpData[ranks[0]];
                SetPlayersRankData(client, ranks);
            }
        }

        #endregion

        #region Server Events

        [ServerEvent(Event.ResourceStartEx)]
        private void ResourceStarted(string resource)
        {
            if(resource == ResourceName)
            {
                _onConnected = NAPI.Resource.GetSetting<bool>(this, "on_connected");
                Log("on_connected: {_onConnected}", args: _onConnected);
            }
        }

        [ServerEvent(Event.PlayerConnected)]
        public void PlayerConnected(Client client)
        {            
            if (_onConnected)
            {
                Log($"client connected: {client.Name}. Setting 0 levels/Xp");
                InitPlayer(client, 0);
            }
            else
                Log($"client connected");                
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Returns the Level based from the XP given
        /// </summary>
        /// <param name="xp"></param>
        /// <returns></returns>
        int GetLevelFromExp(int xp)
        {
            var xpIndex = Array.FindIndex(XpLevels.XpData, x => x >= xp);
            var clamped = Math.Clamp(xpIndex, -1, _maxLevel);
            Log("Get Level from Xp {xp}. clamped {clamped}", args: new object[] { xp, clamped });
            return clamped;
        }

        /// <summary>
        /// Get the shared data
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        private static int[] GetSharedRankData(Client client)
        {
            return JsonConvert.DeserializeObject<int[]>(client.GetSharedData(PLY_LEVEL));
        }       

        private void SetLevel(Client client, int newLevel)
        {
            int[] ranks = GetSharedRankData(client);
            //Set current Level
            var prevLevel = ranks[0];

            ranks[0] = Math.Clamp(newLevel, 0, _maxLevel);
            //Log($"Clamped level {ranks[0]}");

            //if (ranks[0] != prevLevel)
            //{

            //}

            ranks[1] = ranks[2];
            var val = XpLevels.XpData[ranks[0] - 1] + ranks[0] > 1 ? 1 : 0;
            ranks[2] = val;

            if (ranks[2] != ranks[1])
            {
                //Log("Current XP is not previous XP");
                ranks[3] = XpLevels.XpData[val];
                ranks[4] = XpLevels.XpData[ranks[0]];
                SetPlayersRankData(client, ranks);
            }
        }

        /// <summary>
        /// Sets the players ranks. 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="level"></param>
        private void SetPlayersRankData(Client client, int[] level)
        {
            client.SetSharedData(PLY_LEVEL, JsonConvert.SerializeObject(level));
            Log($"Set shared {PLY_LEVEL}, level: {level}", args: level);
        }
        #endregion
    }
}