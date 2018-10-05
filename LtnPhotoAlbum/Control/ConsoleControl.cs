using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using LtnPhotoAlbum.Interface;
using LtnPhotoAlbum.Model;

namespace LtnPhotoAlbum.Control
{
    public class ConsoleControl
    {
        public const string Greeting = "Welcome to photo album.";

        public const string Help = "Commands:" + "\r\n" +
                                         "'photo-album' for an id and title list of all photos in photo album." + "\r\n" +
                                         "'photo-album <#>' for an id and title list of all photos from album #." + "\r\n" +
                                         "'help' brings you to this screen." + "\r\n" +
                                         "'exit' to exit.";
        public const string CommandPrompt = ">";

        public const string RetrievingAllPhotos = "Retrieving all photos...";
        public const string RetrievingPhotosFromAlbum = "Retrieving photos from album id...";

        public const string InvalidPhotoCommand = "Invalid 'photo-album' <#> command. # must be a positive integer.";
        public const string UnrecognizedCommand = "Un-recognized command. Try 'help'";

        public const string ProblemNetworkConnectivity = "There was a problem with the network connection.";
        public const string ProblemRequestTimeout = "The request timed out.";
        public const string ProblemResults = "Unable to retrieve results.";

        private readonly IPhotoAlbumRepository _repository;

        public ConsoleControl(IPhotoAlbumRepository repository) => _repository = repository;

        public void ShowGreeting() => Console.WriteLine(Greeting);

        public void ShowHelp() => Console.WriteLine(Help);

        public void ShowCommandPrompt() => Console.Write(CommandPrompt);

        public void ParseInput(string[] input)
        {
            var command = new Command();

            if (input.Length > 0 && command.SetArg(input[0]))
            {
                if (input.Length > 1 && command.Arg == Command.PhotoAlbum)
                {
                    if (command.SetParam(input[1]))
                    {
                        Execute(command);
                        return;
                    }
                    Console.WriteLine(InvalidPhotoCommand);
                    return;
                }
                Execute(command);
                return;
            }

            Console.WriteLine(UnrecognizedCommand);
        }

        public void ParseInput(string input)
        {
            ParseInput(string.IsNullOrWhiteSpace(input) ? new string[0] : input.Split(new char[0], StringSplitOptions.RemoveEmptyEntries));
        }

        private void Execute(Command command)
        {
            switch (command.Arg)
            {
                case Command.Help:
                    Console.WriteLine(Help);
                    break;
                case Command.PhotoAlbum:
                    Console.WriteLine(command.Param == null ? RetrievingAllPhotos : RetrievingPhotosFromAlbum);
                    var printPhotosTask = PrintPhotos(command.Param);
                    printPhotosTask.Wait();
                    break;
                default:
                    if (command.Arg != Command.Exit)
                    {
                        Console.WriteLine(UnrecognizedCommand);
                    }
                    break;
            }
        }

        private async Task PrintPhotos(ushort? albumId = null)
        {
            List<Photo> results;
            try
            {
                results = albumId == null
                    ? await _repository.GetAllPhotos()
                    : await _repository.GetPhotosByAlbumId((ushort)albumId);
            }
            catch (HttpRequestException e)
            {
                Debug.WriteLine(e.Message);
                Console.WriteLine(ProblemNetworkConnectivity);
                results = null;
            }
            catch (TaskCanceledException e)
            {
                Debug.WriteLine(e.Message);
                Console.WriteLine(ProblemRequestTimeout);
                results = null;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                results = null;
            }

            if (results == null)
            {
                Console.WriteLine(ProblemResults);
                return;
            }

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