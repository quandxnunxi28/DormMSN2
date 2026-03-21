using MailKit.Net.Smtp;
using MimeKit;


namespace DormMS.Service
{
    public class EmailService
    {
        public void SendOtp(string toEmail, string otp)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("DormMSN", "quandxhe181250@fpt.edu.vn"));
            message.To.Add(new MailboxAddress("", toEmail));

            message.Subject = "OTP xác thực tài khoản";

            message.Body = new TextPart("plain")
            {
                Text = $"Mã OTP của bạn là: {otp}"
            };

            using (var client = new SmtpClient())
            {
                client.Connect("smtp.gmail.com", 587, false);

                client.Authenticate("quandxhe181250@fpt.edu.vn", "hkdb xfsj wtqu halx");

                client.Send(message);
                client.Disconnect(true);
            }
        }
        //public async Task SendQR(string email, byte[] qrImage)
        //{
        //    var message = new MimeMessage();
        //    message.From.Add(new MailboxAddress("CuddleBear", "quandxhe181250@fpt.edu.vn"));
        //    message.To.Add(new MailboxAddress("", email));
        //    message.Subject = "You received a QR Message";

        //    var builder = new BodyBuilder();
        //    builder.HtmlBody = "<h3>You received a QR message</h3><img src='cid:qrcode'/>";

        //    builder.LinkedResources.Add("qrcode.png", qrImage)
        //        .ContentId = "qrcode";

        //    message.Body = builder.ToMessageBody();

        //    using (var client = new MailKit.Net.Smtp.SmtpClient())
        //    {
        //        await client.ConnectAsync("smtp.gmail.com", 587, false);
        //        await client.AuthenticateAsync("quandxhe181250@fpt.edu.vn", "hkdb xfsj wtqu halx");

        //        await client.SendAsync(message);
        //        await client.DisconnectAsync(true);
        //    }
        //}
    }
}
