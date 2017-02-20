using BizwebSharp.Services.Authorization;

namespace BizwebSharp.Infrastructure
{
    public class BizwebValidationModel
    {
        public string Store { get; set; }
        public string Hmac { get; set; }
        public string Code { get; set; }
        public string Timestamp { get; set; }
        public string State { get; set; }
        public string Error { get; set; }

        public bool ValidateRequest(string sapoSecretKey, double? requestTimestampSpan = null)
        {
            var kvpInString = !string.IsNullOrEmpty(Code)
                ? $"code={Code}&store={Store}"
                : $"store={Store}";

            if (string.IsNullOrEmpty(Timestamp))
                kvpInString += string.Format("&timestamp={0}", Timestamp);

            return AuthorizationService.ValidateRequest(Hmac, kvpInString, sapoSecretKey, Timestamp,
                requestTimestampSpan);
        }
    }
}