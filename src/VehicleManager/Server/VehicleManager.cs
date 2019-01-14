using GTANetworkAPI;
using GTAN.Extensions;
using RageMpBase;
using RageMpBase.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GTAN.Data;

namespace VehicleManager
{
    [Logger]
    public class VehicleManager : RageScript
    {        
        #region Properties
        public IList<Driver> Drivers { get; set; }

        //Debugging?
        private bool _actPlayerConnected;
        
        /// <summary>
        /// Settings - Default number plate
        /// </summary>
        private string _defaultPlate;
        #endregion

        #region Constructors
        public VehicleManager()
        {
            Drivers = new List<Driver>();
            Log("Innit");
        }
        #endregion        

        #region Server Events

        /// <summary>
        /// Overrides the base to listen for player connects here. <para/> If <see cref="_actPlayerConnected"/> is true then a Driver is created for the player. Make false and use the CreateDriver when need to create drivers after connecting
        /// </summary>
        /// <param name="client"></param>
        /// 
        [ServerEvent(Event.PlayerConnected)]
        public void PlayerConnected(Client client)
        {
            if (_actPlayerConnected)
            {
                CreateDriver(client);
            }
        }

        /// <summary>
        /// Loads settings for the resource
        /// </summary>
        /// <param name="resourceName"></param>
        [ServerEvent(Event.ResourceStartEx)]
        public virtual void ResourceStartedEx(string resourceName)
        {
            if (resourceName == ResourceName)
            {
                if (NAPI.Resource.HasSetting(this, "player_connected"))
                {
                    _actPlayerConnected = NAPI.Resource.GetSetting<bool>(this, "player_connected");
                    //Presuming the other settings are there
                    _defaultPlate = NAPI.Resource.GetSetting<string>(this, "default_plate");
                }                    
                else
                    throw new NullReferenceException("player_connected not found in meta.xml");
            }                
        }

        /// <summary>
        /// Clears all the drivers and their cars
        /// </summary>
        [ServerEvent(Event.ResourceStop)]
        public void OnResourceStopped()
        {
            foreach (var driver in Drivers)
            {
                foreach (var vehicle in driver.Vehicles)
                {
                    vehicle.Delete();
                }
            }

            Drivers.Clear();
            Drivers = null;
        }

        [ServerEvent(Event.PlayerDisconnected)]
        private Task OnPlayerDisconnected(Client client, DisconnectionType disconnectionType, string reason)
        {
            var name = client.Name;
            Log("Removing driver {name}", args: name);

            return Task.Run(() =>
            {
                //Get driver
                var driver = Drivers.FirstOrDefault(x => x.Client == client);
                if (driver != null)
                {
                    //Remove vehicles
                    foreach (var vehicle in driver.Vehicles)
                    {
                        driver.DeleteVehicle(vehicle);
                    }

                    //Remove driver
                    Drivers.Remove(driver);
                    Log("Removed vehicles for {name}", args: name);
                }
                else
                {
                    Log("Driver not found on disconnecting {name}", args: name);
                }
            });            
        }

        [ServerEvent(Event.VehicleDeath)]
        private void VehicleDeath(GTANetworkAPI.Vehicle vehicle)
        {
            var model = vehicle.Model;
            Client client = vehicle.GetSharedData("IsOwnedBy");
            Log($"Vehicle died {model}, belonging to {client.Name}");
                                   
            var driver = Drivers?.FirstOrDefault(x => x.Client == client);
            if (driver != null)
            {
                driver.DeleteVehicle(vehicle);
                SpawnCar(client, model, client.Position);
            }
            else
            {
                Log($"No driver found, removing vehicle no respawn");
                if (!vehicle.IsNull)
                    vehicle.Delete();
            }
        }
        #endregion

        #region public Methods

        /// <summary>
        /// Creates a Driver for the client
        /// </summary>
        /// <param name="client"></param>
        public void CreateDriver(Client client)
        {
            var name = client.Name;
            if (!Drivers.Any(x=> x.Client == client))
            {
                var driver = new Driver(client);
                driver.NumberPlate = _defaultPlate ?? "H0R537";
                Drivers.Add(driver);                
                Log("Added new driver {name}", args: name);
            }
            else
            {
                Log("Can't create when Driver already exists {name}", args: name);
            }
        }

