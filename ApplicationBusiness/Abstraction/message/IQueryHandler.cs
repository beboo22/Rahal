using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstraction.message
{
    internal interface IQueryHandler<in TQuery,TResponce> : IRequestHandler<TQuery, TResponce>
        where TQuery : IQuery<TResponce>
    {
    }
}
