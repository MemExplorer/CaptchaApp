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
        // captcha resolution
        const int CAPTCHA_WIDTH = 1200;
        const int CAPTCHA_HEIGHT = 738;

        [WebMethod]
        public CaptchaResponse GetCaptcha()
        {
            // generate random code for captcha
            string captchaCode = CaptchaGenerator.GenerateCaptchaCode();

            // get new instance of database
            using (var m = MoLeCuLeZDB.GetTransient())
            {
                // generate captcha info
                CaptchaResult captchaData = CaptchaGenerator.GenerateCaptchaImage(CAPTCHA_WIDTH, CAPTCHA_HEIGHT, captchaCode);

                // insert captcha info into database and return the id of the inserted info
                var insertedId = m.ExecuteScalar<ulong>(
                    "INSERT INTO captcha_session_tbl VALUES (?, ?, ?, ?);" +
                    "SELECT LAST_INSERT_ID();", 
                    0, 
                    Convert.ToBase64String(captchaData.CaptchaBytes), 
                    captchaCode, 
                    DateTime.Now.AddMinutes(10)
                );

                // create captcha response
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

            // get new instance of database
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

                // create captcha validation response
                return CaptchaValidationResponse.CreateResponse(captchaResult);
            }
        }
    }
}
