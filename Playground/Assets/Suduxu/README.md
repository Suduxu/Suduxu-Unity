# Suduxu Unity SDK

This folder is UPM-ready and can be installed from Git.

## Install through Package Manager

Use **Add package from Git URL...** with:

`https://github.com/Suduxu/Suduxu-Unity.git?path=/Playground/Assets/Suduxu`

## Setting up a scene with Suduxu

To add Suduxu to a scene, add the `Suduxu Component`-prefab, which can be found under `/Packages/Suduxu/Prefabs`, to the scene.

## Notes

- The native plugin (`/Packages/Suduxu/DLL/suduxu.dll`) and its bundled runtime files are included in this package.
- The package depends on `com.unity.nuget.newtonsoft-json`.
- An example scene with the embedding of QR-Code as well as the usage of the gyroscope-sensor can be found under `/Packages/Suduxu/Scenes/Suduxu-Demo`.