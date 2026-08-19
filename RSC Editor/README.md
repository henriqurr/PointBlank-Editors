# RSC Editor

Windows **viewer** for Point Blank **RSC** resource lists — files that catalog resource names and paths used by the client.

**Credits:** Coyote, **PISTOLA** ([@henriqurr](https://github.com/henriqurr))  
Copyright © Exploit Network.

## What it opens

RSC resource catalog files. The header is a 4-byte type string plus an item count. Each entry stores a short name and a full path/name.

This tool currently **lists** entries. Save / Save As appear in the menu but are not implemented as writers.

## Requirements

- Windows
- .NET Framework 4.6.1
- Visual Studio 2017+ (to build)

**Solution:** `RSC Editor.sln`  
**Project:** `RSC Editor/RSC Editor.csproj`  
**Output:** `RSC Editor.exe`

## How to use

1. Build and run **RSC Editor**.
2. **File → Open** and choose an RSC file.
3. The text view shows `Fullfilename` and `Filename` for every item. The status line shows path and file count.

## File layout

```
Header
  FileType    4 bytes  (Windows-1252)
  ItemsCount  int32

For each item
  rsc1         int32
  Type         int32
  FileSize     int32     (length of Filename)
  FullFileSize int32     (length of Fullfilename)
  unused       int32
  Filename     FileSize bytes
  pad          1 byte
  Fullfilename FullFileSize bytes
  extra        133 bytes if Type != 7, else 1 byte
```

Strings use **Windows-1252**.

## Project layout

| Path                        | Role                |
| --------------------------- | ------------------- |
| `RSCEditorViewer.cs`        | UI and open handler |
| `Managers/HeaderManager.cs` | Header              |
| `Managers/ItemsManager.cs`  | Item list           |
| `Models/Header.cs`          | Header model        |
| `Models/Items.cs`           | Item model          |

## Notes

- Use this to inspect which resources an RSC indexes; pairing with **i3PackTool** helps locate the packed payload.
- There is no write-back yet — do not expect Save to persist changes.
