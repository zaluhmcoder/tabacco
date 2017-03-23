using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TeamTrack.Core.Enums;

namespace TeamTrack.Core.Entities
{
    [Table("Card")]
    public class Card
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public short ProjectId { get; set; }

        [Required]
        [MaxLength(250)]
        public string Name { get; set; }

        [MaxLength(50)]
        public string Identifier { get; set; }

        public byte Type { get; set; }

        public byte Priority { get; set; }

        public byte Difficulty { get; set; }

        public byte Estimation { get; set; }

        public byte Total { get; set; }

        [Column(TypeName = "Date")]
        public DateTime IssueDate { get; set; }

        [Column(TypeName = "Date")]
        public DateTime DueDate { get; set; }

        public bool IsActive { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime CreateDate { get; set; }

        public Project Project { get; set; }

        public ICollection<UserCard> UserCards { get; set; }

        [NotMapped]
        public CardType CardType
        {
            get { return (CardType)Type; }
            set { Type = (byte)value; }
        }

        [NotMapped]
        public PriorityType PriorityType
        {
            get { return (PriorityType)Priority; }
            set { Priority = (byte)value; }
        }

        [NotMapped]
        public DifficultyType DifficultyType
        {
            get { return (DifficultyType)Difficulty; }
            set { Difficulty = (byte)value; }
        }

    }
}
