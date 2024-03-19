using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Infrastructure.Idempotency
{
    public class CommandManager<TContext> : ICommandManager where TContext : DbContext
    {
        private readonly TContext _context;

        public CommandManager(TContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }


        public async Task<bool> ExistAsync(Guid id)
        {
            var request = await _context.
                FindAsync<ClientCommand>(id);

            return request != null;
        }

        public async Task CreateRequestForCommandAsync<T>(Guid id, string command)
        {
            var exists = await ExistAsync(id);

            var request = exists ?
                throw new Exception($"Command Request with {id} already exists") :
                new ClientCommand()
                {
                    RequestClientReferenceNumber = id,
                    Name = typeof(T).Name,
                    Command = command,
                    Time = DateTime.UtcNow
                };

            _context.Add(request);

            await _context.SaveChangesAsync();
        }
    }
}
