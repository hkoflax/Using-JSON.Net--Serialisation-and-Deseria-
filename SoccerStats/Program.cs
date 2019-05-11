using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
namespace SoccerStats
{
    class Program
    {
        static void Main(string[] args)
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            DirectoryInfo directory = new DirectoryInfo(currentDirectory);
            var fileName = Path.Combine(directory.FullName, "SoccerGameResults.csv");
            var fileContents = ReadSoccerResults(fileName);

            fileName = Path.Combine(directory.FullName, "players.json");
            var players = DeserializePlayers(fileName);

            foreach (var player in GetToptenPlayers(players))
            {
                Console.WriteLine($"{player.FirstName}/{player.SecondName}/{player.PointsPerGame}");
            }

            fileName = Path.Combine(directory.FullName, "TopTenPlayers.json");
            SerializeplayerToFile(GetToptenPlayers(players), fileName);
        }

        public static string ReadFile(string fileName)
        {
            using (var reader = new StreamReader(fileName))
            {
                return reader.ReadToEnd();
            }
        }

        public static List<GameResult> ReadSoccerResults(string fileName)
        {
            var soccerResults = new List<GameResult>();
            using (var reader = new StreamReader(fileName))
            {
                string line = "";
                reader.ReadLine();
                while((line = reader.ReadLine()) != null)
                {
                    var gameResult = new GameResult();
                    string[] values = line.Split(',');

                    DateTime gameDate;
                    if (DateTime.TryParse(values[0], out gameDate))
                    {
                        gameResult.GameDate = gameDate;
                    }
                    gameResult.TeamName = values[1];
                    HomeOrAway homeOrAway;
                    if (Enum.TryParse(values[2], out homeOrAway))
                    {
                        gameResult.HomeOrAway = homeOrAway;
                    }
                    int parseInt;
                    if (int.TryParse(values[3], out parseInt))
                    {
                        gameResult.Goals = parseInt;
                    }
                    if (int.TryParse(values[4], out parseInt))
                    {
                        gameResult.GoalAttempts = parseInt;
                    }
                    if (int.TryParse(values[5], out parseInt))
                    {
                        gameResult.ShotsOnGoal = parseInt;
                    }
                    if (int.TryParse(values[6], out parseInt))
                    {
                        gameResult.ShotsOffGoal = parseInt;
                    }

                    double possessionPercent;
                    if (double.TryParse(values[7], out possessionPercent))
                    {
                        gameResult.PossessionPercent = possessionPercent;
                    }
                    soccerResults.Add(gameResult);
                }
            }
            return soccerResults;
        }
        public static List<Player> DeserializePlayers(string filename)
        {
            var players = new List<Player>();
            var serializer = new JsonSerializer();
            using (var reader = new StreamReader(filename))
            using (var jsonReader = new  JsonTextReader(reader))
            {
                players= serializer.Deserialize<List<Player>>(jsonReader);
            }
            return players;
        }
        public static List<Player> GetToptenPlayers(List<Player> players)
        {
            var TopTenPlayers = new List<Player>();
            players.Sort(new PlayerComparer());
            players.Reverse();
            int count = 0;
            foreach (var player in players)
            {
                TopTenPlayers.Add(player);
                count++;
                if (count==10)
                {
                    break;
                }
            }
            return TopTenPlayers;
        }
        public static void SerializeplayerToFile(List<Player> players, string fileName)
        {
            var serializer = new JsonSerializer();
            using (var writer = new StreamWriter(fileName))
            using (var jsonWriter = new JsonTextWriter(writer))
            {
                serializer.Serialize(jsonWriter, players);
            }
        }
    }
}
