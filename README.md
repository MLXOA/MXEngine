# MXEngine
MLXOA's new game engine for fast development, built entirely in C#. Supports Windows 10+[^1], and Linux. MXEngine **can** run on macOS[^2], but is not supported in any way by MLXOA.

Tips for deploying games using MXEngine:
1. Inform users if one or more MXEngine assemblies are incompatible with builds from this repository.

[^1]: Requires Windows 10 build 1608 or above, UWP and WinUI3 are **NOT** supported.
[^2]: Notarization highly recommended for distribution outside the Mac App Store, notarization simplifies opening the app for the first time.