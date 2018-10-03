using System;
using LtnPhotoAlbum.Control;
using LtnPhotoAlbum.Model;
using LtnPhotoAlbum.Repository;

namespace LtnPhotoAlbum
{
    public class Bootstrap
    {
        private readonly ConsoleControl _consoleControl = new ConsoleControl(new PhotoAlbumRepository());

        public void Start(string[] args=null)
        {
            if (args?.Length > 0)
            {
                _consoleControl.ParseInput(args);
            }
            else
            {
                _consoleControl.ShowGreeting();
                _consoleControl.ShowHelp();

                string input = null;
                while (input != Command.Exit)
                {
                    _consoleControl.ShowCommandPrompt();
                    input = Console.ReadLine();
                    _consoleControl.ParseInput(input);
                }
            }
        }
    }
}