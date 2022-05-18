// Copyright (c) 2022 iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/Miru/blob/master/LICENSE for full license details.

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiruLibrary.Models
{
    // data model for the synced user
    public class SyncedMyAnimeListUser : ISyncedMyAnimeListUser
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [MaxLength(16), MinLength(2)]
        public string Username { get; set; }

        public DateTime SyncTime { get; set; }
    }
}