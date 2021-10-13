### MU3Input `MU3Input.sln`
#### IO library to use with segatools
Usage: 
- Copy `MU3Input.dll` into game folder
- Open segatools.ini and add following lines:
```ini
[mu3io]
path=MU3Input.dll

[aimeio]
path=MU3Input.dll
```

You can use Jetbrains Rider or Visual Studio to compile.

Note:
- My lever is a non-linear potentiometer, so it has correction code in `Lever` property of `HidIO.cs`, You may want to change it or remove it.

### mu3controller `mu3controller\ `
#### Arduino Leonardo firmware to use with above IO library.
I'm using CLion to develop this, for CLion you will need to install platform io support plugin and run
```
pio -f -c clion init --ide clion
```
Note: 
- You can change pin settings in `src\components\ongeki_hardware.cpp`