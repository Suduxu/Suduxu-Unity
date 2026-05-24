# Suduxu Unity SDK

This folder is UPM-ready and can be installed from Git.

## Install through Package Manager

Use **Add package from Git URL...** with:

```
https://github.com/Suduxu/Suduxu-Unity.git?path=/Playground/Assets/Suduxu
```

## Setting up a scene with Suduxu

To add Suduxu to a scene, add the `Suduxu Component`-prefab, which can be found under `/Packages/Suduxu Unity SDK/Prefabs`, to the scene. The next step is to create a new Folder `DLL` under the `Assets`-folder, copy the native plugins (`suduxu.dll`, `suduxu.so`, `suduxu.dylib`) from the [Suduxu-Release Repository](https://github.com/Suduxu/Suduxu-Release/releases) and create a new `suduxu.json`-file (for more details on the content of the `suduxu.json`-file, see [here](https://suduxu.com/docs/config/overview))
## Notes

- The package depends on `com.unity.nuget.newtonsoft-json`.