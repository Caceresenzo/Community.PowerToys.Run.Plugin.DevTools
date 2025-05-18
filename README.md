A small developer toolbox for PowerToys Run.

- [Install](#install)
- [Usage](#usage)
  - [Hash](#hash)
- [Develop](#develop)

# Install

1. Download `DevTools.zip`.
2. Unzip it into `%LOCALAPPDATA%\Microsoft\PowerToys\PowerToys Run\Plugins`.

# Usage

## Hash

```
<hash algorithm> <message>
```

Where `<hash algorithm>` can be any (case insensitive) of the following:
- `MD5`
- `SHA1`
- `SHA256`
- `SHA384`
- `SHA512`

> [!TIP]
> ```
> > md5 hello
> < 5D41402ABC4B2A76B9719D911017C592
> ```

# Develop

Run the script [deploy.ps1](./Community.PowerToys.Run.Plugin.DevTools/deploy.ps1).
