using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsAI_Project.Models
{
    public class ReliabilityBreakdown
    {
        public int SourceScore { get; set; }

        public int OfficialityScore { get; set; }

        public int SpecificityScore { get; set; }

        public int DuplicateScore { get; set; }

        public int DartScore { get; set; }

        public int PenaltyScore { get; set; }

        public int FinalScore { get; set; }
    }
}
