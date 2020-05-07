using System;
using System.Collections.Generic;

namespace DevOpsMetrics.Service.Models
{
    public class LeadTimeForChangesModel
    {
        public string branch { get; set; }
        public TimeSpan duration { get; set; }
        public List<Commit> Commits { get; set; }
        public int BuildCount { get; set; } //TODO: Should this actually be the list of builds?
    }
}
