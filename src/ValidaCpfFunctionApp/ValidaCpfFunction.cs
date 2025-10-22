using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Text.Json;
using ValidaCpfFunctionApp.Models;
using ValidaCpfFunctionApp.Services;

namespace ValidaCpfFunctionApp
{
    public class ValidaCpfFunction
    {
        private readonly CpfValidatorService _cpfValidator;
        private readonly ILogger _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        public ValidaCpfFunction(CpfValidatorService cpfValidator, ILoggerFactory loggerFactory,
            IOptions<JsonSerializerOptions> jsonOptions)
        {
            _cpfValidator = cpfValidator;
            _logger = loggerFactory.CreateLogger<ValidaCpfFunction>();
            _jsonOptions = jsonOptions.Value;
        }

        [Function("ValidaCpf")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
        {
            _logger.LogInformation("Iniciando validação de CPF...");

            var body = await JsonSerializer.DeserializeAsync<CpfRequest>(req.Body, _jsonOptions);
            var response = req.CreateResponse();

            if (body == null || string.IsNullOrWhiteSpace(body.Cpf))
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                await response.WriteAsJsonAsync(new { mensagem = "CPF não informado." });
                return response;
            }

            bool valido = _cpfValidator.Validar(body.Cpf);

            var result = new CpfResponse
            {
                Cpf = body.Cpf,
                Valido = valido,
                Mensagem = valido ? "CPF válido." : "CPF inválido."
            };

            response.StatusCode = HttpStatusCode.OK;
            await response.WriteAsJsonAsync(result);

            _logger.LogInformation($"Validação concluída para CPF: {body.Cpf} - {result.Mensagem}");
            return response;
        }
    }
}
