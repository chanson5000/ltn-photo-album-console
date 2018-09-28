using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LtnPhotoAlbum.Interface;
using LtnPhotoAlbum.Model;

namespace LtnPhotoAlbum.Control
{
    public class ConsoleControl
    {
        private const string Exit = "exit";
        private const string Help = "help";
        private const string PhotoAlbum = "photo-album";

        private const string GreetingText = "Welcome to photo album.";
        private const string HelpText = "Commands:\n" +
                                         "'photo-album' for an id and title list of all photos in photo album.\n" +
                                         "'photo-album <#>' for an id and title list of all photos from album #.\n" +
                                         "'help' brings you to this screen.\n" +
                                         "'exit' to exit.";

        private const string CommandPrompt = "Please enter a command: ";

        private const string UnrecognizedCommand = "Un-recognized command. Try 'help'";

        private static readonly List<string> Commands = new List<string>
        {
            Exit,
            Help,
            PhotoAlbum
        };

        private ushort? _arg;
        private bool _argError;
        public string Option { get; private set; }
        private readonly IPhotoAlbumRepository _repository;

        public ConsoleControl(IPhotoAlbumRepository repository)
        {
            _repository = repository;
        }

        public sealed class Command
        {
            public const string Exit = ConsoleControl.Exit;
            public const string Help = ConsoleControl.Help;
            public const string PhotoAlbum = ConsoleControl.PhotoAlbum;
        }

        public void ShowGreeting()
        {
              Console.WriteLine(GreetingText);
        }

        public void ShowHelp()
        {
            Console.WriteLine(HelpText);
        }

        public void ShowCommandPrompt()
        {
            Console.WriteLine(CommandPrompt);
        }

        public void Execute(string input)
        {
            if (!IsValid(input))
            {
                // Input is not valid.
                Console.WriteLine(UnrecognizedCommand);
            }
            else
            {
                // Input is valid and parsed.
                switch (Option)
                {
                    case Help:
                        Console.WriteLine(HelpText);
                        break;
                    case PhotoAlbum when _arg != null:
                        // List all photos with supplied album id.
                        Console.WriteLine($"Retrieving photos for album # {_arg}");
                        var photosByAlbumTask = PrintPhotosFromAlbumId(Convert.ToUInt16(_arg));
                        photosByAlbumTask.Wait();
                        break;
                    case PhotoAlbum when _arg == null && _argError:
                        // PhotoAlbum command had invalid argument.
                        Console.WriteLine("Invalid 'photo-album' argument. Must be a positive number.");
                        break;
                    case PhotoAlbum:
                        // All photos.
                        Console.WriteLine("Retrieving all photos...");
                        var allPhotosTask = PrintAllPhotos();
                        allPhotosTask.Wait();
                        break;
                }
            }
        }

        private bool IsValid(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                // Input is empty, reset all values.
                Option = null;
                _arg = null;
                _argError = false;
                return false;
            }

            input = input.Trim();

            if (!input.Contains(" "))
            {
                // Input contained only one argument, validate.
                if (Commands.Contains(input))
                {
                    // Option was valid.
                    Option = input;
                    _arg = null;
                    _argError = false;
                    return true;
                }
            }
            else
            {
                // Input contained option and at least one argument, split and validate.
                var paramSplit = input.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);

                if (Commands.Contains(paramSplit[0]))
                {
                    // Option is valid.
                    Option = paramSplit[0];

                    // Parse argument as positive integer.
                    if (ushort.TryParse(paramSplit[1], out var parsedInt))
                    {
                        // Integer was valid argument.
                        _arg = parsedInt;
                        _argError = false;
                        return true;
                    }

                    // Integer was invalide argument. Set _argError.
                    _arg = null;
                    _argError = true;
                    return true;
                }

                // Option was not valid.
            }

            // Option was not valid, reset all values.
            Option = null;
            _arg = null;
            _argError = false;
            return false;
        }

        private async Task PrintAllPhotos()
        {
            List<Photo> results = await _repository.GetAllPhotos();
            if (results == null)
            {
                Console.WriteLine("Error retrieving results.");
            }
            else
            {
                int numResults = 0;
                foreach (Photo photo in results)
                {
                    Console.WriteLine($"[{photo.Id}] {photo.Title}");
                    numResults++;
                }
                Console.WriteLine($"Returned {numResults} results.");
            }
        }

        private async Task PrintPhotosFromAlbumId(int albumId)
        {
            List<Photo> results = await _repository.GetPhotosByAlbumId(albumId);
            if (results == null)
            {
                Console.WriteLine("Error retrieving results.");
            }
            else
            {
                int numResults = 0;
                foreach (Photo photo in results)
                {
                    Console.WriteLine($"[{photo.Id}] {photo.Title}");
                    numResults++;
                }
                Console.WriteLine($"Returned {numResults} results.");
            }
        }
    }
}