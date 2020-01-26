using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miru.Models
{
    public class AnimeAiringTime
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long MalId { get; set; }
        public string Broadcast { get; set; }
        public string Title { get; set; }
    }
}
