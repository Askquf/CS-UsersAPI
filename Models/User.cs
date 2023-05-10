using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VKTask
{
    [Table("user")]
    public class User
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("login")]
        [Required]
        [MaxLength(15)]
        public string Login { get; set; }
        [Column("password")]
        [Required]
        [MaxLength(15)]
        public string Password { get; set; }
        [Column("created_date")
        public DateTime? CreatedDate { get; set; }
        [Column("user_group_id")]
        public int UserGroupId { get; set; }
        [Column("user_state_id")]
        public int UserStateId { get; set; }
        public UserGroup UserGroup { get; set; }
        public UserState UserState { get; set; }
        public int Page { get; set; }
    }
}