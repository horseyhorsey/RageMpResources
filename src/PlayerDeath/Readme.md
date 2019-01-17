# <span style="color:#018144">RAGEMP - Player Death</span>

---

## Description

This resource displays a wasted screen when player dies. The message and duration can be customized in the **ResourceSettings**.

- Spawn management added which can easily be removed if undesired.

- Notify all players of kill. Edit the `DeathReasons` dictionary to customize the `DeathReason`


## <span style="color:0453a0">Server (Script)</span>

### Settings.xml example:


	<?xml version="1.0"?>
	<config xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
	  <acl_enabled>true</acl_enabled>
	  <log_console>false</log_console>
	  <resource src="PlayerDeath"></resource>
	</config>

### <span style="color:orangered">Resource Settings</span>

    <setting name="spawn_connected" value="true" description="Enables auto spawning when player connects"/>

	<!--Should be false to use the wasted screen-->
    <setting name="respawn_death"   value="false" description="Enables auto respawning when player dies"/>
    
    <!--Spawn center of map by default, makes a change from Temple Mount - Use empty string for RAGEMP default-->
    <setting name="spawn_location" value="-83.71489 -105.2238 56.88356" description="set the default spawn location"/>
    
    <setting name="wasted_enabled" value="true" description="Show wasted screen" />
    <setting name="wasted_duration" value="2000" description="Duration of wasted screen. Not really 2 seconds but a bit longer" />
    <setting name="wasted_title" value="Wasted" description="Title" />
    <setting name="wasted_message" value="Owned" description="Message" />

    <setting name="notify_death" value="true" description="Send a notification to players with death reasons"/>

### <span style="color:orangered">Commands</span>

n/a

### <span style="color:orangered">Exported Functions</span>

n/a

### <span style="color:orangered">Remote Events</span>

- WastedScreenFinished = Respawns the player

---

## <span style="color:#0453a0">Client (Script)</span>

### Events
	
#### InitSettings

When a client connects on the server the wasted settings are sent.

    _wastedEnabled, _wastedDuration,_wastedTitle, _wastedMessage