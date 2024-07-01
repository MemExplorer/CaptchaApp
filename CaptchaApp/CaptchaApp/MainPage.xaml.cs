
using CaptchaApp.CaptchaSOAP;
using CaptchaApp.SOAP;
using System.Diagnostics;

namespace CaptchaApp;

public partial class MainPage : ContentPage
{
    // change host according to your local ip
    private static string HOST = "http://192.168.157.112:8081";
    private static string CAPTCHA_ENDPOINT = HOST + "/API/CaptchaService.asmx";
    private CaptchaRepository _client;
    private Image _captchaImageElement;
    private Entry _captchaInput;
    private ulong? CaptchaId { get; set; } = null;
    public MainPage()
    {
        InitializeComponent();
        var soapClient = new SOAPClient(CAPTCHA_ENDPOINT);
        _client = new CaptchaRepository(soapClient);
        _captchaImageElement = (Image)FindByName("captchaImg");
        _captchaInput = (Entry)FindByName("captchaInput");
    }

    private async Task RefreshCaptcha()
    {
        var captchaRequest = await _client.GetCaptchaAsync();
        if (captchaRequest != null)
        {
            byte[] imageData = Convert.FromBase64String(captchaRequest.CaptchaByteData);
            CaptchaId = captchaRequest.CaptchaId;
            var stream = new MemoryStream(imageData);
            _captchaImageElement.Source = ImageSource.FromStream(() => stream);
        }
        else
        {
            Page? pageCtx = Application.Current?.MainPage;
            pageCtx?.DisplayAlert("Error", "Failed to fetch captcha!", "OK");
        }
    }

    private async void SubmitBtn_Clicked(object sender, EventArgs e)
    {
        if (CaptchaId == null)
            return;

        Page? pageCtx = Application.Current?.MainPage;
        var validateCaptcha = await _client.ValidateCaptchaAsync(CaptchaId.Value, _captchaInput.Text);
        if (validateCaptcha != null)
        {
            if (validateCaptcha.Result)
            {
                pageCtx?.DisplayAlert("Success!", "Welcome User!", "OK");
            }
            else
            {
                pageCtx?.DisplayAlert("Failed", "Incorrect Code", "OK");
            }
        }
        else
        {
            pageCtx?.DisplayAlert("Error", "Failed to fetch captcha!", "OK");
        }
    }

    private async void ContentPage_Loaded(object sender, EventArgs e)
    {
        if (CaptchaId != null)
            return;

        await RefreshCaptcha();
    }

    private async void RefreshBtn_Clicked(object sender, EventArgs e)
    {
        await RefreshCaptcha();
    }
}
