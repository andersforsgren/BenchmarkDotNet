﻿using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using BenchmarkDotNet.Mathematics;
using BenchmarkDotNet.Portability;
using CommandLine;
using CommandLine.Text;
using JetBrains.Annotations;

namespace BenchmarkDotNet.ConsoleArguments
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public class CommandLineOptions
    {
        [Option('j', "job", Required = false, Default = "Default", HelpText = "Dry/Short/Medium/Long or Default")]
        public string BaseJob { get; set; }

        [Option('r', "runtimes", Required = false, HelpText = "Clr/Core/Mono/CoreRt")]
        public IEnumerable<string> Runtimes { get; set; }

        [Option('e', "exporters", Required = false, HelpText = "GitHub/StackOverflow/RPlot/CSV/JSON/HTML/XML")]
        public IEnumerable<string> Exporters { get; set; }

        [Option('m', "memory", Required = false, Default = false, HelpText = "Prints memory statistics")]
        public bool UseMemoryDiagnoser { get; set; }

        [Option('d', "disasm", Required = false, Default = false, HelpText = "Gets disassembly of benchmarked code")]
        public bool UseDisassemblyDiagnoser { get; set; }

        [Option('f', "filter", Required = false, HelpText = "Glob patterns")]
        public IEnumerable<string> Filters { get; set; }

        [Option('i', "inProcess", Required = false, Default = false, HelpText = "Run benchmarks in Process")]
        public bool RunInProcess { get; set; }

        [Option('a', "artifacts", Required = false, HelpText = "Valid path to accessible directory")]
        public DirectoryInfo ArtifactsDirectory { get; set; }

        [Option("outliers", Required = false, Default = OutlierMode.OnlyUpper, HelpText = "None/OnlyUpper/OnlyLower/All")]
        public OutlierMode Outliers { get; set; }

        [Option("affinity", Required = false, HelpText = "Affinity mask to set for the benchmark process")]
        public int? Affinity { get; set; }

        [Option("allStats", Required = false, Default = false, HelpText = "Displays all statistics (min, max & more)")]
        public bool DisplayAllStatistics { get; set; }

        [Option("allCategories", Required = false, HelpText = "Categories to run. If few are provided, only the benchmarks which belong to all of them are going to be executed")]
        public IEnumerable<string> AllCategories { get; set; }

        [Option("anyCategories", Required = false, HelpText = "Any Categories to run")]
        public IEnumerable<string> AnyCategories { get; set; }

        [Option("attribute", Required = false, HelpText = "Run all methods with given attribute (applied to class or method)")]
        public IEnumerable<string> AttributeNames { get; set; }

        [Option("join", Required = false, Default = false, HelpText = "Prints single table with results for all benchmarks")]
        public bool Join { get; set; }
        
        [Option("cli", Required = false, HelpText = "Path to dotnet cli (optional).")]
        public FileInfo CliPath { get; set; }

        [Option("coreRun", Required = false, HelpText = "Path to CoreRun (optional).")]
        public FileInfo CoreRunPath { get; set; }
        
        [Option("keepFiles", Required = false, Default = false, HelpText = "Determines if all auto-generated files should be kept or removed after running the benchmarks.")]
        public bool KeepBenchmarkFiles { get; set; }

        [Usage(ApplicationAlias = "")]
        [PublicAPI]
        public static IEnumerable<Example> Examples
        {
            get
            {
                var style = new UnParserSettings { PreferShortName = true };

                yield return new Example("Use Job.ShortRun for running the benchmarks", style, new CommandLineOptions { BaseJob = "short" });
                yield return new Example("Run benchmarks in process", style, new CommandLineOptions { RunInProcess = true });
                yield return new Example("Run benchmarks for Clr, Core and Mono", style, new CommandLineOptions { Runtimes = new[] { "Clr", "Core", "Mono" } });
                yield return new Example("Use MemoryDiagnoser to get GC stats", style, new CommandLineOptions { UseMemoryDiagnoser = true });
                yield return new Example("Use DisassemblyDiagnoser to get disassembly", style, new CommandLineOptions { UseDisassemblyDiagnoser = true });
                yield return new Example("Run all benchmarks exactly once", style, new CommandLineOptions { BaseJob = "Dry", Filters = new[] { HandleWildcardsOnUnix("*") } });
                yield return new Example("Run all benchmarks from System.Memory namespace", style, new CommandLineOptions { Filters = new[] { HandleWildcardsOnUnix("System.Memory*") } });
                yield return new Example("Run all benchmarks from ClassA and ClassB using type names", style, new CommandLineOptions { Filters = new[] { "ClassA", "ClassB" } });
                yield return new Example("Run all benchmarks from ClassA and ClassB using patterns", style, new CommandLineOptions { Filters = new[] { HandleWildcardsOnUnix("*.ClassA.*"), HandleWildcardsOnUnix("*.ClassB.*") } });
                yield return new Example("Run all benchmarks called `BenchmarkName` and show the results in single summary", style, new CommandLineOptions { Join = true, Filters = new[] { HandleWildcardsOnUnix("*.BenchmarkName") } });
            }
        }

        private static string HandleWildcardsOnUnix(string input) => !RuntimeInformation.IsWindows() && input.IndexOf('*') >= 0 ? $"'{input}'" : input; // #842
    }
}