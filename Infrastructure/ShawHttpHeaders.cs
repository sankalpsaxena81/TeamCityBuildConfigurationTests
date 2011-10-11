using System.Collections.Generic;

namespace Infrastructure
{
    public static class ShawHttpHeaders
    {
        public const string TransactionId = "X-SHAW_TRANSACTION_ID";
        public const string OriginatingUserId = "X-SHAW_ORIGINATING_USER_ID";
        public const string OriginatingIpAddress = "X-SHAW_ORIGINATING_IP_ADDRESS";
        public const string OriginatingHostName = "X-SHAW_ORIGINATING_HOST_NAME";
        public const string OriginalModuleId = "X-SHAW_ORIGINAL_MODULE_ID";

        public static IEnumerable<string> All
        {
            get
            {
                return new[]
                           {
                               TransactionId,
                               OriginatingUserId,
                               OriginatingIpAddress,
                               OriginatingHostName,
                               OriginalModuleId
                           };
            }
        }
    }
}