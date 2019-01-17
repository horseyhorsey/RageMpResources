using GTANetworkAPI;
using RageMpBase;
using RageMpBase.Attributes;

namespace Money
{
    [Logger]
    public class Money : RageScript
    {
        /// <summary>
        /// Shared data variable
        /// </summary>
        private const string PLY_CASH = "PLY_CASH";

        private bool _onConnected = true;

        /// <summary>
        /// Draw cash hud display
        /// </summary>
        private bool _displayCash;

        #region Server Events

        [ServerEvent(Event.ResourceStartEx)]
        private void ResourceStarted(string resource)
        {
            if (resource == ResourceName)
            {
                _onConnected = NAPI.Resource.GetSetting<bool>(this, "on_connected");
                Log("on_connected: {_onConnected}", args: _onConnected);

                _displayCash = NAPI.Resource.GetSetting<bool>(this, "display_cash");
            }
        }

        [ServerEvent(Event.PlayerConnected)]
        public void OnPlayerConnected(Client client)
        {            
            if (_onConnected) // Init cash with $1069
                InitCash(client, 1069);
            else
                Log("skipped setting shared data.");
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Add cash to shared data
        /// </summary>
        /// <param name="client"></param>
        /// <param name="amount">The amount of cash to increment</param>
        public void AddCash(Client client, int amount)
        {
            int plyCash = GetCash(client) + amount;
            client.SetSharedData(PLY_CASH, plyCash);
            Log($"give {amount} to {client.SocialClubName}, new amount {plyCash}");
        }

        /// <summary>
        /// Draw cash hud?
        /// </summary>
        /// <param name="display"></param>
        public void DisplayCash(Client client, bool display) => NAPI.ClientEvent.TriggerClientEvent(client, "DisplayCash", display);

        /// <summary>
        /// Returns Shared Cash data
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public int GetCash(Client client) => client.GetSharedData(PLY_CASH);

        /// <summary>
        /// Initiliazes the shared data. This is invoked if _onConnected.
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public void InitCash(Client client, int amount = 0)
        {
            Log($"init cash for {client.SocialClubName} - ${amount}");
            client.SetSharedData(PLY_CASH, amount);
            DisplayCash(client, _displayCash);
        }
        #endregion

    }
}