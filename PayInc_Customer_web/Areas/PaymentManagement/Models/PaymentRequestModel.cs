using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PayInc_Customer_web.Areas.PaymentManagement.Models
{
    public class PaymentRequestModel
    {
    }
    public class PaymentTypeRes
    {
        public int paymentModeId { get; set; }
        public string subPaymentModeName { get; set; }
    }

    public class BankListResponse
    {
        public int bankAccountId { get; set; }
        public int bankId { get; set; }
        public string accountNumber { get; set; }
        public string ifscCode { get; set; }
        public string bankLogo { get; set; }
        public int bankAccountStatus { get; set; }
        public string bankName { get; set; }
    }

    public class RequestInputRes
    {
        public int bankId { get; set; }
        public string bankName { get; set; }
        public int paymentModeId { get; set; }
        public string paymentModeName { get; set; }
        public string parameterName { get; set; }
        public int minLength { get; set; }
        public int maxLength { get; set; }
        public int dataTypeId { get; set; }
        public string dataValue { get; set; }
        public int fieldTypeId { get; set; }
        public string fieldValue { get; set; }
        public string hint { get; set; }
        public int mandatory { get; set; }
        public string columnMapping { get; set; }

        [Required(ErrorMessage="This field is required.")]
        public string UsersInput { get; set; }
    }

    public class RequestViewModel
    {
        public List<RequestInputRes> reqList { get; set; }
    }


    public class SubmitPaymentReqInput
    {
        public long customerId { get; set; }
        public long requestedBy { get; set; }
        public string branchIFSCCode { get; set; }
        public string bankRefNo { get; set; }
        public int paymentModeId { get; set; }
        public int loadAmount { get; set; }
        public int loadCategoryId { get; set; }
        public string loadDescription { get; set; }
        public int bankId { get; set; }
        public string loadDepositDate { get; set; }
        public int serviceChannelId { get; set; }
        public string paymentSlipPath { get; set; }
    }

    public class Acknowledgement
    {
        public string BankName { get; set; }
        public string BankRefNo { get; set; }
        public int LoadAmount { get; set; }
        public string LoadDepositDate { get; set; }
        public string Status { get; set; }
        public int LoadRequestId { get; set; }
    }
}
