using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VaporStore.Data.Models
{
    public class GameTag
    {
        public Tag Tag { get; set; }
        public int TagId { get; set; }
        public Game Game { get; set; }
        public int GameId { get; set; }
    }
}