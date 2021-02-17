using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InternalIssues.Models
{
    public class ProjectUser
    {
        public string UserId { get; set; }

        public AppUser User { get; set; }

        public int ProjectUserId { get; set; }

        public AppUser Project { get; set; }

    }
}
