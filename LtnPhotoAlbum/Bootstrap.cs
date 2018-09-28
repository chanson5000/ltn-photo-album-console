using System;
using LtnPhotoAlbum.Control;
using LtnPhotoAlbum.Repository;

namespace LtnPhotoAlbum
{
    public class Bootstrap
    {
        public void Start(string[] args=null)
        {
            PhotoAlbumRepository repository  = new PhotoAlbumRepository();
            ConsoleControl consoleControl = new ConsoleControl(repository);
            
            if (args?.Length > 0)
            {
                consoleControl.Execute(string.Join(" ", args));
            }
            else
            {
                consoleControl.ShowGreeting();
                consoleControl.ShowHelp();

                while (consoleControl.Option != ConsoleControl.Command.Exit)
                {
                    consoleControl.ShowCommandPrompt();
                    consoleControl.Execute(Console.ReadLine());
                }
            }
        }
    }
}