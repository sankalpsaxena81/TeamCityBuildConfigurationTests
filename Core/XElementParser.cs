using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Core
{
    public class XElementParser
    {
        private const string namespacePrefix = "ns";
        public static XElementParser For(
            string xml,
            string @namespace)
        {
            return For(XElement.Parse(xml), @namespace);
        }
        public static XElementParser For(
            XElement rootElement,
            string @namespace)
        {
            var namespaceManager = new XmlNamespaceManager(new NameTable());
            namespaceManager.AddNamespace(namespacePrefix, @namespace);
            return new XElementParser(
                rootElement,
                @namespace,
                namespaceManager);
        }

        private readonly IXmlNamespaceResolver namespaceResolver;

        private XElementParser(
            XElement element,
            string @namespace,
            IXmlNamespaceResolver namespaceResolver)
        {
            XElement = element;
            Namespace = @namespace;
            this.namespaceResolver = namespaceResolver;
        }

        public string Namespace { get; private set; }
        public XElement XElement { get; private set; }

        private string NamespaceQualifiedXPath(string xpath)
        {
            if (string.IsNullOrEmpty(Namespace)) return xpath;
            var namespaceQualifiedXPath = Regex.Replace(xpath, @"([/])(?'Expression'[A-Za-z0-9\-\[\]]+)", x =>
            {
                var expressionIndex1 = x.Groups["Expression"].Index - x.Index;
                var before1 = x.Value.Substring(0, expressionIndex1);
                var after1 = x.Value.Substring(expressionIndex1, x.Value.Length - expressionIndex1);
                return string.Format("{0}{1}:{2}", before1, namespacePrefix, after1);
            });
            return namespaceQualifiedXPath;
        }

        public string Value
        {
            get { return XElement.Value; }
        }

        public XName Name
        {
            get { return XElement.Name; }
        }

        public string AttributeValue(string attributeName)
        {
            return AttributeValue(attributeName, "");
        }

        public TValue AttributeValue<TValue>(string attributeName)
        {
            return AttributeValue(attributeName, default(TValue));
        }

        public TValue AttributeValue<TValue>(string attributeName, TValue defaultValue)
        {
            var attribute = XElement.Attribute(attributeName);
            return null == attribute ? defaultValue : ConvertValue<TValue>(attribute.Value);
        }

        public string LinkUri(string linkName)
        {
            var linkElement = XPathSelectElement(string.Format("./link[@rel='{0}']", linkName));
            return null == linkElement ? null : linkElement.AttributeValue("href", null as string);
        }

        public string ElementValue(string elementName)
        {
            return ElementValue(elementName, "");
        }

        public TValue ElementValue<TValue>(string elementName)
        {
            return ElementValue(elementName, default(TValue));
        }

        public TValue ElementValue<TValue>(string elementName, TValue defaultValue)
        {
            var element = XElement.Element(XName.Get(elementName, Namespace));
            return null == element ? defaultValue : ConvertValue<TValue>(element.Value);
        }

        public XElementParser Element(string elementName)
        {
            var element = XElement.Element(XName.Get(elementName, Namespace));
            return null == element ? null : new XElementParser(element, Namespace, namespaceResolver);
        }

        public IEnumerable<XElementParser> Elements()
        {
            return XElement
                .Elements()
                .Select(x => new XElementParser(x, Namespace, namespaceResolver));
        }

        public IEnumerable<XElementParser> Elements(string elementName)
        {
            return XElement
                .Elements(XName.Get(elementName, Namespace))
                .Select(x => new XElementParser(x, Namespace, namespaceResolver));
        }

        public IEnumerable<XElementParser> XPathSelectElements(string xpath)
        {
            return XElement
                .XPathSelectElements(NamespaceQualifiedXPath(xpath), namespaceResolver)
                .Select(x => new XElementParser(x, Namespace, namespaceResolver));
        }

        public XElementParser XPathSelectElement(string xpath)
        {
            return XPathSelectElements(xpath).FirstOrDefault();
        }

        public string XPathSelectElementValue(string xpath)
        {
            return XPathSelectElementValue(xpath, null as string);
        }

        public TValue XPathSelectElementValue<TValue>(string xpath, TValue defaultValue)
        {
            var elementParser = XPathSelectElement(xpath);
            return null == elementParser ? defaultValue : ConvertValue<TValue>(elementParser.XElement.Value);
        }

        public string XPathSelectAttributeValue(string xpath, string attributeName)
        {
            return XPathSelectAttributeValue(xpath, attributeName, null as string);
        }

        public TValue XPathSelectAttributeValue<TValue>(string xpath, string attributeName, TValue defaultValue)
        {
            var elementParser = XPathSelectElement(xpath);
            return null == elementParser ? defaultValue : elementParser.AttributeValue(attributeName, defaultValue);
        }

        private static TValue ConvertValue<TValue>(string valueString)
        {
            var type = typeof(TValue);
            if (type == typeof(string))
                return (TValue)(object)valueString;

            var typeArgumentIsNullable = type.IsGenericType && typeof(Nullable<>).Equals(type.GetGenericTypeDefinition());
            var valueType = typeArgumentIsNullable
                                ? type.GetGenericArguments()[0]
                                : type;

            if ((valueType == typeof(bool)) ||
                (valueType == typeof(int)) ||
                (valueType == typeof(long)) ||
                (valueType == typeof(decimal)) ||
                (valueType == typeof(double)) ||
                (valueType == typeof(float)))
            {
                if (string.IsNullOrEmpty(valueString))
                    return default(TValue);
                return (TValue)Convert.ChangeType(valueString, typeof(TValue));
            }
            

            if (valueType == typeof(DateTime))
            {
                if (string.IsNullOrEmpty(valueString))
                    return default(TValue);
                //var convertedDate = DateTime.SpecifyKind(
                //    DateTime.Parse(valueString),
                //    DateTimeKind.Utc);
                //return (TValue) (object) convertedDate.ToLocalTime();
                return (TValue)(object)DateTime.Parse(valueString);
            }

            if (valueType == typeof(DateTimeOffset))
            {
                if (string.IsNullOrEmpty(valueString))
                    return default(TValue);
                return (TValue)(object)DateTimeOffset.Parse(valueString);
            }

            throw new NotImplementedException(string.Format("There isn't a conversion available for value type of {0} for {1}", typeof(TValue).FullName, valueString));
        }

        public XElementParser UsingNamespace(string @namespace)
        {
            return For(XElement, @namespace);
        }

        public static XElementParser For(string xml)
        {
            return For(xml, string.Empty);
        }
        public static XElementParser For(XElement rootElement)
        {
            return For(rootElement, string.Empty);
        }
    }
}