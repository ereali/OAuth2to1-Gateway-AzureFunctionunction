using System;

namespace DoorOverrideAPI.Helpers
{
    public class UrlConstants
    {
        //OAuth api's
        public const string temporaryToken = "/transact/api/initiate";
        public const string realToken = "/transact/api/token";
        public const string validateToken = "/transact/api/verify";
        public const string validateUser = "/transact/api/v1/validateuser";
        public const string healthCheck = "/BbTS/api/healthcheck";

        //Customer verify api's
        public const string customerVerifyGetNumber = "/bbts/api/20140501/commerce/CustomerVerification";
        public const string customerVerifyByNumber = "/bbts/api/management/v1/customers/"; //{customer number}

        //Door override api's
        public const string buildingAreas = "/transact/api/merchant/buildingAreas";
        public const string doorListGetAllText = "/transact/api/security/doors/list?pageSize=2000";
        public const string doorOverride = "/transact/api/v1/security/doors/state/override";
        public const string doorListGetAllMaster = "/transact/api/v1/security/doors";

        //Merchant api's
        public const string merchantGetAll = "/transact/api/merchant";
    }
}
