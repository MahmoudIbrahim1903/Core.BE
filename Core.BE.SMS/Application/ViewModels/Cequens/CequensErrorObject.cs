using System.Collections.Generic;

namespace Emeint.Core.BE.SMS.Application.ViewModels.Cequens
{
    public class CequensErrorObject
    {
        public int Status { get; set; }
        public string Description { get; set; }
        public List<CequensInternalErrorObject> InternalErrors{ get; set; }
    }
}
