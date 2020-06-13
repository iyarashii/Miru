using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiruLibrary.Models
{
    // data model for the synced user
    public class SyncedMyAnimeListUser
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [MaxLength(16), MinLength(2)]
        public string Username { get; set; }

        public DateTime SyncTime { get; set; }
    }
}