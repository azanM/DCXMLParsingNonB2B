//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DCGenerateXMLService__NonB2B.DataAccess.EF
{
    using System;
    using System.Collections.Generic;
    
    public partial class CUSTOMPROPOSALPAYMENT
    {
        public int id { get; set; }
        public string ProposalNumber { get; set; }
        public string CompCode { get; set; }
        public string Year { get; set; }
        public string DocumentNumber { get; set; }
        public string PurchaseDocument { get; set; }
        public string Item { get; set; }
        public Nullable<System.DateTime> ApproveDate { get; set; }
        public Nullable<System.TimeSpan> ApproveTime { get; set; }
        public Nullable<byte> Status { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public string CreateBy { get; set; }
    }
}
