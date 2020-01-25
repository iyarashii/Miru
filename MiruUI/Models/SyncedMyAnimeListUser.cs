using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miru.Models
{
    public class SyncedMyAnimeListUser
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [MaxLength(16)]
        public string Username { get; set; }
        
        public DateTime SyncTime { get; set; }
    }
}
