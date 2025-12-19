# FMM 26 Database Editor

FMMEditor is a Windows desktop app for editing Football Manager Mobile 2026/FMM26 database `.dat` files. FMMLibrary powers the binary parsers and serializers you can reuse in your own tools.

## Features

- Load a database folder once and use it across the competitions, clubs, names, and people tabs.
- Competitions: search by name or nation, move a club into a competition, switch two clubs, remove a club from a league, and filter switch targets by nation.
- Clubs: view full club info (reputation, academy, facilities), add new clubs, edit or duplicate existing ones, and copy club UIDs in decimal or hex with nation/competition lookups.
- Names: manage first, second, and common names; quickly add multiple names by pasting newline-separated values; assign gender and nation; search by name or nation.
- People: edit or duplicate players and staff; dialogs link names, nations, clubs, languages, player attributes, and positions; new people automatically get a matching player record; copy UIDs for reference.
- Save/Save As: write updated database to the current or a new folder.

## Requirements

- Windows 10 or later.
- .NET Desktop Runtime (you'll be prompted to install if missing).
- A copy of the FMM26 database files.
- Always back up your original database folder before editing.

## Download and run

1. Download the latest release from the repository's Releases page.
2. Extract the ZIP to any folder.
3. Run `FMMEditor.exe`.
4. Choose **Load Database** and point to the folder that contains the `.dat` files listed above.

### Prebuilt download

You can also grab the prebuilt package here: https://fmmvibe.com/files/file/1472-fmm26-pre-game-database-editor/

## Using the editor

- **Competitions tab**: search competitions, select one, then right-click a club row or use the dialog buttons to move a club in, switch with another club, or remove it from the league. Filter switch targets by nation and save when finished.
- **Clubs tab**: search, add, edit, or duplicate clubs. Fields like nation and league use dropdowns. Use the copy buttons to grab the club UID in decimal or hex. Save or Save As to write `club.dat`.
- **Names tab**: manage first, second, and common names. Add one or many names at once (paste multiple lines), assign gender and nation, edit existing entries, then save to update the three name files.
- **People tab**: edit or create players/staff. The dialog links names, nations, clubs, languages, player attributes, and positions. Adding a person always creates the matching player record automatically. You can duplicate an existing person for faster entry and copy UIDs for reference.

**Save vs Save As:**  
`Save` overwrites the files in the loaded folder. `Save As` writes the updated `.dat` files to a new folder so you can keep your backup untouched.

## FMMLibrary (for developers)

FMMLibrary is a .NET 8 class library that handles parsing and writing Football Manager database files (competitions, clubs, nations, names, people, players, languages, string resources, and related domain objects). Parsers expose `Load` and `Save` methods and models are mutable so you can edit them directly.

```csharp
using FMMLibrary;

// Load, tweak, and persist clubs
var clubs = await ClubParser.Load(@"C:\\path\\to\\club.dat");
clubs.Items[0].FullName = "Example FC";
await clubs.Save(); // or clubs.Save(@"C:\\new-folder\\club.dat");
```

You can compose parsers together (for example, load nations and competitions to hydrate lookups) the same way the editor does.

## Building from source

- Install the .NET 8 SDK and ensure you're on Windows (WPF app).
- Restore and build: `dotnet restore` then `dotnet build FMEditor.sln -c Release`.
- Run the app: `dotnet run --project FMMEditor/FMMEditor.csproj`.
- Output binaries are in `FMMEditor/bin/<Configuration>/net8.0-windows10.0.19041.0/`.

## Contributing

Contributions are welcome. If you find a bug or have a feature idea, open an issue with steps and screenshots where possible. Pull requests are appreciated; describe the change, include any relevant repro steps, and note which `.dat` files or views are affected.

## Donations

If this tool helps you, you can support development via Ko-fi: https://ko-fi.com/nyongrand.

## Known issues

- Newly added players may not appear in-game even though editing existing players works.
- Newly added club may not appear in-game even though editing existing clubs works.
- Editing a person's second nationality currently does not apply.

## Upcoming Features

- Make editing a person's second nationality work.
- Fix newly added players not appearing in-game.
- Fix newly added clubs not appearing in-game.
- Add the ability to edit and add non-players (coach, scout, etc.).
- Add player transfer tools to move players between clubs.

## License

MIT License. See `LICENSE` for details.

## Notes and troubleshooting

- If loading fails, make sure the folder contains all required `.dat` files and that none of them are open elsewhere.
- If the game does not show your changes, confirm you're editing the correct database version and start a new save if needed.
- Keep an untouched backup of your original database; use `Save As` when experimenting.

This project is community-made and not affiliated with Sports Interactive or SEGA. Use at your own risk.
