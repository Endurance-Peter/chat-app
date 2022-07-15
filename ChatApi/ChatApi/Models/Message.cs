using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApi.Models
{
    public class Message
    {
        public Guid Id { get; set; }
        public string MessageText { get; set; }
        public DateTime Time => DateTime.UtcNow;

        public Guid UserId { get; set; }
        public User AppUser { get; set; }
    }
}
