using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KudaGo.Application.Abstractions
{
    public interface IContextService<TContextData>
    {
        Task<TContextData> RefreshContextDataAsync(long chatId);
    }
}
