using System;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;

namespace LtnPhotoAlbum.UnitTests
{
    [TestFixture]
    public class BootstrapTests
    {
        private const string UnrecognizedCommandString = "Un-recognized command. Try 'help'";
        private const int WaitTime = 3000;

        [Test]
        public void OnStartWithNoArgs_ShowGreetingAndHelp()
        {
            var expected = "Welcome to photo album." + Environment.NewLine +
                           "Commands:\n" +
                           "'photo-album' for an id and title list of all photos in photo album.\n" +
                           "'photo-album <#>' for an id and title list of all photos from album #.\n" +
                           "'help' brings you to this screen.\n" +
                           "'exit' to exit." + Environment.NewLine +
                           ">";

            using (var result = new StringWriter())
            {
                Console.SetOut(result);

                Bootstrap bootstrap = new Bootstrap();

                var task = new Task(() => bootstrap.Start());
                task.Start();
                task.Wait(WaitTime);
                var parsedResult = result.ToString();
                Assert.That(parsedResult, Is.EqualTo(expected));
            }
        }

        [Test]
        public void OnStartWithInvalidArgs_ShowError()
        {
            var commandToExecute = new[] {"invalid"};

            const string expected = UnrecognizedCommandString;
            var parsedExpected = expected + Environment.NewLine;

            using (var result = new StringWriter())
            {
                Console.SetOut(result);

                Bootstrap bootstrap = new Bootstrap();
                bootstrap.Start(commandToExecute);

                var parsedResult = result.ToString();
                Assert.That(parsedResult, Is.EqualTo(parsedExpected));
            }
        }

        [Test]
        public void OnStartWithValidArgs_DoesNotReturnError(
            [Values("help", "photo-album", "photo-album 1", "exit")]
            string commandToParse
            )
        {
            var commandToExecute = commandToParse.Split();

            const string expected = UnrecognizedCommandString;
            var parsedExpected = expected + Environment.NewLine + ">";

            using (var result = new StringWriter())
            {
                Console.SetOut(result);

                Bootstrap bootstrap = new Bootstrap();
                bootstrap.Start(commandToExecute);

                var parsedResult = result.ToString();
                Assert.That(parsedResult, Is.Not.EqualTo(parsedExpected));
            }
        }
    }
}