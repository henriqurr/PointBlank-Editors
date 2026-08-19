# mqfEditor

Windows editor for Point Blank **mission / quest files** (`.mqf`). These files define mission cards (objectives, maps, weapons) and card rewards (GP, XP, medals, items).

**Credits:** Coyote, **PISTOLA** ([@henriqurr](https://github.com/henriqurr)), Darkness  
Copyright © Exploit Network.

The assembly name is `mqfDecryptor` (legacy namespace `mqfDecryptor`).

## What it edits

Client or server `.mqf` mission files. The editor loads up to **40 cards** (10 cards × 4 missions) plus reward tables. Quest type `2` uses a second reward block (5 extra item slots, including “units vs days”).

## Requirements

- Windows
- .NET Framework 4.6.1
- Visual Studio 2017+ (to build)

**Solution:** `mqfEditor.sln`  
**Project:** `mqfEditor/mqfEditor.csproj`  
**Output:** `mqfDecryptor.exe`

## How to use

1. Build and run the editor.
2. **Open** (or the Open button) and select a `.mqf`.
3. Review cards in the list (mission type, map, limit, weapon class / id).
4. For quest type `2`, the rewards list shows extra items. Select a row, change **unk / item type / item id / count**, then click **Save** on the form to apply that row.
5. **File → Save** overwrites the opened file. **Save As** writes a copy.

## File layout

```
fileFormat   4 bytes
questType    int32     (1 = standard, 2 = extra reward table)
unkBytes     16 bytes

40 × CardObjects
  reqType, type, mapId, limitCount, weaponClass, weaponId
  extra bytes when questType == 1

Card rewards  (1 pass if questType != 2, 5 passes if questType == 2)
  gp, xp, medals, then nested reward objects

If questType == 2
  goldResult, 8 unknown bytes
  5 × CardRewards2Objects
    unkI, itemType (1 = units, 2 = days), itemId, itemCount
```

## Mission types and weapons

Mission objectives are `MISSION_TYPE` values (C4 plant/defuse, headshots, wins, chain kills, touchdown, and so on) in `Enums/Mission/MISSION_TYPE.cs`.

Weapon / class filters use `ClassType` (knife, handgun, assault, SMG, sniper, shotgun, MG, dual weapons, dino, bow, …) in `Enums/Weapon/ClassType.cs`.

Medal IDs map roughly to:

| Range   | Award    |
| ------- | -------- |
| 1–50    | Brooch   |
| 51–100  | Insignia |
| 101–116 | Medal    |

Quest type `1` multiplies displayed XP by 10 when building award summaries.

## Project layout

| Path                        | Role                     |
| --------------------------- | ------------------------ |
| `EditorWindow.cs`           | Load, UI, save           |
| `Models/MqfFile.cs`         | File, cards, rewards     |
| `Models/cARD.cs`            | Card index / flags       |
| `Enums/Mission/`            | Mission types and awards |
| `Enums/Weapon/ClassType.cs` | Weapon classes           |
| `Enums/Items/ItemsModel.cs` | Item helper              |
| `ReceiveGPacket.cs`         | Binary reader            |

## Notes

- Back up the original `.mqf` before saving.
- Item type `1` = quantity (units), `2` = duration (days) on the extra reward table.
- The UI is oriented around editing **quest type 2** extra rewards; card fields are parsed and written back even when not all of them are exposed as text boxes.
