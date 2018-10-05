using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
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

        private readonly List<Photo> _listOfPhotos = new List<Photo>
        {
            new Photo
            {
                Id = 1,
                AlbumId = 1,
                Title = "Title",
                Url = "http://www.url.com",
                ThumbnailUrl = "http://www.thumbnailurl.com"
            },
            new Photo
            {
                Id = 2,
                AlbumId = 2,
                Title = "Another Title",
                Url = "http://www.url2.com",
                ThumbnailUrl = "http://www.thumbnailurl2.com"
            }
        };

        private readonly List<Photo> _listOfPhotosByAlbum = new List<Photo>
        {
            new Photo
            {
                Id = 1,
                AlbumId = 1,
                Title = "Title",
                Url = "http://www.url.com",
                ThumbnailUrl = "http://www.thumbnailurl.com"
            },
            new Photo
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

        public void SetRepoAllPhotosResponse(List<Photo> mockResponse)
        {
            _mockRepository.Setup(x => x.GetAllPhotos()).Returns(Task.FromResult(mockResponse));
        }

        public void SetRepoPhotosByAlbumResponse(List<Photo> mockResonse)
        {
            _mockRepository.Setup(x => x.GetPhotosByAlbumId(1)).Returns(Task.FromResult(mockResonse));
        }

        public void SetRepoAllPhotos_ThrowsException(Exception exception)
        {
            _mockRepository.Setup(x => x.GetAllPhotos()).Throws(exception);
        }

        public void SetRepoPhotosByAlbumId_ThrowsException(Exception exception)
        {
            _mockRepository.Setup(x => x.GetPhotosByAlbumId(1)).Throws(exception);
        }

        [Test]
        public void ShowGreeting_WritesGreetingString()
        {
            const string expected = ConsoleControl.Greeting + "\r\n";

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
            const string expected = ConsoleControl.Help + "\r\n";

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
        public void InputHelpCommand_ReturnsHelp()
        {
            const string commandToExecute = "help";

            const string expected = ConsoleControl.Help + "\r\n";

            using (_result)
            {
                Console.SetOut(_result);

                _consoleControl.ParseInput(commandToExecute);

                Assert.That(_result.ToString(), Is.EqualTo(expected));
            }
        }

        [Test]
        public void InputInvalidArg_ReturnsUnrecognizedCommand(
            [Values("", "    ", "invalidArg", "invalidArg invalidParam", "invalidArg invalidParam extraParam")]
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
        public void InputValidPhotoAlbumArgWithInvalidParam_ReturnsInvalidPhotoCommand(
            [Values("photo-album -1", "photo-album invalidParam", "photo-album invalidParam extraParam")]
            string commandToExecute
            )
        {
            const string expected = ConsoleControl.InvalidPhotoCommand + "\r\n";

            using (_result)
            {
                Console.SetOut(_result);

                _consoleControl.ParseInput(commandToExecute);

                Assert.That(_result.ToString(), Is.EqualTo(expected));
            }
        }

        [Test]
        public void InputValidPhotoAlbumArg_ReturnsStatusAndAllPhotos(
            [Values("photo-album", "photo-album    ")]
            string commandToExecute
            )
        {
            SetRepoAllPhotosResponse(_listOfPhotos);

            const string expected = ConsoleControl.RetrievingAllPhotos + "\r\n" +
                                 "[1] Title" + "\r\n" +
                                 "[2] Another Title" + "\r\n" +
                                 "Returned 2 results." + "\r\n";

            using (_result)
            {
                Console.SetOut(_result);

                _consoleControl.ParseInput(commandToExecute);

                Assert.That(_result.ToString(), Is.EqualTo(expected));
            }
        }

        [Test]
        public void InputValidPhotoAlbumArgAndParam_ReturnsStatusAndPhotoAlbumResult()
        {
            SetRepoPhotosByAlbumResponse(_listOfPhotosByAlbum);

            const string commandToExecute = "photo-album 1";

            const string expected = ConsoleControl.RetrievingPhotosFromAlbum + "\r\n" +
                                 "[1] Title" + "\r\n" +
                                 "[2] Another Title" + "\r\n" +
                                 "Returned 2 results." + "\r\n";

            using (_result)
            {
                Console.SetOut(_result);

                _consoleControl.ParseInput(commandToExecute);

                Assert.That(_result.ToString(), Is.EqualTo(expected));
            }
        }

        [Test]
        public void InputAllAlbumWithRepoNullResponse_ReturnsStatusAndGenericError()
        {
            SetRepoAllPhotosResponse(_nullListOfPhotos);

            const string commandToExecute = "photo-album";

            const string expected = ConsoleControl.RetrievingAllPhotos + "\r\n" +
                                    ConsoleControl.ProblemResults + "\r\n";

            using (_result)
            {
                Console.SetOut(_result);

                _consoleControl.ParseInput(commandToExecute);

                Assert.That(_result.ToString(), Is.EqualTo(expected));
            }
        }

        [Test]
        public void InputOneAlbumWithRepoNullResponse_ReturnsStatusAndGenericError()
        {
            SetRepoPhotosByAlbumResponse(_nullListOfPhotos);

            const string commandToExecute = "photo-album 1";

            const string expected = ConsoleControl.RetrievingPhotosFromAlbum + "\r\n" +
                                    ConsoleControl.ProblemResults + "\r\n";

            using (_result)
            {
                Console.SetOut(_result);

                _consoleControl.ParseInput(commandToExecute);

                Assert.That(_result.ToString(), Is.EqualTo(expected));
            }
        }

        [Test]
        public void InputAllAlbumsWithRepoThrowsHttpRequestException_ReturnsMatchingExceptionError()
        {
            SetRepoAllPhotos_ThrowsException(new HttpRequestException());

            const string commandToExecute = "photo-album";

            const string expected = ConsoleControl.RetrievingAllPhotos + "\r\n" +
                                    ConsoleControl.ProblemNetworkConnectivity + "\r\n" +
                                    ConsoleControl.ProblemResults + "\r\n";

            using (_result)
            {
                Console.SetOut(_result);

                _consoleControl.ParseInput(commandToExecute);

                Assert.That(_result.ToString(), Is.EqualTo(expected));
            }
        }

        [Test]
        public void InputAllAlbumsWithRepoThrowsTaskCancelledException_ReturnsMatchingExceptionError()
        {
            SetRepoAllPhotos_ThrowsException(new TaskCanceledException());

            const string commandToExecute = "photo-album";

            const string expected = ConsoleControl.RetrievingAllPhotos + "\r\n" +
                                    ConsoleControl.ProblemRequestTimeout + "\r\n" +
                                    ConsoleControl.ProblemResults + "\r\n";

            using (_result)
            {
                Console.SetOut(_result);

                _consoleControl.ParseInput(commandToExecute);

                Assert.That(_result.ToString(), Is.EqualTo(expected));
            }
        }

        [Test]
        public void InputAllAlbumsWithRepoThrowsGenericException_ReturnsGenericError()
        {
            SetRepoAllPhotos_ThrowsException(new Exception());

            const string commandToExecute = "photo-album";

            const string expected = ConsoleControl.RetrievingAllPhotos + "\r\n" +
                                    ConsoleControl.ProblemResults + "\r\n";

            using (_result)
            {
                Console.SetOut(_result);

                _consoleControl.ParseInput(commandToExecute);

                Assert.That(_result.ToString(), Is.EqualTo(expected));
            }
        }

        [Test]
        public void InputByAlbumWithRepoThrowsHttpRequestException_ReturnsMatchingExceptionError()
        {
            SetRepoPhotosByAlbumId_ThrowsException(new HttpRequestException());

            const string commandToExecute = "photo-album 1";

            const string expected = ConsoleControl.RetrievingPhotosFromAlbum + "\r\n" +
                                    ConsoleControl.ProblemNetworkConnectivity + "\r\n" +
                                    ConsoleControl.ProblemResults + "\r\n";

            using (_result)
            {
                Console.SetOut(_result);

                _consoleControl.ParseInput(commandToExecute);

                Assert.That(_result.ToString(), Is.EqualTo(expected));
            }
        }

        [Test]
        public void InputByAlbumWithRepoThrowsTaskCancelledException_ReturnsMatchingExceptionError()
        {
            SetRepoPhotosByAlbumId_ThrowsException(new TaskCanceledException());

            const string commandToExecute = "photo-album 1";

            const string expected = ConsoleControl.RetrievingPhotosFromAlbum + "\r\n" +
                                    ConsoleControl.ProblemRequestTimeout + "\r\n" +
                                    ConsoleControl.ProblemResults + "\r\n";

            using (_result)
            {
                Console.SetOut(_result);

                _consoleControl.ParseInput(commandToExecute);

                Assert.That(_result.ToString(), Is.EqualTo(expected));
            }
        }

        [Test]
        public void InputByAlbumWithRepoThrowsGenericException_ReturnsGenericError()
        {
            SetRepoPhotosByAlbumId_ThrowsException(new Exception());

            const string commandToExecute = "photo-album 1";

            const string expected = ConsoleControl.RetrievingPhotosFromAlbum + "\r\n" +
                                    ConsoleControl.ProblemResults + "\r\n";

            using (_result)
            {
                Console.SetOut(_result);

                _consoleControl.ParseInput(commandToExecute);

                Assert.That(_result.ToString(), Is.EqualTo(expected));
            }
        }
    }
}