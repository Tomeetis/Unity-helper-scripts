This little Editor script allows parsing different keywords in C# script template.

For now it is designed to only add location-based namespace, which has #NAMESPACE# keyword.  
This script also considers RootNamespace that can be changed in Edit --> Project Settings --> Editor --> Root namespace.

---

Out of the box, this Editor script works with the following configuration:

1. "Assets" folder contains "SCRIPTS" folder
2. All scripts are placed within that folder

---

Some examples:

PATH: Assets/SCRIPTS/Runtime/Logic/GameManager.cs
NAMESPACE: {RootNamespace}.Runtime.Logic

PATH: Assets/SCRIPTS/Player/InputProcessing/TouchInputProcessor.cs
NAMESPACE: {RootNamespace}.Player.InputProcessing

---

This script also ignores sub-folders named "Script":

PATH: Assets/SCRIPTS/Runtime/Scripts/Logic/Scripts/GameManager.cs
NAMESPACE: {RootNamespace}.Runtime.Logic

---

Of course, the script can be easily tweaked to add more keywords and/or change the behaviour of namespace parsing.
