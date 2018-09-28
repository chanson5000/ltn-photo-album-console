using System;
using LtnPhotoAlbum.Control;
using LtnPhotoAlbum.Repository;

namespace LtnPhotoAlbum
{
    public class Bootstrap
    {
        private PhotoAlbumRepository _repository;
        private ConsoleControl _consoleControl;
        private string[] _args;

        public void Start(string[] args=null)
        {
            _args = args;
            _repository = new PhotoAlbumRepository();
            _consoleControl = new ConsoleControl(_repository);

            _consoleControl.ShowGreeting();
            _consoleControl.ShowHelp();

            while (_consoleControl.Option != ConsoleControl.Command.Exit)
            {
                _consoleControl.ShowCommandPrompt();
                _consoleControl.Execute(Console.ReadLine());
            }
        }
    }
}