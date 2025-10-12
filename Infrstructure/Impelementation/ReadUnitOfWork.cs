using Domain.Abstraction;
using Domain.Entity;
using Infrastructure.Data;
using System.Collections.Concurrent;

namespace InfraStructure.Impelementation
{
    internal class ReadUnitOfWork : IReadUnitOfWork
    {
        ReadSysDbContext _Rcontext;

        public ReadUnitOfWork(ReadSysDbContext rcontext)
        {
            _Rcontext = rcontext;
        }

        public void Dispose()
        {
            _Rcontext.Dispose();
        }

    }
}
