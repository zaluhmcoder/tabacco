using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeamTrack.Api.Models
{

    public class UserModel
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Title { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}
