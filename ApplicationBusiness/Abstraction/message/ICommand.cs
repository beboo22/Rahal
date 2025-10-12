using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstraction.message
{
    internal interface ICommand<out TResponce> : IRequest<TResponce>
    {
    }
}
