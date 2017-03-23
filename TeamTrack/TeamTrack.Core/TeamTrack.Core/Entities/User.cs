using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TeamTrack.Core.Entities
{
    public class User : IdentityUser<int>
    {
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; }

        [Required]
        [MaxLength(50)]
        public string Title { get; set; }

        public bool IsContributor { get; set; }

        public bool IsActive { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime CreateDate { get; set; }

        public ICollection<UserCard> UserCards { get; set; }

    }
}
