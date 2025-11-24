# FMEViewer

A desktop application for viewing and editing Football Manager database files. Easily manage clubs, competitions, and leagues with an intuitive interface.

## What is FMEViewer?

FMEViewer allows you to view and modify Football Manager game databases. You can browse clubs by competition, search for specific teams, and make changes like moving clubs between leagues or switching their positions.

Perfect for Football Manager enthusiasts who want to customize their game experience by reorganizing leagues and competitions.

## System Requirements

- **Operating System**: Windows 10 or later
- **Framework**: .NET 8.0 (will be installed automatically if missing)
- **Disk Space**: Minimal (under 50 MB)

## Installation

1. Download the latest release from the [Releases page](https://github.com/ngskicker/fm-editor/releases)
2. Extract the ZIP file to a folder of your choice
3. Run `FMEViewer.exe`

> **Note**: If prompted to install .NET 8.0, click "Yes" and follow the installation wizard.

## Getting Started

### Step 1: Load Your Database

1. Launch FMEViewer
2. Click the **"Browse"** or **"Load Database"** button
3. Navigate to your Football Manager database folder
   - Typically located in: `Documents\Sports Interactive\Football Manager [Year]\games\`
4. Select the folder containing your save files
5. Wait for the database to load (may take a few seconds)

### Step 2: Browse Competitions and Clubs

- Use the **Competition dropdown** at the top to select a league or competition
- All clubs in that competition will appear in the list below
- View club details including:
  - Full name and short name
  - Nation
  - City and stadium
  - Competition tier

## How to Use

### Search for Clubs

1. Type in the **search box** at the top of the window
2. Results filter automatically as you type
3. Click the **X button** to clear your search

### Move a Club to Another Competition

Want to move a team to a different league? Here's how:

1. **Select a club** from the list
2. Click the **"Move Club"** button
3. Choose the **destination competition** from the dropdown
4. Click **"Confirm"** to complete the move
5. The club will now appear in the new competition

**Example Use Cases:**
- Promote a lower-league team to a higher division
- Move a club to a different country's league system
- Create custom league structures

### Switch Two Clubs

Want to swap the positions of two teams in their competitions?

1. **Select the first club** you want to switch
2. Click the **"Switch Club"** button
3. *(Optional)* Use the **nation filter** to narrow down options
4. **Select the second club** to switch with
5. Click **"Confirm"** to swap their positions
6. Both clubs will exchange their competition slots

**Example Use Cases:**
- Swap two teams between different divisions
- Exchange clubs between domestic leagues and international competitions

### Remove a Club from Competition

1. **Select a club** from the list
2. Click the **"Remove Club"** button
3. Confirm the action when prompted
4. The club will be removed from the competition

> **?? Warning**: Removing clubs may affect game balance. Make sure to save your original database first!

## Tips and Best Practices

### Before You Start

- ? **Backup your save files** before making any changes
- ? Create a copy of your Football Manager database folder
- ? Test changes in a non-essential save game first

### Working with the Database

- ?? Use the search feature to quickly find clubs by name
- ?? Browse different competitions to see the current league structure
- ?? Changes are applied immediately - make sure you're working with the correct database
- ?? Close and reopen FMEViewer to reload the database after external changes

### Common Tasks

**Restructuring a league pyramid:**
1. Load your database
2. Move clubs between divisions using the Move Club feature
3. Verify all clubs are in the correct competitions
4. Launch Football Manager to see your changes

**Creating a custom competition:**
1. Move your desired clubs to the target competition
2. Use Switch Club to organize them as needed
3. Remove unwanted clubs from the competition

## Troubleshooting

### The database won't load
- ? Make sure you've selected the correct Football Manager folder
- ? Check that the database files aren't corrupted
- ? Ensure Football Manager isn't currently running with that save open

### Changes aren't appearing in Football Manager
- ? Make sure you loaded the correct save game in Football Manager
- ? Try starting a new game to see the changes
- ? Some changes may require a new save to take effect

### The application won't start
- ? Install .NET 8.0 from [Microsoft's website](https://dotnet.microsoft.com/download/dotnet/8.0)
- ? Check Windows compatibility (Windows 10 or later required)
- ? Run as Administrator if permission errors occur

## Frequently Asked Questions

**Q: Will this work with all Football Manager versions?**  
A: FMEViewer is designed for modern Football Manager databases. Compatibility depends on the database format.

**Q: Can I undo changes?**  
A: There's no built-in undo feature. Always keep backup copies of your database files.

**Q: Is this safe to use?**  
A: Yes, but always backup your saves first. FMEViewer modifies database files, so keep originals safe.

**Q: Can I use this for online/multiplayer saves?**  
A: It's recommended for single-player games only. Multiplayer saves may have restrictions.

**Q: Does this violate game terms of service?**  
A: This tool modifies local save files for personal use. It doesn't modify the game itself or interact with online services.

## Support and Feedback

- ?? **Found a bug?** Report it on the [Issues page](https://github.com/ngskicker/fm-editor/issues)
- ?? **Have a suggestion?** Share your ideas in the Issues section
- ?? **Need help?** Check existing issues or create a new one

## Related Tools

This is part of the **FM-Editor** toolkit:
- **FMEViewer** (this tool) - GUI application for viewing and editing
- **FMEConsole** - Command-line tool for advanced users
- **FMELibrary** - Core parsing library (for developers)

---

## Disclaimer

**FMEViewer is a community-created tool and is not affiliated with, endorsed by, or connected to Sports Interactive or SEGA.**

This tool is provided "as-is" without warranty. Always backup your data before making modifications. Use at your own risk.

Football Manager is a registered trademark of Sports Interactive Limited.
