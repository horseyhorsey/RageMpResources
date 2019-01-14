using GTANetworkAPI;
using RageMpBase;
using RageMpBase.Attributes;
using System.Resources;
using GTAN.Extensions;

namespace Admin
{
    [Logger(RageLogLevel.Debug)]
    public class Admin : RageScript
    {
        #region Fields
        /// <summary>
        /// Localization of chat messages
        /// </summary>
        private ResourceManager _chatResource;

        #endregion

        #region Constructors

        public Admin()
        {
            _chatResource = Localization.ChatMessages.ResourceManager;
        }
        #endregion

        #region Server Events
        [ServerEvent(Event.PlayerConnected)]
        public void PlayerConnected(Client player)
        {
            //Change player name because they can change this in the menu.....
            var socialName = player.SocialClubName;
            Log($"Player login : {player.Name}, SocialName: {socialName} IP:{player.Address}, ping: {player.Ping}");

            //Login the player
            try
            {
                var log = NAPI.ACL.LoginPlayer(player, "");
                switch (log)
                {
                    case LoginResult.LoginSuccessful:
                    case LoginResult.LoginSuccessfulNoPassword:
                        player.SendChatMessage("SERVER", _chatResource.GetString("Login_Success") + " " + NAPI.ACL.GetPlayerAclGroup(player) + "~w~.");
                        break;

                    case LoginResult.WrongPassword:
                        var passMsg = _chatResource.GetString("Login_BadPassword");
                        player.SendChatMessage("SERVER", passMsg);
                        Log($"{passMsg}");
                        break;
                }
            }
            catch (System.Exception ex)
            {
                LogEx(ex, "Player connected");
            }
        }

        [ServerEvent(Event.PlayerDisconnected)]
        public void OnPlayerDisconnected(Client player, DisconnectionType type, string reason)
        {
            Log($"{player.Name} {_chatResource.GetString("Disconnect")} {reason}");
        }

        /// <summary>
        /// Loads the settings for this resource
        /// </summary>
        /// <param name="resource"></param>
        [ServerEvent(Event.ResourceStartEx)]
        public void ResourceStart(string resource)
        {
            if (resource == ResourceName)
            {
                Log("Admin resource started, populating settings.");

                //Set default messages from settings
                var defaultMessages = NAPI.Resource.GetSetting<bool>(this, "SetGlobalDefaultCommandMessages");                
                NAPI.Server.SetGlobalDefaultCommandMessages(defaultMessages);
                Log($"GlobalDefaultCommandMessages: {defaultMessages}");
            }            
        }
        #endregion

        #region Commands

        /// <summary>
        /// Send the skincmds as help to the player
        /// </summary>
        /// <param name="client"></param>
        [Command(ACLRequired = true, Group = "ahelp")]
        public void AdminHelp(Client client)
        {
            client.SendChatMessage("admin", _chatResource.GetString("AdminCmds"));
        }

        /// <summary>
        /// Print all commands
        /// </summary>
        /// <param name="sender"></param>
        [Command(ACLRequired = true, Alias = "acmds")]
        public void AdminCommands(Client sender)
        {
            var cmds = GetResourceCommands(sender);
            if (cmds?.Length > 0)
                sender.SendChatMessage(cmds);
        }

        #region World Options

        [Command(ACLRequired = true, Description = "Set weather 0 - 13")]
        public void SetWeather(Client sender, int newWeather)
        {
            NAPI.World.SetWeather((Weather)newWeather);
        }
        #endregion

        #region Player Manage

        /// <summary>
        /// Bans the target player
        /// </summary>
        /// <param name="client"></param>
        /// <param name="target"></param>
        /// <param name="reason"></param>
        [Command(GreedyArg = true, ACLRequired = true)]
        public void Ban(Client client, Client target, string reason)
        {
            var adminName = client.Name;
            var targetName = target.Name;
            Log("{adminName} has banned {targetName} for {reason}", loglvl: RageLogLevel.Information, 
                args: new object[] { adminName, targetName, reason });
            target.Ban(reason);
        }

