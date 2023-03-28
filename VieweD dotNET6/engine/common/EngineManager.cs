using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using VieweD.Forms;
using VieweD.Properties;

namespace VieweD.engine.common
{
    public class EngineManager
    {
        private static EngineManager? InstanceObject { get; set; }
        public static EngineManager Instance => InstanceObject ?? new EngineManager();

        public static readonly List<BaseInputReader> AllInputReaders = new();
        public static readonly List<BaseParser> AllParsers = new();
        public static List<string> PluginFiles = new();
        public static List<string> PluginReferences = new();
        public static List<string> PluginErrors = new();
        public static TimeSpan CompileTime { get; set; } = TimeSpan.FromSeconds(0);
        private static Assembly? _pluginsAssembly;
        public static Assembly? PluginAssembly => _pluginsAssembly;

        private EngineManager()
        {
            InstanceObject = this;
            CompilePlugins();
            AllInputReaders.Clear();
            AllParsers.Clear();

            // Load local in-app engines
            var allClasses = Assembly.GetExecutingAssembly().GetTypes();
            foreach (var aClass in allClasses)
            {
                if (aClass.BaseType == typeof(BaseInputReader))
                {
                    // Add input reader
                    if (Activator.CreateInstance(aClass) is BaseInputReader reader)
                        AllInputReaders.Add(reader);
                }

                if (aClass.BaseType == typeof(BaseParser))
                {
                    // Add parser
                    if (Activator.CreateInstance(aClass) is BaseParser parser)
                        AllParsers.Add(parser);
                }

            }

            // Load plugin engines
            var pluginClasses = _pluginsAssembly?.GetTypes();
            if (pluginClasses != null)
            {
                foreach (var aClass in pluginClasses)
                {
                    if (aClass.BaseType == typeof(BaseInputReader))
                    {
                        // Add input reader
                        if (Activator.CreateInstance(aClass) is BaseInputReader reader)
                            AllInputReaders.Add(reader);
                    }

                    if (aClass.BaseType == typeof(BaseParser))
                    {
                        // Add parser
                        if (Activator.CreateInstance(aClass) is BaseParser parser)
                            AllParsers.Add(parser);
                    }
                }
            }

            AllInputReaders.Sort();
            AllParsers.Sort();
        }

        private static IEnumerable<SyntaxTree> ParseSourceIntoSyntaxTrees(IEnumerable<string> fileList)
        {
            var syntaxTrees = new List<SyntaxTree>();
            foreach (var pluginFile in fileList)
            {
                var script = File.ReadAllText(pluginFile);
                syntaxTrees.Add(CSharpSyntaxTree.ParseText(script));
            }
            return syntaxTrees;
        }

