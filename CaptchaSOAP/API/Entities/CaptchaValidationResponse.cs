
namespace CaptchaSOAP.API.Entities
{
    public class CaptchaValidationResponse
    {
        public bool Result { get; set; }

        public static CaptchaValidationResponse CreateResponse(bool r)
        {
            return new CaptchaValidationResponse { Result = r };
        }
    }
}