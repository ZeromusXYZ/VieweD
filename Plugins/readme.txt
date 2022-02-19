Plugins Folder
---------------

You can place run-time plugins in here.
Preferably you group all files of a specific plugin into it's own folder. 
All *.cs files will get loaded, and all descendants of EngineBase will be automatically registered into the engine handler.
If needed you can load additional reference files by using add reference.txt anywhere in plugins with 1 file per line