        [Command(GreedyArg = true, ACLRequired = true)]
        public void Kick(Client sender, Client target, string reason)
        {
            NAPI.Player.KickPlayer(target, reason);
            Log($"{sender.Name} {_chatResource.GetString("Kicked")} {reason}");
        }

        [Command(ACLRequired = true)]
        public void Kill(Client sender, Client target)
        {
            if (NAPI.ACL.DoesPlayerHaveAccessToCommand(sender, "Kill"))
            {
                NAPI.Player.SetPlayerHealth(target, -1);
            }
            else { return; }
        }

        #endregion

        #region Account

        [Command(SensitiveInfo = true, ACLRequired = false)]
        public void Login(Client sender, string password)
        {
            Log($"user logging {sender.Name}");
            var logResult = NAPI.ACL.LoginPlayer(sender, password);
            switch (logResult)
            {
                case LoginResult.NoAccountFound:
                    sender.SendChatMessage("SERVER", _chatResource.GetString("Login_NoAccount"));
                    break;
                case LoginResult.LoginSuccessfulNoPassword:
                case LoginResult.LoginSuccessful:
                    sender.SendChatMessage("SERVER", _chatResource.GetString("Login_LogSuccess") + " " + NAPI.ACL.GetPlayerAclGroup(sender) + "~w~.");
                    break;
                case LoginResult.WrongPassword:
                    sender.SendChatMessage("SERVER", _chatResource.GetString("Login_Success"));
                    break;
                case LoginResult.AlreadyLoggedIn:
                    sender.SendChatMessage("SERVER", _chatResource.GetString("Login_AlreadyLogged"));
                    break;
                case LoginResult.ACLDisabled:
                    sender.SendChatMessage("SERVER", _chatResource.GetString("Login_ACLDisabled"));
                    break;
            }
        }

        [Command(ACLRequired = true)]
        public void Logout(Client sender)
        {
            if (NAPI.ACL.IsPlayerLoggedIn(sender))
            {
                NAPI.ACL.LogoutPlayer(sender);
                sender.SendChatMessage(_chatResource.GetString("Login_Logout"));
            }
        }
        #endregion

        #region Resources Management
        [Command(ACLRequired = true)]
        public void Start(Client sender, string resource)
        {
            if (!NAPI.Resource.DoesResourceExist(resource))
            {
                Log($"Loaded Resource: {NAPI.Resource.GetThisResource(this)}");
                sender.SendChatMessage("SERVER", _chatResource.GetString("ResourceNotFound") + resource + "\"");
            }
            else if (NAPI.Resource.IsResourceRunning(resource))
            {
                sender.SendChatMessage("SERVER", "~r~Resource \"" + resource + _chatResource.GetString("ResourceRunning"));
            }
            else
            {
                NAPI.Resource.StartResource(resource);
                sender.SendChatMessage("SERVER", "~g~Started resource \"" + resource + "\"");
            }
        }

        [Command(ACLRequired = true)]
        public void Stop(Client sender, string resource)
        {
            if (!NAPI.Resource.DoesResourceExist(resource))
            {
                sender.SendChatMessage("SERVER", _chatResource.GetString("ResourceNotFound") + resource + "\"");
            }
            else if (!NAPI.Resource.IsResourceRunning(resource))
            {
                sender.SendChatMessage("SERVER", "~r~Resource \"" + resource + _chatResource.GetString("ResourceNotRunning"));
            }
            else
            {
                NAPI.Resource.StopResource(resource);
                sender.SendChatMessage("SERVER", _chatResource.GetString("ResourceStopped") + resource + "\"");
            }
        }

        [Command(ACLRequired = true)]
        public void Restart(Client sender, string resource)
        {
            if (NAPI.Resource.DoesResourceExist(resource))
            {
                NAPI.Resource.StopResource(resource);
                NAPI.Resource.StartResource(resource);
                sender.SendChatMessage("SERVER", _chatResource.GetString("ResourceStopped") + resource + "\"");
            }
            else
            {
                sender.SendChatMessage("SERVER", _chatResource.GetString("ResourceNotFound") + resource + "\"");
            }
        }
        #endregion
        #endregion
    }
}