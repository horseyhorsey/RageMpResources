# <span style="color:#018144">RAGEMP - XpLevel</span>

---

## Description

Based on Rootcause's JS version this resource can award and keep track of a Players experience.

This project tests the usage of synced client data by using the **"PLY_LEVEL"** variable server side.

	PLY_LEVEL = new int[] { currLevel, prevXp, currXp, limit, nextLimit };
	client.SetSharedData("PLY_LEVEL", PLY_LEVEL)

The `Client` receives the `PLY_LEVEL` as a hashed value `4930075823048855057` (so it doesn't know what it's name is)

> Use the test method in **RageMpClient.Tests.SharedDataHasher** to see how to generate your own hashes. I have created a method which you can adjust the array and print hashes to text file for convenience.

---

## <span style="color:0453a0">Server (Script)</span>

### Settings.xml example:

	<?xml version="1.0"?>
	<config xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
	  <acl_enabled>true</acl_enabled>
	  <log_console>false</log_console>
	  <resource src="XpLevel"></resource>
	</config>

### <span style="color:orangered">Resource Settings</span>

- <!--    When this is used in a gamemode you might want to switch this off and call InitPlayer-->

		<setting name="on_connected" value="true" 
				 description="InitPlayer when player connects" />

### <span style="color:orangered">Exported Functions</span>

- **NAPI.Exported.XpLevel.InitPlayer (int currentXp)**

		<export class="XpLevel" function="InitPlayer" />

This initializes the player and sets the currentLevels and limits. Effectively you only need to store CurrentXp when player leaves.

- **NAPI.Exported.XpLevel.AddXp (int xpAmount)**

		<export class="XpLevel" function="AddXp" />

- **NAPI.Exported.XpLevel.SetXp (int xpAmount)**

		<export class="XpLevel" function="SetXp" />

-- 

## <span style="color:#0453a0">Client (Script)</span>

The Client listens for when the shared data changes.

	Events.EnableKeyDataChangeEvent = true;
	Events.OnEntityDataChangeByKey += DataChanged;

When a data change is picked up for `SET_LEVEL` the levels are displayed on screen.
