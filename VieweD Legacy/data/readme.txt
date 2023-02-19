Data Folder
---------------

Each Engine should use their own subfolder inside data.

You can place lookup data and run-time plugins in here.
Preferably you group all files of a specific plugin into its own folder. 
All *.cs files will get loaded, and all descendants of EngineBase will be automatically registered into the engine handler.
If needed you can load additional reference files by using add reference.txt anywhere in the folders using with 1 file per line

By default included with VieweD is the Final Fantasy XI Engine (ffxi)