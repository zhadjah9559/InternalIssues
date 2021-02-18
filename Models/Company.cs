using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InternalIssues.Models
{
    public class Company
    {
        public int Id { get; set; }     //FK
        public string Name { get; set; }
        public string Description { get; set; }


        //Navigatoinal propertys
        public virtual ICollection<AppUser> Collaborators { get; set; }

        public virtual ICollection<Project> Projects { get; set; }

    }
}
