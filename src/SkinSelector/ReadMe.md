# <span style="color:#018144">RAGEMP - Skin Selector</span>

---

## Description

Std player model selector including peds and mp_outfits.

Use commands or the interactive button to use NativeUI........ <kbd>M</kbd>

<kbd>M</kbd> isn't set to close the menu. You can close with <kbd>Backspace</kbd> or <kbd>Esc</kbd>

Note debug in Client. If set to false then Chat debug messages are turned off:

	private bool _debug = true;

---

## <span style="color:0453a0">Server (Script)</span>

### Settings.xml example:

	<?xml version="1.0"?>
	<config xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
	  <acl_enabled>true</acl_enabled>
	  <log_console>false</log_console>
	  <resource src="SkinSelector"></resource>
	</config>

### <span style="color:orangered">Resource Settings</span>

- use_setting_skin

	    <setting name="use_setting_skin" value="true" description="Enable Starting skin for client"/>

- skin

	    <setting name="skin" value="mp_freemode_male" description="The starting skin for client"/>

- outfit

	    <setting name="outfit" value="24" description="Starting outfit for player"/>

- show_on_connect

	    <setting name="show_on_connect" value="true" description="shows selector menu when client connects"/>  
   
- debug (print chat messages when in debug)

    	<setting name="debug" value="true"/>

### <span style="color:orangered">Commands</span>

**These commands are not ACL restricted**

- Help

		<right name="command.skinhelp" />
		<right name="command.skincmds" />

- Outfit (Id)

		<right name="command.outfit" />

### <span style="color:orangered">Remote Events</span>

- ClientMenuClosed (client, args)

		This remote event expects the model hash. The Client menu invokes this when just closed

- ChangeOutfit (client, args)

		See `ChangeOutfit` remote which expects an `OutFitId` 


### <span style="color:orangered">Exported Functions</span>

- ChangeModel (client, hash)

		<export class="SkinSelector" function="ChangeModel" returns="bool"/>	

- ChangeOutfit (client, id)

		<export class="SkinSelector" function="ChangeModel" returns="bool"/>	


## <span style="color:#0453a0">Client (Script)</span>

### Events
	
#### ShowSkinSelector

This shows the skin selector menu. Is used in server if set to show skin selector on connect.

**Ped dicts are loaded when the client selects menu items. Menuitems are cleared after use.**