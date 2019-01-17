using System.Collections.Generic;
using RAGE;
using RAGE.Game;
using RageMpClientHelpers;

namespace PlayerDeath.Client
{
    public class PlayerDeath : Events.Script
    {
        #region Fields

        #region Wasted Screen
        private bool _wastedEnabled;
        private int _wastedDuration;
        private string _wastedTitle;
        private string _wastedMessage;
        private Scaleform _wastedScaleformMsg;
        #endregion 

        /// <summary>
        /// Time the wasted screen started
        /// </summary>
        int timeStarted = 0;

        #endregion

        #region Constructors
        public PlayerDeath()
        {
            Events.OnPlayerDeath += PlayerDied;
            Events.Add("InitSettings", OnInitSettings);
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Set up wasted screen settings
        /// </summary>
        /// <param name="args"></param>
        private void OnInitSettings(object[] args)
        {
            _wastedEnabled = (bool)args[0];
            _wastedDuration = (int)args[1];
            _wastedTitle = args[2].ToString();
            _wastedMessage = args[3].ToString();

            //Create scaleform for wasted message
            if (_wastedEnabled)
                _wastedScaleformMsg = ScaleformHelper.BigMessageShard($"~r~{_wastedTitle}", $"{_wastedMessage}");
        }

        /// <summary>
        /// Starts the wasted screen if enabled, which subscribes to the <see cref="Events.Tick"/>
        /// </summary>
        /// <param name="player"></param>
        /// <param name="reason"></param>
        /// <param name="killer"></param>
        /// <param name="cancel"></param>
        private void PlayerDied(RAGE.Elements.Player player, uint reason,
            RAGE.Elements.Player killer, Events.CancelEventArgs cancel)
        {
            if (_wastedEnabled)
            {
                if (_wastedScaleformMsg != null)
                {
                    this.timeStarted = RAGE.Game.Misc.GetGameTimer();
                    PlayerHelper.WastedStart();
                    Events.Tick -= OnTick;
                    Events.Tick += OnTick;
                }
            }
        }

        /// <summary>
        /// Shows the wasted message if enabled. <para/> UnSubscribes from the <see cref="Events.Tick"/> when duration has passed and let server know we're done
        /// </summary>
        /// <param name="nametags"></param>
        private void OnTick(List<Events.TickNametagData> nametags)
        {
            if (_wastedScaleformMsg != null && _wastedEnabled)
            {
                _wastedScaleformMsg.Render2D();
            }

            //Remove after duration finished
            if (Misc.GetGameTimer() > (this.timeStarted + _wastedDuration))
            {
                //Chat.Output("Clearing wasted screen");                
                PlayerHelper.WastedClear();
                Events.Tick -= OnTick;
                Events.CallRemote("WastedScreenFinished");
            }
        } 
        #endregion
    }
}
