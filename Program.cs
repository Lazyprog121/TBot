using System;
using System.Threading;
using System.Text;
using System.Text.RegularExpressions;
using System.Net.Http;
using System.Net.Http.Headers;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Newtonsoft.Json;

namespace Bot
{
    public class Program
    {
        static TelegramBotClient bot = new TelegramBotClient("1289327932:AAH-wW6txUMzhDpEKpSbwUnqLmhpUYmSNac");

        public static void Main(string[] args)
        {
            bot.OnMessage += Bot_OnMessage;
            bot.StartReceiving();
            Thread.Sleep(150);
            Console.ReadLine();
        }

        private static async void Bot_OnMessage(object sender, MessageEventArgs e)
        {
            Message message = e.Message;

            if (message == null || message.Type != MessageType.Text)
            {
                return;
            }

            string patternMac = @"(/mac){1}\s{1}\w{2}\:\w{2}\:\w{2}\:\w{2}\:\w{2}\:\w{2}";
            string patternIp = @"(/ip){1}\s{1}\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}";
            string patternGmail = @"(/gmail){1}\s{1}([a-z0-9_-]+\.)*[a-z0-9_-]+\@\w+\.\w+";

            switch (message.Text)
            {
                case "/start":
                    await bot.SendTextMessageAsync(message.From.Id, "Чекаю на ваші команди.");
                    break;
                case "/help":
                    await bot.SendTextMessageAsync(message.From.Id, "Ось список команд які ви можете використати:\n\n" +
                        "/start - почати працювати з ботом.\n" +
                        "/mac {Mac-адрес} - отримати інформацію про виробника мережевої карти.\n" +
                        "/ip {Ip-адрес} - отримати інформацію про місцезнаходження хоста.\n" +
                        "/gmail {Gmail-пошта} - дізнатись чи існує задана пошта.(Працює лише 2 рази на добу)\n\n" +
                        "Всі запити до бота виконуються одним повідомленням та за наступним шаблоном: назва команди, пробіл, дані.\nПриклади:\n" +
                        "/mac 44:38:39:ff:ef:57\n/ip 8.8.8.8\n/gmail example@gmail.com");
                    break;
                case string s when new Regex(patternMac).IsMatch(message.Text):

                    StringBuilder macSearch = new StringBuilder(message.Text);
                    macSearch.Remove(0, 4);

                    string urlApiMac = $"http://localhost:53305/tbot/mac/{macSearch}";
                    try
                    {
                        using (HttpClient http = new HttpClient())
                        {
                            http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                            HttpResponseMessage response = await http.GetAsync(urlApiMac);
                            string resp = await response.Content.ReadAsStringAsync();

                            StringBuilder remove = new StringBuilder(resp);
                            remove.Replace(@"\", "");
                            remove.Remove(0, 1);
                            remove.Remove(remove.Length - 1, 1);
                            resp = Convert.ToString(remove);

                            Mac mac = JsonConvert.DeserializeObject<Mac>(resp);

                            await bot.SendTextMessageAsync(message.From.Id, $"isPrivate: {mac.vendorDetails.isPrivate}\n" +
                            $"companyName: {mac.vendorDetails.companyName}\ncompanyAddress: {mac.vendorDetails.companyAddress}\n" +
                            $"countryCode: {mac.vendorDetails.countryCode}\nisValid: {mac.macAddressDetails.isValid}\n" +
                            $"searchTerm: {mac.macAddressDetails.searchTerm}");

                        }
                    }
                    catch
                    {
                        await bot.SendTextMessageAsync(message.From.Id, "Такого MAC-адресу не існує");
                    }
                    break;
                case string s when new Regex(patternIp).IsMatch(message.Text):

                    StringBuilder ipSearch = new StringBuilder(message.Text);
                    ipSearch.Remove(0, 4);

                    string urlApiIp = $"http://localhost:53305/tbot/ip/{ipSearch}";
                    try
                    {
                        using (HttpClient http = new HttpClient())
                        {
                            http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                            HttpResponseMessage response = await http.GetAsync(urlApiIp);
                            string resp = await response.Content.ReadAsStringAsync();

                            StringBuilder remove = new StringBuilder(resp);
                            remove.Replace(@"\", "");
                            remove.Remove(0, 1);
                            remove.Remove(remove.Length - 1, 1);
                            resp = Convert.ToString(remove);

                            Ip ip = JsonConvert.DeserializeObject<Ip>(resp);

                            await bot.SendTextMessageAsync(message.From.Id, $"ip: {ip.ip}\ntype: {ip.type}\n" +
                                $"country: {ip.country}\nlatitude: {ip.latitude}\nlongitude: {ip.longitude}\n" +
                                $"city: {ip.city}");

                        }
                    }
                    catch
                    {
                        await bot.SendTextMessageAsync(message.From.Id, "Такого IP-адресу не існує");
                    }
                    break;
                case string s when new Regex(patternGmail).IsMatch(message.Text):

                    StringBuilder gmailSearch = new StringBuilder(message.Text);
                    gmailSearch.Remove(0, 5);

                    string urlApiGmail = $"http://localhost:53305/tbot/gmail/{gmailSearch}";
                    try
                    {
                        using (HttpClient http = new HttpClient())
                        {
                            http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                            HttpResponseMessage response = await http.GetAsync(urlApiGmail);
                            string resp = await response.Content.ReadAsStringAsync();

                            StringBuilder remove = new StringBuilder(resp);
                            remove.Replace(@"\", "");
                            remove.Replace(" ", "");
                            remove.Remove(0, 1);
                            remove.Remove(remove.Length - 1, 1);
                            resp = Convert.ToString(remove);

                            Regex reg = new Regex(@".(exist){1}.\:(true|false)");
                            Match gmail = reg.Match(resp);

                            await bot.SendTextMessageAsync(message.From.Id, gmail.Value);
                        }
                    }
                    catch
                    {
                        await bot.SendTextMessageAsync(message.From.Id, "Щось пішло не так..");
                    }
                    break;
                default:
                    await bot.SendTextMessageAsync(message.From.Id, "Я не знаю такої команди або некоректно введені дані. /help");
                    break;
            }
        }
    }
}
