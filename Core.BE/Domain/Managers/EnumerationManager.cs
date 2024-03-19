using Emeint.Core.BE.Domain.SeedWork;
using System.Collections.Generic;
using System.Linq;


namespace Emeint.Core.BE.Domain.Managers
{
    public class EnumerationManager<TEnumeration> : IEnumerationManager<TEnumeration>
        where TEnumeration : Enumeration
    {
        private readonly IEnumerationRepository<TEnumeration> _lookupsRepository;
        public EnumerationManager(IEnumerationRepository<TEnumeration> lookupsRepository)
        {
            _lookupsRepository = lookupsRepository;
        }

        public List<TEnumeration> Get()
        {
            return _lookupsRepository.GetAll().ToList();
        }

        public TEnumeration Get(string code)
        {
            return _lookupsRepository.GetAll().FirstOrDefault(e => e.Code == code);
        }
    }
}
