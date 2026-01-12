# Odyssey Browser - C# Version

A simple, configurable web browser built with C# WPF and Microsoft Edge WebView2.

## Features

- Clean, minimal browser interface
- Configurable fullscreen mode
- Custom overlay close button
- Cookie management
- XML-based configuration
- Navigation controls (Back, Forward, Reload)
- Address bar for URL entry

## Requirements

- .NET 8.0 SDK or Runtime
- Windows 10/11
- Microsoft Edge WebView2 Runtime (usually pre-installed on Windows 10/11)

## Building

```powershell
# Restore dependencies and build
dotnet build Odyssey.csproj

# Run the application
dotnet run --project Odyssey.csproj

# Publish as self-contained executable
dotnet publish -c Release -r win-x64 --self-contained
```

## Configuration

Edit `config.xml` to customize the browser:

```xml
<?xml version="1.0"?>
<config>
    <fullscreen>true</fullscreen>
    <startup_url>http://10.35.49.56:9001</startup_url>
    <ignore_cookies>true</ignore_cookies>
    <button_offset>1535,17</button_offset>
    <button_size>120,125</button_size>
</config>
```

### Configuration Options

- **fullscreen**: `true` or `false` - Enable fullscreen mode on startup
- **startup_url**: Initial URL to load (http:// will be added if not specified)
- **ignore_cookies**: `true` or `false` - Disable persistent cookies
- **button_offset**: `x,y` - Position of close button in fullscreen mode (pixels from top-left)
- **button_size**: `width,height` - Size of close button in pixels

## Project Structure

- `Odyssey.csproj` - Project file
- `App.xaml` / `App.xaml.cs` - Application entry point
- `MainWindow.xaml` / `MainWindow.xaml.cs` - Main browser window
- `ConfigLoader.cs` - XML configuration parser
- `ResourceHelper.cs` - Resource path resolution
- `config.xml` - Configuration file
- `farmo.png` - Close button image

## License

MIT License - See readme.txt for details

## Author

Denis Manole (2026)
