using B2BClasses.Services.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace B2BClasses.Models
{
    public class mdlTicket
    {
        [Required]
        [Display(Name = "Customer Name")]
        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 3)]
        [RegularExpression("[a-zA-Z0-9/.,\\s-]*$", ErrorMessage = "Invalid {0}, no special charcter")]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Customer")]
        public int CustomerId { get; set; }
        [Required]
        [Display(Name = "Category")]
        public int TicketCategory { get; set; }
        [Required]
        [Display(Name = "Ticket Type")]
        public int TicketMasterId { get; set; }
        [Required]
        [Display(Name = "Status")]
        public enmTicketStatus TicketStatus { get; set; }
        [Required]
        [Display(Name = "Sub Status")]
        public enmTicketSubStatus TicketSubStatus { get; set; }
        [Required]
        [Display(Name = "Heading")]
        public string Reason { get; set; }
        [Required]
        [Display(Name = "Details")]
        public string Description { get; set; }
        [Display(Name = "Attached File")]
        public string AttachedFile { get; set; }
        [Display(Name = "Reffrence No")]
        public int ReffTransactionNo { get; set; }
        [Display(Name = "Requested By")]
        public int RequestedBy { get; set; }
        [Display(Name = "Requested Date")]
        public DateTime RequestedDate { get; set; }
        [Display(Name = "Closed By")]
        public int ClosedBy { get; set; }
        public string ClosedRemarks { get; set; }
        [Display(Name = "Closed Date")]
        public DateTime ClosedDate { get; set; }
        [Display(Name = "Expected Time")]
        public int EstimatedTime { get; set; }
        [Display(Name = "Expected Time")]
        public DateTime EstimatedCloseTime { get; set; }
        [Display(Name = "Actual Time")]        
        public int ActualTime { get; set; }
        [Display(Name = "Details")]
        public List<mdlTicketDetails> mdlTicketDetails { get; set; }
    }

    public class mdlTicketDetails
    {
        [Display(Name = "DetailId")]
        public int DetailId { get; set; }
        [Display(Name = "Ticket Status")]
        public enmTicketStatus TicketStatus { get; set; }
        [Display(Name = "Ticket Sub Status")]
        public enmTicketSubStatus TicketSubStatus { get; set; }
        [Display(Name = "Process By")]
        public int ProcessBy { get; set; }
        [Display(Name = "Remarks")]
        public string ProcessRemarks { get; set; }
        [Display(Name = "Process Date")]
        public DateTime ProcessDate { get; set; }        
        
    }
}
