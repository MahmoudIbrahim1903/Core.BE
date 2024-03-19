namespace Emeint.Core.BE.SMS.Application.ViewModels.Cequens
{
    public class CequensSendSmsResponseVm
    {
        public int ReplyCode { get; set; }
        public string ReplyMessage { get; set; }
        public string RequestId { get; set; }
        public string ClientRequestId { get; set; }
        public string RequestTime { get; set; }
        public CequensDataObject Data { get; set; }
        public CequensErrorObject Error { get; set; }
    }
}
