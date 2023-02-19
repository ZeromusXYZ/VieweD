using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using VieweD.Engine.Common;

namespace VieweD.Engine
{
    public class Engines
    {
        public static Engines Instance; 

        public static readonly List<EngineBase> AllEngines = new List<EngineBase>();
        public static List<string> PluginFiles = new List<string>();
        public static List<string> PluginReferences = new List<string>();
        public static List<string> PluginErrors = new List<string>();
        public static TimeSpan CompileTime { get; set; } = TimeSpan.FromSeconds(0);
        private static Assembly _pluginsAssembly;
        public static bool PreParseData => Properties.Settings.Default.PreParseData;
        public static bool UseGameClientData => Properties.Settings.Default.UseGameClientData;
        public static bool ShowStringHexData => Properties.Settings.Default.ShowStringHexData;

        public Engines()
        {
            CompilePlugins();
            AllEngines.Clear();

            // Load local in-app engines
            var allClasses = Assembly.GetExecutingAssembly().GetTypes();
            foreach (var aClass in allClasses)
            {
                if (aClass.BaseType != typeof(EngineBase)) 
                    continue;

                var engine = Activator.CreateInstance(aClass) as EngineBase;
                RegisterEngine(engine);
            }

            // Load plugin engines
            if (Engines._pluginsAssembly != null)
            {
                var pluginClasses = Engines._pluginsAssembly.GetTypes();
                foreach (var aClass in pluginClasses)
                {
                    if (aClass.BaseType == typeof(EngineBase))
                    {
                        var engine = Activator.CreateInstance(aClass) as EngineBase;
                        RegisterEngine(engine);
                    }
                }
            }

            AllEngines.Sort();
        }

        public static bool CompilePlugins()
        {
            var appDir = Path.GetDirectoryName(Application.ExecutablePath) ?? "";
            var pluginsDir = Path.Combine(appDir, "data");
            // var oldDirectory = Directory.GetCurrentDirectory();
            var startTime = DateTime.UtcNow;
            using (var loadForm = new LoadingForm())
            {
                loadForm.Text = @"VieweD - Compiling plugins ...";
                loadForm.pb.Maximum = 100;

                // Only actually show the form if we got debug enabled
                if (Properties.Settings.Default.ShowDebugInfo)
                    loadForm.Show();
                _pluginsAssembly = null;

                // Get all source code files in Plugins folder
                if (Directory.Exists(pluginsDir))
                    PluginFiles = Directory.GetFiles(pluginsDir, "*.cs", SearchOption.AllDirectories).ToList();

                loadForm.pb.Value = 10;

                if (Directory.Exists(pluginsDir))
                {
                    var pluginReferencesFiles = Directory.GetFiles(pluginsDir, "reference.txt", SearchOption.AllDirectories).ToList();
                    foreach (var pluginReference in pluginReferencesFiles)
                    {
                        var s = File.ReadAllLines(pluginReference);
                        PluginReferences.AddRange(s);
                    }
                }

                loadForm.pb.Value = 15;

                // Define a temp file to hold data
                var tempAssemblyFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName()); // name of temporary DLL/Assembly file
                MainForm.ThisMainForm.AllUsedTempFiles.Add(tempAssemblyFile);

                loadForm.pb.Value = 20;

                // Read all .cs files into a syntaxTree
                var syntaxTrees = new List<SyntaxTree>();
                foreach (var pluginFile in PluginFiles)
                {
                    var script = File.ReadAllText(pluginFile);
                    syntaxTrees.Add(CSharpSyntaxTree.ParseText(script));
                    loadForm.pb.Value = (int)(PluginFiles.Count / 20f * syntaxTrees.Count) + 20;
                }

                loadForm.pb.Value = 40;

                // Get all references from main program
                var references = new List<MetadataReference>();
                var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(p => !p.IsDynamic && !string.IsNullOrEmpty(p.Location)).ToList();
                foreach (var asm in assemblies)
                {
                    references.Add(MetadataReference.CreateFromFile(asm.Location));
                    loadForm.pb.Value = (int)(assemblies.Count / 10f * references.Count) + 40;
                }

                loadForm.pb.Value = 50;

                // Add references specified in plugins
                foreach (var reference in PluginReferences)
                {
                    var referenceFileName = Path.Combine(appDir, reference);
                    if (File.Exists(referenceFileName))
                        references.Add(MetadataReference.CreateFromFile(referenceFileName));
                }

                loadForm.pb.Value = 60;

                // Compile
                var compilation = CSharpCompilation.Create(
                    Path.GetFileName(tempAssemblyFile),
                    syntaxTrees,
                    references,
                    new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

                loadForm.pb.Value = 70;

                // Write/Compile Temp Assembly
                var emitResult = compilation.Emit(tempAssemblyFile);

                loadForm.pb.Value = 90;

                if (!emitResult.Success)
                {
                    foreach (var error in emitResult.Diagnostics)
                        if (error.Severity == DiagnosticSeverity.Error)
                            PluginErrors.Add(error.ToString());

                    // Directory.SetCurrentDirectory(oldDirectory);
                    return false;
                }

                loadForm.pb.Value = 95;

                // Load Temp Assembly
                _pluginsAssembly = Assembly.LoadFile(tempAssemblyFile); ;

                loadForm.pb.Value = 100;
            }
            var endTime = DateTime.UtcNow;
            CompileTime = endTime - startTime;

            // Directory.SetCurrentDirectory(oldDirectory);
            return true;
        }

        private static void RegisterEngine(EngineBase engine)
        {
            if (engine == null)
                return;
            AllEngines.Add(engine);
        }

        public static string GetRegisteredFileExtensionForOpen(bool includeProjects)
        {
            var res = string.Empty;
            res += "Known Files|";
            if (includeProjects)
            {
                res += "*.pvd;*.pvlv;*.7z";
                // res += "*.pvd;*.pvlv";
            }

            foreach (var e in AllEngines)
            {
                var exts = string.Empty;
                foreach(var ext in e.FileExtensions)
                {
                    exts += ";*" + ext.Key;
                }
                res += exts;
            }

            if (includeProjects)
            {
                res += "|Project Files|*.pvd;*.pvlv|7-Zip Archive Files|*.7z";
                // res += "|Project Files|*.pvd;*.pvlv";
            }

            foreach (var e in AllEngines)
            {
                res += "|" + e.EngineName + " Files|";
                var exts = string.Empty;
                foreach (var ext in e.FileExtensions)
                {
                    if (exts != string.Empty)
                        exts += ";";
                    exts += "*" + ext.Key;
                }
                res += exts;
            }
            res += "|All Files|*.*";
            return res;
        }

        public static EngineBase GetEngineByFileName(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLower();
            foreach (var engine in AllEngines)
            {
                if (engine.FileExtensions.ContainsKey(extension))
                    return engine;
            }
            return new EngineBase();
        }

        public static EngineBase GetEngine(string engineId)
        {
            foreach(var e in AllEngines)
            {
                if (e.EngineId == engineId)
                    return e;
            }
            return null;
        }

        public static EngineBase CreateEngineByFileExtension(string ext)
        {
            foreach (var e in AllEngines)
            {
                if (e.FileExtensions.ContainsKey(ext))
                    return (EngineBase)Activator.CreateInstance(e.GetType());
            }
            return null;
        }
    }
}
