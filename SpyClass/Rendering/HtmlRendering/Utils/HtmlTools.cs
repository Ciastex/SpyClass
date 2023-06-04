using System;
using System.Collections.Generic;
using System.IO;
using HtmlAgilityPack;
using SpyClass.Analysis.DataModel.Documentation;

namespace SpyClass.Rendering.HtmlRendering.Utils
{
    internal static class HtmlTools
    {
        internal static HtmlNode NodeTree(string tagName, IEnumerable<string> classes, string id, string content,
            Func<HtmlNode, HtmlNodeCollection> generator)
        {
            var classAttr = string.Empty;
            var idAttr = string.Empty;
            content ??= string.Empty;

            if (classes != null)
            {
                classAttr = $" class='{string.Join(' ', classes)}'";
            }

            if (!string.IsNullOrEmpty(id))
            {
                idAttr = $" id='{id}'";
            }

            var ret = HtmlNode.CreateNode($"<{tagName}{classAttr}{idAttr}>{content}</{tagName}>");

            ret.AppendChildren(generator(ret));

            return ret;
        }

        internal static HtmlNode Div(string content, string id = null)
        {
            if (!string.IsNullOrEmpty(id))
            {
                return HtmlNode.CreateNode($"<div id='{id}'>{content}</div>");
            }

            return HtmlNode.CreateNode($"<div>{content}</div>");
        }

        internal static HtmlNode Span(string content, params string[] classes)
        {
            if (classes.Length > 0)
            {
                return HtmlNode.CreateNode($"<span class='{string.Join(' ', classes)}'>{content}</span>");
            }

            return HtmlNode.CreateNode($"<span>{content}</span>");
        }

        internal static HtmlNode Header(string content, int level)
        {
            return HtmlNode.CreateNode($"<h{level}>{content}</h{level}>");
        }

        internal static HtmlNode Hyperlink(string url, string content, params object[] classes)
        {
            if (classes.Length != 0)
            {
                return HtmlNode.CreateNode($"<a href='{url}' class='{string.Join(' ', classes)}'>{content}</a>");
            }

            return HtmlNode.CreateNode($"<a href='{url}'>{content}</a>");
        }

        internal static HtmlNode CreateTypeSynopsis(TypeDoc typeDoc)
        {
            return NodeTree("div", new[] { "class-info" }, null, null, (n) =>
            {
                return new(n)
                {
                    Header("Synopsis", 3),
                    NodeTree("span", new[] { "doc-entry" }, null, null, (n) =>
                    {
                        return new(n)
                        {
                            Span(TypeDoc.BuildBasicIdentityString(typeDoc) + " ", "keyword"),
                            Span(StringTools.FlattenTypeName(typeDoc.DisplayName), "type"),
                            ((Func<HtmlNode>)(() =>
                            {
                                if (typeDoc.GenericParameters.Any)
                                {
                                    return Span(
                                        HtmlEntity.Entitize(
                                            typeDoc.GenericParameters.BuildGenericParameterListString()
                                        )
                                    );
                                }

                                return Span(string.Empty);
                            }))(),
                            ((Func<HtmlNode>)(() =>
                            {
                                if (typeDoc.BaseTypeInfo != null)
                                {
                                    var fnvFullName = StringTools.FNV1A64(typeDoc.BaseTypeInfo.FullName);

                                    var spanContent = HtmlEntity.Entitize(
                                        StringTools.FlattenTypeName(typeDoc.BaseTypeInfo.DisplayName)
                                    );

                                    return NodeTree("span", new[] { "type" }, null, " : ", (n) =>
                                    {
                                        return new(n)
                                        {
                                            Hyperlink("#" + fnvFullName, spanContent)
                                        };
                                    });
                                }

                                return Span(string.Empty);
                            }))()
                        };
                    })
                };
            });
        }
    }
}