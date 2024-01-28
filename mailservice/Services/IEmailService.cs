using mailservice.Models;

namespace mailservice.Services
{
    public interface IEmailService
    {
        void Send(Message message);
    }
}