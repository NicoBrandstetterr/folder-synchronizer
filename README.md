# folder-synchronizer
Utility to keep a replica folder identical to a source folder

- Synchronizes two folders named "source" and "replica.

- The program maintain a full, identical copy of source folder at replica folder.

- The synchronization is one-way: 
    - replica folder never affects source folder.
    - Any new or updated files in source are copied to replica folder.
    - Any files deleted from source folder are also deleted from replica folder.
    - If replica folder has extra files not present in source, they are removed.

- File creation, copying and removal operations are logged to a file and to the console output.
