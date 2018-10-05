using System;
using System.IO;
using System.Threading.Tasks;
using LtnPhotoAlbum.Control;
using NUnit.Framework;

namespace LtnPhotoAlbum.UnitTests
{
    [TestFixture]
    public class BootstrapTests
    {
        private StringWriter _result;
        private Bootstrap _bootstrap;

        [SetUp]
        public void SetUp()
        {
            _bootstrap = new Bootstrap();
            _result = new StringWriter();
        }

        [Test]
        public void OnStartWithNoArgs_ShowGreetingAndHelp()
        {
            const string expected = "Welcome to photo album.\r\n" +
                                    "Commands:\r\n" +
                                    "'photo-album' for an id and title list of all photos in photo album.\r\n" +
                                    "'photo-album <#>' for an id and title list of all photos from album #.\r\n" +
                                    "'help' brings you to this screen.\r\n" +
                                    "'exit' to exit.\r\n" +
                                    ">";

            using (_result)
            {
                Console.SetOut(_result);

                var task = new Task(() => _bootstrap.Start());
                task.Start();
                task.Wait(2000);

                Assert.That(_result.ToString(), Is.EqualTo(expected));
            }
        }

        [Test]
        public void OnStartWithInvalidArgs_ShowError()
        {
            var commandToExecute = new[] { "invalid" };

            const string expected = ConsoleControl.UnrecognizedCommand + "\r\n";

            using (_result)
            {
                Console.SetOut(_result);

                _bootstrap.Start(commandToExecute);

                Assert.That(_result.ToString(), Is.EqualTo(expected));
            }
        }

        [Test]
        public void OnStartWithValidArgs_DoesNotReturnError(
            [Values("help", "photo-album", "photo-album 1", "exit")]
            string commandToParse
            )
        {
            var commandToExecute = commandToParse.Split();

            const string expected = "Un-recognized command. Try 'help'\r\n>";

            using (_result)
            {
                Console.SetOut(_result);

                _bootstrap.Start(commandToExecute);

                var parsedResult = _result.ToString();
                Assert.That(parsedResult, Is.Not.EqualTo(expected));
            }
        }
    }
}