using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmPulse.AppDomain.Models.Telegram
{
    public record TelegramMessage(string? Url, string? Text, string? Photo, DateTime CreatedAt);
}
