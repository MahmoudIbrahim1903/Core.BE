
using Emeint.Core.BE.Domain.SeedWork;
using System.ComponentModel.DataAnnotations.Schema;

namespace Emeint.Core.BE.Chatting.Infractructure.Model
{
    public class ChannelMember : Entity
    {
        public int ChannelId { set; get; }
        [ForeignKey("ChannelId")]
        public virtual Channel Channel { set; get; }

        public string UserId { set; get; }
        [ForeignKey("UserId")]
        public virtual User User { set; get; }
    }
}
