using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Community.PowerToys.Run.Plugin.Community.PowerToys.Run.Plugin.DevTools
{
    public interface IDataGenerator
    {
        List<GeneratedValue> GenerateValues(string commandName, string arguments);

        List<Recommandation> Recommand();
    }

    public class GeneratedValue
    {
        public string Value { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
    }

    public class Recommandation
    {
        public string SubCommand { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
    }

    public class HashDataGenerator : IDataGenerator
    {
        public static readonly ImmutableDictionary<string, Func<byte[], byte[]>> HashAlgorithms =
            ImmutableDictionary.CreateRange([
                KeyValuePair.Create("md5", (byte[] input) => MD5.HashData(input)),
                KeyValuePair.Create("sha1", (byte[] input) => SHA1.HashData(input)),
                KeyValuePair.Create("sha256", (byte[] input) => SHA256.HashData(input)),
                KeyValuePair.Create("sha384", (byte[] input) => SHA384.HashData(input)),
                KeyValuePair.Create("sha512", (byte[] input) => SHA512.HashData(input)),
            ]);

        public List<GeneratedValue> GenerateValues(string commandName, string arguments)
        {
            if (!HashAlgorithms.ContainsKey(commandName) || string.IsNullOrWhiteSpace(arguments))
            {
                return null;
            }

            var algorithm = HashAlgorithms.GetValueOrDefault(commandName);

            var inputBytes = System.Text.Encoding.UTF8.GetBytes(arguments);
            var hashBytes = algorithm(inputBytes);

            var hashString = Convert.ToHexString(hashBytes);
            var lowerHashString = hashString.ToLowerInvariant();

            return
            [
                new GeneratedValue { Value = hashString, SubTitle = $"{commandName}({arguments})" },
                new GeneratedValue
                {
                    Value = lowerHashString,
                    SubTitle = $"{commandName}({arguments})",
                },
            ];
        }

        public List<Recommandation> Recommand()
        {
            var recommandations = new List<Recommandation>();

            foreach (var entry in HashAlgorithms)
            {
                var algorithmName = entry.Key.ToLowerInvariant();

                recommandations.Add(
                    new Recommandation
                    {
                        SubCommand = algorithmName,
                        Title =
                            $"{algorithmName} - Hash a string with {algorithmName.ToUpperInvariant()}",
                        SubTitle = $"Example: {algorithmName} <your input>",
                    }
                );
            }

            return recommandations;
        }
    }

    public class UuidDataGenerator : IDataGenerator
    {
        public const string UuidCommandName = "uuid";

        public List<GeneratedValue> GenerateValues(string commandName, string arguments)
        {
            if (commandName != UuidCommandName)
            {
                return null;
            }

            return
            [
                new GeneratedValue
                {
                    Value = Guid.NewGuid().ToString("D"),
                    SubTitle = "Version 4: Randomly generated UUID",
                },
            ];
        }

        public List<Recommandation> Recommand()
        {
            return
            [
                new Recommandation
                {
                    SubCommand = UuidCommandName,
                    Title = $"{UuidCommandName} - Generate a random UUID",
                    SubTitle = $"Example: {UuidCommandName}",
                },
            ];
        }
    }

    public class LoremDataGenerator : IDataGenerator
    {
        public const string LoremCommandName = "lorem";

        private static readonly string FirstSentence =
            "Lorem ipsum dolor sit amet, consectetur adipiscing elit";

        private static readonly string[] Words =
        [
            "lorem",
            "ipsum",
            "dolor",
            "sit",
            "amet",
            "consectetuer",
            "adipiscing",
            "elit",
            "sed",
            "diam",
            "nonummy",
            "nibh",
            "euismod",
            "tincidunt",
            "ut",
            "laoreet",
            "dolore",
            "magna",
            "aliquam",
            "erat",
        ];

        public List<GeneratedValue> GenerateValues(string commandName, string arguments)
        {
            if (commandName != LoremCommandName)
            {
                return null;
            }

            var repeatCount = 1;
            if (int.TryParse(arguments, out var parsedCount) && parsedCount > 0)
            {
                repeatCount = parsedCount;
            }

            var random = new Random();

            var sentenceRepeatCount = Math.Min(repeatCount, 200);
            var sentencesString = GenerateSentence(random, sentenceRepeatCount);

            var paragraphRepeatCount = Math.Min(repeatCount, 100);
            var paragraphsString = GenerateParagraph(random, paragraphRepeatCount);

            var wordRepeatCount = Math.Min(repeatCount, 10000);
            var wordsString = Generate(random, wordRepeatCount);

            return
            [
                new GeneratedValue
                {
                    Value = sentencesString,
                    Title = $"{sentenceRepeatCount} sentence(s), include the first sentence",
                    SubTitle = $"{sentencesString.Length} character(s)",
                },
                new GeneratedValue
                {
                    Value = paragraphsString,
                    Title = $"{paragraphRepeatCount} paragraph(s), include the first sentence",
                    SubTitle = $"{paragraphsString.Length} character(s)",
                },
                new GeneratedValue
                {
                    Value = wordsString,
                    Title = $"{wordRepeatCount} words(s), fully random",
                    SubTitle = $"{wordsString.Length} character(s)",
                },
            ];
        }

        private static string Generate(Random random, int wordCount)
        {
            var loremWords = Words.OrderBy(_ => random.Next()).Take(wordCount).ToArray();

            return string.Join(" ", loremWords);
        }

        private static string GenerateSentence(Random random, int count)
        {
            var sentences = new List<string>();
            sentences.Add(FirstSentence);

            for (var index = 1; index < count; index++)
            {
                var wordCount = random.Next(5, 15);
                sentences.Add(Generate(random, wordCount));
            }

            return JoinSentences(sentences);
        }

        private static string GenerateParagraph(Random random, int sentenceCount)
        {
            var sentences = new List<string>();
            for (var index = 0; index < sentenceCount; index++)
            {
                var wordCount = random.Next(5, 15);
                sentences.Add(GenerateSentence(random, wordCount));
            }

            return string.Join("\n", sentences);
        }

        private static string JoinSentences(List<string> wordSets)
        {
            var first = wordSets.FirstOrDefault();
            wordSets[0] = first[..1].ToUpperInvariant() + first[1..];

            return string.Join(". ", wordSets) + ".";
        }

        public List<Recommandation> Recommand()
        {
            return
            [
                new Recommandation
                {
                    SubCommand = LoremCommandName,
                    Title = $"{LoremCommandName} - Generate a lorem ipsum text",
                    SubTitle = $"Example: {LoremCommandName} [<number of repeat>]",
                },
            ];
        }
    }

    public partial class CaseDataGenerator : IDataGenerator
    {
        public const string LowerCommandName = "lower";
        public const string UpperCommandName = "upper";
        public const string CamelCommandName = "camel";
        public const string PascalCommandName = "pascal";
        public const string SnakeCommandName = "snake";
        public const string KebabCommandName = "kebab";
        public const string SpaceCommandName = "space";

        public List<GeneratedValue> GenerateValues(string commandName, string arguments)
        {
            if (string.IsNullOrWhiteSpace(arguments))
            {
                return null;
            }

            if (commandName == LowerCommandName)
            {
                return
                [
                    new GeneratedValue
                    {
                        Value = arguments.ToLowerInvariant(),
                        SubTitle = $"LOWER({arguments})",
                    },
                ];
            }

            if (commandName == UpperCommandName)
            {
                return
                [
                    new GeneratedValue
                    {
                        Value = arguments.ToUpperInvariant(),
                        SubTitle = $"UPPER({arguments})",
                    },
                ];
            }

            if (commandName == CamelCommandName)
            {
                return
                [
                    new GeneratedValue
                    {
                        Value = ToCamel(arguments),
                        SubTitle = $"CAMEL({arguments})",
                    },
                    new GeneratedValue
                    {
                        Value = ToCamel(arguments, " "),
                        SubTitle = $"SPACE_CAMEL({arguments})",
                    },
                ];
            }

            if (commandName == PascalCommandName)
            {
                return
                [
                    new GeneratedValue
                    {
                        Value = ToPascal(arguments),
                        SubTitle = $"PASCAL({arguments})",
                    },
                    new GeneratedValue
                    {
                        Value = ToPascal(arguments, " "),
                        SubTitle = $"SPACE_PASCAL({arguments})",
                    },
                ];
            }

            if (commandName == SnakeCommandName)
            {
                var snakeCase = ToSnake(arguments);

                return
                [
                    new GeneratedValue { Value = snakeCase, SubTitle = $"SNAKE({arguments})" },
                    new GeneratedValue
                    {
                        Value = snakeCase.ToUpperInvariant(),
                        SubTitle = $"UPPER_SNAKE({arguments})",
                    },
                ];
            }

            if (commandName == KebabCommandName)
            {
                var kebabCase = ToKebab(arguments);

                return
                [
                    new GeneratedValue { Value = kebabCase, SubTitle = $"KEBAB({arguments})" },
                    new GeneratedValue
                    {
                        Value = kebabCase.ToUpperInvariant(),
                        SubTitle = $"UPPER_KEBAB({arguments})",
                    },
                ];
            }

            if (commandName == SpaceCommandName)
            {
                var spaceCase = ToSpace(arguments);

                return
                [
                    new GeneratedValue { Value = spaceCase, SubTitle = $"SPACE({arguments})" },
                    new GeneratedValue
                    {
                        Value = spaceCase.ToLowerInvariant(),
                        SubTitle = $"LOWER_SPACE({arguments})",
                    },
                    new GeneratedValue
                    {
                        Value = spaceCase.ToUpperInvariant(),
                        SubTitle = $"UPPER_SPACE({arguments})",
                    },
                ];
            }

            return null;
        }

        public List<Recommandation> Recommand()
        {
            return
            [
                new Recommandation
                {
                    SubCommand = LowerCommandName,
                    Title = $"{LowerCommandName} - Lowercase a string",
                    SubTitle = $"Example: {LowerCommandName} <your input>",
                },
                new Recommandation
                {
                    SubCommand = UpperCommandName,
                    Title = $"{UpperCommandName} - Uppercase a string",
                    SubTitle = $"Example: {UpperCommandName} <your input>",
                },
                new Recommandation
                {
                    SubCommand = CamelCommandName,
                    Title = $"{CamelCommandName} - Camel case a string",
                    SubTitle = $"Example: {CamelCommandName} <your input>",
                },
                new Recommandation
                {
                    SubCommand = PascalCommandName,
                    Title = $"{PascalCommandName} - Pascal case a string",
                    SubTitle = $"Example: {PascalCommandName} <your input>",
                },
                new Recommandation
                {
                    SubCommand = SnakeCommandName,
                    Title = $"{SnakeCommandName} - Snake case a string",
                    SubTitle = $"Example: {SnakeCommandName} <your input>",
                },
                new Recommandation
                {
                    SubCommand = KebabCommandName,
                    Title = $"{KebabCommandName} - Kebab case a string",
                    SubTitle = $"Example: {KebabCommandName} <your input>",
                },
                new Recommandation
                {
                    SubCommand = SpaceCommandName,
                    Title = $"{SpaceCommandName} - Space a string",
                    SubTitle = $"Example: {SpaceCommandName} <your input>",
                },
            ];
        }

        public static string SplitOnCaps(string input)
        {
            var splits = new List<int>();
            var characters = input.ToCharArray();

            for (var index = 1; index < characters.Length; index++)
            {
                var previousIsLower = !IsUpperCase(characters[index - 1]);
                var nextIsLower =
                    index + 1 >= characters.Length || !IsUpperCase(characters[index + 1]);

                if (IsUpperCase(characters[index]) && (nextIsLower || previousIsLower))
                {
                    splits.Add(index);
                }
            }

            var builder = new StringBuilder();

            var lastSplit = 0;
            foreach (var split in splits)
            {
                builder.Append(input.Substring(lastSplit, split - lastSplit) + " ");
                lastSplit = split;
            }

            builder.Append(input.Substring(lastSplit));

            return builder.ToString();
        }

        public static bool IsUpperCase(char character)
        {
            return char.IsUpper(character);
        }

        [GeneratedRegex(@"[^\p{L}\p{N}]")]
        private static partial Regex OnlyLettersAndNumbersRegex();

        public static string Chunk(string input, Func<string, string> selector, string separator)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return string.Empty;
            }

            return string.Join(
                separator,
                SplitOnCaps(OnlyLettersAndNumbersRegex().Replace(input, " "))
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Select(selector)
            );
        }

        public static string ToPascal(string input, string separator = "")
        {
            return Chunk(
                input,
                part => char.ToUpperInvariant(part[0]) + part[1..].ToLowerInvariant(),
                separator
            );
        }

        public static string ToCamel(string input, string separator = "")
        {
            var pascalCase = ToPascal(input, separator);
            if (string.IsNullOrEmpty(pascalCase))
            {
                return string.Empty;
            }

            return char.ToLowerInvariant(pascalCase[0]) + pascalCase[1..];
        }

        public static string ToSnake(string input)
        {
            return Chunk(input, part => part.ToLowerInvariant(), "_");
        }

        public static string ToKebab(string input)
        {
            return Chunk(input, part => part.ToLowerInvariant(), "-");
        }

        public static string ToSpace(string input)
        {
            return Chunk(input, part => part, " ");
        }
    }

    public class UrlDataGenerator : IDataGenerator
    {
        public const string ShortEncodeCommandName = "urle";
        public const string EncodeCommandName = "urlencode";
        public const string ShortDecodeCommandName = "urld";
        public const string DecodeCommandName = "urldecode";

        public List<GeneratedValue> GenerateValues(string commandName, string arguments)
        {
            if (commandName == EncodeCommandName || commandName == ShortEncodeCommandName)
            {
                return
                [
                    new GeneratedValue
                    {
                        Value = Uri.EscapeDataString(arguments),
                        SubTitle = $"URL_ENCODE({arguments})",
                    },
                ];
            }

            if (commandName == DecodeCommandName || commandName == ShortDecodeCommandName)
            {
                return
                [
                    new GeneratedValue
                    {
                        Value = Uri.UnescapeDataString(arguments),
                        SubTitle = $"URL_DECODE({arguments})",
                    },
                ];
            }

            return null;
        }

        public List<Recommandation> Recommand()
        {
            return
            [
                new Recommandation
                {
                    SubCommand = EncodeCommandName,
                    Title = $"{EncodeCommandName} - URL Encode a string",
                    SubTitle = $"Example: {EncodeCommandName} <your input>",
                },
                new Recommandation
                {
                    SubCommand = DecodeCommandName,
                    Title = $"{DecodeCommandName} - URL Decode a string",
                    SubTitle = $"Example: {DecodeCommandName} <your input>",
                },
            ];
        }
    }

    public class Base64DataGenerator : IDataGenerator
    {
        public const string ShortEncodeCommandName = "base64e";
        public const string EncodeCommandName = "base64encode";
        public const string ShortDecodeCommandName = "base64d";
        public const string DecodeCommandName = "base64decode";

        public List<GeneratedValue> GenerateValues(string commandName, string arguments)
        {
            if (commandName == EncodeCommandName || commandName == ShortEncodeCommandName)
            {
                var encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(arguments));
                var encodedUrlSafe = encoded.TrimEnd('=').Replace('+', '-').Replace('/', '_');

                return
                [
                    new GeneratedValue
                    {
                        Value = encoded,
                        SubTitle = $"BASE64_ENCODE({arguments})",
                    },
                    new GeneratedValue
                    {
                        Value = encodedUrlSafe,
                        SubTitle = $"URL_SAFE_BASE64_ENCODE({arguments})",
                    },
                ];
            }

            if (commandName == DecodeCommandName || commandName == ShortDecodeCommandName)
            {
                arguments = arguments
                    .Replace('-', '+')
                    .Replace('_', '/')
                    .PadRight(arguments.Length + (4 - arguments.Length % 4) % 4, '=');

                var decoded = "(invalid base64 input)";
                try
                {
                    decoded = Encoding.UTF8.GetString(Convert.FromBase64String(arguments));
                }
                catch (FormatException) { }

                return
                [
                    new GeneratedValue
                    {
                        Value = decoded,
                        SubTitle = $"BASE64_DECODE({arguments})",
                    },
                ];
            }

            return null;
        }

        public List<Recommandation> Recommand()
        {
            return
            [
                new Recommandation
                {
                    SubCommand = EncodeCommandName,
                    Title = $"{EncodeCommandName} - Encode a string to Base64",
                    SubTitle = $"Example: {EncodeCommandName} <your input>",
                },
                new Recommandation
                {
                    SubCommand = DecodeCommandName,
                    Title = $"{DecodeCommandName} - Decode a string from Base64",
                    SubTitle = $"Example: {DecodeCommandName} <your input>",
                },
            ];
        }
    }

    public class SlashDataGenerator : IDataGenerator
    {
        public const string ShortWindowCommandName = "winslash";
        public const string WindowCommandName = "windowsslash";
        public const string UnixCommandName = "unixslash";

        public List<GeneratedValue> GenerateValues(string commandName, string arguments)
        {
            if (commandName == ShortWindowCommandName || commandName == WindowCommandName)
            {
                return
                [
                    new GeneratedValue
                    {
                        Value = arguments.Replace("/", "\\"),
                        SubTitle = $"WIN_SLASH({arguments})",
                    },
                ];
            }

            if (commandName == UnixCommandName)
            {
                return
                [
                    new GeneratedValue
                    {
                        Value = arguments.Replace("\\", "/"),
                        SubTitle = $"UNIX_SLASH({arguments})",
                    },
                ];
            }

            return null;
        }

        public List<Recommandation> Recommand()
        {
            return
            [
                new Recommandation
                {
                    SubCommand = ShortWindowCommandName,
                    Title = $"{ShortWindowCommandName} - Convert slashes to Windows separator",
                    SubTitle = $"Example: {ShortWindowCommandName} <your input>",
                },
                new Recommandation
                {
                    SubCommand = UnixCommandName,
                    Title = $"{UnixCommandName} - Convert slashes to Unix separator",
                    SubTitle = $"Example: {UnixCommandName} <your input>",
                },
            ];
        }
    }
}
