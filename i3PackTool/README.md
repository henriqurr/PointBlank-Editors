# i3PackTool

Windows tool for Point Blank **i3Pack** archives — the packed resource containers used by the i3 engine client (textures, scripts, models, and other packed files).

**Credits:** Coyote, **PISTOLA** ([@henriqurr](https://github.com/henriqurr))  
Special thanks to **Abujafar** for the original C++ source.  
Copyright © Exploit Network.

Version shown in the UI: `i3PackTool (release-20210115)`.

## What it edits

`.i3pack` (and similarly packed i3 resource files). The tool parses the pack header, string table, and directory nodes, then lets you inspect files, dump them, or replace an entry and rebuild the pack.

## Requirements

- Windows
- .NET Framework 4.6.1
- Visual Studio 2017+ (to build)
- Newtonsoft.Json 12.x (already referenced in the project)

**Solution:** `i3PackTool.sln`  
**Project:** `i3PackTool/i3PackTool.csproj`  
**Output:** `i3PackTool.exe`

`i3ResourceFile.cs` P/Invokes `i3BaseDx.dll` (`Encrypt` / `Decrypt`). That DLL is the client’s i3 helper; it is **not** required for the bit-rotate path used when reading node/file info.

## How to use

1. Build and run **i3PackTool**.
2. **File → Open** and select an i3Pack.
3. The left tree is the pack directory (root / folders / leaves). Select a node to list its files (name, offset, size, CRC32).
4. Right-click a file:
   - **Dump** — extract that file to disk.
   - **Dump all files** — extract every file in the selected node into `_<nodeName>/`.
   - **Replace this file with...** — swap the selected entry with another file. Offsets and sizes of later entries are updated (including bit-rotated file-info records). You are then prompted to save a new pack.
5. **Settings** can show offsets/sizes in hex and toggle the debug console.

## Pack layout

```
Header
  HeaderID, VersionMajor, VersionMinor
  StringTableCount, StringTableOffset, StringTableSize
  NodeCount, DirTableOffset, DirTableSize, NodeSize
  extra blobs (16 + 108 bytes)

String table   (at StringTableOffset)
Directory / nodes (at DirTableOffset)
File payloads  (offsets stored per file; some metadata is bit-rotated)
```

File-info records may be encrypted with **bit rotate** (2 or 3 bits depending on the record). Offsets and sizes are split into shift/or word pairs (`OffsShift << 16 | OffsOr`).

## Project layout

| Path                             | Role                               |
| -------------------------------- | ---------------------------------- |
| `Forms/i3PackToolView.cs`        | Main UI: open, dump, replace, save |
| `Forms/SettingsView.cs`          | Hex display and console            |
| `Managers/HeaderManager.cs`      | Pack header                        |
| `Managers/StringTableManager.cs` | Names                              |
| `Managers/NodeManager.cs`        | Nodes and file list                |
| `BitRotate.cs`                   | Metadata decrypt / encrypt         |
| `CrcStream.cs`                   | CRC32 of dumped payloads           |
| `Models/`                        | Header, nodes, file info           |

## Notes

- Back up the original pack before replacing files.
- Replacing a file with a **different size** shifts later offsets; always save to a new location and test in the client.
- Dump/replace currently parse list offsets as decimal unless you convert hex yourself (hex mode is display-only in several paths).
