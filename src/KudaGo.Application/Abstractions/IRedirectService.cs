using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace KudaGo.Application.Abstractions
{
    public interface IRedirectService
    {
        Task RedirectAsync(string commandName, Message message, CancellationToken cancellationToken);
    }
}
