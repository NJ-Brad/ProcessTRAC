using System.Text.Json;
using System.Text.Json.Nodes;

namespace ProcessTRAC
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            foreach (string file in Directory.EnumerateFiles(
                pathToFolder, 
                "*" , 
                SearchOption.AllDirectories) 
                )
            {
                // do something

            }

            return;

            List<Competency> results = new List<Competency>();

            try
            {
                Summarizer summarizer = new Summarizer();

                if (args.Length > 0)
                {
                    //using (StreamReader sr = new StreamReader(@"c:\users\brad\downloads\self.json"))
                    using (StreamReader sr = new StreamReader(args[0]))
                    {
                        summarizer.LoadSelf(sr);
                    }
                }

                for (int i = 1; i < args.Length - 1; i++)
                {
                    using (StreamReader sr = new StreamReader(args[i]))
                    {
                        summarizer.LoadOther(sr);
                    }
                }

                string outputFile = args[args.Length - 1];

                string summary = summarizer.Generate();

                Console.WriteLine(outputFile);
                File.WriteAllText(outputFile, summary);
            }
            catch (IOException e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }

            // https://learn.microsoft.com/en-us/dotnet/core/deploying/single-file/overview?tabs=cli
        }
    }
}