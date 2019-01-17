using RAGE;
using RAGE.Elements;
using System.Collections.Generic;

namespace Money
{
    public class Money : Events.Script
    {
        /// <summary>
        /// Client variable name, server is named different
        /// </summary>
        const string SET_CASH = "SET_CASH";
        const ulong SET_CASH_VAL = 5395921141301356805;
        private bool _displayCash;

        protected Player CurrentPlayer { get; }

        #region Constructor
        public Money()
        {
            //Init data variable for set_cash
            CurrentPlayer = Player.LocalPlayer;
            CurrentPlayer.SetData(SET_CASH, 0);
            DisplayHud(true);

            Events.EnableKeyDataChangeEvent = true;
            Events.OnEntityDataChangeByKey += DataChanged;

            Events.Add("DisplayCash", OnDisplayCash);

            Events.OnPlayerJoin += OnPlayerJoin;
            Events.OnPlayerSpawn += PlayerSpawned;
            Events.OnPlayerDeath += PlayerDeath;            
        }

        #endregion

        #region Virtual Methods
        /// <summary>
        /// Registers the <see cref="Events.Tick"/> to show money on the screen
        /// </summary>
        /// <param name="player"></param>
        protected void OnPlayerJoin(RAGE.Elements.Player player)
        {
            Chat.Output("Player joined");
            SetTickRender();
        }

        private void SetTickRender()
        {
            Events.Tick -= Render;
            Events.Tick += Render;
        }

        /// <summary>
        /// Unregisters the <see cref="Events.Tick"/> to show money
        /// </summary>
        /// <param name="player"></param>
        /// <param name="reason"></param>
        /// <param name="killer"></param>
        /// <param name="cancel"></param>
        protected virtual void PlayerDeath(RAGE.Elements.Player player, uint reason, RAGE.Elements.Player killer, Events.CancelEventArgs cancel)
        {
            Events.Tick -= Render;
        }

        /// <summary>
        /// Registers the <see cref="Events.Tick"/> to show money on the screen.
        /// </summary>
        /// <param name="cancel"></param>
        protected virtual void PlayerSpawned(Events.CancelEventArgs cancel)
        {
            Chat.Output("Player spawned");                  
            SetTickRender();
        }

        /// <summary>
        /// Renders the singleplayer cash using RAGE.Game.Ui.ShowHudComponentThisFrame((int)RAGE.Game.HudComponent.Cash)
        /// </summary>
        /// <param name="nametags"></param>
        public void Render(List<Events.TickNametagData> nametags)
        {            
            if (CurrentPlayer.GetData<bool>("DISPLAY_CASH"))
                RAGE.Game.Ui.ShowHudComponentThisFrame((int)RAGE.Game.HudComponent.Cash);
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Update cash when is adjusted server side
        /// </summary>
        /// <param name="key"></param>
        /// <param name="entity"></param>
        /// <param name="arg"></param>
        private void DataChanged(ulong key, Entity entity, object arg)
        {
            ///This variable name should be unknown to the client
            if (key == SET_CASH_VAL)
            {
                entity.SetData(SET_CASH, arg);
                UpdateCash((int)arg);
            }
        }

        /// <summary>
        /// Enables the hud
        /// </summary>
        /// <param name="enable"></param>
        private void DisplayHud(bool enable)
        {
            RAGE.Game.Ui.DisplayHud(enable);
        }

        /// <summary>
        /// Server setting whether cash is displayed. 
        /// </summary>
        /// <param name="args"></param>
        private void OnDisplayCash(object[] args)
        {
            bool.TryParse(args[0].ToString(), out var display);
            CurrentPlayer.SetData("DISPLAY_CASH", display);
            Chat.Output($"display cash {CurrentPlayer.GetData<bool>("DISPLAY_CASH")}");
        }

        /// <summary>
        /// Updates the single player cash stat
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        private bool UpdateCash(int amount)
        {
            var cashHash = RAGE.Game.Misc.GetHashKey($"SP0_TOTAL_CASH");
            return RAGE.Game.Stats.StatSetInt(cashHash, amount, false);
        }

        #endregion
    }
}