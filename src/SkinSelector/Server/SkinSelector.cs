using GTANetworkAPI;
using GTAN.Data;
using GTAN.Extensions;
using RageMpBase;
using RageMpBase.Attributes;
using System;
using System.Threading.Tasks;

namespace SkinSelector
{
    [Logger]
    public class SkinSelector : RageScript
    {
        #region Fields
        private int _defaultOutfit;
        private uint _defaultModelHash;
        private bool _setDefaultSkin;
        private bool _showSelectorOnConnect;
        #endregion

        #region Constructors
        public SkinSelector()
        {
            Log("Initializing multiplayer outfits..");
            var mCnt = MpOutFitDicts.MaleOutfits.Count;
            var fCnt = MpOutFitDicts.FemaleOutfits.Count;
            Log($"Male count {mCnt} | Female count {fCnt}");            
        } 
        #endregion

        #region Public Methods
        /// <summary>
        /// Changes the model on the Client.<para/>
        /// EXPORTED Example: NAPI.Exported.SkinMenu.ChangeModel(client, 1885233650); 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="hash"></param>
        /// <returns>True if successful no error</returns>
        public bool ChangeModel(Client client, uint hash)
        {
            try
            {
                var name = client.Name;
                Log("skin req: {hash}", args:hash);
                NAPI.Task.Run(() =>
                {
                    client.SetSkin(hash);
                    Log("{namey} Set skin successfull. {hash}",  args: new object[] { name, hash });
                });

                return true;
            }
            catch (Exception ex)
            {
                LogEx(ex, "Error changing skin. {hash}");
                return false;
            }
        }
        #endregion

        #region Server Events

        [ServerEvent(Event.PlayerConnected)]
        private Task OnPlayerConnected(Client client)
        {
            return Task.Run(() =>
            {
                if (_showSelectorOnConnect)
                {
                    Log("triggered client event: ShowSkinSelector");
                    NAPI.ClientEvent.TriggerClientEvent(client, "ShowSkinSelector");
                    return;
                }

                if (_setDefaultSkin)
                {
                    Log("connected: setting default skin from settings hash {_defaultModelHash}", args: _defaultModelHash);

                    NAPI.Task.Run(() =>
                    {
                        NAPI.Player.SetPlayerSkin(client, _defaultModelHash);

                        if (_defaultOutfit > 0)
                        {
                            Log("changing outfit to {_defaultOutfit}", args: _defaultOutfit);
                            if (client.Model == (uint)PedHash.FreemodeFemale01)
                                client.ChangeOutfit(_defaultOutfit, false);
                            else
                                client.ChangeOutfit(_defaultOutfit, false);                            
                        }
                        else
                        {
                            Log("No outfit selected. id: {_defaultOutfit}", args: _defaultOutfit);
                        }
                    });
                }
            });         
        }

        /// <summary>
        /// Gets the default skin settings from meta.xml.
        /// </summary>
        /// <param name="resourceName"></param>
        [ServerEvent(Event.ResourceStartEx)]
        private void OnResourceStartEx(string resourceName)
        {            
            if (resourceName == this.GetType().Name)
            {
                if (NAPI.Resource.HasSetting(this, "use_setting_skin"))
                {
                    Log("Loading skin from resource");
                    _setDefaultSkin = NAPI.Resource.GetSetting<bool>(this, "use_setting_skin");
                    if (_setDefaultSkin)
                    {
                        var model = NAPI.Resource.GetSetting<string>(this, "skin");
                        _defaultOutfit = NAPI.Resource.GetSetting<int>(this, "outfit");

                        _defaultModelHash = NAPI.Util.GetHashKey("mp_freemode_male");
                        if (_defaultModelHash == 0)
                            throw new NullReferenceException("Couldn't set default skin, check model name");                        
                    }
                }
                else
                    Log("No use_setting_skin found", loglvl: RageLogLevel.Warning);

                if (NAPI.Resource.HasSetting(this, "show_on_connect"))
                {
                    _showSelectorOnConnect = NAPI.Resource.GetSetting<bool>(this, "show_on_connect");                        
                }
            }
        }
        #endregion

        #region Commands

        /// <summary>
        /// Change outfit on command <para/>
        /// EXPORTED
        /// </summary>
        /// <param name="client"></param>
        /// <param name="outfitid"></param>
        [Command("outfit")]
        public void ChangeOutfit(Client client, int outfitid = 0)
        {
            var name = client.Name;

            if (client.Model == (uint)PedHash.FreemodeMale01)
            {
                Log("changeoufit {name} Id {outfitid}", args: new object[] { name, outfitid });
                client.ChangeOutfit(outfitid);
            }
            else if (client.Model == (uint)PedHash.FreemodeFemale01)
            {
                Log("changeoufit {name} Id {outfitid}",  args: new object[] { name, outfitid });
                client.ChangeOutfit(outfitid, false);
            }
            else
            {
                client.SendChatMessage("~r~Skin model needs to be set to freemode model");
            }
        }

        /// <summary>
        /// Send the skincmds as help to the player
        /// </summary>
        /// <param name="client"></param>
        [Command(Group = "skinhelp")]
        public void Help(Client client)
        {
            client.SendChatMessage("Skin-Help", "Use /skincmds or Interactive menu");
        }

        /// <summary>
        /// Print all commands used for SkinSelector
        /// </summary>
        /// <param name="sender"></param>
        [Command(Alias = "skincmds")]
        public void SkinCommands(Client client)
        {            
            var cmds = GetResourceCommands(client);            
            if (cmds?.Length > 0)
                client.SendChatMessage(cmds);
        }

        #endregion        

        #region Remote Events

        /// <summary>
        /// Invoked when client closes the skin selector menu. Only changes player model if a ped was selected
        /// </summary>
        /// <param name="client"></param>
        /// <param name="args"></param>
        [RemoteEvent("ClientMenuClosed")]
        public void OnClientMenuClosed(Client client, params object[] args)
        {
            try
            {
                Log($"selected Model: {args[0]}");

                var outfit = (int)args[1];
                if (outfit != 0)
                {
                    Log("player selected an outfit, we've already changed the model");
                }
                else
                {
                    Log("changing model player model");
                    uint.TryParse((string)args[0], out var model);
                    ChangeModel(client, model);
                }
            }
            catch (Exception ex)
            {
                Log($"skin req: {args}");
                LogEx(ex, "Error changing skin.");
            }
        }

        /// <summary>
        /// Client requests an outfit change with outfit ID
        /// </summary>
        /// <param name="client"></param>
        /// <param name="args"></param>
        [RemoteEvent("ChangeOutfit")]
        public void OnOutfitChanged(Client client, params object[] args)
        {
            int id = (int)args[0];
            bool gender = Convert.ToBoolean((int)args[1]);
            Log("Changing Outfit. Gender male: {gender}, Id: {id}", args: new object[] { gender, id });

            client.ChangeOutfit(id, gender);
        }
        #endregion
    }
}