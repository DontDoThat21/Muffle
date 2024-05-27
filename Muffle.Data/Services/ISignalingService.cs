using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Muffle.Data.Services
{
    public interface ISignalingService
    {
        Task ConnectAsync(Uri serverUri);
        Task SendMessageAsync(string message);
        Task<string> ReceiveMessageAsync();
        event Action<string> OnMessageReceived;

    }
}
