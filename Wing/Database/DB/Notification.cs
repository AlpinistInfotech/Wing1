using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Database.DB
{
    public class tblNotificationContent :d_ModifiedBy
    {
        [Key]
        public enmNotificationType NotificationType { get; set; }
        [MaxLength(256)]
        public string SMSContent { get; set; }
        [MaxLength(128)]
        public string MailTitle { get; set; }
        [MaxLength(2048)]
        public string MailContent { get; set; }
        [MaxLength(128)]
        public string NotificationTitle { get; set; }
        [MaxLength(512)]
        public string NotificationContent { get; set; }
        [MaxLength(512)]
        public string url { get; set; }
    }


    public class tblNotificationMessage : d_ModifiedBy
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Sno { get; set; }
        public  enmNotificationMode NotificationMode { get; set; }
        [MaxLength(2048)]
        public string Message { get; set; }
        public bool IsRead { get; set; }
        public int FromUserId { get; set; }
        public int ToUserId { get; set; }
        [MaxLength(512)]
        public string url { get; set; }
    }
}
