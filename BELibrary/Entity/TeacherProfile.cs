using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BELibrary.Entity
{
    public class TeacherProfile
    {
        public int Id { get; set; }

        [StringLength(50)]
        public string Username { get; set; }

        public string Hobby { get; set; }
        public string Description { get; set; }
        public string FacebookLink { get; set; }
        public string TwitterLink { get; set; }
        public string SkypeLink { get; set; }
        public string Experiences { get; set; }
        public string Address { get; set; }
        public string ContactPhone { get; set; }
        public string ContactEmail { get; set; }
        public string ContactLocation { get; set; }
    }
}