using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Security.Cryptography;

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
                KeyValuePair.Create("md5",    (byte[] input) => MD5.HashData(input)),
                KeyValuePair.Create("sha1",   (byte[] input) => SHA1.HashData(input)),
                KeyValuePair.Create("sha256", (byte[] input) => SHA256.HashData(input)),
                KeyValuePair.Create("sha384", (byte[] input) => SHA384.HashData(input)),
                KeyValuePair.Create("sha512", (byte[] input) => SHA512.HashData(input)),
            ]);

        public List<GeneratedValue> GenerateValues(string commandName, string arguments)
        {
            if (!HashAlgorithms.ContainsKey(commandName))
            {
                return null;
            }

            var algorithm = HashAlgorithms.GetValueOrDefault(commandName);

            var inputBytes = System.Text.Encoding.UTF8.GetBytes(arguments);
            var hashBytes = algorithm(inputBytes);

            var hashString = Convert.ToHexString(hashBytes);
            var lowerHashString = hashString.ToLowerInvariant();

            return [
                new GeneratedValue
                {
                    Value = hashString,
                    SubTitle = $"{commandName}({arguments})",
                },
                new GeneratedValue
                {
                    Value = lowerHashString,
                    SubTitle = $"{commandName}({arguments})",
                }
            ];
        }

        public List<Recommandation> Recommand()
        {
            var recommandations = new List<Recommandation>();

            foreach (var entry in HashAlgorithms)
            {
                var algorithmName = entry.Key.ToLowerInvariant();

                recommandations.Add(new Recommandation
                {
                    SubCommand = algorithmName,
                    Title = $"{algorithmName} - Hash a string with {algorithmName.ToUpperInvariant()}",
                    SubTitle = $"Example: {algorithmName} <your input>",
                });
            }

            return recommandations;
        }
    }

    public class UuidDataGenerator : IDataGenerator
    {
        public List<GeneratedValue> GenerateValues(string commandName, string arguments)
        {
            if (commandName != "uuid")
            {
                return null;
            }

            return [
                new GeneratedValue
                {
                    Value = Guid.NewGuid().ToString("D"),
                    SubTitle = "Version 4: Randomly generated UUID",
                },
            ];
        }

        public List<Recommandation> Recommand()
        {
            return [
                new Recommandation
                {
                    SubCommand = "uuid",
                    Title = "uuid - Generate a random UUID",
                    SubTitle = "Example: uuid",
                }
            ];
        }
    }

    public class LoremDataGenerator : IDataGenerator
    {
        private static readonly string FirstSentence = "Lorem ipsum dolor sit amet, consectetur adipiscing elit";

        private static readonly string[] Words = [
            "lorem", "ipsum", "dolor", "sit", "amet", "consectetuer",
            "adipiscing", "elit", "sed", "diam", "nonummy", "nibh", "euismod",
            "tincidunt", "ut", "laoreet", "dolore", "magna", "aliquam", "erat"
        ];

        public List<GeneratedValue> GenerateValues(string commandName, string arguments)
        {
            if (commandName != "lorem")
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

            return [
                new GeneratedValue
                {
                    Value = sentencesString,
                    Title = $"{sentenceRepeatCount} sentence(s), include the first sentence",
                    SubTitle = $"{sentencesString.Length} character(s)"
                },
                new GeneratedValue
                {
                    Value = paragraphsString,
                    Title = $"{paragraphRepeatCount} paragraph(s), include the first sentence",
                    SubTitle = $"{paragraphsString.Length} character(s)"
                },
                new GeneratedValue
                {
                    Value = wordsString,
                    Title = $"{wordRepeatCount} words(s), fully random",
                    SubTitle = $"{wordsString.Length} character(s)"
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
            return [
                new Recommandation
                {
                    SubCommand = "lorem",
                    Title = "lorem - Generate a lorem ipsum text",
                    SubTitle = "Example: lorem [<number of repeat>]",
                }
            ];
        }
    }

    public class CaseDataGenerator : IDataGenerator
    {
        public const string LowerCommandName = "lower";
        public const string UpperCommandName = "upper";

        public List<GeneratedValue> GenerateValues(string commandName, string arguments)
        {
            if (commandName == LowerCommandName)
            {
                return [
                    new GeneratedValue
                    {
                        Value = arguments.ToLowerInvariant(),
                        SubTitle = $"LOWER({arguments})"
                    },
                ];
            }

            if (commandName == UpperCommandName)
            {
                return [
                    new GeneratedValue
                    {
                        Value = arguments.ToUpperInvariant(),
                        SubTitle = $"UPPER({arguments})"
                    },
                ];
            }

            return null;
        }

        public List<Recommandation> Recommand()
        {
            return [
                new Recommandation
                {
                    SubCommand = LowerCommandName,
                    Title = $"{LowerCommandName} - Lowercase a string",
                    SubTitle = $"Example: ${LowerCommandName} <input>",
                },
                new Recommandation
                {
                    SubCommand = UpperCommandName,
                    Title = $"{UpperCommandName} - Uppercase a string",
                    SubTitle = $"Example: {UpperCommandName} <input>",
                }
            ];
        }
    }

}