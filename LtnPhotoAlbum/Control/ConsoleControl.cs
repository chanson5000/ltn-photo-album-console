using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LtnPhotoAlbum.Interface;
using LtnPhotoAlbum.Model;

namespace LtnPhotoAlbum.Control
{
    public class ConsoleControl
    {
        private const string GreetingText = "Welcome to photo album.";
        private const string HelpText = "Commands:\n" +
                                         "'photo-album' for an id and title list of all photos in photo album.\n" +
                                         "'photo-album <#>' for an id and title list of all photos from album #.\n" +
                                         "'help' brings you to this screen.\n" +
                                         "'exit' to exit.";

        private const string CommandPrompt = ">";

        private const string UnrecognizedCommand = "Un-recognized command. Try 'help'";

        private readonly IPhotoAlbumRepository _repository;

        public ConsoleControl(IPhotoAlbumRepository repository)
        {
            _repository = repository;
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
            Console.Write(CommandPrompt);
        }

        public void ParseInput(string[] input)
        {
            var command = new Command(input);
            Execute(command);
        }

        public void ParseInput(string input)
        {
            var command = new Command(input);
            Execute(command);
        }

        public void Execute(Command command)
        {
            if (command.GetArg() == Command.Help)
            {
                Console.WriteLine(HelpText);
            }
            else if (command.GetArg() == Command.PhotoAlbum)
            {
                var printPhotosTask = PrintPhotos(command.GetParam());
                printPhotosTask.Wait();
            }
            else if (command.GetArg() != Command.Exit)
            {
                Console.WriteLine(UnrecognizedCommand);
            }
        }

        private async Task PrintPhotos(ushort? albumId = null)
        {
            var results = albumId == null
                ? await _repository.GetAllPhotos()
                : await _repository.GetPhotosByAlbumId((ushort)albumId);

            if (results == null)
            {
                Console.WriteLine("There was a problem retrieving the results.");
            }
            else
            {
                var numResults = 0;
                foreach (var photo in results)
                {
                    Console.WriteLine($"[{photo.Id}] {photo.Title}");
                    numResults++;
                }
                Console.WriteLine($"Returned {numResults} results.");
            }
        }
    }
}
