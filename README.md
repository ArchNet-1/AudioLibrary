<p align="center">
  <a href="https://archnet-plateform.fr"><img src="https://archnet-plateform.fr/ArchNetLogoWithBck.png" alt="ArchNet" width="240"></a>
</p>

# ArchNet · Audio Library

Audio clips stored by enum or int key, so code plays sounds by name and a clip can be replaced in one place.

- Unity **6000.0** or newer · Needs `com.archnet.library.core` (the ArchNet installer adds them for you).
- Part of [ArchNet](https://archnet-plateform.fr), free and open-source Unity packages

## Install

*Window > Package Manager > + > Install package from git URL…*

```
https://github.com/ArchNet-1/AudioLibrary.git
```

## Use

*Assets > Create > ArchNet > Audio Library*, then:

```csharp
using ArchNet.Library;

_sounds.PlayOneShot(_audioSource, Sfx.Jump);
```

## License

MIT, see [LICENSE.md](LICENSE.md).
