A small developer toolbox for PowerToys Run.

- [Install](#install)
- [Usage](#usage)
  - [Hash](#hash)
    - [Example](#example)
  - [UUID](#uuid)
    - [Example](#example-1)
  - [Lorem Ipsum](#lorem-ipsum)
    - [Example](#example-2)
  - [Case Transform](#case-transform)
    - [Example](#example-3)
- [Develop](#develop)

# Install

1. Download [`DevTools-{version}-{arch}.zip`](https://github.com/Caceresenzo/Community.PowerToys.Run.Plugin.DevTools/releases).
2. Unzip it into `%LOCALAPPDATA%\Microsoft\PowerToys\PowerToys Run\Plugins`.

# Usage

## Hash

Use an algorithm to hash a message.

```
<hash algorithm> <message>
```

Where `<hash algorithm>` can be any (case insensitive) of the following:
- `MD5`
- `SHA1`
- `SHA256`
- `SHA384`
- `SHA512`

### Example

```
> md5 hello
< 5D41402ABC4B2A76B9719D911017C592
```

## UUID

Generate a random UUID.

```
uuid
```

### Example

```
> uuid
< 06bf70d9-70ea-4a66-8772-a43dc4c719f8
```

> [!TIP]
> Press <kbd>space</kbd> to get a new UUID.

## Lorem Ipsum

Generate random Lorem Ipsum text.

```
lorem [<number of repeat>]
```

### Example

```
> lorem 10
< 10 sentences
< 10 paragraphs
< 10 words
```

## Case Transform

Uppercase or lowercase a message.

```
upper <input>
lower <input>
```

### Example

```
> upper Hello
< HELLO
```

```
> lower Hello
< hello
```

# Develop

Run the script [deploy.ps1](./Community.PowerToys.Run.Plugin.DevTools/deploy.ps1).

The logs will be located at: `%LOCALAPPDATA%\Microsoft\PowerToys\PowerToys Run\Logs\DevTools`
