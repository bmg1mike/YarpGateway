using Microsoft.Extensions.Options;
using Sterling.Gateway.Application;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Sterling.Gateway.Api;

public class AesEncryptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly AESSettings _aesSettings;
    private readonly byte[] _key;
    private readonly byte[] _iv;
    private readonly ILogger<AesEncryptionMiddleware> logger;

    public AesEncryptionMiddleware(RequestDelegate next, IOptions<AESSettings> aesSettings, ILogger<AesEncryptionMiddleware> logger)
    {
        _next = next;
        _aesSettings = aesSettings.Value;
        _key = Encoding.UTF8.GetBytes(_aesSettings.Key);
        _iv = Encoding.UTF8.GetBytes(_aesSettings.IV);
        this.logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            if (!context.Request.Headers.ContainsKey("Accept-Encoding"))
            {
                context.Request.Headers.Add("Accept-Encoding", "*");
            }
            else
            {
                context.Request.Headers["Accept-Encoding"] = "*";
            }

            if (context.Request.Method == HttpMethods.Post || context.Request.Method == HttpMethods.Patch || context.Request.Method == HttpMethods.Put)
            {
                // Decrypt the request body
                context.Request.EnableBuffering();
                var requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();
                context.Request.Body.Position = 0;

                if (!string.IsNullOrWhiteSpace(requestBody))
                {
                    var requestJson = JsonDocument.Parse(requestBody);
                    if (requestJson.RootElement.TryGetProperty("request", out JsonElement dataElement))
                    {
                        var encryptedData = dataElement.GetString();
                        var decryptedRequest = Decrypt(encryptedData);
                        var newBody = new MemoryStream(Encoding.UTF8.GetBytes(decryptedRequest));
                        context.Request.Body = newBody;
                        context.Request.ContentLength = newBody.Length;
                    }
                    else
                    {
                        logger.LogError(requestBody);
                        throw new Exception("int)HttpStatusCode.BadRequest");
                    }
                }
            }

            // Capture the response
            var originalResponseBodyStream = context.Response.Body;
            using (var responseBody = new MemoryStream())
            {
                context.Response.Body = responseBody;

                await _next(context);

                context.Response.Body.Seek(0, SeekOrigin.Begin);
                var responseBodyText = await new StreamReader(context.Response.Body).ReadToEndAsync();
                context.Response.Body.Seek(0, SeekOrigin.Begin);

                // Encrypt the response body
                var encryptedResponse = Encrypt(responseBodyText);
                var jsonResponse = JsonSerializer.Serialize(new { response = encryptedResponse });
                var encryptedResponseBytes = Encoding.UTF8.GetBytes(jsonResponse);
                context.Response.Body = originalResponseBodyStream;
                context.Response.ContentLength = encryptedResponseBytes.Length;
                context.Response.ContentType = "application/json";
                await context.Response.Body.WriteAsync(encryptedResponseBytes, 0, encryptedResponseBytes.Length);
            }
        }
        catch (Exception ex)
        {
            if (ex.InnerException != null)
            {
                logger.LogError("{ExceptionType} {ExceptionMessage}", ex.InnerException.GetType().ToString(), ex.InnerException.Message);
            }
            else
            {
                logger.LogError("{ExceptionType} {ExceptionMessage}", ex.GetType().ToString(), ex.Message);
            }
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            await context.Response.WriteAsync("An Error occurred, please try again later.");
        }
    }

    private string Encrypt(string plainText)
    {
        using (var aes = Aes.Create())
        {
            aes.Key = _key;
            aes.IV = _iv;

            using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                using (var sw = new StreamWriter(cs))
                {
                    sw.Write(plainText);
                }
                return Convert.ToBase64String(ms.ToArray());
            }
        }
    }

    private string Decrypt(string cipherText)
    {
        using (var aes = Aes.Create())
        {
            aes.Key = _key;
            aes.IV = _iv;

            using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
            using (var ms = new MemoryStream(Convert.FromBase64String(cipherText)))
            using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
            using (var sr = new StreamReader(cs))
            {
                return sr.ReadToEnd();
            }
        }
    }
}
