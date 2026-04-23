# Suduxu Unity SDK

This folder is UPM-ready and can be installed from Git.

## Install through Package Manager

Use **Add package from Git URL...** with:

`https://github.com/Suduxu/Suduxu-Unity.git?path=/Playground/Assets/Suduxu`

## Notes

- The native plugin (`DLL/suduxu.dll`) and its bundled runtime files are included in this package.
- The package depends on `com.unity.nuget.newtonsoft-json`.
- Under /Prefabs, there are three three prefabs that are used in the Suduxu Playground scene. The required component is `Suduxu Component` it needs to be dragged into the scene and configured. Optionally you can also add the components `Phone` to have a mock phone that rotates with a connected device, and `QR-Code` to have a QR code that can be scanned to connect a device (needs to be dragged into a `Canvas`-element and referenced in the `Suduxu Component`).