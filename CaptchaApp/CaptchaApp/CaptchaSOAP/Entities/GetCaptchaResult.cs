
using System.Xml.Serialization;

namespace CaptchaApp.CaptchaSOAP.Entities;

[XmlRoot(ElementName = "GetCaptchaResult", Namespace = "http://tempuri.org/")]
public class GetCaptchaResult
{
    [XmlElement(ElementName = "CaptchaId", Namespace = "http://tempuri.org/")]
    public ulong CaptchaId { get; set; }
    [XmlElement(ElementName = "CaptchaByteData", Namespace = "http://tempuri.org/")]
    public string CaptchaByteData { get; set; }
}
