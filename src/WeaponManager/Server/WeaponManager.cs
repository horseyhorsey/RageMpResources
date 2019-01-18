using System.Collections.Generic;
using System.IO;
using System.Linq;
using GTAN.Data;
using GTANetworkAPI;
using RageMpBase;
using GTAN.Extensions;
using System;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WeaponManager
{
    public class WeaponManager : RageScript
    {
        private Dictionary<string, WeaponData> _weaponData;

        public WeaponManager()
        {
            InitWeaponData();
        }

        /// <summary>
        /// Gets the weapon data from bridge folder
        /// </summary>
        private void InitWeaponData()
        {            
            var json = string.Empty;
            try
            {
                json = Path.Combine(Directory.GetCurrentDirectory(), "bridge//weapon_data.json");
                Log("deserializing weapon data: {json}", args: json);
                using (var stringReader = File.OpenText(json))
                {
                    _weaponData = WeaponData.FromJson(stringReader.ReadToEnd());
                }
            }
            catch (System.Exception ex)
            {
                LogEx(ex, $"weapon data file : {json}");
                //Don't start without data, force server to rpfail
                throw;
            }
        }

        [ServerEvent(Event.ResourceStart)]
        public void ResourceStart()
        {
            NAPI.Util.ConsoleOutput("Script running");
        }

        #region Commands
        /// <summary>
        /// Print all commands
        /// </summary>
        /// <param name="sender"></param>
        [Command("wcmd", ACLRequired = true, Alias = "wcmds")]
        public void WeaponCommands(Client sender)
        {
            var cmds = GetResourceCommands(sender);
            if (cmds?.Length > 0)
                sender.SendChatMessage(cmds, 4);
        }

        [Command("giveweapon", Alias = "givew", ACLRequired = true)]
        public Task GiveWeapon(Client client, string weaponName, int ammo = 12)
        {
            WeaponData weaponData = null;
            if (_weaponData.Keys.Contains(weaponName.ToUpper()))
            {
                weaponData = _weaponData[weaponName.ToUpper()];
            }

            if (weaponData == null)
                Log($"Failed to find weapon for {weaponName}");
            else
            {
                GiveWeapon(client, weaponData, ammo);
            }

            return Task.CompletedTask;
        }

        [Command("weapongroup", Alias = "wg", ACLRequired = true, Description = "Print all weapons from given group")]
        public void PrintWeaponGroup(Client client, string weaponGroup)
        {
            var vals = _weaponData.Values.Where(x => x.Group.Contains(weaponGroup.ToUpper()))
                .Select(x => x.Name.Replace(" ",""));
                         
            client.SendChatMessage(vals, 3);
        }

        [Command("weapongroups", Alias = "wgs", ACLRequired = true, Description = "Print all groups")]
        public void PrintWeaponGroups(Client client)
        {
            client.SendChatMessage(_weaponData.Values
                .Select(x => x.Group)
                .Distinct(), 4);
        }

        #endregion

        #region Private Methods
        private void GiveWeapon(Client client, WeaponData weaponData, int ammo = 12)
        {
            var hash = (WeaponHash)uint.Parse(weaponData.HashKey);
            if (weaponData != null)
            {
                Log($"giving {weaponData.Name} to {client.Name}");
                int currAmmo = client.GetWeaponAmmo(hash);

                if (weaponData.DefaultClipSize > 0)
                    client.GiveWeapon(hash, weaponData.DefaultClipSize * 10);
                else
                    client.GiveWeapon(hash, ammo * 10);
            }
        } 
        #endregion
    }
}