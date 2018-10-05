using System;
using System.Collections.Generic;

namespace LtnPhotoAlbum.Model
{
    public class Command
    {
        public const string Exit = "exit";
        public const string Help = "help";
        public const string PhotoAlbum = "photo-album";

        public static readonly List<string> ValidArgs = new List<string>
        {
            Exit,
            Help,
            PhotoAlbum
        };

        private readonly string[] _commandArgs;

        public Command(string commandString) => _commandArgs = Normalize(ToArgArray(commandString));

        public Command(IReadOnlyList<string> commandArray) => _commandArgs = Normalize(commandArray);

        public string GetArg() => _commandArgs[0];

        public ushort? GetParam() => ushort.TryParse(_commandArgs[1], out var result) ? (ushort?)result : null;

        private static string[] Normalize(IReadOnlyList<string> toNormalize)
        {
            var argument = ParseArg(toNormalize[0]);
            var parameter = toNormalize.Count > 1 ? ParseParam(toNormalize[1]) : null;

            return new[] { argument, parameter };
        }

        private static string[] ToArgArray(string input) => string.IsNullOrWhiteSpace(input) ? new[] { null, (string)null } : input.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);

        private static string ParseArg(string input) => ValidArgs.Contains(input) ? input : null;

        private static string ParseParam(string input) => ushort.TryParse(input, out var result) ? result.ToString() : null;
    }
}