using CommandLine;

namespace ParallelFileCopy
{
    public class CopyOptions
    {
        [Option('s', "source", Required = true, HelpText = "Source Folder")]
        public string Source { get; set; }

        [Option('d', "destination", Required = true, HelpText = "Destination Folder")]
        public string Destination { get; set; }

        [Option('t', "threads", Required = false, HelpText = "Number of copy threads", Default = 2)]
        public int Threads { get; set; }

        [Option('x', "dryrun", Required = false, HelpText = "Do a dry run", Default = false)]
        public bool DryRun { get; set; }

        [Option('v', "verbose", Required = false, HelpText = "List the files as they are copied/enumerated",
            Default = false)]
        public bool Verbose { get; set; }
        [Option('f', "force", Required = false, HelpText = "Overwrite files",
            Default = false)]
        public bool Overwrite { get; set; }
        [Option("checklength", Required = false, HelpText = "Check the file length and overwrite if it differs",
            Default = false)]
        public bool CheckLength { get; set; }

            
    }
}