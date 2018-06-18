
How to install and configure the proxy:

1. Move the Release folder in an appropriate place. This will be a proxy setup folder.

2. Apply all sql scripts from the SQLScripts to your database as follow:
	- CreateTables.sql - to create new tables designed for the proxy needs
	- InsertInitialData.sql - to put initial data (correct servers ips and ports in the 'real_ilasst')

3. Correct the real_ilasst table data (data from ilasst table). The proxy will send data on each port from real_ilasst.
	
4. Correct the ilasst table's data: the proxy will listen only a localhost port. So when start it will check if there is a record
	that corresponds a current host ip. If it is found it will be used as a listen port. 
	Ilasst table's data must point to the proxy location.
	
5. Configure the connection string in SISPOSProxy.Service.exe.config. The proxy will use it to connect to the database.
	
6. Run InstallService.bat as administrator to register the proxy windows service (UninstallService.bat to unisrall)

7. Open the windows services console, find SIS POS Proxy service and start it. Restart the service each time his settings are changed.



!!! Important !!!

POS PROXY SERVICE must be started before the other POS's services.
	