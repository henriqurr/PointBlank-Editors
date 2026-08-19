# BitRotate

Standalone Windows utility to **encrypt or decrypt** Point Blank files that use **bit rotation** in 2048-byte blocks. The same algorithm is embedded in Property Editor and i3PackTool.

**Credits:** Exploit Network  
Copyright © Exploit Network.

## What it is for

Many Point Blank client files (including some `.pef` property files) are stored with a cyclic bit shift instead of a named cipher. If the file does not start with a known magic (for property files, `I3R2`), it is usually rotated.

Property Editor uses **key = 3** on 2048-byte chunks. This app lets you pick any key and process one or more files without opening the full editors.

## Requirements

- Windows
- .NET Framework 4.6.1
- Visual Studio 2017+ (to build)

**Solution:** `BitRotate.sln`  
**Project:** `BitRotate/BitRotate.csproj`  
**Output:** `BitRotate.exe`

## How to use

1. Build and run **BitRotate**.
2. Choose **Encrypt** (shift) or **Decrypt** (unshift).
3. Set **Key** (number of bits, typically `3` for property files).
4. Browse and select one or more files (multiselect is enabled).
5. Click **Encrypt** / **Decrypt**.

Output:

| Mode    | Suffix           |
| ------- | ---------------- |
| Encrypt | `<original>.enc` |
| Decrypt | `<original>.dec` |

The source file is not overwritten.

## Algorithm

Each 2048-byte block is rotated independently. If the last block is shorter, only the remaining bytes are processed.

- **Shift (encrypt):** each byte becomes `(next >> (8 - bits)) | (current << bits)`, wrapping the first byte of the block at the end.
- **Unshift (decrypt):** reverse walk of the same block.

This matches `property editor/PropertyEditor V2/Utils/BitRotate.cs` (with an explicit block length) and is the same idea as `i3PackTool/i3PackTool/BitRotate.cs` (used there on smaller metadata records, often 2 or 3 bits).

## Project layout

| Path                | Role                                          |
| ------------------- | --------------------------------------------- |
| `BitRotareView.cs`  | UI and file I/O                               |
| `BitRotate.cs`      | Shift / Unshift                               |
| `ReceiveGPacket.cs` | Binary helper (shared pattern with mqfEditor) |

## Notes

- Wrong key produces garbage. For `.pef`, decrypt with key **3** and confirm the file starts with `I3R2`.
- Encrypting an already encrypted file will not match the client.
- Use this when you need a decrypted copy for hex inspection, then re-encrypt before putting the file back in the client.
