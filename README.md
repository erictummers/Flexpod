# Flexpod
Powershell demo project to manage users

## Debugging
For debugging (VisualStudio) edit the project properties of Flexpod.powershell.

Go to the Debug tab and select Start Action "Start external program:"
use *C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe*.

Under Start Options the "Command line argument:" should be 
*-noexit -command "&{ import-module .\Flexpod.powershell.dll -verbose}"*.

This information is saved in the csproj.user file and not committed to Git

## Deployment

Just click the button to host in Azure
[![Deploy to Azure](http://azuredeploy.net/deploybutton.png)](https://azuredeploy.net/)