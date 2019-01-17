# <span style="color:#018144">RAGEMP - Money</span>

---

## Description

Credit root cause finding SP_CASH and snippets.

Displays cash on the screen and in the pause menu.

This project tests the usage of synced client data by using the **"PLY_CASH"** variable server side.

	int PLY_CASH = 1068;
	client.SetSharedData("PLY_CASH", PLY_CASH)

The `Client` receives the `PLY_CASH` as a hashed value `5395921141301356805` (so it doesn't know what it's name is)

> Use the test method in **RageMpClient.Tests.SharedDataHasher** to see how to generate your own hashes. I have created a method which you can adjust the array and print hashes to text file for convenience.

**Render events are disabled when player dies and back on again when respawn.**

---

## <span style="color:0453a0">Server (Script)</span>

### Settings.xml example:

	<?xml version="1.0"?>
	<config xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
	  <acl_enabled>true</acl_enabled>
	  <log_console>false</log_console>
	  <resource src="Money"></resource>
	</config>

### <span style="color:orangered">Resource Settings</span>

- **on_connected** (When this is used in a gamemode you might want to switch this off and call InitCash)

		<setting name="on_connected" value="true" 
				 description="Init player cash when player connects" />

- **display_cash** (Display cash when starting client)

		<setting name="display_cash" value="true" 
				 description="Display cash hud" />

### <span style="color:orangered">Exported Functions</span>

- **NAPI.Exported.Money.InitCash (int amount)**

		<export class="Money" function="InitCash" />

- **NAPI.Exported.Money.AddCash(Client client, int amount)**

		<export class="Money" function="AddCash" />

- **NAPI.Exported.Money.GetCash(Client client, int amount)**

		<export class="Money" function="GetCash" />

- **NAPI.Exported.Money.DisplayCash (bool display)**

		<export class="Money" function="DisplayCash" />
		

---

## <span style="color:#0453a0">Client (Script)</span>

The Client listens for when the shared data changes.

	Events.EnableKeyDataChangeEvent = true;
	Events.OnEntityDataChangeByKey += DataChanged;

When data change is picked up for `SET_CASH` the cash hud is displayed on screen.

### Events
	
#### DisplayCash

Server to initially trigger whether cash is displayed, then this is stored in the clients local data.
