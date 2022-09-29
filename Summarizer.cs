using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ProcessTRAC
{
    internal class Summarizer
    {
        //        List<Competency> Results = new List<Competency>();
        public Dictionary<string, Competency> Results = new Dictionary<string, Competency>();
        public List<string> CompetencyNames = new List<string>();

        public void LoadSelf(StreamReader sr)
        {
            try
            {
                Results.Clear();
                CompetencyNames.Clear();

                String line = sr.ReadToEnd();
                //Console.WriteLine(line);

                var options = new JsonDocumentOptions
                {
                    AllowTrailingCommas = true,
                    CommentHandling = JsonCommentHandling.Skip
                };
                using (JsonDocument document = JsonDocument.Parse(line, options))
                {
                    JsonElement sections = document.RootElement.GetProperty("sections");

                    foreach (JsonProperty section in sections.EnumerateObject())
                    {
                        System.Diagnostics.Debug.WriteLine(section.Name);
                        System.Diagnostics.Debug.WriteLine(section.Value);
                        JsonElement competencies = section.Value.GetProperty("competencies");
                        foreach (JsonProperty competency in competencies.EnumerateObject())
                        {
                            Competency comp = new Competency();
                            comp.Section = section.Name;
                            comp.Name = competency.Name;
                            comp.Category = competency.Value.GetProperty("category").GetString();
                            comp.Text = competency.Value.GetProperty("competency").GetString();
                            foreach (JsonProperty example in competency.Value.GetProperty("examples").EnumerateObject())
                            {
                                comp.Examples.Add(example.Value.GetString());
                            }

                            comp.SelfRating = int.Parse(competency.Value.GetProperty("rating").GetString());
                            Results.Add(comp.Name, comp);
                            CompetencyNames.Add(comp.Name);
                        }
                    }
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }

            // https://learn.microsoft.com/en-us/dotnet/core/deploying/single-file/overview?tabs=cli

        }
        public void LoadLeader(StreamReader sr)
        {
            try
            {
                String line = sr.ReadToEnd();

                var options = new JsonDocumentOptions
                {
                    AllowTrailingCommas = true,
                    CommentHandling = JsonCommentHandling.Skip
                };
                using (JsonDocument document = JsonDocument.Parse(line, options))
                {
                    JsonElement sections = document.RootElement.GetProperty("sections");

                    foreach (JsonProperty section in sections.EnumerateObject())
                    {
                        JsonElement competencies = section.Value.GetProperty("competencies");
                        foreach (JsonProperty competency in competencies.EnumerateObject())
                        {
                            string key = competency.Name;

                            Competency value = Results[key];

                            value.LeaderRating = int.Parse(competency.Value.GetProperty("rating").GetString());
                        }
                    }
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }

            // https://learn.microsoft.com/en-us/dotnet/core/deploying/single-file/overview?tabs=cli

        }

        public void LoadOther(StreamReader sr)
        {
            try
            {
                String line = sr.ReadToEnd();

                var options = new JsonDocumentOptions
                {
                    AllowTrailingCommas = true,
                    CommentHandling = JsonCommentHandling.Skip
                };
                using (JsonDocument document = JsonDocument.Parse(line, options))
                {
                    JsonElement sections = document.RootElement.GetProperty("sections");

                    foreach (JsonProperty section in sections.EnumerateObject())
                    {
                        JsonElement competencies = section.Value.GetProperty("competencies");
                        foreach (JsonProperty competency in competencies.EnumerateObject())
                        {
                            string key = competency.Name;

                            Competency value = Results[key];

                            value.OtherRatings.Add(int.Parse(competency.Value.GetProperty("rating").GetString()));
                        }
                    }
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }

            // https://learn.microsoft.com/en-us/dotnet/core/deploying/single-file/overview?tabs=cli

        }

        public string Generate()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<html>");
            sb.AppendLine("<body>");

            string prevSection = "";
            string prevCategory = "";

            foreach(string competencyNames in CompetencyNames)
            {
                Competency thisComp = Results[competencyNames];

                // handle section header
                if (thisComp.Section != prevSection)
                {
                    sb.AppendLine(SectionHeading(thisComp.Section));
                    sb.AppendLine("</br>");
                    prevSection = thisComp.Section;
                    prevCategory = "";
                }

                // handle category header
                //if (thisComp.Category != prevCategory)
                //{
                //    sb.AppendLine(CategoryHeading(thisComp.Category));
                //    prevCategory = thisComp.Category;
                //}
                sb.AppendLine($"<h3>{thisComp.Text}</h3>");
                sb.AppendLine("</br>");
                sb.AppendLine(StartTable());
                int counter = 0;
                // this loop is for display
                foreach (string example in thisComp.Examples)
                {
                    int numVotes = 0;
                    foreach (int vote in thisComp.OtherRatings)
                    {
                        if(vote == counter)
                        {
                            numVotes++;
                        }
                    }

                    sb.AppendLine(ExampleLine(thisComp.SelfRating == counter, thisComp.LeaderRating == counter, numVotes, example));
                    counter++;
                }
                sb.AppendLine(EndTable());
            }

            sb.AppendLine("</body>");
            sb.AppendLine("</html>");
            return sb.ToString();
        }

        private string SectionHeading(string sectionName)
        {
            return $"<h1>{sectionName}</h1>";
        }
        private string CategoryHeading(string sectionName)
        {
            return $"<h2>{sectionName}</h2>";
        }
        private string StartTable()
        {
            return "<table border=1><tr><th>Self<br>Rating</th>th>Leader<br>Rating</th><th>Other<br>Ratings</th><th>Description</th></tr>";
        }
        private string EndTable()
        {
            return "</table>";
        }
        private string ExampleLine(bool selfRating, bool leaderRating, int numVotes, string text)
        {
            string sr = selfRating ? "*" : "";
            string lr = leaderRating ? "*" : "";
            string votes = numVotes==0 ? "" : numVotes.ToString();
            return $"<tr><td style=\"text-align:center\">{sr}</td><td style=\"text-align:center\">{lr}</td><td style=\"text-align:center\">{votes}</td><td  style=\"width:600px\">{text}</td></tr>";
        }
    }
}
