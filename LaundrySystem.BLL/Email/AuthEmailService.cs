//namespace LaundrySystem.BLL.Email
//{
//    using Microsoft.AspNetCore.Identity;
//    using System.Threading.Tasks;

//    /// <summary>
//    /// Service for sending authentication-related emails.
//    /// </summary>
//    public class AuthEmailService : IEmailSender<AppUser>
//    {
//        private readonly IEmailSender<AppUser> _emailSender;

//        /// <summary>
//        /// Initializes a new instance of the <see cref="AuthEmailService"/> class.
//        /// </summary>
//        /// <param name="emailSender">The email sender service.</param>
//        public AuthEmailService(IEmailSender<AppUser> emailSender)
//        {
//            _emailSender = emailSender;
//        }

//        /// <summary>
//        /// Sends a confirmation link to the specified email.
//        /// </summary>
//        /// <param name="user">The user to whom the email is sent.</param>
//        /// <param name="email">The email address to send the confirmation link to.</param>
//        /// <param name="confirmationLink">The confirmation link to be sent.</param>
//        /// <returns>A task that represents the asynchronous operation.</returns>
//        public async Task SendConfirmationLinkAsync(AppUser user, string email, string confirmationLink)
//        {
//            var subject = "Confirm your email";
//            var message = $"Please confirm your email by clicking this link: {confirmationLink}";

//            await _emailSender.SendConfirmationLinkAsync(user, email, confirmationLink);
//        }

//        /// <summary>
//        /// Sends a password reset link to the specified email.
//        /// </summary>
//        /// <param name="user">The user to whom the email is sent.</param>
//        /// <param name="email">The email address to send the reset link to.</param>
//        /// <param name="resetLink">The password reset link to be sent.</param>
//        /// <returns>A task that represents the asynchronous operation.</returns>
//        public async Task SendPasswordResetLinkAsync(AppUser user, string email, string resetLink)
//        {
//            var subject = "Reset your password";
//            var message = $"You can reset your password by clicking this link: {resetLink}";

//            await _emailSender.SendPasswordResetLinkAsync(user, email, resetLink);
//        }

//        /// <summary>
//        /// Sends a password reset code to the specified email.
//        /// </summary>
//        /// <param name="user">The user to whom the email is sent.</param>
//        /// <param name="email">The email address to send the reset code to.</param>
//        /// <param name="resetCode">The password reset code to be sent.</param>
//        /// <returns>A task that represents the asynchronous operation.</returns>
//        public async Task SendPasswordResetCodeAsync(AppUser user, string email, string resetCode)
//        {
//            var subject = "Password Reset Code";
//            var message = $"Your password reset code is: {resetCode}";

//            await _emailSender.SendPasswordResetCodeAsync(user, email, resetCode);
//        }
//    }
//}