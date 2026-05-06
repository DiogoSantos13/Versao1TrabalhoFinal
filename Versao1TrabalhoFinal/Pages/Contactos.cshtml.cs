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
                ErrorMessage = "Verifique os campos do formulário e tente novamente.";
                return Page();
            }

            try
            {
                var smtpServer = _configuration["EmailSettings:SmtpServer"];
                var port = int.Parse(_configuration["EmailSettings:Port"] ?? "587");
                var senderName = _configuration["EmailSettings:SenderName"];
                var senderEmail = _configuration["EmailSettings:SenderEmail"];
                var username = _configuration["EmailSettings:Username"];
                var password = _configuration["EmailSettings:Password"];
                var destinationEmail = _configuration["EmailSettings:DestinationEmail"];
                var useStartTls = bool.Parse(_configuration["EmailSettings:UseStartTls"] ?? "true");

                var email = new MimeMessage();

                email.From.Add(new MailboxAddress(senderName, senderEmail));
                email.To.Add(MailboxAddress.Parse(destinationEmail));
                email.ReplyTo.Add(MailboxAddress.Parse(Input.Email));
                email.Subject = $"Novo contacto: {Input.Assunto}";

                email.Body = new TextPart(MimeKit.Text.TextFormat.Plain)
                {
                    Text =
$@"Novo pedido de contacto

Nome: {Input.Nome} {Input.Apelido}
Email: {Input.Email}
Telefone: {Input.Telefone}
Assunto: {Input.Assunto}

Mensagem:
{Input.Mensagem}"
                };

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
            catch (Exception)
            {
                ErrorMessage = "Ocorreu um erro ao enviar a mensagem. Confirme a configuração do email e tente novamente.";
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