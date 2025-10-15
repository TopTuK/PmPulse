using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Fizzler;
using Fizzler.Systems.HtmlAgilityPack;

namespace PmPulse.TestApp
{
    /// <summary>
    /// https://habr.com/ru/articles/273807/
    /// </summary>
    internal static class ChannelParserEx
    {
        /*
           TELEGRAM_MESSAGE_CLASS = ".tgme_widget_message"
           TELEGRAM_MESSAGE_TEXT_CLASS = ".tgme_widget_message_text"
           TELEGRAM_MESSAGE_PHOTO_CLASS = ".tgme_widget_message_photo_wrap"
           TELEGRAM_MESSAGE_DATE_CLASS = ".tgme_widget_message_date"
        */

        private const string TelegramMessageClass = ".tgme_widget_message";
        private const string TelegramMessageTextClass = ".tgme_widget_message_text";
        private const string TelegramMessagePhotoClass = ".tgme_widget_message_photo_wrap";
        private const string TelegramMessageDateClass = ".tgme_widget_message_date";

        private const string TelegramChannelWebviewPrefix = "https://t.me/s/";
        private static readonly Regex BackgroundImageRegex = new(@"url\('(https://.+?)'\)", RegexOptions.Compiled);

        private static readonly Dictionary<string, string> DefaultRequestHeaders = new()
        {
            ["User-Agent"] = "Mozilla/5.0 (Linux; Android 6.0.1; Nexus 5X Build/MMB29P) " +
                        "AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.169 " +
                        "Mobile Safari/537.36 (compatible; Googlebot/2.1; +http://www.google.com/bot.html)"
        };

        private const int DefaultRequestTimeout = 10;

        public static async Task<TelegramChannel> ParseChannelAsync(
            string channelName, bool onlyText = false, int limit = 100)
        {
            var channelUrl = TelegramChannelWebviewPrefix + channelName;

            using var httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromSeconds(DefaultRequestTimeout);

            foreach (var header in DefaultRequestHeaders)
            {
                httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
            }

            var response = await httpClient.GetStringAsync(channelUrl);

            var html = new HtmlDocument();
            html.LoadHtml(response);

            var doc = html.DocumentNode;

            var messages = new List<TelegramMessage>();
            var messageNodes = doc.QuerySelectorAll(TelegramMessageClass);

            foreach (var messageNode in messageNodes)
            {
                string? messageText = null;
                var messageTextNode = messageNode.QuerySelector(TelegramMessageTextClass);
                if (messageTextNode != null)
                {
                    messageText = messageTextNode.InnerHtml;
                }

                string? messagePhoto = null;
                var messagePhotoNode = messageNode.QuerySelector(TelegramMessagePhotoClass);
                if (messagePhotoNode != null)
                {
                    var match = BackgroundImageRegex.Match(messagePhotoNode.OuterHtml);
                    if (match.Success)
                    {
                        messagePhoto = match.Groups[1].Value;
                    }
                }

                string? messageUrl = null;
                var messageTime = DateTime.UtcNow;
                var messageDateNode = messageNode.QuerySelector(TelegramMessageDateClass);
                if (messageDateNode != null)
                {
                    messageUrl = messageDateNode.GetAttributeValue("href", null);
                    var messageDateTimeNode = messageDateNode.SelectSingleNode(".//time");
                    if (messageDateTimeNode != null)
                    {
                        var datetimeValue = messageDateTimeNode.GetAttributeValue("datetime", null);
                        if (!string.IsNullOrEmpty(datetimeValue) && datetimeValue.Length >= 19)
                        {
                            if (DateTime.TryParseExact(datetimeValue[..19], "yyyy-MM-ddTHH:mm:ss",
                                null, System.Globalization.DateTimeStyles.None, out var parsedTime))
                            {
                                messageTime = parsedTime;
                            }
                        }
                    }
                }

                if ((messageText != null) || (messagePhoto != null))
                {
                    messages.Add(new TelegramMessage(
                        Url: messageUrl,
                        Text: messageText,
                        Photo: messagePhoto,
                        CreatedAt: messageTime
                    ));
                }
            }

            if (onlyText)
            {
                messages = messages
                    .Where(m => !string.IsNullOrEmpty(m.Text))
                    .ToList();
            }

            var reversedMessages = messages
                //.AsEnumerable()
                //.Reverse()
                .OrderByDescending(m => m.CreatedAt)
                .Take(limit)
                .ToList();

            return new TelegramChannel(
                Url: channelUrl,
                Name: channelName,
                Messages: reversedMessages
            );
        }
    }
}
