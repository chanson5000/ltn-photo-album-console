using System;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;

namespace LtnPhotoAlbum.UnitTests
{
    [TestFixture]
    public class BootstrapTests
    {
        [Test]
        public void OnStartWithNoArgs_ShowGreetingAndHelp()
        {
            var expected = "Welcome to photo album." + Environment.NewLine +
                           "Commands:\n" +
                           "'photo-album' for an id and title list of all photos in photo album.\n" +
                           "'photo-album <#>' for an id and title list of all photos from album #.\n" +
                           "'help' brings you to this screen.\n" +
                           "'exit' to exit." + Environment.NewLine +
                           "Please enter a command: " + Environment.NewLine;

            using (var result = new StringWriter())
            {
                Console.SetOut(result);

                Bootstrap bootstrap = new Bootstrap();

                var task = new Task(() => bootstrap.Start());
                task.Start();
                task.Wait(5000);
                var parsedResult = result.ToString();
                Assert.That(parsedResult, Is.EqualTo(expected));
            }
        }
    }
}