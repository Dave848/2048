using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2048.Models.Entities
{
    public class Achievement
    {
        public int Id { get; set; }
        public int TargetValue { get; set; }
        public required string IconPath { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public AchievementType Type { get; set; }
        public bool IsCompleted { get; set; } = false;
        public DateTime? CompletedAt { get; set; } = null;
    }
}
