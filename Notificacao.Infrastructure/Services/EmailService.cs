using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Notificacao.Domain;
using Notificacao.Domain.Interface;
using Notificacao.Domain.Model;
using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;


namespace Notificacao.ConsumerEmail
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly IParametroRepository _parametroRepository;
        private readonly IConfiguration _configuration;
        private readonly IHostingEnvironment _hostingEnvironment;

        public EmailService(ILogger<EmailService> logger, IParametroRepository parametroRepository,
            IConfiguration configuration,
            IHostingEnvironment hostingEnvironment)
        {
            _logger = logger;
            _parametroRepository = parametroRepository;
            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
        }

        public void EnviarEmail(EmailScope request)
        {
            var emailInvalido = false;
            foreach(var item in request.To)
            {
                if(!string.IsNullOrEmpty(item) && !item.Contains("@"))
                {
                    _logger.LogDebug($"--Mensagem com email invalido - Assunto:{request.Subject}");
                    emailInvalido = true;
                }
            }

            if (!emailInvalido)
            {
                _logger.LogDebug($"--Mongo DB - Buscar parâmetros SMTP ");

                var parametro = _parametroRepository
                                 .GetParametroAsync("Email");

                if (parametro != null)
                {
                    var param = parametro.Result.ValorObj;
                    if (param != null)
                    {
                        var parametros = JsonConvert.DeserializeObject<ParametroEmailDto>(param.ToString());
                        try
                        {
                            var client = new SmtpClient(parametros.Host);

                            if (!string.IsNullOrEmpty(parametros.Porta))
                            {
                                client.Port = Convert.ToInt32(parametros.Porta);
                            }

                            if (Convert.ToBoolean(parametros.SSL))
                            {
                                client.Credentials = new NetworkCredential(parametros.Usuario, parametros.Senha);
                            }

                            MailMessage message = new MailMessage();
                            message.From = new MailAddress(parametros.EmailRemetente, parametros.NomeRemetente);
                            message.IsBodyHtml = true;

                            foreach (var to in request.To)
                                message.To.Add(to);
                            
                            message.Subject = request.Subject;

                            //Busca template html                             
                            string _path = _hostingEnvironment.ContentRootPath;
                            string emailHtml = _path + @"/email.html";
                            _logger.LogDebug($"--Busca template HTML {emailHtml}");

                            StreamReader reader = new StreamReader(emailHtml);
                            string EMAIL_TEMPLATE = reader.ReadToEnd();
                            EMAIL_TEMPLATE = EMAIL_TEMPLATE.Replace("%Assunto%", request.Subject)
                                                           .Replace("%Mensagem%", request.Body);
            
                            //Inclui as imagens como anexo
                            string img_logo = _path + @"/irisLogoemail.png";
                            img_logo = img_logo.Replace("~//", "");

                            //inclui logo na mensagem
                            message.AlternateViews.Add(getEmbeddedImage(img_logo, EMAIL_TEMPLATE));

                            _logger.LogDebug($"--Preparando Envio: {JsonConvert.SerializeObject(request.To, Formatting.Indented)}");
                            client.Send(message);
                            message.Dispose();

                            _logger.LogInformation($"--Email Enviado: {message.To.ToString()}");
                            _logger.LogDebug($"--Parametros Envio: {param.ToString()}");
                        }
                        catch (Exception ex)
                        {
                            _logger.LogDebug($"--Erro \nMensagem: {ex.Message}");
                            _logger.LogDebug($"--Parametros Envio: {param.ToString()}");

                        }
                    }
                }
            }
        }

        private AlternateView getEmbeddedImage(string filePath, string htmlBody)
        {
            LinkedResource res = new LinkedResource(filePath);
            res.ContentId = Guid.NewGuid().ToString();
            htmlBody = htmlBody.Replace("%CID%", res.ContentId );
            AlternateView alternateView = AlternateView.CreateAlternateViewFromString(htmlBody, null, MediaTypeNames.Text.Html);
            alternateView.LinkedResources.Add(res);
            return alternateView;
        }
    }
}
