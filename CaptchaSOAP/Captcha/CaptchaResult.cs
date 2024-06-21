using System;

namespace CaptchaSOAP.Captcha
{
    public class CaptchaResult
    {
        public string CaptchaCode { get; set; }
        public byte[] CaptchaBytes { get; set; }
        public DateTime Expiration { get; set; }
    }
}