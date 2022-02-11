using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualBasic.FileIO;

namespace ScoreSystem
{
    public class Player : IEquatable<Player>
    {
        public string Name { get; set; }
        public int Score { get; set; }
        public override string ToString()
        {
            return $"{Name}:{Score}";
        }
        public bool Equals(Player other)
        {
            return true;
        }
    }
    class Program
    {
        static string readLine(string toSay)
        {
            Console.WriteLine(toSay);
            return Console.ReadLine();
        }

        static string sortSuffix(int pos)
        {
            string[] suffixes = { "st", "nd", "rd" };
            if (pos >= 3)
            {
                return $"{pos + 1}th";
            } else
            {
                return $"{pos + 1}{suffixes[pos]}";
            }
        }
        static void Main(string[] args)
        {
            List<string> list = new List<string>();
            if (File.Exists("scores.dat"))
            {
                Console.WriteLine("Would you like to edit or view the file's contents?");
                if (Console.ReadLine().ToLower().Contains("edit"))
                {
                    using (TextFieldParser parser = new TextFieldParser("scores.dat"))
                    {
                        parser.SetDelimiters("},");
                        var lines = parser.ReadFields();
                        string editTeam = readLine("Which team would you like to edit?");
                        if(File.ReadAllText("scores.dat").ToString().ToLower().Contains(editTeam.ToLower()))
                        {
                            foreach (var line in lines)
                            {
                                if (line.Contains(":{"))
                                {
                                    var fields = line.Split(":{");
                                    if (editTeam.ToLower() == fields[0].ToLower())
                                    {
                                        var pSc = fields[1].Split("[");
                                        Console.WriteLine($"{fields[0]} Team:");
                                        foreach (var player in pSc)
                                        {
                                            var scores = player.Split("]");
                                            if (scores[0].Length > 0)
                                            {
                                                var score = scores[0].Split(":");
                                                Console.WriteLine($"{scores[0].Replace(":", " scored ")} points");
                                            }
                                        }
                                        string editPlayer = readLine("Which player would you like to edit?");
                                        foreach (var player in pSc)
                                        {
                                            var scores = player.Split("]");
                                            if(scores[0].Contains(editPlayer))
                                            {
                                                string newScore = readLine($"What would you like to change {editPlayer}'s score to?");
                                                int scoreSave;
                                                if(Int32.TryParse(newScore, out scoreSave))
                                                {
                                                    string oldStr = scores[0];
                                                    string newStr = $"{scores[0].Split(":")[0]}:{scoreSave}";
                                                    File.WriteAllText("scores.dat", $"{File.ReadAllText("scores.dat").Replace(oldStr, newStr)}");
                                                    Console.WriteLine($"{scores[0].Split(":")[0]}'s score was successfully changed to {scoreSave}!");
                                                } else
                                                {
                                                    Console.WriteLine("That is not a number!");
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        } else {
                            Console.WriteLine($"{editTeam} isn't a valid team!");
                        }
                    }
                }
                else
                {
                    List<int> totalScores = new List<int>();
                    Hashtable teams = new Hashtable(); 
                    using (TextFieldParser parser = new TextFieldParser("scores.dat"))
                    {
                        parser.SetDelimiters("},");
                        var lines = parser.ReadFields();
                        foreach (var line in lines)
                        {
                            if(line.Contains(":{")) {
                                var fields = line.Split(":{");
                                var pSc = fields[1].Split("[");
                                Console.WriteLine($"{fields[0]} Team:");
                                int tScore = 0;
                                foreach(var player in pSc)
                                {
                                    var scores = player.Split("]");
                                    if(scores[0].Length > 0)
                                    {
                                        var score = scores[0].Split(":");
                                        int sco;
                                        Console.WriteLine($"    {scores[0].Replace(":", " scored ")} points");
                                        if(Int32.TryParse(score[1], out sco))
                                        {
                                            tScore += sco;
                                        }
                                    }
                                }
                                Console.WriteLine($"{fields[0]} Team got {tScore} total points\n");
                                totalScores.Add(tScore);
                                teams.Add(tScore, fields[0]);
                            }
                        }
                    }
                    Console.WriteLine($"{teams[totalScores.Max()]} team won with {totalScores.Max()} points!");
                }
            }
            else
            {
                var newFile = File.Create("scores.dat");
                newFile.Close();
                Console.WriteLine("A new scores file has been created, please enter the name of each team");
                string teamScores = string.Empty;
                for (int i = 0; i < 4; i++)
                {
                    teamScores = string.Empty;
                    string newTeam = readLine($"Please enter the name of the {sortSuffix(i)} team");
                    teamScores += newTeam + ":{";
                    for(int p = 0; p < 5; p++)
                    {
                        string playerName = readLine($"Please enter the name of the {sortSuffix(p)} player of the team '{newTeam}'");
                        Player newPlayer = new Player();
                        newPlayer.Name = playerName;
                        string playerScore = readLine($"Please enter the score of player '{newPlayer.Name}' on the team '{newTeam}'");
                        newPlayer.Score = Convert.ToInt32(playerScore);
                        if(p == 4)
                        {
                            teamScores += $"[{newPlayer.ToString()}]" + "}";
                        } else
                        {
                            teamScores += $"[{newPlayer.ToString()}],";
                        }
                    }
                    list.Add(teamScores);
                }
                string newStr = string.Empty;
                for(int i = 0;i < list.Count;i++)
                {
                    if(i == 3)
                    {
                        newStr += list[i];
                    } else
                    {
                        newStr += $"{list[i]},";
                    }
                }
                using (StreamWriter sw = new StreamWriter("scores.dat", true))
                {
                    sw.Write(newStr);
                }
            }
        }
    }
}