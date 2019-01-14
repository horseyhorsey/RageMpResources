using GTANetworkAPI;
using System;
using System.Collections.Generic;

namespace VehicleManager
{
    public class Driver
    {
        #region Fields
        private Random Rnd = new Random();
        #endregion

        #region Properties
        public Client Client { get; }
        public IList<Vehicle> Vehicles { get; set; }
        public string NumberPlate { get; internal set; }
        #endregion

        #region Constructors
        public Driver(Client client)
        {
            Client = client;
            Vehicles = new List<Vehicle>();
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Adds a vehicle to this drivers list TODO
        /// </summary>
        /// <param name="model"></param>
        /// <param name="removeOld"></param>
        /// <param name="putInDriver"></param>
        public void AddVehicle(uint model, Vector3 position, bool removeOld = true, bool putInDriver = true)
        {
            //Remove all vehicles
            if (removeOld && this.Vehicles.Count > 0)
            {
                foreach (var vehicleObj in this.Vehicles)
                {
                    if (vehicleObj !=null)
                    {
                        DeleteVehicle(vehicleObj);
                    }                        
                }

                Vehicles.Clear();
            }
            
            Vehicle vehicle = null;
            NAPI.Task.Run(() =>
            {
                vehicle = NAPI.Vehicle.CreateVehicle(model, position, 0f, 0, 0, engine:false);
                vehicle.PrimaryColor = Rnd.Next(158);
                vehicle.SecondaryColor = Rnd.Next(158);
                vehicle.NumberPlate = this.NumberPlate;
                vehicle.NumberPlateStyle = Rnd.Next(6);                

                if (putInDriver)
                    NAPI.Player.SetPlayerIntoVehicle(Client, vehicle, -1);

                NAPI.Pools.GetAllVehicles().Add(vehicle);
                this.Vehicles.Add(vehicle);
            });
        }

        internal void DeleteVehicle(Vehicle vehicle)
        {            
            if (!vehicle.IsNull)
                NAPI.Task.Run(() => vehicle.Delete());            
        }
        #endregion
    }
}
