using B2BClasses.Database;
using B2BClasses.Models;
using B2BClasses.Services.Enums;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace B2BClasses
{
    public class Ticketing
    {
        private readonly DBContext _context;        
        private readonly IConfiguration _config;
        private int _UserId;
        private List<ValidationResult> _validationResultList = new List<ValidationResult>();

        public List<ValidationResult> validationResultList { get { return _validationResultList; } }
        public Ticketing(DBContext context, IConfiguration config, int UserId)
        {
            _config = config;
            _context = context;
            _UserId = UserId;
        }

        

        public bool SaveTicket(mdlTicket mdl)
        {
            _validationResultList.Clear();
            if (mdl.Id == 0)
            {
                tblTicket tblticket = new tblTicket()
                {
                    CustomerId = mdl.CustomerId,
                    TicketCategory = mdl.TicketCategory,
                    TicketMasterId = mdl.TicketMasterId,
                    TicketStatus = mdl.TicketStatus,
                    TicketSubStatus = mdl.TicketSubStatus,
                    Reason = mdl.Reason,
                    Description = mdl.Description,
                    AttachedFile = mdl.AttachedFile,
                    ReffTransactionNo = mdl.ReffTransactionNo,
                    RequestedBy = mdl.RequestedBy,
                    RequestedDate = DateTime.Now,
                    ClosedBy = mdl.ClosedBy,
                    ClosedRemarks = mdl.ClosedRemarks,
                    ClosedDate = mdl.ClosedDate,
                    EstimatedTime = mdl.EstimatedTime,
                    EstimatedCloseTime = mdl.EstimatedCloseTime,
                    ActualTime = mdl.ActualTime,
                    
                    tblTicketDetails = mdl.mdlTicketDetails.Select(p => new tblTicketDetails { ProcessBy = _UserId, ProcessDate = DateTime.Now, ProcessRemarks = p.ProcessRemarks, TicketStatus = p.TicketStatus, TicketSubStatus = p.TicketSubStatus }).ToList()
                };
                _context.tblTicket.Add(tblticket);
                _context.SaveChanges();
                mdl.Id = tblticket.Id;
            }
            else
            {
                var Existing =_context.tblTicket.Where(p => p.Id == mdl.Id).FirstOrDefault();


                if (Existing == null)
                {
                    _validationResultList.Add(new ValidationResult("Invalid Ticket Id"));
                    return false;
                }
                else if (Existing.TicketStatus == enmTicketStatus.Close)
                {
                    _validationResultList.Add(new ValidationResult("Ticket is Already Closed"));
                    return false;
                }

                var mdlTicketDetail=mdl.mdlTicketDetails.Where(p => p.DetailId == 0).FirstOrDefault();
                if (mdlTicketDetail != null)
                {
                    if (mdlTicketDetail.TicketStatus == enmTicketStatus.Close)
                    {

                        TimeSpan span = mdlTicketDetail.ProcessDate.Subtract(Existing.RequestedDate);
                        Existing.TicketStatus = enmTicketStatus.Close;
                        Existing.ClosedBy = _UserId;
                        Existing.ClosedDate = mdlTicketDetail.ProcessDate;
                        Existing.ClosedRemarks = mdlTicketDetail.ProcessRemarks;
                        Existing.ActualTime = span.Hours * 60 + span.Minutes;
                    }
                    Existing.TicketSubStatus = mdlTicketDetail.TicketSubStatus;
                    _context.tblTicketDetails.Add(new tblTicketDetails()
                    {
                        ProcessBy = _UserId,
                        ProcessRemarks = mdlTicketDetail.ProcessRemarks,
                        ProcessDate = mdlTicketDetail.ProcessDate,
                        TicketId = Existing.Id,
                        TicketStatus = mdlTicketDetail.TicketStatus,
                        TicketSubStatus = mdlTicketDetail.TicketSubStatus
                    });
                    _context.Update(Existing);
                    _context.SaveChanges();
                    return true;
                }
                else
                {
                    _validationResultList.Add(new ValidationResult("No data found for update"));
                    return false;
                }

                


            }

            return true;
                }


    }
}
