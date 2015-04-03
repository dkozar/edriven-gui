-------------------------------------------------------
eDriven.Gui - Unity3d RIA Solution
Copyright © Danko Kozar 2010-2014. All rights reserved.
http://edrivengui.com/
-------------------------------------------------------


Congratulations,

You've just installed the eDriven.Gui package!

Just a few words...

1. DLL-s
--------

eDriven (core, free) package, normally contains 3 DLL-s: eDriven.Core, eDriven.Audio and eDriven.Networking.
These libraries are already contained inside the eDriven.Gui package, so no need to install eDriven (core) separatelly. But, if you have eDriven (core) already installed, just delete eDriven (core) DLL-s (if you don't do that, you could have clashes).

In general, DLL-s that has to be present inside your project are:
- eDriven.Animation
- eDriven.Audio
- eDriven.Core
- eDriven.Core.Designer
- eDriven.Core.Editor
- eDriven.Gui
- eDriven.Networking


2. Reducing the build size
--------------------------

If not using features of some DLLs, you are free to delete them from the project (to reduce build size).
For instance, if not using designer, you could delete eDriven.Core.Designer.dll and eDriven.Core.Editor.dll from the project. 
Or, if not using networking, remove eDriven.Networking.


3. Editor folder
----------------

If using eDriven.Core.Editor.dll, it has to be inside the folder named Editor, or else you get errors.


4. Errors
------------------------------

A. Compromised display quality

When exporting to web or other platforms and noticing problems with graphics (dark, blurry or missing GUI elements), go to Edit -> Project Settings -> Quality.
If the quality level for your platform is set to Fastest, set it to Fast or any other setting.

B. Ran out of trampolines of type 2

There is an exception specific for iOS build (related to AOT compilation) stating "Ran out of trampolines of type 2".
How to fix it: you have to go to player options for iOS platform, and manually insert:
nimt-trampolines=1024

More on this issue here: http://forum.edrivengui.com/index.php?threads/ran-out-of-trampolines-of-type-2.73/

5. Documentation
----------------

A number of starting demo scenes with corresponding scripts are located inside the eDriven.Gui/Demo folder.

Use "Help -> eDriven" menu to get more information.

You could use the folowing video material to get up and running quickly: http://www.youtube.com/watch?v=5-lrbAV9brk

eDriven.Gui homepage: http://edrivengui.com/
eDriven forum: http://forum.edrivengui.com
eDriven API: http://edriven.dankokozar.com/api/2-0/
eDriven manual: http://edriven.dankokozar.com/manual/eDriven_Manual_2-0.pdf
eDriven.Core source: https://github.com/dkozar/eDriven
eDriven WebPlayer demos: http://edrivenunity.com

Have fun! ^_^


6. Update check
----------------

eDriven.Gui Editor connects to eDriven server to check for plugin updates and news.
This feature is turned ON by default and can be turned OFF in the option panel.

When checking for update eDriven sends metadata to the server (i.e. the current eDriven framework version).

Note: When checking for updates, your IP address is visible to eDriven server.


7. Licensing
----------------

eDriven.Gui is (as all the Unity Asset Store assets are) licenced PER-USER, not per-project (per-game) or per-team.

So please: be fair and purchase a separate license for each team member.
This way I could spend more time with eDriven (you are actually speeding up its further development!)

Thank you!

Danko Kozar