        public static bool CompilePlugins()
        {
            var appDir = Path.GetDirectoryName(Application.ExecutablePath) ?? "";
            var pluginsDir = Path.Combine(appDir, "data");
            // var oldDirectory = Directory.GetCurrentDirectory();
            var startTime = DateTime.UtcNow;
            var loadingTitle = Resources.CompilingPluginsTitle;
            {
                MainForm.Instance?.UpdateStatusBarProgress(0, 100, loadingTitle, null);

                _pluginsAssembly = null;

                // Get all source code files in Plugins folder
                if (Directory.Exists(pluginsDir))
                    PluginFiles = Directory.GetFiles(pluginsDir, "*.cs", SearchOption.AllDirectories).ToList();

                MainForm.Instance?.UpdateStatusBarProgress(10, 100, loadingTitle, null);

                if (Directory.Exists(pluginsDir))
                {
                    var pluginReferencesFiles = Directory.GetFiles(pluginsDir, "reference.txt", SearchOption.AllDirectories).ToList();
                    foreach (var pluginReference in pluginReferencesFiles)
                    {
                        var s = File.ReadAllLines(pluginReference);
                        PluginReferences.AddRange(s);
                    }
                }

                MainForm.Instance?.UpdateStatusBarProgress(15, 100, loadingTitle, null);

                // Define a temp file to hold data
                var tempAssemblyFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName()); // name of temporary DLL/Assembly file
                MainForm.Instance?.AllTempFiles.Add(tempAssemblyFile);

                MainForm.Instance?.UpdateStatusBarProgress(20, 100, loadingTitle, null);

                // Read all .cs files into a syntaxTree
                var syntaxTrees = ParseSourceIntoSyntaxTrees(PluginFiles);

                MainForm.Instance?.UpdateStatusBarProgress(40, 100, loadingTitle, null);

                // Get all references from main program
                var references = new List<MetadataReference>();
                var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(p => !p.IsDynamic && !string.IsNullOrEmpty(p.Location)).ToArray();
                foreach (var asm in assemblies)
                    references.Add(MetadataReference.CreateFromFile(asm.Location));

                MainForm.Instance?.UpdateStatusBarProgress(50, 100, loadingTitle, null);

                // Add references specified in plugins
                foreach (var reference in PluginReferences)
                {
                    var referenceFileName = Path.Combine(appDir, reference);
                    if (File.Exists(referenceFileName))
                        references.Add(MetadataReference.CreateFromFile(referenceFileName));
                }

                MainForm.Instance?.UpdateStatusBarProgress(60, 100, loadingTitle, null);

                // Compile
                var compilation = CSharpCompilation.Create(
                    Path.GetFileName(tempAssemblyFile),
                    syntaxTrees,
                    references,
                    new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

                MainForm.Instance?.UpdateStatusBarProgress(70, 100, loadingTitle, null);

                // Write/Compile Temp Assembly
                var emitResult = compilation.Emit(tempAssemblyFile);

                MainForm.Instance?.UpdateStatusBarProgress(90, 100, loadingTitle, null);

                if (!emitResult.Success)
                {
                    foreach (var error in emitResult.Diagnostics)
                        if (error.Severity == DiagnosticSeverity.Error)
                            PluginErrors.Add(error.ToString());

                    // Directory.SetCurrentDirectory(oldDirectory);
                    CompileTime = DateTime.UtcNow - startTime;
                    return false;
                }

                MainForm.Instance?.UpdateStatusBarProgress(95, 100, loadingTitle, null);

                // Load Temp Assembly
                _pluginsAssembly = Assembly.LoadFile(tempAssemblyFile);

                MainForm.Instance?.UpdateStatusBarProgress(100, 100, loadingTitle, null);
            }
            CompileTime = DateTime.UtcNow - startTime;

            // Directory.SetCurrentDirectory(oldDirectory);
            return true;
        }

        public BaseInputReader? GetExpectedInputReaderForFile(ViewedProjectTab project, string fileName)
        {
            List<BaseInputReader> res = new();
            var ext = Path.GetExtension(fileName).ToLower();

            foreach (var inputReader in AllInputReaders)
            {
                if (inputReader.ExpectedFileExtensions.Contains(ext))
                    res.Add(inputReader);
            }

            if (res.Count > 0)
                return res[0].CreateNew(project);

            return null;
        }

        public BaseInputReader? GetInputReaderByName(string readerName, ViewedProjectTab project)
        {
            List<BaseInputReader> res = new();

            foreach (var inputReader in AllInputReaders)
            {
                if (string.Equals(inputReader.Name, readerName, StringComparison.CurrentCultureIgnoreCase))
                    res.Add(inputReader);
            }

            if (res.Count > 0)
                return res[0].CreateNew(project);

            return null;
        }

        public BaseParser? GetParserByName(string parserName, ViewedProjectTab project)
        {
            List<BaseParser> res = new();

            foreach (var parser in AllParsers)
            {
                if (string.Equals(parser.Name, parserName, StringComparison.CurrentCultureIgnoreCase))
                    res.Add(parser);
            }

            if (res.Count > 0)
                return res[0].CreateNew(project);

            return null;
        }
    }
}
