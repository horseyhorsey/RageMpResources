# <span style="color:#018144">RAGEMP - Vehicle Manager</span>

---

## Description

Manages Drivers. Currently used for spawning vehicles from commands.

You can spawn vehicles and search vehicle classes.

---

## <span style="color:0453a0">Server (Script)</span>

### Settings.xml example:

	<?xml version="1.0"?>
	<config xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
	  <acl_enabled>true</acl_enabled>
	  <log_console>false</log_console>
	  <resource src="VehicleManager"></resource>
	</config>

### <span style="color:orangered">Resource Settings</span>

- player_connected - Act when player connects, which creates a driver list and new driver for the player.

	    <setting name="player_connected" value="true"/>

- default_plate - Drive custom vehicle plate

	    <setting name="default_plate" value="H0R53 1"/>


### <span style="color:orangered">Commands</span>

**These commands are not ACL restricted**

- Help

		<right name="command.vcmd" />
		<right name="command.vcmds" />

- Class - print car classes
 
		<right name="command.getclasses" /> 
		<right name="command.getc" />       

- Class - print vehicles in class
 
		<right name="command.getvehicles" /> ClassName 
		<right name="command.getv" />        ClassName

- Spawn - spawn a vehicle
 
		<right name="command.car" /> Vehicle Name
		<right name="command.v" />   Vehicle Name

- Owned  (TODO??)

		<right name="command.vowned" />


### <span style="color:orangered">Exported Functions</span>

- void CreateDriver(Client client)

		<export class="VehicleManager" function="CreateDriver" returns="bool"/>	

- void SpawnCar(Client sender, uint model, Vector3 position = null)

		<export class="VehicleManager" function="SpawnCar" returns="bool"/>	


## <span style="color:#0453a0">Client (Script)</span>

n/a