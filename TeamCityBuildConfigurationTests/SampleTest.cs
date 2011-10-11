using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Core
{
    public class SampleTest
    {
        [Test]

        // Except: Artifacts,BRM,FFM 
        public void AllBranchBuildConfigurationShouldPointToReleaseBranch()
        {
            var svnBase = "https://svn.oss.prd/repos/SHAW";
            var vcsDict = new Dictionary<string, string>();
            // method under test
            var projectsNotPointingToBranch = new List<string>
                                                  {
                                                      "Artifacts-Release1.0",
                                                      
                                                  };
            var allProjects = TcProject.LoadAllProjects();
            allProjects.FindAll(p => p.Name.EndsWith("-Release1.0"))
                .ForEach(bp =>{
                            vcsDict[bp.Name] = "";
                            bp.BuildTypes.FindAll(b => b.Name.Contains("build"))
                            .ForEach(b =>{
                                        if (b.VcsRoot != null)
                                        {
                                            vcsDict[bp.Name] = b.VcsRoot.Location == svnBase
                                                                ? b.CheckoutRules
                                                                : b.VcsRoot.Location;
                                        }
                                        });
                        });


            string failedString=string.Empty;
            vcsDict.Keys.ForEach(k =>
                    {
                        if (!projectsNotPointingToBranch.Contains(k))
                        {
                            if (!vcsDict[k].Contains("/branches/release1.0"))
                            {
                                failedString += k + " does not point to branch. It points to " +
                                                vcsDict[k] + " /n";
                            }
                        }
                    });

            Assert.AreEqual(string.Empty,failedString,failedString);

        }




        
    }
}



