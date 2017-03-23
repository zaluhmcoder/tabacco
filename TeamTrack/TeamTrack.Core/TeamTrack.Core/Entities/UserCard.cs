using System.ComponentModel.DataAnnotations.Schema;

namespace TeamTrack.Core.Entities
{
    [Table("UserCard")]
    public class UserCard
    {
        public int UserId { get; set; }

        public int CardId { get; set; }

        public User User { get; set; }

        public Card Card { get; set; }

    }
}