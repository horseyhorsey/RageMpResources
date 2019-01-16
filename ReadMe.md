# RageMp - Server / Client Resources

*Project examples for working with RageMp*

---

## RageMp 3.7.0

### Server

- Dotnet core 2.2 environment
- Dotnet 2.2 runtime binaries installed to **RAGEMP/server-files/bridge/runtime** 

### Client

- **enable-clientside-cs.txt** = Empty file must exist for Client scripts


---

## Build

This repo relies on sub modules to keep the bread and butter helpers out of the resources.

	git submodule update --recursive --remote

### Server

All server resources build to **C:/RAGEMP/server-files/bridge/resources/$(TargetName)**

Shared and Submodules are built to runtime.

Server projects include **launchsettings.json** for launching server debug.

### Client

All client scripts build to **C:/RAGEMP/server-files/client__packages/cs_packages/$(TargetName)**