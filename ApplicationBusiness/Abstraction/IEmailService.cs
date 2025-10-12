using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Abestraction
{
    public interface IEmailService
    {
        Task<bool> SendEmail(string to, string subject, string body);
    }
}
