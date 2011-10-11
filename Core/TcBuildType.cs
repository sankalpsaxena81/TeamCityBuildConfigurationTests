using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Core
{
    public class TcBuildType
    {
        private TcBuildType()
        {
            _checkoutRules=new Lazy<string>(() => GetCheckoutRules());
            _vcsRoot= new Lazy<TcVCSRoot>(()=>GetVCSRoot());
        }

        public static List<TcBuildType> Parse(string response)
        {
            var rootElement = XElementParser.For(response);
            var rootBuild = rootElement.Element("buildTypes");
            var buildTypes = rootBuild.Elements("buildType");
            var buildTypesList = new List<TcBuildType>();
            if(buildTypes!=null)
            {
                buildTypes.ForEach(bt=>
                    {
                    buildTypesList.Add(new TcBuildType
                                            {
                                                Id=bt.AttributeValue("id"),
                                                Name=bt.AttributeValue("name"),
                                                DetailsLink=bt.AttributeValue("href"),
                                            });
                    }
                    );
                
            }
            return buildTypesList;
        }

        public string DetailsLink { get; set; }

        public string Name { get; set; }

        public string Id { get; set; }
        private Lazy<TcVCSRoot> _vcsRoot;
        public TcVCSRoot VcsRoot
        {
            get { return _vcsRoot.Value; }
        }

        private  string GetCheckoutRules()
        {
            var response = new Request().MakeGetRequest(DetailsLink);
            var rootElement = XElementParser.For(response);
            var vcsRoot = rootElement.Element("vcs-root");
            var vcsRootEntry = vcsRoot.Element("vcs-root-entry");
            var rule = vcsRootEntry.ElementValue("checkout-rules");
            return rule.Substring(rule.IndexOf("+:")+2, (rule.Length-7));
        }
        
        private Lazy<string> _checkoutRules;
        public string CheckoutRules
        {
            get { return _checkoutRules.Value; }
            
        }

        private TcVCSRoot GetVCSRoot()
        {
            var response = new Request().MakeGetRequest(DetailsLink);
            var rootElement = XElementParser.For(response);
            var vcsRoot = rootElement.Element("vcs-root");
            var vcsRootEntry = vcsRoot.Element("vcs-root-entry");
             var uri= vcsRootEntry.Element("vcs-root");
            return TcVCSRoot.Load(uri.AttributeValue("href"));
        }
    }

   

    public class TcVCSRoot
    {
        public static TcVCSRoot Load(string uri)
        {
            if (null == uri) return null;
            var response = new Request().MakeGetRequest(uri.Replace("id:",""));
            return Parse(response);
        }

        private static TcVCSRoot Parse(string response)
        {
            var rootElement = XElementParser.For(response);
            var properties = rootElement.Element("properties");
            var property = properties.Elements("property");
            foreach (var element in property)
            {
                if (element.AttributeValue("name").Equals("url"))
                {
                    return new TcVCSRoot {Location = element.AttributeValue("value")};
                }
            }
            return null;
        }

        public string Location { get; set; }
    }
}