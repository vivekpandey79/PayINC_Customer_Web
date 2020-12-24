using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PayInc_Customer_web.Areas.AEPS.Models
{
    public class AEPSInput
    {
        public string PidData { get; set; }
        [Required(ErrorMessage ="Please enter Bank Name")]
        public string BankName { get; set; }
        [Required(ErrorMessage = "Please enter Aadhar Number")]
        public string AadharNumber { get; set; }
        [Required(ErrorMessage = "Please enter Customer Number")]
        public string CustomerNumber { get; set; }
        [Required(ErrorMessage = "Please enter Amount")]
        public string Amount { get; set; }
    }
    public class LocationDatum
    {
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string IpAddress { get; set; }
    }



    #region Transaction Acknowledgement

    public class resAEPSTransaction
    {
        public int Status { get; set; }
        public string TransactionReference { get; set; }
        public string AgentId { get; set; }
        public string ChannelCode { get; set; }
        public string ClientTransactionId { get; set; }
        public string AadhaarNumber { get; set; }
        public string AcknowledgementNumber { get; set; }
        public string BankReferenceNumber { get; set; }
        public decimal BalanceAmount { get; set; }
        public decimal Amount { get; set; }
        public string ResponseMessage { get; set; }
        public string BankResponseMessage { get; set; }

        public string AEPSModeType { get; set; }
        public string MobileNumber { get; set; }
        public AdditionalDataDatum AdditionalData { get; set; }
    }
    public class AdditionalDataDatum
    {
        public IList<MiniStatementDatum> MiniStatement { get; set; }
    }
    public class MiniStatementDatum
    {
        public string Date { get; set; }
        public string TransactionType { get; set; }
        public string Amount { get; set; }
        public string Narration { get; set; }
    }
    #endregion

    public class AEPSAgentRes
    {
        public int agentRecordId { get; set; }
        public int customerId { get; set; }
        public string agentId { get; set; }
        public string mobileNumber { get; set; }
        public string name { get; set; }
        public string shopName { get; set; }
        public string emailId { get; set; }
        public int pinCode { get; set; }
        public string address { get; set; }
        public int districtId { get; set; }
        public string panCard { get; set; }
        public string panCardImagePath { get; set; }
        public string aadharNumber { get; set; }
        public string longitude { get; set; }
        public string latitude { get; set; }
        public string ipAddress { get; set; }
        public string boardedOn { get; set; }
        public int agentStatus { get; set; }
        public int boardedBy { get; set; }
        public int vendorId { get; set; }
        public string modifiedOn { get; set; }
        public int modifiedBy { get; set; }
        public string remark { get; set; }
    }
}
