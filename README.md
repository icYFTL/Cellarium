# Cellarium
## CLI file manager for yandex drive

Type `-h` or `--help` for help.

Example of dockerfile using:  
1. `cd <THIS_CELLARIUM_PATH>`  
2. `docker build . -t cellarium`  
3. `docker run -it -v <OUTPUT_PATH>:/opt/app/ cellarium dotnet build Cellarium/Cellarium.csproj -c Release -o /opt/app --os linux`  

After these commands, cellarium will be build via docker