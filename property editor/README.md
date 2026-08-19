# Property Editor V2

Windows editor for Point Blank client **property files** (`.pef`). These files store item, weapon, and environment data as a tree of folders and typed values, often per client nation.

**Credits:** Coyote, **PISTOLA** ([@henriqurr](https://github.com/henriqurr))  
Copyright © Exploit Network.

## What it edits

Point Blank property resources identified by the magic `I3R2`. Typical files live in the client and include names such as `EnvSet`. The editor can open both **plain** and **bit-rotated (encrypted)** copies of the same format.

## Requirements

- Windows
- .NET Framework 4.7.2
- Visual Studio 2017+ (to build)

**Solution:** `PropertyEditor.sln`  
**Project:** `PropertyEditor V2/PropertyEditor V2.csproj`  
**Output:** `Property Editor.exe`

## How to use

1. Build and run **Property Editor**.
2. Open **Settings** and pick the **client nation** you are editing (Brazil, Korea, Russia, etc.). This selects which nation slot is shown for items that store one value per region.
3. **File → Open** and choose a `.pef` (or similarly named property file).
   - If the first 4 bytes are not `I3R2`, the tool treats the file as encrypted and decrypts it with **bit rotate, 3 bits, 2048-byte blocks**.
   - If a matching profile exists under `Profiles/`, you can load, keep, or delete previous edits.
4. Browse the tree. Folders are `TRN3` nodes; leaves are typed properties.
5. Edit a value (INT32, REAL32, STRING, vectors, COLOR, MATRIX). Changes are tracked in memory.
6. **Save** overwrites the original file, or **Save As** writes a copy. If the source was encrypted, the result is encrypted again the same way.

## Settings and profiles

| File                      | Location        | Purpose                                |
| ------------------------- | --------------- | -------------------------------------- |
| `config.data`             | next to the exe | Console visibility and selected nation |
| `Profiles/<filename>.dat` | next to the exe | Saved edit list for that property file |

Encoding used for names/strings is **Windows-1252** (`1252`). Unicode strings inside objects are read/written as UTF-16.

## File layout (I3R2)

```
Header
  ResourceType     4 bytes   ("I3R2")
  Unk              int32
  VersionMajor     uint16
  VersionMinor     uint16
  StringTableCount int32
  StringTableOffset uint64
  StringTableSizes  uint64
  ObjectInfoCount  int32
  ObjectInfoOffset uint64
  ObjectInfoSize   uint64
  padding          132 bytes (on write)

String table   (at StringTableOffset)
Object index   (at ObjectInfoOffset): Type, Id, Offset, Size per object
Object bodies  (at each Offset)
```

Folders (`TRN3` / magic `860770900`) hold child folder IDs and item IDs. Items with type `9` store **one value per nation**.

## Project layout

| Path                              | Role                        |
| --------------------------------- | --------------------------- |
| `Forms/PropertyEditorView.cs`     | Main UI: open, tree, save   |
| `Forms/EditView.cs`               | Value editor                |
| `Forms/SettingsView.cs`           | Nation and console          |
| `Managers/HeaderManager.cs`       | I3R2 header                 |
| `Managers/StringTablesManager.cs` | String table                |
| `Managers/ObjectsManager.cs`      | Objects, values, write-back |
| `Utils/BitRotate.cs`              | Encrypt / decrypt           |
| `Models/Enums/Nation.cs`          | Client nations              |
| `Models/Enums/ValueType.cs`       | Property types              |

## Notes

- Always back up the original `.pef` before saving.
- Nation must match the client you intend to patch, or you will edit the wrong slot on type-9 items.
- `EnvSet` files skip the last string-table terminator on write (`isEnvSet`).
