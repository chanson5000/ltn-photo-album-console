using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using LtnPhotoAlbum.Control;
using LtnPhotoAlbum.Interface;
using LtnPhotoAlbum.Model;
using Moq;
using NUnit.Framework;

namespace LtnPhotoAlbum.UnitTests
{
    [TestFixture]
    public class ConsoleControlTests
    {
        private Mock<IPhotoAlbumRepository> _mockRepository;
        private ConsoleControl _consoleControl;
        private StringWriter _result;

        private readonly List<Photo> _listOfPhotos = new List<Photo>()
        {
            new Photo()
            {
                Id = 1,
                AlbumId = 1,
                Title = "Title",
                Url = "http://www.url.com",
                ThumbnailUrl = "http://www.thumbnailurl.com"
            },
            new Photo()
            {
                Id = 2,
                AlbumId = 2,
                Title = "Another Title",
                Url = "http://www.url2.com",
                ThumbnailUrl = "http://www.thumbnailurl2.com"
            }
        };

        private readonly List<Photo> _listOfPhotosByAlbum = new List<Photo>()
        {
            new Photo()
            {
                Id = 1,
                AlbumId = 1,
                Title = "Title",
                Url = "http://www.url.com",
                ThumbnailUrl = "http://www.thumbnailurl.com"
            },
            new Photo()
            {
                Id = 2,
                AlbumId = 1,
                Title = "Another Title",
                Url = "http://www.url2.com",
                ThumbnailUrl = "http://www.thumbnailurl2.com"
            }
        };

        private readonly List<Photo> _nullListOfPhotos = null;

        [SetUp]
        public void SetUp()
        {
            _mockRepository = new Mock<IPhotoAlbumRepository>();
            _consoleControl = new ConsoleControl(_mockRepository.Object);
            _result = new StringWriter();
        }

        [Test]
        public void ShowGreeting_WritesGreetingString()
        {
            const string expected = ConsoleControl.GreetingText + "\r\n";

            using (_result)
            {
                Console.SetOut(_result);

                _consoleControl.ShowGreeting();

                Assert.That(_result.ToString(), Is.EqualTo(expected));
            }
        }

        [Test]
        public void ShowHelp_WritesHelpString()
        {
            const string expected = ConsoleControl.HelpText + "\r\n";

            using (_result)
            {
                Console.SetOut(_result);

                _consoleControl.ShowHelp();

                Assert.That(_result.ToString(), Is.EqualTo(expected));
            }
        }

        [Test]
        public void ShowCommandPrompt_WritesCommandPromptString()
        {
            const string expected = ConsoleControl.CommandPrompt;

            using (_result)
            {
                Console.SetOut(_result);

                _consoleControl.ShowCommandPrompt();

                Assert.That(_result.ToString(), Is.EqualTo(expected));
            }
        }

        [Test]
        public void ExecuteHelpCommand_ReturnsHelp()
        {
            const string commandToExecute = "help";

            const string expected = ConsoleControl.HelpText + "\r\n";

            using (_result)
            {
                Console.SetOut(_result);

                _consoleControl.ParseInput(commandToExecute);

                Assert.That(_result.ToString(), Is.EqualTo(expected));
            }
        }

        [Test]
        public void ExecuteWithInvalidOption_ReturnsUnrecognizedCommand(
            [Values("", "    ", "asdf", "adfsdf asdfas", "asdf asdf asdf")]
            string commandToExecute
            )
        {
            const string expected = ConsoleControl.UnrecognizedCommand + "\r\n";

            using (_result)
            {
                Console.SetOut(_result);

                _consoleControl.ParseInput(commandToExecute);

                Assert.That(_result.ToString(), Is.EqualTo(expected));
            }
        }

        [Test]
        public void ExecuteWithSingleInvalidOption_ReturnsUnrecognizedCommand()
        {
            const string commandToExecute = "  ";

            const string expected = ConsoleControl.UnrecognizedCommand + "\r\n";

            using (_result)
            {
                Console.SetOut(_result);

                _consoleControl.ParseInput(commandToExecute);

                Assert.That(_result.ToString(), Is.EqualTo(expected));
            }
        }

        //[Test]
        //public void ExecuteWithValidPhotoAlbumOptionButInvalidArgument_ReturnsArgumentUnrecognized(
        //    [Values("photo-album -1")]
        //    string commandToExecute
        //    )
        //{
        //    const string expected = InvalidPhotoAlbumArgumentString;
        //    var parsedExpected = expected + Environment.NewLine;

        //    using (var result = new StringWriter())
        //    {
        //        Console.SetOut(result);

        //        _consoleControl.Execute(commandToExecute);

        //        var parsedResult = result.ToString();

        //        Assert.That(parsedResult, Is.EqualTo(parsedExpected));
        //    }
        //}

        [Test]
        public void ExecuteWithValidPhotoOption_ReturnsAllPhotos(
            [Values("photo-album", "photo-album    ")]
            string commandToExecute
            )
        {
            _mockRepository.Setup(x => x.GetAllPhotos()).Returns(Task.FromResult(_listOfPhotos));

            const string expected = "[1] Title\r\n" +
                                 "[2] Another Title\r\n" +
                                 "Returned 2 results.\r\n";

            using (_result)
            {
                Console.SetOut(_result);

                _consoleControl.ParseInput(commandToExecute);

                Assert.That(_result.ToString(), Is.EqualTo(expected));
            }
        }

        [Test]
        public void ExecuteWithValidPhotoAlbumArgument_ReturnsAlbumOne()
        {
            _mockRepository.Setup(x => x.GetPhotosByAlbumId(1)).Returns(Task.FromResult(_listOfPhotosByAlbum));

            const string commandToExecute = "photo-album 1";

            const string expected = "[1] Title\r\n" +
                                 "[2] Another Title\r\n" +
                                 "Returned 2 results.\r\n";

            using (_result)
            {
                Console.SetOut(_result);

                _consoleControl.ParseInput(commandToExecute);

                Assert.That(_result.ToString(), Is.EqualTo(expected));
            }
        }

        [Test]
        public void ExecuteAllPhotoWithNullRepoResponse_ReturnsErrorString()
        {
            _mockRepository.Setup(x => x.GetAllPhotos()).Returns(Task.FromResult(_nullListOfPhotos));

            const string commandToExecute = "photo-album";

            const string expected = ConsoleControl.ProblemResults + "\r\n";

            using (_result)
            {
                Console.SetOut(_result);

                _consoleControl.ParseInput(commandToExecute);

                Assert.That(_result.ToString(), Is.EqualTo(expected));
            }
        }

        [Test]
        public void ExecutPhotoAlbumWithNullResponse_ReturnsErrorString()
        {
            _mockRepository.Setup(x => x.GetAllPhotos()).Returns(Task.FromResult(_nullListOfPhotos));

            const string commandToExecute = "photo-album 1";

            const string expected = ConsoleControl.ProblemResults + "\r\n";

            using (_result)
            {
                Console.SetOut(_result);

                _consoleControl.ParseInput(commandToExecute);

                Assert.That(_result.ToString(), Is.EqualTo(expected));
            }
        }
    }
}