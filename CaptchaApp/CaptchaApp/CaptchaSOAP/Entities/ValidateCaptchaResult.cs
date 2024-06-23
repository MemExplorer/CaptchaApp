
using System.Xml.Serialization;

namespace CaptchaApp.CaptchaSOAP.Entities;

[XmlRoot(ElementName = "ValidateCaptchaResult", Namespace = "http://tempuri.org/")]
public class ValidateCaptchaResult
{
    [XmlElement(ElementName = "Result", Namespace = "http://tempuri.org/")]
    public bool Result { get; set; }
}
