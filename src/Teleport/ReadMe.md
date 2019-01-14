# <span style="color:#018144">RAGEMP - Teleport</span>

---

## Description

Teleporting and safe waypoint teleporting. 

---

## <span style="color:0453a0">Server (Script)</span>

### Settings.xml example:

	<?xml version="1.0"?>
	<config xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
	  <acl_enabled>true</acl_enabled>
	  <log_console>false</log_console>
	  <resource src="Teleport"></resource>
	</config>

### <span style="color:orangered">Resource Settings</span>

n/a

### <span style="color:orangered">Commands</span>

- View Commands

		<right name="command.tpcmd" access="true" />
		<right name="command.tpcmds" access="true" />

- Teleport = **/tp x y z**

		<right name="command.tele" access="true" /> // X Y Z
		<right name="command.tp" access="true" /> // X Y Z

- Teleport client = **/tp targetname x y z**

		<right name="command.tptarget" access="true" /> // X Y Z
		<right name="command.teletg" access="true" /> // X Y Z

- Teleport waypoint = See Client events **TpPlayerWaypoint**

		<right name="command.telewp" access="true" /> // X Y Z
		<right name="command.tpwp" access="true" /> // X Y Z

### <span style="color:orangered">Exported Functions</span>

n/a

-- 

## <span style="color:#0453a0">Client (Script)</span>

### Events
	
#### TpPlayerWaypoint

This teleports the player to any waypoint coord without falling through the map. Screen fades Out and In when teleported.