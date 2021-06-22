using B2BClasses.Services.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace B2BClasses.Database
{
    public class tblTicket
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int TicketCategory { get; set; }
        public int TicketMasterId { get; set; }
        public enmTicketStatus TicketStatus { get; set; }
        public enmTicketSubStatus TicketSubStatus { get; set; }
        public string Reason { get; set; }
        public string Description { get; set; }
        public string AttachedFile{ get; set; }
        public int ReffTransactionNo { get; set; }
        public int RequestedBy { get; set; }        
        public DateTime RequestedDate { get; set; }
        public int ClosedBy { get; set; }
        public string ClosedRemarks { get; set; }
        public DateTime ClosedDate { get; set; }
        public int EstimatedTime { get; set; }
        public int ActualTime { get; set; }
        public DateTime EstimatedCloseTime { get; set; }        
        [InverseProperty("tblTicket")]
        public ICollection<tblTicketDetails> tblTicketDetails { get; set; }
    }

    public class tblTicketDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public enmTicketStatus TicketStatus { get; set; }
        public enmTicketSubStatus TicketSubStatus { get; set; }
        public int ProcessBy { get; set; }
        public string ProcessRemarks { get; set; }
        public DateTime ProcessDate { get; set; }
        [ForeignKey("tblTicket")]
        public int? TicketId { get; set; }
        public tblTicket tblTicket { get; set; }

    }


}
