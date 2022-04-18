using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using RimDumper.Parsers;
using Verse;

namespace RimDumper
{
    public class CustomParsers
    {
        private const string DllNamePrefix = "CustParser.";
        delegate void LogFn(string msg);

        public static Parser[] GenerateCustomParsers()
        {
            var outputFileName = $"{GenFilePaths.FolderUnderSaveData("RimDumper")}\\tmpDll.dll";
            outputFileName = Path.GetFullPath(outputFileName);
            var outputFileNameCecil = $"{outputFileName}.new";
            var (compiler, parameters) = CreateCompiler(outputFileName);

            AddReferencedAssemblies(parameters);
            var results = CompileAssembly(compiler, parameters);
            if (results.Errors.Count > 0)
            {
                foreach (CompilerError CompErr in results.Errors)
                {
                    LogFn log = CompErr.IsWarning ? (LogFn)Log.Warning : (LogFn)Log.Error;
                    log($"{Path.GetFileName(CompErr.FileName)}:{CompErr.Line}, CS{CompErr.ErrorNumber} - '{CompErr.ErrorText};");
                }
                return Array.Empty<Parser>();
            }

            if (!File.Exists(outputFileName))
            {
                Log.Error($"Compiled assembly {outputFileName} not found!");
                return Array.Empty<Parser>();
            }

            //var asm = results.CompiledAssembly;
            string internalAssemblyFileName = Guid.NewGuid().ToString(); // need to change internal dll name so that you can inject it many times
            RenameInternalDllName(outputFileName, outputFileNameCecil, internalAssemblyFileName);

            var asm = Assembly.Load(File.ReadAllBytes(outputFileNameCecil));

            // clean dlls
            if (File.Exists(outputFileName))
                File.Delete(outputFileName);
            if (File.Exists(outputFileNameCecil))
                File.Delete(outputFileNameCecil);
            
            return GetParserInstances(asm);
        }

        private static Parser[] GetParserInstances(Assembly asm)
        {
            var parsers = new List<Parser>();
            var customParsers = asm.GetTypes().Where(t => t.IsSubclassOf(typeof(Parser))).ToList();
            foreach (var parser in customParsers)
            {
                Log.Message($"Found custom parser: {parser}");
                Parser? parserInst = null;
                try
                {
                    parserInst = Activator.CreateInstance(parser) as Parser;
                }
                catch (Exception ex)
                {
                    Log.Error($"Can't instantiate type: {parser}. {ex}");
                }
                if (parserInst != null)
                {
                    parsers.Add(parserInst);
                }
            }
            return parsers.ToArray();
        }

        private static void RenameInternalDllName(string? outputFileName, string outputFileNameCecil, string internalAssemblyFileName)
        {
            var asmCecil = Mono.Cecil.AssemblyDefinition.ReadAssembly(outputFileName);
            asmCecil.Name = new Mono.Cecil.AssemblyNameDefinition(DllNamePrefix + internalAssemblyFileName, Version.Parse("1.0.0.0"));
            asmCecil.Write(outputFileNameCecil);
            asmCecil.Dispose();
        }

        private static (CSharpCompiler.CodeCompiler compiler, CompilerParameters parameters) CreateCompiler(string outputFileName)
        {
            CSharpCompiler.CodeCompiler cSharpCompiler = new();
            CompilerParameters parameters = new()
            {
                GenerateInMemory = false, // true - generate dynamic module with some restrictions
                GenerateExecutable = false,
                OutputAssembly = outputFileName,
                CompilerOptions = "/unsafe /langversion:latest" // latest supported version 8!
            };
            return (cSharpCompiler, parameters);
        }

        private static CompilerResults CompileAssembly(CSharpCompiler.CodeCompiler cSharpCompiler, CompilerParameters parameters)
        {
            var cs = Directory.GetFiles($"{RimDumperMod.RootDir}/CustomParsers", "*.cs", SearchOption.AllDirectories);
            var results = cSharpCompiler.CompileAssemblyFromFileBatch(parameters, cs);
            return results;
        }

        private static void AddReferencedAssemblies(CompilerParameters parameters)
        {
            var dlls = AppDomain.CurrentDomain.GetAssemblies().Where(p => !p.IsDynamic);
            foreach (var a in dlls)
            {
                if (a.GetName().Name.StartsWith(DllNamePrefix))
                    continue; // skip previous injected parsers
                parameters.ReferencedAssemblies.Add(a.GetName().Name + ".dll");
            }
        }
    }
}