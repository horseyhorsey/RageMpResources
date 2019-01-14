# <span style="color:#018144">RAGEMP - Admin</span>
---

### Description

This is an attempt to learn and build upon the default Admin script example provided with the RAGEMP install.

**Change paths in csproj and launchSettings.json if not using default C:/RAGEMP/**

---

## <span style="color:green">Server</span>

### Settings.xml example:

	<?xml version="1.0"?>
	<config xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
	  <acl_enabled>true</acl_enabled>
	  <log_console>false</log_console>
	  <resource src="Admin"></resource>
	</config>


### Acl.xml ---- (VS linked file @ C:/RAGEMP/server-files/bridge)

If you don't have the above Acl.xml then an example is provided in example folder.

For more help on acl.xml see Wiki link.

###[Detailed ACL instructions (Wiki)](https://wiki.gtanet.work/index.php?title=Getting_Started_with_Access_Control_List_(ACL))

---

This resource doesn't use a database and relies on SocialClub logins.

You can add your SocialClub name or your launcher Name to the Admin group like so:

	  <group name="Admin">
	    <acl name="Moderator" />
	    <acl name="Admin" />
	    <object name="user.SocialClub" />
	  </group>

You could specify a password which gets converted to a hash when this resource runs. Check file after running.

	<object name="user.SocialClub" password="yourpassword"/>

---

### <span style="color:orangered">Resource Settings</span>

- Set Default Cmd Messages

		<setting name="SetGlobalDefaultCommandMessages" value="true" 
				 description="Enable the default command messages" />


### <span style="color:orangered">Commands</span>

- ahelp = Displays help on this resource

		<right name="command.ahelp" access="true" />

- acmds = Displays available commands

		<right name="command.acmds" access="true" />

- login/out

		<right name="command.login" access="true" /> // Expects password
		<right name="command.logout" access="true" />

- player control

		<right name="command.kill" access="true" /> // Expects Target name
		<right name="command.kick" access="true" /> // Expects Target name
		<right name="command.ban" access="true" />  // Expects Target name

- resource management

		<right name="command.stop" access="true" /> // Expects resource name
		<right name="command.start" access="true" /> // Expects resource name
		<right name="command.restart" access="true" /> // Expects resource name

- various

		<right name="command.setweather" access="true" /> // weather 0 - 13

### <span style="color:orangered">Exported Functions</span>

- **NAPI.Exported.Admin.PlayerConnected (hours, mins)**

		<export class="Admin" function="PlayerConnected" /> // Expects a Client


### <span style="color:orangered">Resource management</span> 

To start/stop/restart a resource a dummy directory is required. * This resource creates when built	
