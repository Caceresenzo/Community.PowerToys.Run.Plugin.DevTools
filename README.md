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
  - [URL Encoding](#url-encoding)
    - [Example](#example-4)
  - [Slash Conversion](#slash-conversion)
    - [Example](#example-5)
  - [Base64 Conversion](#base64-conversion)
    - [Example](#example-6)
- [Develop](#develop)
- [Release](#release)

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
camel <input>
pascal <input>
snake <input>
kebab <input>
space <input>
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

```
> camel Hello World
< helloWorld
< hello World
```

```
> pascal hello World
< HelloWorld
< Hello World
```

```
> snake Hello World
< hello_world
< HELLO_WORLD
```

```
> kebab Hello World
< hello-world
< HELLO-WORLD
```

```
> space Hello_World
< Hello World
< hello world
< HELLO WORLD
```

## URL Encoding

Encode or decode a string for use in a URL.

```
urle <input>
urld <input>
```

### Example

```
> urle Hello/World
< Hello%2FWorld
```

```
> urld Hello%2FWorld
< Hello/World
```

## Slash Conversion

Replace slash to either Window or Unix separators.

```
slash <input>
```

### Example

```
> slash Hello/World
< Hello/World
< Hello\World
```

```
> slash Hello\World
< Hello/World
< Hello\World
```

## Base64 Conversion

Encode or decode a string in Base64.

```
base64e <input>
base64d <input>
```

### Example

```
> base64e Hello World
< SGVsbG8gV29ybGQ=
< SGVsbG8gV29ybGQ
```

```
> base64d SGVsbG8gV29ybGQ=
< Hello World
> base64d SGVsbG8gV29ybGQ
< Hello World
```

# Develop

Run the script [deploy.ps1](./Community.PowerToys.Run.Plugin.DevTools/deploy.ps1).

The logs will be located at: `%LOCALAPPDATA%\Microsoft\PowerToys\PowerToys Run\Logs\DevTools`

# Release

Run the script [pack.ps1](./Community.PowerToys.Run.Plugin.DevTools/pack.ps1).
