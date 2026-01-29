using Odasoft.XBOL.Commons.Requests.Filters;
using Odasoft.XBOL.Data.Repositories.Client;
using XBOL.Admin.Core.DTO;

namespace Odasoft.XBOL.Business.Services
{
    public class ClientService(ClientRepository repository)
    {
        public async Task<ClientSeasonEvent> GetClientSeasonEventInfoAsync(ClientFilter filter)
        {
            return await repository.GetClientSeasonEventInfoAsync(filter);
        }
    }
}
