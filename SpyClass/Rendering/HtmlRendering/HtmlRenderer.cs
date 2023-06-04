using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HtmlAgilityPack;
using SpyClass.Analysis.DataModel.Documentation;
using SpyClass.Hierarchization;
using SpyClass.Rendering.HtmlRendering.Utils;

namespace SpyClass.Rendering.HtmlRendering
{
    public class HtmlRenderer : DocTreeRenderer
    {
        private string _contentStyleString;
        
        private HtmlDocument _indexDocument;
        private Stack<DocFile> _contentStack = new();

        private HtmlNode _navtreeRootNode;
        private Stack<HtmlNode> _navtreeStack = new();

        private string _outDirectory;

        public HtmlRenderer(string outDirectory)
        {
            _outDirectory = outDirectory;

            CreateOutDirectory();
            LoadTemplate();
        }

        protected override void OnRender(RootNode root)
        {
            base.OnRender(root);

            _indexDocument.Save(Path.Combine(_outDirectory, "index.html"));
        }

        private void CreateOutDirectory()
        {
            if (Directory.Exists(_outDirectory))
                Directory.Delete(_outDirectory, true);

            Directory.CreateDirectory(_outDirectory);
            Directory.CreateDirectory(Path.Combine(_outDirectory, "types"));

            FileSystem.CopyDirectory(
                Path.Combine(
                    AppContext.BaseDirectory,
                    "SpyClass.WebTemplate"
                ),
                _outDirectory
            );
        }

        private void LoadTemplate()
        {
            _indexDocument = new HtmlDocument();
            _indexDocument.Load(Path.Combine(_outDirectory, "index.html"));

            _navtreeRootNode = _indexDocument.DocumentNode.SelectSingleNode("//ul[@role='tree']");

            _contentStyleString = File.ReadAllText(Path.Combine(_outDirectory, "css", "style-content.css"));
        }

        protected override void Visit(NamespaceNode node)
        {
            var ulNode = CreateGroupTreeItemNode(node.Name);
            _navtreeStack.Push(ulNode);
            base.Visit(node);
            _navtreeStack.Pop();
        }

        protected override void Visit(TypeDoc typeDoc)
        {
            var name = StringTools.FlattenType(typeDoc);
            var fnvName = StringTools.FNV1A64(typeDoc.FullName);
            
            var docFile = CreateTypeDocument(fnvName);
            _contentStack.Push(docFile);

            if (typeDoc.NestedTypes.Any())
            {
                var ulNode = CreateGroupTypeTreeItemNode(name, fnvName);
                _navtreeStack.Push(ulNode);

                foreach (var nestedType in typeDoc.NestedTypes)
                {
                    Visit(nestedType);
                }

                _navtreeStack.Pop();
            }
            else
            {
                CreateFlatTypeTreeItemNode(name, fnvName);
            }

            BuildTypeDocFile(docFile, typeDoc);

            _contentStack.Pop();
            docFile.WriteToDisk(_outDirectory);
        }

        protected override void Visit(FieldDoc fieldDoc)
        {
        }

        protected override void Visit(EventDoc eventDoc)
        {
        }

        protected override void Visit(IndexerDoc indexerDoc)
        {
        }

        protected override void Visit(PropertyDoc propertyDoc)
        {
        }

        protected override void Visit(ConstructorDoc constructorDoc)
        {
        }

        protected override void Visit(MethodDoc methodDoc)
        {
        }

        private HtmlNode CreateGroupTreeItemNode(string name)
        {
            var liNode = HtmlNode.CreateNode("<li></li>");
            liNode.Attributes.Add("role", "treeitem");
            liNode.Attributes.Add("aria-level", $"{_navtreeStack.Count + 1}");
            liNode.Attributes.Add("aria-expanded", "false");
            liNode.Attributes.Add("tabindex", $"{(_navtreeStack.Count == 0 ? "0" : "-1")}");

            var spanNode = HtmlNode.CreateNode($"<span>{name}</span>");
            liNode.AppendChild(spanNode);

            var ulNode = HtmlNode.CreateNode("<ul></ul>");
            ulNode.Attributes.Add("role", "group");
            liNode.AppendChild(ulNode);

            if (_navtreeStack.Count == 0)
            {
                _navtreeRootNode.AppendChild(liNode);
            }
            else
            {
                _navtreeStack.Peek().AppendChild(liNode);
            }

            return ulNode;
        }

        private HtmlNode CreateFlatTypeTreeItemNode(string name, string fnvName)
        {
            var liNode = HtmlNode.CreateNode("<li></li>");
            liNode.Attributes.Add("role", "treeitem");
            liNode.Attributes.Add("id", fnvName);
            liNode.Attributes.Add("doc-file", "types/" + fnvName + ".html");

            var span = HtmlNode.CreateNode($"<span>{name}.cs</span>");

            liNode.AppendChild(span);

            _navtreeStack.Peek().AppendChild(liNode);
            return liNode;
        }

        private HtmlNode CreateGroupTypeTreeItemNode(string name, string fnvName)
        {
            var liNode = HtmlNode.CreateNode("<li></li>");
            liNode.Attributes.Add("role", "treeitem");
            liNode.Attributes.Add("aria-level", $"{_navtreeStack.Count + 1}");
            liNode.Attributes.Add("aria-expanded", "false");
            liNode.Attributes.Add("tabindex", $"{(_navtreeStack.Count == 0 ? "0" : "-1")}");
            liNode.Attributes.Add("id", fnvName);
            liNode.Attributes.Add("doc-file", "types/" + fnvName + ".html");

            var spanNode = HtmlNode.CreateNode($"<span>{name}.cs</span>");

            liNode.AppendChild(spanNode);

            var ulNode = HtmlNode.CreateNode("<ul></ul>");
            ulNode.Attributes.Add("role", "group");
            liNode.AppendChild(ulNode);

            _navtreeStack.Peek().AppendChild(liNode);

            return ulNode;
        }

        private DocFile CreateTypeDocument(string name)
        {
            return new DocFile(name + ".html");
        }

        private void BuildTypeDocFile(DocFile docFile, TypeDoc typeDoc)
        {
            var styleChild = HtmlNode.CreateNode($"<style type='text/css'>{_contentStyleString}</style>");
            docFile.Document.DocumentNode.AppendChild(styleChild);

            var h1Element = HtmlNode.CreateNode("<h1></h1>");
            var spanElement = HtmlNode.CreateNode($"<span class='keyword'>namespace</span>");
            var span2Element = HtmlNode.CreateNode($"<span> {typeDoc.Namespace}</span>");

            h1Element.AppendChild(spanElement);
            h1Element.AppendChild(span2Element);
            docFile.Document.DocumentNode.AppendChild(h1Element);
            docFile.Document.DocumentNode.AppendChild(HtmlTools.CreateTypeSynopsis(typeDoc));
        }


    }
}