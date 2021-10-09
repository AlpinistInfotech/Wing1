using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Database.DB
{
    public class tblFlightSerivceProvider  :d_ModifiedBy
    {
        [Key]
        public enmServiceProvider ServiceProvider { get; set; }
        public bool IsEnabled { get; set; }        
    }

    public class tblSchduleDisableServiceProvider : d_ModifiedBy
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Sno { get; set; }
        public enmBookingType BookingType { get; set; }
        public int ServiceProvider { get; set; }
        public DateTime SchduleDate { get; set; }
        public bool IsDisable { get; set; }
        public bool IsActive { get; set; }
        public bool IsProcessed { get; set; }
    }

    public class tblPaymentGatewayProvider : d_ModifiedBy
    {
        [Key]
        public enmPaymentGateway PaymentGatewayProvider { get; set; }
        public bool IsEnabled { get; set; }
    }

    public class tblSchduleDisablePaymentGatewayProvider : d_ModifiedBy
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Sno { get; set; }        
        public enmPaymentGateway PaymentGatewayProvider { get; set; }
        public DateTime SchduleDate { get; set; }
        public bool IsDisable { get; set; }
        public bool IsActive { get; set; }
        public bool IsProcessed { get; set; }
    }


}
