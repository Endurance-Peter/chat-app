using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApi.Models
{
    public class User : IdentityUser<Guid>
    {
        public virtual IEnumerable<Message> Messages => _messages;
        private readonly ISet<Message> _messages = new HashSet<Message>();
    }
}
