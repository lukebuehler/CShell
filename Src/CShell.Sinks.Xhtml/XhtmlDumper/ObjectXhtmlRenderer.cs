using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Web.UI;

namespace CShell.Sinks.Xhtml.XhtmlDumper
{
    public class ObjectXhtmlRenderer : IXhtmlRenderer
    {
        public bool Render(object o, string description, int depth, XhtmlTextWriter writer)
        {
            //there are two principal ways of rendering an object,
            // if it's an enumerable we render each object as a row in a table
            // if it's jut an object we render each property of the object as a row in a table

            if(!String.IsNullOrWhiteSpace(description))
            {
                writer.RenderBeginTag(HtmlTextWriterTag.H3);
                writer.Write(description);
                writer.RenderEndTag();
                writer.WriteLine();
            }

            if (depth <= 0)
                return true;

            if (o is IEnumerable && !(o is String))
                return RenderEnumerable((IEnumerable)o, depth, writer);
            else
                return RenderObject(o, depth, writer);
        }


        public bool RenderObject(object o, int depth, XhtmlTextWriter writer)
        {
            var objectType = o.GetType();

            if(IsSimpleType(objectType))
            {
                writer.RenderBeginTag(HtmlTextWriterTag.P);
                writer.Write(o.ToString());
                writer.RenderEndTag();

                writer.WriteLine();
                return true;
            }

            //get the members (fields and properties)
            var members = GetMembers(objectType);

            //create a two column table containing member and then value
            writer.RenderBeginTag(HtmlTextWriterTag.Table);

            //write a header containing the class name
            writer.RenderBeginTag(HtmlTextWriterTag.Tr);
            writer.AddAttribute(HtmlTextWriterAttribute.Colspan, "2");
            writer.RenderBeginTag(HtmlTextWriterTag.Th);
            writer.Write(objectType.Name);
            writer.RenderEndTag(); //th
            writer.RenderEndTag(); //tr

            foreach (var member in members)
            {
                writer.RenderBeginTag(HtmlTextWriterTag.Tr);
                writer.AddAttribute(HtmlTextWriterAttribute.Class, "left");
                writer.RenderBeginTag(HtmlTextWriterTag.Th);
                writer.Write(member.Name);
                writer.RenderEndTag(); //th

                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                RenderMember(member, o, depth, writer);
                writer.RenderEndTag(); //td
                writer.RenderEndTag(); //tr
            }

            writer.RenderEndTag(); //table

            writer.WriteLine();
            return true;
        }

        public bool RenderEnumerable(IEnumerable enumerable, int depth, XhtmlTextWriter writer)
        {
            var elementType = GetElementTypeOfEnumerable(enumerable);
            if (elementType == null)
                return false;

            //get the members (fields and properties)
            var members = GetMembers(elementType);
            
            //write the headers and start the table
            writer.RenderBeginTag(HtmlTextWriterTag.Table);

            //if the element type is an element that needs to be rendered atomically we use a different method 
            if(IsSimpleType(elementType))
            {
                writer.RenderBeginTag(HtmlTextWriterTag.Tr);
                writer.RenderBeginTag(HtmlTextWriterTag.Th);
                writer.Write(elementType.Name);
                writer.RenderEndTag();
                writer.RenderEndTag();

                foreach (var element in enumerable)
                {
                    writer.RenderBeginTag(HtmlTextWriterTag.Tr);
                    writer.RenderBeginTag(HtmlTextWriterTag.Td);
                    writer.Write(element.ToString());
                    writer.RenderEndTag();
                    writer.RenderEndTag();
                }
            }
            else
            {
                writer.RenderBeginTag(HtmlTextWriterTag.Tr);
                foreach (var member in members)
                {
                    writer.RenderBeginTag(HtmlTextWriterTag.Th);
                    writer.Write(member.Name);
                    writer.RenderEndTag();
                }
                writer.RenderEndTag(); //tr
                writer.WriteLine();

                //write all the members
                foreach (var element in enumerable)
                {
                    writer.RenderBeginTag(HtmlTextWriterTag.Tr);
                    foreach (var member in members)
                    {
                        writer.RenderBeginTag(HtmlTextWriterTag.Td);
                        RenderMember(member, element, depth, writer);
                        writer.RenderEndTag(); //td
                    }
                    writer.RenderEndTag(); //tr
                    writer.WriteLine();
                }
            }

            //end the table
            writer.RenderEndTag();

            writer.WriteLine();
            return true;
        }

