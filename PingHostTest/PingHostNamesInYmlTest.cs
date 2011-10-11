using System.IO;
using NUnit.Framework;

namespace PingHostTest
{
    [TestFixture]
    public class PingHostNamesInYmlTest
    {

        [Test]
        public void ShouldSuccessfulyPingAllHostNamesInYml()
        {
            string path = "C:\\Projects\\DeployScripts\\Config";
            var fileNames = Directory.GetFiles(path,"*.yml");
            foreach (var fileName in fileNames)
            {
                
            }


        }
    }
}