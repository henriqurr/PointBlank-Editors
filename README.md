# PointBlank Editors

A collection of editors and tools for **Point Blank** files, developed by **Exploit Network**.

## Credits

**PISTOLA** ([@henriqurr](https://github.com/henriqurr)) — developer of this repository.
**Coyote** ([@erikvinicius](https://github.com/erikvinicius)).

| Tool                   | Credits                                                             |
| ---------------------- | ------------------------------------------------------------------- |
| **Property Editor V2** | Coyote, **PISTOLA**                                                 |
| **i3PackTool**         | Coyote, **PISTOLA** — special thanks to Abujafar for the C++ source |
| **RSC Editor**         | Coyote, **PISTOLA**                                                 |
| **mqfEditor**          | Coyote, **PISTOLA**, Darkness                                       |
| **BitRotate**          | Exploit Network                                                     |

Copyright © Exploit Network.

## Tools

Each folder has its own README (file format, usage, and layout).

| Folder                                         | File type                    | Docs                                  |
| ---------------------------------------------- | ---------------------------- | ------------------------------------- |
| [property editor](property%20editor/README.md) | `.pef` (I3R2 properties)     | [README](property%20editor/README.md) |
| [i3PackTool](i3PackTool/README.md)             | i3Pack resource archives     | [README](i3PackTool/README.md)        |
| [RSC Editor](RSC%20Editor/README.md)           | RSC resource catalogs        | [README](RSC%20Editor/README.md)      |
| [mqfEditor](mqfEditor/README.md)               | `.mqf` missions / quests     | [README](mqfEditor/README.md)         |
| [BitRotate](BitRotate/README.md)               | bit-rotate encrypt / decrypt | [README](BitRotate/README.md)         |

## Requirements

- Windows
- [.NET Framework 4.6.1](https://dotnet.microsoft.com/download/dotnet-framework/net461) (Property Editor V2 targets 4.7.2)
- Visual Studio 2017+ (to build)

## Build

Open the `.sln` for the tool you want in Visual Studio and build in **Debug** or **Release**.

## Related: i3EngineEditor (legacy AIO)

Before these tools were split, **[i3EngineEditor](https://github.com/henriqurr/i3EngineEditor)** served as the all-in-one editor for i3/Point Blank files (2019, TeamExploit). While it is now considered **legacy** and no longer maintained, it still provides features that are not fully available in the split tools—such as advanced handling for **I3I / i3VTex images**, **`.sif` text editing**, **PEF XML dump**, and in-app bit-rotate functionality. Note that i3Pack support in that project is incomplete; for i3Pack files, use **i3PackTool** instead.
