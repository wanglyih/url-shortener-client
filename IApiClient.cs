using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace URLShortener_client
{
    public interface IApiClient
    {
        Task<ResponseModel> GetResponse(string arg);
    }
}
