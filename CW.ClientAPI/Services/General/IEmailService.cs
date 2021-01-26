using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CW.ClientAPI.Services.General
{
    public interface IEmailService
    {
        void Send(string msgSubject, string message);
    }
}
