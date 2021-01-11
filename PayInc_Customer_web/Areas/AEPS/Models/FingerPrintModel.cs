using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PayInc_Customer_web.Areas.AEPS.Models
{
    public class FingerPrintModel
    {

    }
    public class _clsPidXml
    {
    }
    [XmlRoot(ElementName = "Resp")]
    public class Resp
    {
        [XmlAttribute(AttributeName = "errCode")]
        public string ErrCode { get; set; }
        [XmlAttribute(AttributeName = "errInfo")]
        public string ErrInfo { get; set; }
        [XmlAttribute(AttributeName = "fCount")]
        public string FCount { get; set; }
        [XmlAttribute(AttributeName = "fType")]
        public string FType { get; set; }
        [XmlAttribute(AttributeName = "nmPoints")]
        public string NmPoints { get; set; }
        [XmlAttribute(AttributeName = "qScore")]
        public string QScore { get; set; }
    }

    [XmlRoot(ElementName = "Param")]
    public class Param
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "value")]
        public string Value { get; set; }
    }

    [XmlRoot(ElementName = "additional_info")]
    public class Additional_info
    {
        [XmlElement(ElementName = "Param")]
        public List<Param> Param { get; set; }
    }

    [XmlRoot(ElementName = "DeviceInfo")]
    public class DeviceInfo
    {
        [XmlElement(ElementName = "additional_info")]
        public Additional_info Additional_info { get; set; }
        [XmlAttribute(AttributeName = "dpId")]
        public string DpId { get; set; }
        [XmlAttribute(AttributeName = "rdsId")]
        public string RdsId { get; set; }
        [XmlAttribute(AttributeName = "rdsVer")]
        public string RdsVer { get; set; }
        [XmlAttribute(AttributeName = "mi")]
        public string Mi { get; set; }
        [XmlAttribute(AttributeName = "mc")]
        public string Mc { get; set; }
        [XmlAttribute(AttributeName = "dc")]
        public string Dc { get; set; }
    }

    [XmlRoot(ElementName = "Skey")]
    public class Skey
    {
        [XmlAttribute(AttributeName = "ci")]
        public string Ci { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "Data")]
    public class Data
    {
        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "PidData")]
    public class PidData
    {
        [XmlElement(ElementName = "Resp")]
        public Resp Resp { get; set; }
        [XmlElement(ElementName = "DeviceInfo")]
        public DeviceInfo DeviceInfo { get; set; }
        [XmlElement(ElementName = "Skey")]
        public Skey Skey { get; set; }
        [XmlElement(ElementName = "Hmac")]
        public string Hmac { get; set; }
        [XmlElement(ElementName = "Data")]
        public Data Data { get; set; }
    }



    #region Request ICICI AEPS

    public class DetailsBalanceEnqAepsReq
    {
        public long customerId { get; set; }
        public string agentId { get; set; }
        public string customerNumber { get; set; }
        public string adhaarNumber { get; set; }
        public string transactionType { get; set; }
        public string accessModeType { get; set; }
        public string nbin { get; set; }
        public string req_Timestamp { get; set; }
        public string longitude { get; set; }
        public string latitude { get; set; }
        public string ipAddress { get; set; }
        public string deviceSerialNumber { get; set; }
        public string deviceTransactionId { get; set; }
        public string clientTransactionId { get; set; }
        public string merchantTransactionId { get; set; }
        public long vendorId { get; set; }
        public string requestRemarks { get; set; }
    }

    public class DetailsAepsRequestsReq
    {
        public long customerId { get; set; }
        public string agentId { get; set; }
        public string customerNumber { get; set; }
        public string adhaarNumber { get; set; }
        public double transactionAmount { get; set; }
        public string transactionType { get; set; }
        public string accessModeType { get; set; }
        public string nbin { get; set; }
        public string req_Timestamp { get; set; }
        public string longitude { get; set; }
        public string latitude { get; set; }
        public string ipAddress { get; set; }
        public string deviceSerialNumber { get; set; }
        public string deviceTransactionId { get; set; }
        public string clientTransactionId { get; set; }
        public string merchantTransactionId { get; set; }
        public long vendorId { get; set; }
        public string requestRemarks { get; set; }
    }

    public class cardnumberORUID
    {
        public String adhaarNumber;
        public int indicatorforUID;
        public String nationalBankIdentificationNumber;
    }

    public class captureResponse
    {
        public String errCode;
        public String errInfo;
        public String fCount;
        public String fType;
        public String iCount;
        public String iType;
        public String pCount;
        public String pType;
        public String nmPoints;
        public String qScore;
        public String dpID;
        public String rdsID;
        public String rdsVer;
        public String dc;
        public String mi;
        public String mc;
        public String ci;
        public String sessionKey;
        public String hmac;
        public String PidDatatype;
        public String Piddata;

    }

    public class AepsBalanceEnquiryInput
    {
        public cardnumberORUID cardnumberORUID;
        public string mobileNumber;
        public string paymentType;// LU - Last Used Bank, Bank- B,PayTM - P, Mpesa - M, Mobikwik - K, Bitcoin - C        
        public string timestamp;// dd/MM/yyyy HH:mm:ss
        public string transactionType;// Merchant - M, Cash withdraw - C, Bill Payments Prepaid Phone - P, Postpaid Phone- Q, Electricity- E
        public string requestRemarks;
        public string deviceTransactionId;
        public captureResponse captureResponse;
        public string subMerchantId;
        public string merchantTransactionId;
    }
    public class AepsCashWithdrawalInput
    {
        public cardnumberORUID cardnumberORUID;
        public string mobileNumber;
        public string paymentType;// LU - Last Used Bank, Bank- B,PayTM - P, Mpesa - M, Mobikwik - K, Bitcoin - C        
        public string timestamp;// dd/MM/yyyy HH:mm:ss
        public string transactionType;// Merchant - M, Cash withdraw - C, Bill Payments Prepaid Phone - P, Postpaid Phone- Q, Electricity- E
        public string requestRemarks;
        public string deviceTransactionId;
        public captureResponse captureResponse;
        public int transactionAmount { get; set; }
        public string subMerchantId;
        public string merchantTransactionId;
    }
    #endregion


    #region new AEPS
    public class DetailsAepReq
    {
        public long customerId { get; set; }
        public int serviceChannelId { get; set; }
        public string agentId { get; set; }//subMerchantId
        public string customerNumber { get; set; }
        public string adhaarNumber { get; set; }
        public int transactionAmount { get; set; }
        public string transactionType { get; set; }
        public string accessModeType { get; set; }
        public string nbin { get; set; }
        public string longitude { get; set; }
        public string latitude { get; set; }
        public string ipAddress { get; set; }
        public string deviceSerialNumber { get; set; }
        public string deviceTransactionId { get; set; }
        public string merchantTransactionId { get; set; }
        public string requestRemarks { get; set; }
        public int indicatorforUID { get; set; }
        public string paymentType { get; set; }
        public string virtualId { get; set; }
        public IciciCaptureInternalResponse iciciAepsCaptureRes { get; set; }
    }

    public class IciciCaptureInternalResponse
    {
        public string errCode { get; set; }
        public string errInfo { get; set; }
        public string fCount { get; set; }
        public string fType { get; set; }
        public string iCount { get; set; }
        public string iType { get; set; }
        public string pCount { get; set; }
        public string pType { get; set; }
        public string nmPoints { get; set; }
        public string qScore { get; set; }
        public string dpID { get; set; }
        public string rdsID { get; set; }
        public string rdsVer { get; set; }
        public string dc { get; set; }
        public string mi { get; set; }
        public string mc { get; set; }
        public string ci { get; set; }
        public string sessionKey { get; set; }
        public string hmac { get; set; }
        public string PidDatatype { get; set; }
        public string Piddata { get; set; }
    }

    #endregion

    #region aeps response
    public class AEPSData
    {
        public string terminalId { get; set; }
        public string requestTransactionTime { get; set; }
        public double transactionAmount { get; set; }
        public string transactionStatus { get; set; }
        public double balanceAmount { get; set; }
        public string bankRRN { get; set; }
        public string transactionType { get; set; }
        public string fpTransactionId { get; set; }
        public object merchantTxnId { get; set; }
    }

    public class AEPSResponse
    {
        public bool status { get; set; }
        public string message { get; set; }
        public AEPSData data { get; set; }
        public int statusCode { get; set; }
    }
    #endregion

    #region MINI Statement
    public class MiniStatementStructureModel
    {
        public string date { get; set; }
        public string txnType { get; set; }
        public string amount { get; set; }
        public string narration { get; set; }
    }

    public class MINIStatementData
    {
        public string terminalId { get; set; }
        public string requestTransactionTime { get; set; }
        public string transactionStatus { get; set; }
        public double balanceAmount { get; set; }
        public string bankRRN { get; set; }
        public string transactionType { get; set; }
        public string fpTransactionId { get; set; }
        public object merchantTxnId { get; set; }
        public List<MiniStatementStructureModel> miniStatementStructureModel { get; set; }
        public object miniOffusStatementStructureModel { get; set; }
        public bool miniOffusFlag { get; set; }
    }

    public class MINIStatementResponse
    {
        public bool status { get; set; }
        public string message { get; set; }
        public MINIStatementData data { get; set; }
        public int statusCode { get; set; }
    }

    public class MINIStatRoot
    {
        public bool status { get; set; }
        public int errorCode { get; set; }
        public MINIStatementResponse response { get; set; }
        public string message { get; set; }
    }


    #endregion
}
