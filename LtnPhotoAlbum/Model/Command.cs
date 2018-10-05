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

        public string Arg { get; private set; }
        public ushort? Param { get; private set; }

        public bool SetArg(string arg)
        {
            if (ParseArg(arg) != null)
            {
                Arg = arg;
                return true;
            }
            Arg = null;
            return false;
        }

        public bool SetParam(ushort? param)
        {
            if (param != null)
            {
                Param = param;
                return true;
            }
            Param = null;
            return false;
        }

        public bool SetParam(string param) => SetParam(ParseParam(param));

        private static string ParseArg(string input) => ValidArgs.Contains(input) ? input : null;

        private static ushort? ParseParam(string input) => ushort.TryParse(input, out var result) ? (ushort?)result : null;
    }
}