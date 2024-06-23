
using CaptchaApp.CaptchaSOAP.Entities;
using CaptchaApp.SOAP;

namespace CaptchaApp.CaptchaSOAP;

internal class CaptchaSoapClient
{
    private SOAPClient _client;
    public CaptchaSoapClient(SOAPClient client)
    {
        _client = client;
    }

    public async Task<GetCaptchaResult?> GetCaptchaAsync()
    {
        return await _client.Post<GetCaptchaResult>("GetCaptcha", "http://tempuri.org/");
    }

    public async Task<ValidateCaptchaResult?> ValidateCaptchaAsync(ulong id, string captchaCode)
    {
        return await _client.Post<ValidateCaptchaResult>("ValidateCaptcha", "http://tempuri.org/", new Dictionary<string, object>
        {
            {"id", id },
            {"captchaCode", captchaCode }
        });
    }

}
