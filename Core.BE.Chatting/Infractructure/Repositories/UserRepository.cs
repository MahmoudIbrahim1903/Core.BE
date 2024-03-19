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
    public class UserRepository : BaseRepository<User, ChattingDbContext>, IUserRepository
    {
        public UserRepository(ChattingDbContext chattingDbContext) : base(chattingDbContext)
        {
        }


        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public bool IsRegistered(string userId)
        {
            return _context.Users.Any(u => u.Id == userId);
        }

        public void RegisterUser(string id, string name, string type, string imageUrl)
        {
            var userExist = _context.Users.Any(u => u.Id == id);
            if (!userExist)
            {
                _context.Users.Add(new User
                {
                    Id = id,
                    Name = name,
                    Type = type,
                    ImageUrl = imageUrl
                });
                SaveChanges();
            }
        }

        public void UpdateRegisteredUser(string userId, string name, string type, string imageUrl)
        {
            var existingUser = _context.Users.FirstOrDefault(u => u.Id == userId);

            bool saveChanges = false;

            if (!string.IsNullOrEmpty(imageUrl) && existingUser.ImageUrl != imageUrl)
            {
                saveChanges = true;
                existingUser.ImageUrl = imageUrl;
            }

            if (!string.IsNullOrEmpty(type) && existingUser.Type != type)
            {
                saveChanges = true;
                existingUser.Type = type;
            }

            if (!string.IsNullOrEmpty(name) && existingUser.Name.Trim() != name.Trim())
            {
                saveChanges = true;
                existingUser.Name = name;
            }

            if (saveChanges)
                SaveChanges();
        }



        public List<User> GetUsers()
        {
            return _context.Users.AsNoTracking().ToList();
        }
    }
}