        private void RenderMember(MemberInfo memberInfo, object o, int depth, XhtmlTextWriter writer)
        {
            //try to get the value
            object value = null;
            try
            {
                value = GetValue(memberInfo, o);
            }
            catch (Exception ex)
            {
                value = "Error getting member value: " + ex.Message;
            }

            if(value == null)
            {
                writer.Write("null");
                return;
            }

            var valueType = GetValueType(memberInfo);
            if (valueType == null)
            {
                writer.Write("Error getting member type.");
            }
            else if (valueType == typeof(double) ||
                valueType == typeof(decimal) ||
                valueType == typeof(float))
            {
                //if the property seems to contain percent data we format it appropriately
                if (memberInfo.Name.ToLower().Contains("percent") || memberInfo.Name.ToLower().Contains("prc"))
                    writer.Write("{0:0.00%}", value);
                else
                    writer.Write("{0:0.00}", value);
            }
            else if (valueType == typeof(DateTime) ||
                valueType == typeof(DateTimeOffset))
            {
                //todo: optional date time formatting
                writer.Write(value);
            }
            else if (IsSimpleType(valueType))
            {
                writer.Write(value);
            }
            else
            {
                //recurively call the render method to descend one level in the object tree
                Render(value, null, depth - 1, writer);
            }
        }

        private Type GetElementTypeOfEnumerable(object o)
        {
            var enumerable = o as IEnumerable;
            if(enumerable == null)
                return null;

            Type elementType = null;
            Type[] interfaces = enumerable.GetType().GetInterfaces();
            foreach (Type i in interfaces)
            {
                if (i.IsGenericType && i.GetGenericTypeDefinition().Equals(typeof(IEnumerable<>)))
                {
                    elementType = i.GetGenericArguments()[0];
                    break;
                }
            }

            if(elementType == null)
            {
                var firstElement = enumerable.Cast<object>().FirstOrDefault();
                if (firstElement != null)
                    elementType = firstElement.GetType();
            }
            return elementType;
        }

        private static IList<MemberInfo> GetMembers(Type itemType)
        {
            var properties = itemType.GetProperties();
            //only get the properties that are browsable
            var filteredProperties = properties.Where(p =>
            {
                var attributes = Attribute.GetCustomAttributes(p, true);
                var browsable = attributes.OfType<BrowsableAttribute>();
                return !browsable.Any() || browsable.First().Browsable;
            }).ToList();

            var fields = itemType.GetFields();
            //only get the properties that are browsable
            var filteredFields = fields.Where(p =>
            {
                var attributes = Attribute.GetCustomAttributes(p, true);
                var browsable = attributes.OfType<BrowsableAttribute>();
                return !browsable.Any() || browsable.First().Browsable;
            }).ToList();

            return filteredProperties
                .Cast<MemberInfo>()
                .Concat(filteredFields.Cast<MemberInfo>())
                .ToList();
        }

        private static object GetValue(MemberInfo memberInfo, object o)
        {
            var pi = memberInfo as PropertyInfo;
            if (pi != null)
                return pi.GetValue(o, null);
            var fi = memberInfo as FieldInfo;
            if (fi != null)
                return fi.GetValue(o);
            return null;
        }

        private static Type GetValueType(MemberInfo memberInfo)
        {
            var pi = memberInfo as PropertyInfo;
            if (pi != null)
                return pi.PropertyType;
            var fi = memberInfo as FieldInfo;
            if (fi != null)
                return fi.FieldType;
            return null;
        }

        private static bool IsSimpleType(Type type)
        {
            return type.IsPrimitive ||
                   type.IsValueType ||
                   type == typeof(DateTime) ||
                   type == typeof(DateTimeOffset) ||
                   type == typeof(TimeSpan) ||
                   type == typeof(string);
        }
    }
}
