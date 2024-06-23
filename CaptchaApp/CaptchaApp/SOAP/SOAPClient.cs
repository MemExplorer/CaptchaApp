
using CaptchaApp.SOAP.Entities;
using System;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace CaptchaApp.SOAP;

internal class SOAPClient
{
    private HttpClient _client;
    public SOAPClient(string endpoint)
    {
        _client = new HttpClient();
        _client.BaseAddress = new Uri(endpoint);
    }

    private string CraftMessage(Dictionary<string, object>? soapArgs, string action, string @namespace)
    {
        var sb = new StringBuilder();
        if (soapArgs != null)
        {
            foreach (var k in soapArgs)
            {
                sb.Append('<');
                sb.Append(k.Key);
                sb.Append('>');
                sb.Append(k.Value);
                sb.Append('<');
                sb.Append('/');
                sb.Append(k.Key);
                sb.Append('>');
                sb.AppendLine();
            }
        }

        string message = $"""
            <?xml version="1.0" encoding="utf-8"?>
            <soap:Envelope xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/">
              <soap:Body>
                <{action} xmlns="{@namespace}">
                {sb}
                </{action}>
              </soap:Body>
            </soap:Envelope>
            """;

        return message;
    }

    private ResponseType? ExtractResponse<ResponseType>(string s, string action)
    {
        var xDoc = XDocument.Parse(s);
        var res = xDoc.Root!.Descendants().FirstOrDefault(d => d.Name.LocalName.Equals(action + "Result"));
        var serializer = new XmlSerializer(typeof(ResponseType));
        using (var reader = res!.CreateReader())
        {
            var deserializedResult = serializer.Deserialize(reader);
            if (deserializedResult != null)
                return (ResponseType)deserializedResult;
        }

        return default(ResponseType);
    }

    public async Task<ResponseType?> Post<ResponseType>(string action, string @namespace, Dictionary<string, object>? body = null)
    {
        try
        {
            var message = CraftMessage(body, action, @namespace);
            HttpRequestMessage webRequest = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                Content = new StringContent(message)
            };
            webRequest.Headers.Add("SOAPAction", @namespace + action);
            webRequest.Headers.Host = "localhost"; //_client.BaseAddress!.Host;
            webRequest.Content.Headers.ContentType!.MediaType = "text/xml";
            var result = await _client.SendAsync(webRequest);
            string strResponse = await result.Content.ReadAsStringAsync();
            return ExtractResponse<ResponseType>(strResponse, action);
        }
        catch (Exception e)
        {

        }

        return default(ResponseType);
    }
}
