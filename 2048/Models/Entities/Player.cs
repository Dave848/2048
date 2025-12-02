using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2048.Models.Entities
{
    public class Player
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public ICollection<Game>? Games { get; set; }
    }
}
