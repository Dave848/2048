using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2048.Models.DTO
{
    public record RecordDTO
    {
        public required string PlayerName { get; set; }
        public int Score { get; set; }
        public double TimeElapsed { get; set; }
        public DateTime FinishedDate { get; set; }
    }
}
