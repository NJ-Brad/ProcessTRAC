using System.Text.Json;
using System.Text.Json.Nodes;

namespace ProcessTRAC
{
    internal class Program
    {
        static void Main(string[] args)
        {
            bool selfLoaded = false;
            List<Competency> results = new List<Competency>();

            try
            {
                Summarizer summarizer = new Summarizer();
            // load self
            foreach (string fileName in Directory.EnumerateFiles(
                Directory.GetCurrentDirectory(), 
                "self.json" , 
                SearchOption.TopDirectoryOnly) 
                )
            {
                using(StreamReader sr = new StreamReader(fileName))
                {
                    summarizer.LoadSelf(sr);
                }
                selfLoaded = true;
            }
            if(!selfLoaded)
            {
                Console.WriteLine("There is no self.json in the current directory");
                return;
            }

            // load leader
            foreach (string fileName in Directory.EnumerateFiles(
                Directory.GetCurrentDirectory(), 
                "leader.json" , 
                SearchOption.TopDirectoryOnly) 
                )
            {
                using(StreamReader sr = new StreamReader(fileName))
                {
                summarizer.LoadLeader(sr);
                }
            }

            foreach (string fileName in Directory.EnumerateFiles(
                Directory.GetCurrentDirectory(), 
                "*.json" , 
                SearchOption.TopDirectoryOnly) 
                )
            {
                if(Path.GetFileNameWithoutExtension(fileName).ToUpper() == "SELF")
                {
                }
                if(Path.GetFileNameWithoutExtension(fileName).ToUpper() == "LEADER")
                {
                using(StreamReader sr = new StreamReader(fileName))
                {
                    summarizer.LoadOther(sr);
                }
                }
            }

                string outputFile = Path.Combine(Directory.GetCurrentDirectory(), "Results.html");

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