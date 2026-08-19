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

### Property Editor V2

Editor for Point Blank client `.pef` (property) files. View and edit properties, strings, and objects, with client nation support.

- Solution: `property editor/PropertyEditor.sln`

### i3PackTool

Tool for opening and editing **i3Pack** archives (client resources). Lists nodes, files, and string tables in the pack.

- Solution: `i3PackTool/i3PackTool.sln`

### RSC Editor

Viewer for **RSC** files, listing items and internal resource names.

- Solution: `RSC Editor/RSC Editor.sln`

### mqfEditor

Editor for `.mqf` mission/quest files: cards, rewards, and related items.

- Solution: `mqfEditor/mqfEditor.sln`

### BitRotate

**Bit rotate** utility to encrypt/decrypt files in 2048-byte blocks (shift/unshift).

- Solution: `BitRotate/BitRotate.sln`

## Requirements

- Windows
- [.NET Framework 4.6.1](https://dotnet.microsoft.com/download/dotnet-framework/net461) (or later)
- Visual Studio 2017+ (to build)

## Build

Open the `.sln` for the tool you want in Visual Studio and build in **Debug** or **Release**.
