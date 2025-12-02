using _2048.Models;
using _2048.Models.Database;
using _2048.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2048.Services
{
    public class PlayerService
    {
        private readonly DatabaseContext _db;

        public PlayerService(DatabaseContext db)
        {
            _db = db;
        }

        public async Task<Player> CreatePlayer(string name)
        {
            Player player = new Player { Name = name };

            _db.Players.Add(player);
            await _db.SaveChangesAsync();

            return player;
        }

        public async Task<Player?> GetPlayer(string name)
        {
            return await _db.Players.FirstOrDefaultAsync(p => p.Name == name);
        }

        public async Task<Player?> GetGuestPlayer()
        {
            return await GetPlayer("Guest");
        }
    }
}
