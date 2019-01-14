# <span style="color:#018144">RAGEMP - WorldDateTime</span>

---

## Description

This resource helps manage the game time. When the resource is loaded, a timer is created which updates the game time. 

Stopping the resource disposes of the timer.

Adjust **meta.xml** settings for start times and time speed. 

	World.GetTimeGood() // Use to get the time with extension.

---

## <span style="color:0453a0">Server (Script)</span>

### Settings.xml example:

	<?xml version="1.0"?>
	<config xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
	  <acl_enabled>true</acl_enabled>
	  <log_console>false</log_console>
	  <resource src="WorldDateTime"></resource>
	</config>

### <span style="color:orangered">Resource Settings</span>

- Time to start game on

		<setting name="time" value="19:00" 
				 description="Time of day to start" />

- Enable start time

    	<setting name="enable_start_time" value="true" 
			     description="Whether to set the time when resource starts" />
        
- Default timer tick **Multiplier is * 60000 ms. (minute)**

    	<setting name="time_multiplier" value="0.2" 
                 description="1.0 is real time" />

### <span style="color:orangered">Commands</span>

- SetTime (hours, mins)

		<right name="command.settime" access="true" />

### <span style="color:orangered">Exported Functions</span>

- **NAPI.Exported.WorldDateTime.SetTime (hours, mins)**

		<export class="WorldDateTime" function="SetTime" />

-- 

## <span style="color:#0453a0">Client (Script)</span>

### No resource available ToDo: display clock?