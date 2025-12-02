using _2048.Models.Entities;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace _2048.Models.Database
{
    public static class DatabaseSeeder
    {
        public static void Seed(DatabaseContext db)
        {
            if (!db.Players.Any())
            {
                AddPlayers(db);
            }

            if (!db.Achievements.Any())
            {
                AddAchievements(db);
            }

            db.SaveChanges();
        }

        private static void AddPlayers(DatabaseContext db)
        {
            db.Players.Add(new Player { Name = "Guest" });
        }

        private static void AddAchievements(DatabaseContext db)
        {
            var achievementsJsons = new List<string>([
                "Resources/Achievements/reach.json",
                "Resources/Achievements/reach_within_time.json",
                "Resources/Achievements/moves.json",
                "Resources/Achievements/time_played.json"
                ]);

            foreach (var path in achievementsJsons)
                AddAchievementsFromFile(db, path);

            db.SaveChanges();
        }

        private static void AddAchievementsFromFile(DatabaseContext db, string path)
        {
            var fullPath = Path.Combine(AppContext.BaseDirectory, path);

            if (!File.Exists(fullPath))
                throw new FileNotFoundException("Achievements file not found.", fullPath);

            var jsonData = File.ReadAllText(fullPath);

            JsonSerializerOptions options = new()
            {
                Converters = { new JsonStringEnumConverter() }
            };

            var achievements = JsonSerializer.Deserialize<List<Achievement>>(jsonData, options);

            if (achievements != null)
            {
                db.Achievements.AddRange(achievements);
            }
        }
    }
}
