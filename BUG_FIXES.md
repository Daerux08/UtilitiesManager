# Bugs Fixed:
- Fixed an issue where the `UtilMan wifi list` command would not display available networks correctly due to a parsing error in the `nmcli` output.
- Removed the dependency of the GUI for the CLI version
- reinforced the WiFi Parsing by making nmcli output send it in machine readable format
- fixed the detection for systemctl, 
- fixed and optimize the install packages script (DownloadScript.cs)
- Refactored CLI_UTILMAN.cs