# Hunter Reborn – Installation Guide

## 1. Initial Setup

When running the Hunter Reborn launcher for the first time, a setup window will appear asking you to locate your **theHunter Classic executable (`theHunter.exe`)**.

1. Locate your theHunter Classic installation folder.
2. Select `theHunter.exe`.
3. Click **Finish** to complete the initial setup.

The launcher will remember this location, so you wont need to repeat this process every time you start Hunter Reborn.

If you receive an error stating that your game version is not supported, complete the hosts file configuration below, then skip to **Section 3 – Installing the Supported Version Through Steam.**

### Configuring the Hosts File

Hunter Reborn requires changes to your computer's **hosts file** to redirect theHunter's domains to your local machine.

To do this automatically:

1. Locate `setup.bat` in the Hunter Reborn installation folder.
2. Right-click `setup.bat`.
3. Select **Run as administrator**.

This script modifies your Windows hosts file to redirect theHunter's domains.

**Prefer to make the changes manually?**

If you don't want to grant the setup script administrator privileges, you can manually edit your hosts file instead.

The hosts file is located at:

`C:\Windows\System32\drivers\etc\hosts`

1. Open Notepad as Administrator.
2. In Notepad, open the hosts file using the location above. You may need to change the file type filter to **All Files** to see it.
3. Add the following entries to the bottom of the file:

```text
127.0.0.1 api.thehunter.com
127.0.0.1 gameserver.thehunter.com
127.0.0.1 apiv2.thehunter.com
127.0.0.1 thehunter.com
127.0.0.1 www.thehunter.com
127.0.0.1 static.thehunter.com
127.0.0.1 signup.thehunter.com
127.0.0.1 avatar.thehunter.com
127.0.0.1 photo.thehunter.com
127.0.0.1 map.thehunter.com
127.0.0.1 tracking2.avalanchestudios.com
127.0.0.1 static.thehunterbeta.com
127.0.0.1 apiv2.thehunterbeta.com
127.0.0.1 oldtracking.avalanchestudios.com
127.0.0.1 metrics.avalanchestudios.com
```

4. Save the file.

## 2. Supported Game Version

Hunter Reborn currently supports the following version of theHunter Classic:

**Supported version:** `2026081804`

If you receive a message stating that your game version is not supported, you will need to install the supported version through Steam.

Follow the instructions below to download the correct version.

## 3. Installing the Supported Version Through Steam

Steam allows you to download previous versions of games through its built-in console.

### Step 1 – Open the Steam Console

1. Open Steam.
2. Press `Win + R` to open the Windows Run dialog.
3. Enter the following command:

```text
steam://open/console
```

4. Press Enter.

The Steam Console should now appear in your Steam client.

### Step 2 – Download the Previous Game Version

Copy and paste the following command into the Steam Console:

```text
download_depot 253710 253711 4122928050078291366
```

Press Enter and wait for Steam to finish downloading the files.

Steam will display the download location once the process is complete.

**Important:** You must own theHunter Classic on Steam to use this installation method. If you don't own the game, you'll need to obtain a legitimate copy through another supported distribution method. We cannot provide assistance with obtaining pirated copies of the game.

### Step 3 – Disable Automatic Updates

We recommend avoiding launching theHunter Classic directly through Steam after downloading the older version. Steam may attempt to update the game when launched, potentially replacing the version required by Hunter Reborn.

To reduce the chance of this happening:

1. Open your Steam Library.
2. Right-click **theHunter Classic**.
3. Select **Properties**.
4. Navigate to **Updates**.
5. Change Automatic Updates to **Only update this game when I launch it**.

Avoid launching the game directly through Steam after installing the older version, as doing so may trigger an update.

### Step 4 – Move the Downloaded Game Files

We recommend moving the downloaded game files to a separate location outside your Steam installation directory.

For example:

`C:\Games\Hunter Reborn\theHunter Classic\`

1. Open the download location displayed by Steam.
2. Copy the downloaded game files.
3. Paste them into your chosen installation directory.

Keeping the downloaded files separate from your Steam installation ensures that Steam updates won't overwrite your theHunter Classic game files.

### Step 5 – Configure the Hunter Reborn Launcher

If you haven't already configured the launcher:

1. Start the Hunter Reborn launcher.
2. When prompted, locate the `theHunter.exe` file in your new installation directory.
3. Click **Finish**.

The launcher should now be configured to use your downloaded game version.

