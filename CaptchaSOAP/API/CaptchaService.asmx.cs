using CaptchaSOAP.API.Entities;
using CaptchaSOAP.Captcha;
using System;
using System.Web.Services;

namespace CaptchaSOAP.API
{
    /// <summary>
    /// Summary description for CaptchaService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class CaptchaService : System.Web.Services.WebService
    {
        [WebMethod]
        public CaptchaResponse GetCaptcha()
        {
            int width = 1200;
            int height = 738;
            string captchaCode = CaptchaGenerator.GenerateCaptchaCode();
            using (var m = MoLeCuLeZDB.GetTransient())
            {
                CaptchaResult captchaData = CaptchaGenerator.GenerateCaptchaImage(width, height, captchaCode);
                var insertedId = m.ExecuteScalar<ulong>(
                    "INSERT INTO captcha_session_tbl VALUES (?, ?, ?, ?);" +
                    "SELECT LAST_INSERT_ID();", 
                    0, 
                    Convert.ToBase64String(captchaData.CaptchaBytes), 
                    captchaCode, 
                    DateTime.Now.AddMinutes(10)
                );

                
                return new CaptchaResponse()
                {
                    CaptchaId = insertedId,
                    CaptchaByteData = captchaData.CaptchaBytes
            };
            }
        }

        [WebMethod]
        public CaptchaValidationResponse ValidateCaptcha(ulong id, string captchaCode)
        {
            // simple format validation
            if (string.IsNullOrWhiteSpace(captchaCode) || captchaCode.Length == 0)
                return CaptchaValidationResponse.CreateResponse(false);

            using (var m = MoLeCuLeZDB.GetTransient())
            {
                // check if id exists and if captcha is still valid
                var hasId = m.ExecuteScalar<int>(
                    "SELECT EXISTS(SELECT 1 FROM captcha_session_tbl WHERE id = ? AND NOW() < expiration LIMIT 1) as result", 
                    id
                ) == 1;

                if (!hasId)
                    return CaptchaValidationResponse.CreateResponse(false);

                // validate input from user
                var captchaResult = m.ExecuteScalar<int>(
                    "SELECT EXISTS(SELECT 1 FROM captcha_session_tbl WHERE id = ? AND captcha_code = ? LIMIT 1) as result",
                    id,
                    captchaCode
                ) == 1;

                return CaptchaValidationResponse.CreateResponse(captchaResult);
            }
        }
    }
}
