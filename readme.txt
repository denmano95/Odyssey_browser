=========================================
          ODYSSEY
=========================================
          ODYSSEY
=========================================

GENERAL INFORMATION
-------------------
Author:            Denis Manole
Creation Date:     January 9, 2026
Updated:           January 13, 2026 (C# Version)
License:           Open Source (MIT License)

DESCRIPTION
-----------
This is a C# WPF-based web browser application using 
Microsoft Edge WebView2 control.

The software provides a simple, configurable browser with 
fullscreen mode support and customizable UI elements.

NEW FEATURES (v1.1)
-------------------
- Minimize to Tray: The overlay button now minimizes the application 
  instead of closing it.
- Single Instance: The application enforces a single running instance. 
  Launching a second instance restores the existing window.

REQUIREMENTS
------------
- .NET 8.0 SDK or Runtime
- Windows 10/11
- Microsoft Edge WebView2 Runtime (usually pre-installed)

INSTALLATION & USAGE
--------------------
1. Ensure .NET 8.0 SDK is installed on your system.
2. Download the project files.
3. Build and run the program:

   Via Command Line:
   dotnet build Odyssey.csproj
   dotnet run --project Odyssey.csproj

   Or build executable:
   dotnet publish -c Release -r win-x64 --self-contained

4. Configure settings in config.xml before running.

CONFIGURATION
-------------
Edit config.xml to customize:
- fullscreen: Enable/disable fullscreen mode
- startup_url: Initial URL to load
- ignore_cookies: Cookie handling
- button_offset: Close/Minimize button position (x,y)
- button_size: Close/Minimize button dimensions (width,height)

LICENSE
-------
This project is licensed under the MIT License. You are free 
to use, copy, modify, and distribute the code as long as 
the original author is credited.

NOTE
----
Original Python version (browser.py) is still available.
This C# version provides the same functionality with 
native Windows integration.