using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PayInc_Customer_web.Areas.BBPS.Models
{
    public class BBPSModel
    {
        [Required(ErrorMessage ="Please select operator")]
        public string Operator { get; set; }

    }
    public class ServiceProviderResp
    {
        public int serviceProviderId { get; set; }
        public int serviceProviderTypeId { get; set; }
        public string serviceProviderName { get; set; }
        public string serviceProviderShortName { get; set; }
        public int serviceProviderStatus { get; set; }
        public string serviceProviderImage { get; set; }
        public object serviceProviderTypeName { get; set; }
    }

    public class InputParamsRes
    {
        public int parameterId { get; set; }
        public int serviceProviderId { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        public string UserInput { get; set; }
        public string SSCode { get; set; }
        public string parameterName { get; set; }
        public string dataType { get; set; }
        public string minLength { get; set; }
        public string maxLength { get; set; }
        public int isMandatory { get; set; }
        public string allowedValues { get; set; }
        public string hint { get; set; }
        public string columnMapping { get; set; }
        public int sequenceId { get; set; }
        public string serviceProviderName { get; set; }
        public string serviceProviderShortName { get; set; }
        public string serviceProviderTypeName { get; set; }
        public string serviceProviderTypeShortName { get; set; }
    }


    #region Fetch Bill
    public class Addinfo
    {
        public string bill_number { get; set; }
        public string bill_date { get; set; }
        public string due_date { get; set; }
        public string bill_amount { get; set; }
        public string bill_partial { get; set; }
        public string bill_customer { get; set; }
    }

    public class BillFetchRes
    {
        public object balance { get; set; }
        public Addinfo addinfo { get; set; }
        public string errormsg { get; set; }
    }

    #endregion


    #region Key Value Parameter
    public class AdditonalParam
    {
        public string key { get; set; }
        public string value { get; set; }
    }
    #endregion
    public class TransactionResult
    {
        public bool status { get; set; }
        public int errorCode { get; set; }
        public DoTransactionRes response { get; set; }
        public string message { get; set; }
    }

    public class DoTransactionRes
    {
        public string statusCode { get; set; }
        public string clientTransactionId { get; set; }
        public string vendorTransactionId { get; set; }
        public string operatorId { get; set; }
        public string OperatorImage { get; set; }
        public string amount { get; set; }
        public string customerNumber { get; set; }
        public string txnId { get; set; }
        public string message { get; set; }
        public List<AdditonalParam> addlParameters { get; set; }
    }
}
