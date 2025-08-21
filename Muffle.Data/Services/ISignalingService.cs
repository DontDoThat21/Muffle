using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Muffle.Data.Models;

namespace Muffle.Data.Services
{
    public interface ISignalingService
    {
        Task ConnectAsync(Uri serverUri);
        Task SendMessageAsync(string message);
        Task SendMessageWrapperAsync(MessageWrapper messageWrapper);
        Task<string> ReceiveMessageAsync();
        Task<MessageWrapper?> ReceiveMessageWrapperAsync();
        event Action<string> OnMessageReceived;

    }
}
