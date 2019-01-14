using RAGE;
using RAGE.Elements;
using RageMpClientHelpers;

namespace Teleporter
{
    /// <summary>
    /// OsFlash
    /// </summary>
    public class Teleporter : Events.Script
    {
        private Player _player;

        public Teleporter()
        {
            _player = RAGE.Elements.Player.LocalPlayer;
            Events.Add("TpPlayerWaypoint", OnTeleportPlayer);
        }

        private void OnTeleportPlayer(object[] args)
        {
            TeleportToWaypoint();
        }

        /// <summary>
        /// Teleports the player to their map waypoint (purple) not route (yellow)
        ///  </summary>
        /// <remarks>
        /// Disables chat and fades out the screen
        /// </remarks>
        private void TeleportToWaypoint()
        {
            //Sprite 8 is the waypoint blip (purple)
            var waypointCord = RAGE.Game.Ui.GetFirstBlipInfoId(8);
            if (waypointCord > 0)
            {
                RAGE.Game.Player.StopPlayerTeleport();

                ChatHelper.EnableChat(false);
                CamHelper.SetPlayerLoading(true);

                //var gametime = RAGE.Game.Utils.Timestep();
                var g = RAGE.Game.Misc.GetGameTimer();
                Chat.Output($"Game timer: {g}");

                //Get waypoint vector
                var wpPosition = RAGE.Game.Ui.GetBlipInfoIdCoord(waypointCord);
                RAGE.Game.Entity.SetEntityCoords(_player.Handle, wpPosition.X, wpPosition.Y, 700f, true, false, false, true);

                //_player.FreezePosition(true);
                
                float z = 0f;
                wpPosition.Z = 1000f;
                bool groundFound = false;              

                RAGE.Game.Player.StartPlayerTeleport(wpPosition.X, wpPosition.Y, wpPosition.Z, 0f, true, true, false);

                for (int i = 0; i < 1000f; i++)
                {
                    Chat.Output($"{i}");
                    _player.Position.Z = wpPosition.Z;

                    if (RAGE.Game.Misc.GetGroundZFor3dCoord(wpPosition.X, wpPosition.Y,
                        i, ref z, false))
                    {
                        groundFound = true;                        
                        if(!RAGE.Game.Player.IsPlayerTeleportActive())
                            RAGE.Game.Player.StartPlayerTeleport(wpPosition.X, wpPosition.Y, z, 0f, true, true, false);
                        _player.Position.Z = wpPosition.Z;
                        Chat.Output($"ground found at ref {z}. count {i}");
                        break;
                    }

                    wpPosition.Z = z;
                    RAGE.Game.Invoker.Wait(25);                                 
                }

                if (!groundFound)
                {
                    _player.Position.Z = 1000;
                    Chat.Output($"~r~Teleported: {_player.Position.X}, {_player.Position.Y}, {_player.Position.Z}");
                }
                {
                    Chat.Output($"~r~Teleported: {_player.Position.X}, {_player.Position.Y}, {_player.Position.Z}");
                }

                CamHelper.SetPlayerLoading(false, 750);
                ChatHelper.EnableChat(true);
            }
            else
            {
                Chat.Output($"~r~Set a waypoint...");
            }
        }
    }
}
