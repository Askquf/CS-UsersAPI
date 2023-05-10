using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VKTask
{
    [Table("user_state")]
    public class UserState
    { 
        [Column("id")]
        public int Id { get; set; }
        [Column("code")]
        [Required]
        public string Code { get; set; }
        [Column("description")]
        [MaxLength(300)]
        public string Description { get; set; }
    }
}