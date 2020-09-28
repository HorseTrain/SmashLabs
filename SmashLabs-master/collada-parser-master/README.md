# collada-parser
.NET Core collada file loader and viewer using OpenTK

Tested with models exported from blender

## Support
- Load model data including normals, texture coordinates and colors
- Load textures (BMP only)
- Render textured and/or colored models
- Materials (shininess, specular etc.)
- Transparent textures

### Planned support
- Multiple meshes
- Change material settings
- Animations

## Setup
Have .NET Core 1.1 installed then run:
```bash
dotnet restore
dotnet run
```
### Publish self contained application
To publish a self contained version of collada-parser
```bash
dotnet restore
dotnet publish -c release
```
Published files will be found under /bin/release/netcoreapp1.1/`<platform>`/publish
