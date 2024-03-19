using Emeint.Core.BE.API.Application.ValueObjects.ViewModels;
using Emeint.Core.BE.Chatting.Infractructure.Data;
using Emeint.Core.BE.Chatting.Infractructure.IRepositories;
using Emeint.Core.BE.Chatting.Infractructure.Model;
using Emeint.Core.BE.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Chatting.Infractructure.Repositories
{
    public class ChannelRepository : BaseRepository<Channel, ChattingDbContext>, IChannelRepository
    {
        public ChannelRepository(ChattingDbContext chattingDbContext) : base(chattingDbContext)
        {

        }

        public void CreateChannel(List<ChannelMember> channelMembers, string channelUrl)
        {
            _context.Channels.Add(new Channel()
            {
                ChannelUrl = channelUrl,
                CreatedBy = "system",
                CreationDate = DateTime.UtcNow,
                ChannelMembers = channelMembers
            });

            SaveEntities();
        }

        public Channel GetChannel(List<ChannelMember> channelMembers)
        {
            var memberIds = channelMembers.Select(m => m.UserId).ToList();
            var channelId = _context.ChannelMembers
               .Where(cm => memberIds.Contains(cm.UserId)).GroupBy(c => c.ChannelId)
               .Select(group => new { chId = group.Key, memCount = group.Count() })
               .FirstOrDefault(result => result.memCount == channelMembers.Count())?.chId ?? null;

            if (channelId != null)
                return _context.Channels.FirstOrDefault(c => c.Id == channelId);
            else
                return null;
        }

        public Channel GetChannel(string channelUrl)
        {
            return GetAll().Include(ch => ch.ChannelMembers).ThenInclude(chm => chm.User).AsNoTracking()
                .FirstOrDefault(ch => ch.ChannelUrl.Trim() == channelUrl.Trim());
        }

        public PagedList<Channel> GetChannels(string memberId, string type, string contactName, PaginationVm pagination)
        {
            var query = _context.Channels.Include(c => c.ChannelMembers).ThenInclude(cm => cm.User).AsNoTracking()
                .Where(c => c.ChannelMembers.Any(m => m.UserId == memberId));

            if (!string.IsNullOrEmpty(type))
                query = query.Where(c => c.ChannelMembers.Any(m => m.User.Type.Trim().ToLower() == type.Trim().ToLower()));

            if (!string.IsNullOrEmpty(contactName))
                query = query.Where(c => c.ChannelMembers.Any(m => m.User.Name.ToLower().Contains(contactName.Trim().ToLower())));


            PagedList<Channel> channels = new PagedList<Channel>(query.Count(), pagination);

            if (pagination != null)
                query = query.OrderByDescending(c => c.CreationDate).Skip(channels.SkipCount).Take(channels.TakeCount);

            channels.List = query.ToList();

            return channels;
        }

        public bool IsRegistered(string userId)
        {
            return _context.Users.Any(u => u.Id == userId);
        }


    }
}
