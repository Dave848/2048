using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2048.Models.DTO
{
    public record GameDTO
    {
        public int Id { get; set; }
        public int Score { get; set; }
        public int MovesCount { get; set; }
        public double TimeElapsed { get; set; }
        public required int[] BoardState { get; set; }
    }
}
