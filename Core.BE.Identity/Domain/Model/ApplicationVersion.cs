using Emeint.Core.BE.Domain.SeedWork;
using Emeint.Core.BE.Identity.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Identity.Domain.Model
{
    public class ApplicationVersion : Entity
    {
        [Required]
        public Enums.Platform Platform { get; set; }
        [Required]
        public string Version { get; set; }
        public bool IsDiscontinued { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string ReleasedBy { get; set; }
        public bool IsBeta { get; set; }
        public string StoreUrl { get; set; }
        public bool IsDeleted { get; set; }
        //public bool ForceUpdated { get; set; }

        public ApplicationVersion(Domain.Enums.Platform platform, string version, bool isDiscontinued, DateTime releaseDate, string releasedBy, bool isBeta, string storeUrl, string createdBy)
        {
            Code = $"V-{DateTime.UtcNow.ToString("yyyyMMddHHmmss")}{new Random().Next(1, 100000)}";
            CreatedBy = createdBy;
            CreationDate = DateTime.UtcNow;
            Platform = platform;
            Version = version;
            IsDiscontinued = isDiscontinued;
            ReleaseDate = releaseDate;
            ReleasedBy = releasedBy;
            IsBeta = isBeta;
            StoreUrl = storeUrl;
        }

        public ApplicationVersion()
        {
        }
    }
}
