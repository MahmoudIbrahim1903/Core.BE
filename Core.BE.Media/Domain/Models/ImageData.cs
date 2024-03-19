using Emeint.Core.BE.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Media.Domain.Models
{
    public class ImageData : Entity
    {
        public ImageData()
        {

        }
        public ImageData(byte[] binaryData, string createdBy)
        {
            BinaryData = binaryData;
            CreatedBy = createdBy;
            CreationDate = DateTime.UtcNow;
        }
        public byte[] BinaryData { get; set; }
    }
}
