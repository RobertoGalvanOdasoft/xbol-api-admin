using Odasoft.XBOL.Commons.Enums;

namespace Odasoft.XBOL.DTO.QueryParams
{
    public class ClientsQueryParams : BaseQueryParams
    {
        public List<CreditStatus>? CreditStatus { get; set; }
        public bool? HasCredit { get; set; }
    }
}
