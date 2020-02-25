using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;

namespace ParallelFileCopy
{
    public class Program
    {
        public static Progress _Progress = new Progress();
        public static Dictionary<string, string> FailedCopies = new Dictionary<string, string>();

        static async Task Main(string[] args)
        {
            Parser.Default.ParseArguments<CopyOptions>(args)
                .WithParsed<CopyOptions>(o => { Program.ParsedOptions = o; });

            if (string.IsNullOrWhiteSpace(Program.ParsedOptions?.Destination)
                || string.IsNullOrWhiteSpace(Program.ParsedOptions?.Source))
            {
                Environment.Exit(1);
            }
            RecursiveCopy();
            foreach (KeyValuePair<string, string> failed in FailedCopies)
            {
                Console.WriteLine($"Failed: {failed.Key} to {failed.Value}");
            }
        }

        public static CopyOptions ParsedOptions { get; set; }

        private static void RecursiveCopy()
        {
            List<FileInfo> files = new DirectoryInfo(Program.ParsedOptions.Source)
                .EnumerateFiles("*.*", SearchOption.AllDirectories).ToList();
            _Progress.TotalFileCopyCount = files.Count;
            Parallel.ForEach(files, new ParallelOptions()
            {
                MaxDegreeOfParallelism = Program.ParsedOptions.Threads
            }, file => CopyFile(file));
        }

        private static void CopyFile(FileInfo file)
        {
            string sourceFileName = file.FullName;
            string destFileName = Path.Combine(Program.ParsedOptions.Destination,
                sourceFileName.Replace(Program.ParsedOptions.Source, ""));
            if (Program.ParsedOptions.Verbose)
            {
                Console.WriteLine($"{sourceFileName} to {destFileName}");
            }

            if (!Program.ParsedOptions.DryRun)
            {
                try
                {
                    Directory.CreateDirectory(new FileInfo(destFileName).Directory.FullName);
                }
                catch
                {
                }

                bool _copyFile = true;
                if (Program.ParsedOptions.CheckLength && File.Exists(destFileName) && !Program.ParsedOptions.Overwrite)
                {
                    _copyFile = new FileInfo(destFileName).Length != new FileInfo(sourceFileName).Length;
                }

                if (_copyFile)
                {
                    try
                    {
                        File.Copy(sourceFileName, destFileName, Program.ParsedOptions.Overwrite);
                    }
                    catch (Exception e)
                    {
                        FailedCopies.Add(sourceFileName, destFileName);
                    }

                    Interlocked.Increment(ref _Progress.FileCopyCount);
                    if (!Program.ParsedOptions.Verbose)
                    {
                        Console.Clear();
                        Console.WriteLine($"{Program._Progress.FileCopyCount}/{Program._Progress.TotalFileCopyCount}");
                    }
                }
            }
        }
    }
}