using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstraction.message
{
    internal interface ICommandHandler<in TCommand,TResponce> : IRequestHandler<TCommand, TResponce>
        where TCommand : ICommand<TResponce>
    {
    }
}
