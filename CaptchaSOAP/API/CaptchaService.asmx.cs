using CaptchaSOAP.Captcha;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
        public CaptchaResult GetCaptcha()
        {
            int width = 224;
            int height = 80;
            string c = CaptchaGenerator.GenerateCaptchaCode();
            CaptchaResult ret = CaptchaGenerator.GenerateCaptchaImage(width, height, c);
            return ret;
        }
    }
}