        /// <summary>
        /// Spawns a vehicle for the Driver. If position is null then the player is spawned into it.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="model"></param>
        /// <param name="position"></param>
        public void SpawnCar(Client client, uint model, Vector3 position = null)
        {
            var driver = GetDriverClient(client);

            if (driver == null)
                Log($"No driver found", loglvl: RageLogLevel.Warning);
            else
            {
                if (position == null)
                {                    
                    driver.AddVehicle(model, client.Position, true, true);
                    Log($"Added vehicle for driver {client.Name}");
                }
                else
                {
                    driver.AddVehicle(model, position, true, false);
                    Log($"Added vehicle at pos.");
                }
            }
        }
        #endregion

        #region Commands        

        /// <summary>
        /// Print all commands
        /// </summary>
        /// <param name="sender"></param>
        [Command(command:"vmcds", ACLRequired = true, Alias = "vcmd")]
        public void VehicleCommands(Client sender)
        {
            var name = this.GetType().Name;
            sender.SendChatMessage("SERVER", $"{name} Commands:");
            var cmds = NAPI.Resource.GetResourceCommands(name);
            if (cmds?.Length > 0)
                sender.SendChatMessage(cmds);
        }

        [Command(Alias = "vowned", ACLRequired = true, Description = "Send owned / spawned vehicles to client")]
        public void OwnedVehicles(Client client)
        {
            var driver = GetDriverClient(client);
            if (driver != null)
            {
                var vehicles = driver.Vehicles?.Select(x => x.DisplayName);
                client.SendChatMessage(vehicles);
                Log("Vehicles owned: ", args: vehicles);
            }
            else
            {
                Log("No driver found", loglvl: RageLogLevel.Warning);
            }
        }

        [Command("getvehicles", Alias = "getv", ACLRequired = true, Description = "Prints all the vehicles for the given class")]
        public void PrintVehicles(Client client, string vehicleType = null)
        {
            Enum.TryParse(typeof(VehicleType), vehicleType, out var vType);
            if (vType != null)
            {
                if ((VehicleType)vType != VehicleType.None)
                {
                    var vehicles = NAPI.Vehicle.GetVehicleHashes((VehicleType)vType);
                    client.SendChatMessage($"~r~{vType} vehicles");
                    client.SendChatMessage(vehicles.Keys);
                }
                else
                    client.SendChatMessage($"~r~Couldn't find vehicle class for {vehicleType}");
            }
            else
                client.SendChatMessage($"~r~Couldn't find vehicle class for {vehicleType}");
        }

        /// <summary>
        /// Prints all the vehicle classes
        /// </summary>
        /// <param name="client"></param>
        [Command("getclasses", Alias = "getc", ACLRequired = true, Description = "Prints all the vehicle classes")]
        public void PrintVehicleTypes(Client client)
        {
            client.SendChatMessage("Car Classes:");
            client.SendChatMessage(Enum.GetNames(typeof(VehicleType)));
        }

        [Command("car", Alias = "v", ACLRequired = true)]
        public async Task SpawnCarCommand(Client sender, string modelName)
        {
            await Task.Run(() =>
            {
                if (NAPI.Player.IsPlayerInAnyVehicle(sender))
                {
                    sender.SendChatMessage("~r~Please exit your current vehicle first.");
                    return;
                }

                var model = NAPI.Vehicle.GetModel(modelName);
                if (model == 0)
                {
                    sender.SendChatMessage($"~r~No vehicle found for {modelName}");
                    return;
                }

                Log($"Cmd: Adding vehicle {modelName} for driver {sender.Name}");
                SpawnCar(sender, model);
            });
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Gets the driver from the Client
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        private Driver GetDriverClient(Client client) => Drivers.FirstOrDefault(x => x.Client == client);
        #endregion
    }
}