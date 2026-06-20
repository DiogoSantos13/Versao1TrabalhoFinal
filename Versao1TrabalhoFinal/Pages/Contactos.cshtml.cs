using System.ComponentModel.DataAnnotations;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace Versao1TrabalhoFinal.Pages
{
    /// <summary>
    /// Página pública de contactos do Stand Premium.
    /// </summary>
    public class ContactosModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private const string AdminEmail = "dfpds2005@gmail.com";

        /// <summary>
        /// Inicializa a página de contactos.
        /// </summary>
        /// <param name="configuration">Configuração da aplicação.</param>
        public ContactosModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Dados submetidos pelo formulário de contacto.
        /// </summary>
        [BindProperty]
        public FormularioContacto Input { get; set; } = new();

        /// <summary>
        /// Mensagem temporária de sucesso apresentada após submissão válida.
        /// </summary>
        [TempData]
        public string? SuccessMessage { get; set; }

        /// <summary>
        /// Mensagem temporária de erro apresentada após falha na submissão.
        /// </summary>
        [TempData]
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// Carrega a página de contactos.
        /// </summary>
        public void OnGet()
        {
        }

        /// <summary>
        /// Processa a submissão do formulário de contacto e envia o conteúdo por email.
        /// </summary>
        /// <returns>Página atual em caso de erro; redirecionamento para GET em caso de sucesso.</returns>
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                var erros = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => string.IsNullOrWhiteSpace(e.ErrorMessage) ? e.Exception?.Message : e.ErrorMessage)
                    .Where(m => !string.IsNullOrWhiteSpace(m));

                ErrorMessage = "ModelState inválido: " + string.Join(" | ", erros);
                return Page();
            }

            try
            {
                var smtpServer = _configuration["EmailSettings:SmtpServer"];
                var senderName = _configuration["EmailSettings:SenderName"] ?? "Stand Premium";
                var senderEmail = _configuration["EmailSettings:SenderEmail"];
                var username = _configuration["EmailSettings:Username"];
                var password = _configuration["EmailSettings:Password"];

                if (string.IsNullOrWhiteSpace(smtpServer) ||
                    string.IsNullOrWhiteSpace(senderEmail) ||
                    string.IsNullOrWhiteSpace(username) ||
                    string.IsNullOrWhiteSpace(password))
                {
                    ErrorMessage = "A configuração de email está incompleta.";
                    return Page();
                }

                var portValue = _configuration["EmailSettings:Port"];
                if (!int.TryParse(portValue, out var port))
                {
                    port = 587;
                }

                var useStartTlsValue = _configuration["EmailSettings:UseStartTls"];
                if (!bool.TryParse(useStartTlsValue, out var useStartTls))
                {
                    useStartTls = true;
                }

                var email = new MimeMessage();

                email.From.Add(new MailboxAddress(senderName, senderEmail));
                email.To.Add(MailboxAddress.Parse(AdminEmail));
                email.ReplyTo.Add(new MailboxAddress($"{Input.Nome} {Input.Apelido}".Trim(), Input.Email));
                email.Subject = $"[Stand Premium] Novo pedido de contacto - {Input.Assunto}";

                var bodyBuilder = new BodyBuilder();

                bodyBuilder.TextBody = $@"Novo pedido de contacto

Nome: {Input.Nome} {Input.Apelido}
Email: {Input.Email}
Telefone: {Input.Telefone}
Assunto: {Input.Assunto}

Mensagem:
{Input.Mensagem}";

                bodyBuilder.HtmlBody = $@"
<!DOCTYPE html>
<html lang='pt'>
<head>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Novo pedido de contacto</title>
</head>
<body style='margin:0; padding:0; background-color:#f4f6f8; font-family:Arial, Helvetica, sans-serif; color:#1f2937;'>
    <div style='width:100%; background-color:#f4f6f8; padding:32px 16px;'>
        <table role='presentation' cellspacing='0' cellpadding='0' border='0' width='100%' style='max-width:680px; margin:0 auto; background:#ffffff; border-radius:16px; overflow:hidden; border:1px solid #e5e7eb;'>
            <tr>
                <td style='background:linear-gradient(135deg, #0f766e, #14b8a6); padding:28px 32px; color:#ffffff;'>
                    <div style='font-size:12px; letter-spacing:1px; text-transform:uppercase; opacity:0.9;'>Stand Premium</div>
                    <h1 style='margin:8px 0 0 0; font-size:26px; line-height:1.2;'>Novo pedido de contacto</h1>
                    <p style='margin:10px 0 0 0; font-size:14px; line-height:1.6; opacity:0.95;'>
                        Foi recebida uma nova mensagem através do formulário do site.
                    </p>
                </td>
            </tr>

            <tr>
                <td style='padding:32px;'>
                    <table role='presentation' cellspacing='0' cellpadding='0' border='0' width='100%' style='border-collapse:collapse;'>
                        <tr>
                            <td style='padding:0 0 18px 0;'>
                                <div style='font-size:12px; font-weight:bold; color:#6b7280; text-transform:uppercase; margin-bottom:6px;'>Nome</div>
                                <div style='font-size:16px; color:#111827;'>{System.Net.WebUtility.HtmlEncode(Input.Nome)} {System.Net.WebUtility.HtmlEncode(Input.Apelido)}</div>
                            </td>
                        </tr>
                        <tr>
                            <td style='padding:0 0 18px 0;'>
                                <div style='font-size:12px; font-weight:bold; color:#6b7280; text-transform:uppercase; margin-bottom:6px;'>Email</div>
                                <div style='font-size:16px; color:#111827;'>
                                    <a href='mailto:{System.Net.WebUtility.HtmlEncode(Input.Email)}' style='color:#0f766e; text-decoration:none;'>
                                        {System.Net.WebUtility.HtmlEncode(Input.Email)}
                                    </a>
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td style='padding:0 0 18px 0;'>
                                <div style='font-size:12px; font-weight:bold; color:#6b7280; text-transform:uppercase; margin-bottom:6px;'>Telefone</div>
                                <div style='font-size:16px; color:#111827;'>{System.Net.WebUtility.HtmlEncode(Input.Telefone ?? "Não indicado")}</div>
                            </td>
                        </tr>
                        <tr>
                            <td style='padding:0 0 18px 0;'>
                                <div style='font-size:12px; font-weight:bold; color:#6b7280; text-transform:uppercase; margin-bottom:6px;'>Assunto</div>
                                <div style='font-size:16px; color:#111827;'>{System.Net.WebUtility.HtmlEncode(Input.Assunto)}</div>
                            </td>
                        </tr>
                        <tr>
                            <td style='padding:8px 0 0 0;'>
                                <div style='font-size:12px; font-weight:bold; color:#6b7280; text-transform:uppercase; margin-bottom:8px;'>Mensagem</div>
                                <div style='background:#f9fafb; border:1px solid #e5e7eb; border-radius:12px; padding:18px; font-size:15px; line-height:1.7; color:#111827; white-space:pre-line;'>
                                    {System.Net.WebUtility.HtmlEncode(Input.Mensagem)}
                                </div>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>

            <tr>
                <td style='padding:20px 32px; background:#f9fafb; border-top:1px solid #e5e7eb; font-size:13px; color:#6b7280; line-height:1.6;'>
                    Este email foi enviado automaticamente a partir do formulário de contacto do site Stand Premium.
                </td>
            </tr>
        </table>
    </div>
</body>
</html>";

                email.Body = bodyBuilder.ToMessageBody();

                using var smtp = new SmtpClient();

                await smtp.ConnectAsync(
                    smtpServer,
                    port,
                    useStartTls ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto);

                await smtp.AuthenticateAsync(username, password);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);

                var nomeCompleto = $"{Input.Nome} {Input.Apelido}".Trim();
                SuccessMessage = $"Mensagem enviada com sucesso. Obrigado pelo contacto, {nomeCompleto}!";

                return RedirectToPage();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erro ao enviar email: {ex.Message}";
                return Page();
            }
        }

        /// <summary>
        /// Modelo do formulário de contacto.
        /// </summary>
        public class FormularioContacto
        {
            /// <summary>
            /// Primeiro nome do utilizador.
            /// </summary>
            [Required(ErrorMessage = "O nome é obrigatório.")]
            [StringLength(60, ErrorMessage = "O nome não pode ter mais de 60 caracteres.")]
            [Display(Name = "Nome")]
            public string Nome { get; set; } = string.Empty;

            /// <summary>
            /// Apelido do utilizador.
            /// </summary>
            [Required(ErrorMessage = "O apelido é obrigatório.")]
            [StringLength(80, ErrorMessage = "O apelido não pode ter mais de 80 caracteres.")]
            [Display(Name = "Apelido")]
            public string Apelido { get; set; } = string.Empty;

            /// <summary>
            /// Endereço de email do utilizador.
            /// </summary>
            [Required(ErrorMessage = "O email é obrigatório.")]
            [EmailAddress(ErrorMessage = "Introduza um endereço de email válido.")]
            [StringLength(150, ErrorMessage = "O email não pode ter mais de 150 caracteres.")]
            [Display(Name = "Email")]
            public string Email { get; set; } = string.Empty;

            /// <summary>
            /// Número de telefone do utilizador.
            /// </summary>
            [Phone(ErrorMessage = "Introduza um número de telefone válido.")]
            [StringLength(20, ErrorMessage = "O telefone não pode ter mais de 20 caracteres.")]
            [Display(Name = "Telefone")]
            public string? Telefone { get; set; }

            /// <summary>
            /// Assunto do contacto.
            /// </summary>
            [Required(ErrorMessage = "O assunto é obrigatório.")]
            [StringLength(120, ErrorMessage = "O assunto não pode ter mais de 120 caracteres.")]
            [Display(Name = "Assunto")]
            public string Assunto { get; set; } = string.Empty;

            /// <summary>
            /// Mensagem enviada pelo utilizador.
            /// </summary>
            [Required(ErrorMessage = "A mensagem é obrigatória.")]
            [StringLength(2000, MinimumLength = 10, ErrorMessage = "A mensagem deve ter entre 10 e 2000 caracteres.")]
            [Display(Name = "Mensagem")]
            public string Mensagem { get; set; } = string.Empty;
        }
    }
}