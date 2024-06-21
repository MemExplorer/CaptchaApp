
namespace CaptchaSOAP.API.Entities
{
    public class CaptchaResponse
    {
        public ulong CaptchaId { get; set; }
        public byte[] CaptchaByteData { get; set; }
    }
}