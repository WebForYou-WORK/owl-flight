using System;
using System.Net;
using System.Net.Mail;
using System.Text;
using Domain.Abstrac;
using Domain.Entityes;

namespace Domain.Concrete
{
    /// <summary>
    /// Класс для отправки писем на почту
    /// </summary>
    public class EmailSending : IEmailSending
    {
        /// <summary>
        /// Отправка письма администратору
        /// </summary>
        public void SendMailToAdministrator(Basket basket, Order details, string mailFrom, string mailPasword, string hostName, int port, bool enableSsl, string mailAdmin, string attachFile = null)
        {
            try
            {
                //c какой почты отправляем письмо 
                MailMessage mail = new MailMessage { From = new MailAddress(mailFrom) };
                mail.To.Add(new MailAddress(mailAdmin)); // E-mail Администратора
                //("owlflight17@gmail.com")); 
                mail.Subject = "Нове замовлення";
                mail.Body = EmailMessageToAdministrator(basket, details);
                if (!string.IsNullOrEmpty(attachFile))
                    mail.Attachments.Add(new Attachment(attachFile));
                SmtpClient client = new SmtpClient
                {
                    Host = hostName,
                    Port = port,
                    EnableSsl = enableSsl,
                    //c какой почты отправляем письмо + пароль от этой почты
                    Credentials = new NetworkCredential(mailFrom, mailPasword),
                    DeliveryMethod = SmtpDeliveryMethod.Network
                };
                client.Send(mail);
                mail.Dispose();
            }
            catch
            {
                // ignored
            }
        }
        /// <summary>
        /// ОТПРАВКА ПИСЬМА ПОЛЬЗОВАТЕЛЮ
        /// </summary>
        public void SendMail(Basket basket, Order details, string mailFrom, string mailPasword, string hostName, int port, bool enableSsl, string attachFile = null)
        {
            try
            {
                //c какой почты отправляем письмо
                MailMessage mail = new MailMessage { From = new MailAddress(mailFrom) };
                mail.To.Add(new MailAddress(details.Email)); //получаем E-mail который ввел пользователь
                mail.Subject = "owl-flight.com"; // тема письма
                mail.Body = EmailMessage(basket, details);
                if (!string.IsNullOrEmpty(attachFile))
                    mail.Attachments.Add(new Attachment(attachFile));
                SmtpClient client = new SmtpClient
                {
                    Host = hostName,
                    Port = port,
                    EnableSsl = enableSsl,
                    //c какой почты отправляем письмо + пароль от этой почты
                    Credentials = new NetworkCredential(mailFrom, mailPasword),
                    DeliveryMethod = SmtpDeliveryMethod.Network
                };
                client.Send(mail);
                mail.Dispose();
                basket.AnswerList.Add($"Шановний(а), {details.ClientName}! ");
                basket.AnswerList.Add("Дякуємо Вам за покупку!");
                basket.AnswerList.Add($"На Ваш E-mail ({details.Email}) вислані всі деталі замовлення.");
                basket.AnswerList.Add("Замовлення відправлений в обробку, скоро ми з вами зв'яжемося, гарного вам дня!");
            }
            catch (Exception e)
            {
                //basket.AnswerList.Clear();
                //basket.AnswerList.Add($"{e} - {e.Message} - ресурс :{e.Source} - таргет Сайт:{e.TargetSite}");
                basket.AnswerList.Add($"Щось пішло не так, ми не змогли відправити лист на Ваш E-mail ({details.Email}) ");
                basket.AnswerList.Add("");
                basket.AnswerList.Add("Просимо вибачення, за технічні проблеми.");
                basket.AnswerList.Add("");
                basket.AnswerList.Add("Ваше замовлення передано адміністратору, скоро ми з вами зв'яжемося, гарного вам дня!");
            }
        }

        /// <summary>
        /// формирование текста письма для пользователя
        /// </summary>
        public static string EmailMessage(Basket basket, Order details)
        {
            StringBuilder str = new StringBuilder();
            str.AppendLine("");
            str.AppendFormat($"Доброго дня, шановний(а) {details.ClientName}!");
            str.AppendLine("");
            str.AppendLine("Дякуемо за покупку на сайті - owl-flight.com !");
            str.AppendLine("");
            str.AppendLine("");
            str.AppendLine("Детали Вашого заказу:");
            str.AppendLine("");
            foreach (var i in basket.Lines)
            {
                str.AppendFormat($" Футболка  -  \"{i.Name}\"  вартістью - {i.Price} грн. Розмір - {i.SelectedSize}");
                str.AppendLine("");
            }
            str.AppendLine("");
            str.AppendFormat($"Загальна сума замовлення - {basket.ComputeTotalValue()} грн.");
            str.AppendLine("");
            str.AppendLine("");

            str.AppendFormat($"обрана Вами доставка - {details.Delivery}");
            str.AppendLine("");
            str.AppendFormat($"обрана Вами форма оплаты - {details.Payment}");
            str.AppendLine("");
            if (!string.IsNullOrEmpty(details.Address))
            {
                str.AppendLine($"Адреса доставки - {details.Address}");
                str.AppendLine("");
            }
            if (!string.IsNullOrEmpty(details.Сomment))
            {
                str.AppendLine($"Ваш коментар - {details.Сomment}");
                str.AppendLine("");
            }
            str.AppendLine("");
            str.AppendFormat("Дата заказу -  {0:U}", DateTime.Now);
            str.AppendLine("");
            str.AppendLine("Замовлення відправлений в обробку, скоро ми з вами зв'яжемося, гарного вам дня!");

            return str.ToString();
        }
        /// <summary>
        /// метод для формирования тела письма Администратору
        /// </summary>
        public static string EmailMessageToAdministrator(Basket basket, Order details)
        {
            StringBuilder str = new StringBuilder();
            str.AppendLine("");
            str.AppendLine("Доброго дня!");
            str.AppendLine("Ви отримали новий заказ з сайту - owl-flight.com!");
            str.AppendLine("");
            str.AppendLine("");
            str.AppendLine("Замовлені товари:");
            str.AppendLine("");
            foreach (var i in basket.Lines)
            {
                str.AppendFormat($"Футболка  -  \"{i.Name}\"  вартістью - {i.Price} грн. Розмір - {i.SelectedSize}");
                str.AppendLine("");
            }
            str.AppendLine("");
            str.AppendLine("");
            str.AppendLine("Подробиці замовлення:");
            str.AppendFormat($"Им'я покупця - {details.ClientName}");
            str.AppendLine("");
            str.AppendFormat($"E-mail покупця - {details.Email}");
            str.AppendLine("");
            str.AppendFormat($"Телефон покупця - {details.Phone}");
            str.AppendLine("");
            str.AppendFormat($"Доставка - {details.Delivery}");
            str.AppendLine("");
            str.AppendFormat($"Оплата - {details.Payment}");
            str.AppendLine("");
            if (!string.IsNullOrEmpty(details.Address))
            {
                str.AppendLine($"Адреса - {details.Address}");
                str.AppendLine("");
            }
            if (!string.IsNullOrEmpty(details.Сomment))
            {
                str.AppendLine($"Коментар до заказу - {details.Сomment}");
                str.AppendLine("");
            }
            str.AppendLine("");
            str.AppendLine("");
            str.AppendFormat("Дата заказу -  {0:U}", DateTime.Now);
            return str.ToString();
        }
        }
}
