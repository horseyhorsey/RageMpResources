using GTANetworkAPI;
using RageMpBase;
using RageMpBase.Attributes;
using System.Linq;
using GTAN.Extensions;

namespace Teleport
{
    [Logger]
    public class Teleport : RageScript
    {
        [ServerEvent(Event.ResourceStartEx)]
        public void ResourceStart(string resourceName)
        {
            if (ResourceName == "Teleport")
            {
                Log($"Resource started: {ResourceName}");
            }
        }

        #region Commands
        /// <summary>
        /// Send the skincmds as help to the player
        /// </summary>
        /// <param name="client"></param>
        [Command(ACLRequired = true)]
        public void TpHelp(Client client)
        {
            client.SendChatMessage($"{ResourceName}", "");            
        }

        /// <summary>
        /// Print all commands used for Teleport
        /// </summary>
        /// <param name="sender"></param>
        [Command(command:"tpcmd", Alias = "tpcmds")]
        public void TpCommands(Client client)
        {
            var cmds = GetResourceCommands(client);
            if (cmds?.Length > 0)
                client.SendChatMessage(cmds);
        }

        /// <summary>
        /// Triggers a client event to teleport to their Waypoint on the map
        /// </summary>
        /// <param name="sender"></param>
        [Command(command:"tpwp", ACLRequired = true, Alias = "telewp")]
        public void TeleportWaypoint(Client sender)
        {
            NAPI.ClientEvent.TriggerClientEvent(sender, "TpPlayerWaypoint", null);
            Log($"Sent teleport client to waypoint for {sender.Name}");
        }

        /// <summary>
        /// Triggers a client event to teleport to their Waypoint on the map
        /// </summary>
        /// <param name="sender"></param>
        [Command(command:"tele",ACLRequired = true, Alias = "tp", GreedyArg = true)]
        public void TeleportPlayer(Client sender, string x, string y, string z)
        {
            MoveClientPosition(sender, $"{x} {y} {z}");
        }

        /// <summary>
        /// Teleports a target Client if not the sender
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args">ClientName, Vector3</param>
        [Command(command: "tptarget", ACLRequired = true, Alias = "teletg", GreedyArg = true)]
        public void TeleportTarget(Client sender, string target, string x, string y, string z)
        {
            try
            {
                var targetClient = NAPI.Pools.GetAllPlayers().FirstOrDefault(o => o.Name == target);
                if (targetClient != null)
                {
                    if (targetClient != sender)
                    {
                        TeleportPlayer(targetClient, x, y, z);
                    }
                    else
                        Log($"Cannot teleport the same client using this command.");
                }
                else
                    Log("Cannot find target for {target}", loglvl: RageLogLevel.Warning, args: target);
            }
            catch (System.Exception ex)
            {
                LogEx(ex, "Errort teleporting target");
                throw;
            }
        }
        #endregion

        #region Private Methods

        private void MoveClientPosition(Client targetClient, string vec3Str)
        {
            Log($"moving client position: {targetClient.Name} - {vec3Str}");
            var vec3 = (new Vector3()).ConvertToVector3(vec3Str);            
            targetClient.MovePosition(vec3, 0);
        }

        #endregion
    }
}