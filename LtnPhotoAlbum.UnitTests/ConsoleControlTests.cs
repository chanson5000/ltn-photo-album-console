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
        private const string GreetingString = "Welcome to photo album.";
        private const string HelpString = "Commands:\n" +
                                            "'photo-album' for an id and title list of all photos in photo album.\n" +
                                            "'photo-album <#>' for an id and title list of all photos from album #.\n" +
                                            "'help' brings you to this screen.\n" +
                                            "'exit' to exit.";

        private const string CommandPromptString = "Please enter a command: ";
        private const string UnrecognizedCommandString = "Un-recognized command. Try 'help'";
        private const string InvalidPhotoAlbumArgumentString = "Invalid 'photo-album' argument. Must be a positive number.";
        private const string RetrievingAllPhotosString = "Retrieving all photos...";
        private const string ErrorRetrievingResultsString = "Error retrieving results.";



        [SetUp]
        public void SetUp()
        {
            _mockRepository = new Mock<IPhotoAlbumRepository>();
            _consoleControl = new ConsoleControl(_mockRepository.Object);

        }

        [Test]
        public void ShowGreeting_WritesGreetingString()
        {
            const string expected = GreetingString;
            var parsedExpected = expected + Environment.NewLine;

            using (var result = new StringWriter())
            {
                Console.SetOut(result);

                _consoleControl.ShowGreeting();

                var parsedResult = result.ToString();

                Assert.That(parsedResult, Is.EqualTo(parsedExpected));
            }
        }

        [Test]
        public void ShowHelp_WritesHelpString()
        {
            const string expected = HelpString;
            var parsedExpected = expected + Environment.NewLine;

            using (var result = new StringWriter())
            {
                Console.SetOut(result);

                _consoleControl.ShowHelp();

                var parsedResult = result.ToString();

                Assert.That(parsedResult, Is.EqualTo(parsedExpected));
            }
        }

        [Test]
        public void ShowCommandPrompt_WritesCommandPromptString()
        {
            const string expected = CommandPromptString;
            var parsedExpected = expected + Environment.NewLine;

            using (var result = new StringWriter())
            {
                Console.SetOut(result);

                _consoleControl.ShowCommandPrompt();

                var parsedResult = result.ToString();

                Assert.That(parsedResult, Is.EqualTo(parsedExpected));
            }
        }

        [Test]
        public void ExecuteHelpCommand_ReturnsHelp()
        {
            var commandToExecute = "help";

            const string expected = HelpString;
            var parsedExpected = expected + Environment.NewLine;

            using (var result = new StringWriter())
            {
                Console.SetOut(result);

                _consoleControl.Execute(commandToExecute);

                var parsedResult = result.ToString();

                Assert.That(parsedResult, Is.EqualTo(parsedExpected));
            }
        }

        [Test]
        public void ExecuteWithInvalidOption_ReturnsUnrecognizedCommand(
            [Values("", "    ", "asdf", "adfsdf asdfas", "asdf asdf asdf")]
            string commandToExecute
            )
        {
            const string expected = UnrecognizedCommandString;
            var parsedExpected = expected + Environment.NewLine;

            using (var result = new StringWriter())
            {
                Console.SetOut(result);

                _consoleControl.Execute(commandToExecute);

                var parsedResult = result.ToString();

                Assert.That(parsedResult, Is.EqualTo(parsedExpected));
            }
        }

        [Test]
        public void ExecuteWithSingleInvalidOption_ReturnsUnrecognizedCommand()
        {
            var commandToExecute = "  ";

            const string expected = UnrecognizedCommandString;
            var parsedExpected = expected + Environment.NewLine;

            using (var result = new StringWriter())
            {
                Console.SetOut(result);

                _consoleControl.Execute(commandToExecute);

                var parsedResult = result.ToString();

                Assert.That(parsedResult, Is.EqualTo(parsedExpected));
            }
        }

        [Test]
        public void ExecuteWithValidPhotoAlbumOptionButInvalidArgument_ReturnsArgumentUnrecognized(
            [Values("photo-album -1", "photo-album somestring")]
            string commandToExecute
            )
        {
            const string expected = InvalidPhotoAlbumArgumentString;
            var parsedExpected = expected + Environment.NewLine;

            using (var result = new StringWriter())
            {
                Console.SetOut(result);

                _consoleControl.Execute(commandToExecute);

                var parsedResult = result.ToString();

                Assert.That(parsedResult, Is.EqualTo(parsedExpected));
            }
        }

        [Test]
        public void ExecuteWithValidPhotoOption_ReturnsAllPhotos(
            [Values("photo-album", "photo-album    ")]
            string commandToExecute
            )
        {
             List<Photo> listOfPhotos = new List<Photo>()
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

            _mockRepository.Setup(x => x.GetAllPhotos()).Returns(Task.FromResult(listOfPhotos));

            const string expected = RetrievingAllPhotosString;
            var parsedExpected = expected + Environment.NewLine +
                                 "[1] Title" + Environment.NewLine +
                                 "[2] Another Title" + Environment.NewLine +
                                 "Returned 2 results." + Environment.NewLine;

            using (var result = new StringWriter())
            {
                Console.SetOut(result);

                _consoleControl.Execute(commandToExecute);

                var parsedResult = result.ToString();

                Assert.That(parsedResult, Is.EqualTo(parsedExpected));
            }
        }

        [Test]
        public void ExecuteWithValidPhotoAlbumArgument_ReturnsAlbumOne()
        {
            var commandToExecute = "photo-album 1";

            List<Photo> listOfPhotos = new List<Photo>()
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

            _mockRepository.Setup(x => x.GetPhotosByAlbumId(1)).Returns(Task.FromResult(listOfPhotos));

            const string expected = "Retrieving photos for album # 1";
            var parsedExpected = expected + Environment.NewLine +
                                 "[1] Title" + Environment.NewLine +
                                 "[2] Another Title" + Environment.NewLine +
                                 "Returned 2 results." + Environment.NewLine;

            using (var result = new StringWriter())
            {
                Console.SetOut(result);

                _consoleControl.Execute(commandToExecute);

                var parsedResult = result.ToString();

                Assert.That(parsedResult, Is.EqualTo(parsedExpected));
            }
        }

        [Test]
        public void ExecuteAllPhotoWithNullRepoResponse_ReturnsErrorString()
        {
            var commandToExecute = "photo-album";

            List<Photo> photos = null;

            _mockRepository.Setup(x => x.GetAllPhotos()).Returns(Task.FromResult(photos));

            const string expected = RetrievingAllPhotosString;
            var parsedExpected = expected + Environment.NewLine +
                                 "Error retrieving results." + Environment.NewLine;

            using (var result = new StringWriter())
            {
                Console.SetOut(result);

                _consoleControl.Execute(commandToExecute);

                var parsedResult = result.ToString();

                Assert.That(parsedResult, Is.EqualTo(parsedExpected));
            }
        }

        [Test]
        public void ExecutPhotoAlbumWithNullResponse_ReturnsErrorString()
        {
            var commandToExecute = "photo-album 1";

            List<Photo> photos = null;

            _mockRepository.Setup(x => x.GetAllPhotos()).Returns(Task.FromResult(photos));

            const string expected = "Retrieving photos for album # 1";
            var parsedExpected = expected + Environment.NewLine +
                                ErrorRetrievingResultsString + Environment.NewLine;

            using (var result = new StringWriter())
            {
                Console.SetOut(result);

                _consoleControl.Execute(commandToExecute);

                var parsedResult = result.ToString();

                Assert.That(parsedResult, Is.EqualTo(parsedExpected));
            }
        }
    }
}