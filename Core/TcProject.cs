using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Core
{
    public class TcProject
    {
        private static string projectsUri = "https://teamcity.oss.prd/httpAuth/app/rest/projects";
        private TcProject()
        {
            _buildType= new Lazy<List<TcBuildType>>(()=>LoadBuildTypes());
        }

        public static List<TcProject> LoadAllProjects()
        {
            var response= new Request().MakeGetRequest(projectsUri);
            return Parse(response);
        }

        private static List<TcProject> Parse(string response)
        {
            var rootElement = XElementParser.For(response);
            var projects = rootElement.Elements("project");
            var tcProjects = new List<TcProject>();
            if(projects!=null)
            {
                projects.ForEach(p =>
                    {
                        tcProjects.Add(new TcProject
                                                    {
                                                        Name = p.AttributeValue("name"),
                                                        Id = p.AttributeValue("id"),
                                                        DetailsLink = p.AttributeValue("href"),
                                                    });
                    });
            }
            return tcProjects;
        }


        private List<TcBuildType> LoadBuildTypes()
        {
            var response = new Request().MakeGetRequest(DetailsLink);
             return TcBuildType.Parse(response);
        }

        public string Id { get; set; }

        public string DetailsLink { get; set; }

        public string Name { get; set; }


        private Lazy<List<TcBuildType>> _buildType;
        public List<TcBuildType> BuildTypes
        {
            get { return _buildType.Value; }
        }
    }
}