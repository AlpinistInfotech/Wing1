using System;
using System.Collections.Generic;
using System.Text;

namespace Database
{
    public enum enmApplication : int
    {
        [Application(IsArea: false, DisplayOrder: 0, Name: "Basic", Description: "Basic", Icon: "nav-icon fas fa-tree", AreaName: "")]
        Gateway = 1,
        [Application(IsArea: false, DisplayOrder: 1, Name: "HRMS", Description: "Manage the Human Resource of an Organisation", Icon: "nav-icon fas fa-tree", AreaName: "")]
        CRM = 2,
    }

    public enum enmModule : int
    {
        [Module(EnmApplication: enmApplication.Gateway, IsArea: false, DisplayOrder: 0, Name: "Profile", Description: "", Icon: "nav-icon far fa-plus-square", AreaName: "",CntrlName : "Profile")]
        Gateway_Profile = 1,
        [Module(EnmApplication: enmApplication.Gateway, IsArea: false, DisplayOrder: 0, Name: "Booking", Description: "", Icon: "nav-icon far fa-plus-square", AreaName: "", CntrlName: "Booking")]
        Gateway_Booking = 2,
        [Module(EnmApplication: enmApplication.Gateway, IsArea: false, DisplayOrder: 0, Name: "Incentive", Description: "", Icon: "nav-icon far fa-plus-square", AreaName: "", CntrlName: "Incentive")]
        Gateway_Incentive = 3,
        [Module(EnmApplication: enmApplication.Gateway, IsArea: false, DisplayOrder: 0, Name: "Promotion", Description: "", Icon: "nav-icon far fa-plus-square", AreaName: "", CntrlName: "Promotion")]
        Gateway_Promotion = 4,
        [Module(EnmApplication: enmApplication.Gateway, IsArea: false, DisplayOrder: 0, Name: "Team", Description: "", Icon: "nav-icon far fa-plus-square", AreaName: "", CntrlName: "Team")]
        Gateway_Team = 5,
        [Module(EnmApplication: enmApplication.Gateway, IsArea: false, DisplayOrder: 0, Name: "Setting", Description: "", Icon: "nav-icon far fa-plus-square", AreaName: "", CntrlName: "Setting")]
        Gateway_Setting = 6,
        
    }

    public enum enmSubModule : int
    {
        [SubModule(EnmModule: enmModule.Gateway_Profile, DisplayOrder: 1, Name: "Personal", Description: "Address,Email, Contact", Icon: "nav-icon fas fa-file", CntrlName: "Profile")]
        Gateway_Personal_Profile = 1,
        [SubModule(EnmModule: enmModule.Gateway_Incentive, DisplayOrder: 2, Name: "Wallet", Description: "Add Wallet Money,See Ledger", Icon: "nav-icon fas fa-file", CntrlName: "Wallet")]
        Gateway_Incentive_wallet = 2,
    }

    public enum enmDocumentMaster : int
    {

        [Document( enmDocumentType.Report,1, "Dashboard", "Dashboard","far fa-circle nav-icon", "Dashboard")]
        Gateway_Dashboard = 1,
        [Document(enmDocumentType.Report, 1, "Notification", "Notifications", "far fa-circle nav-icon", "Notifications")]
        Gateway_Notifications = 2,

        [Document(EnmSubModule: enmSubModule.Departments, DocumentType: enmDocumentType.Create | enmDocumentType.Update | enmDocumentType.Report,
        DisplayOrder: 2, Name: "Sub Department", Description: "Create,Update Subdepartment", EnmParentDocument: enmDocumentMaster.None,
        Icon: "far fa-circle nav-icon", EnmDocumentPartitionType: enmDocumentPartitionType.None, IsTransactional: false)]
        SubDepartment = 1002,
    }



    public interface IDocuments
    {
        int DisplayOrder { get; set; }
        string Name { get; set; }
        string Description { get; set; }
        string Icon { get; set; }
    }

    [AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = false)]
    public class EnumGrouping : Attribute
    {
        public EnumGrouping(string Name, string Description, string GroupName, string DefaultValue)
        {
            this.Name = Name;
            this.Description = Description;
            this.GroupName = GroupName;
            this.DefaultValue = DefaultValue;
        }
        public string Name { get; set; }
        public string Description { get; set; }
        public string GroupName { get; set; }
        public string DefaultValue { get; set; }
    }
    [AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = false)]
    public class Application : Attribute, IDocuments
    {
        public Application(bool IsArea , int DisplayOrder,string Name,string Description,string Icon,string AreaName)
        {
            
            this.IsArea = IsArea;
            this.DisplayOrder = DisplayOrder;
            this.Name = Name;
            this.Description = Description;
            this.Icon = Icon;
            this.AreaName = AreaName;
        }
        public virtual int Id { get; set; }
        public bool IsArea { get; set; }
        public int DisplayOrder { get; set; }
        public string Name { get; set; }        
        public string Description { get; set; }
        public string Icon { get; set; }
        public  string AreaName { get; set; }
        public virtual List<Module> Modules { get; set; }
    }

    [AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = false)]
    public class Module : Attribute, IDocuments
    {
        public Module(enmApplication EnmApplication, bool IsArea, int DisplayOrder, string Name, string Description, string Icon,string AreaName, string CntrlName)
        {
            this.EnmApplication = EnmApplication;
            this.IsArea = IsArea;
            this.DisplayOrder = DisplayOrder;
            this.Description = Description;
            this.Name = Name;
            this.Icon = Icon;
            this.AreaName = AreaName;
            this.CntrlName = CntrlName;
        }
        public virtual int Id { get; set; }
        public enmApplication EnmApplication { get; set; }
        public bool IsArea { get; set; }
        public int DisplayOrder { get; set; }
        public string Name { get; set; }        
        public string Description { get; set; }
        public string Icon { get; set; }
        public string AreaName { get; set; }
        public string CntrlName { get; set; }
        public virtual List<SubModule> SubModules { get; set; }
        public virtual List<Document> Documents { get; set; }
    }

    [AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = false)]
    public class SubModule : Attribute, IDocuments
    {
        public SubModule(enmModule EnmModule,  int DisplayOrder, string Name, string Description, string Icon, string CntrlName)
        {
            this.EnmModule = EnmModule;            
            this.DisplayOrder = DisplayOrder;
            this.Description = Description;
            this.Name = Name;
            this.CntrlName = CntrlName;
            this.Icon = Icon;
        }
        public virtual int Id { get; set; }
        public enmModule EnmModule { get; set; }
        public int DisplayOrder { get; set; }
        public string Name { get; set; }        
        public string Description { get; set; }
        public string Icon { get; set; }
        public string CntrlName { get; set; }
        public virtual List<Document> Documents { get; set; }
        
    }

    [AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = false)]
    public class Document : Attribute, IDocuments
    {
        
        public Document(enmDocumentType DocumentType, int DisplayOrder,string Name,string Description, 
            string Icon,string ActionName)
        {
            
            this.DocumentType = DocumentType;                        
            this.DisplayOrder = DisplayOrder;
            this.Description = Description;
            this.Name = Name;
            this.Icon = Icon;
            this.ActionName = ActionName;
        }
        public virtual int Id { get; set; }
        public enmSubModule? EnmSubModule { get; set; }
        public enmDocumentType DocumentType { get; set; }
        public string Description { get; set; }
        public enmModule? EnmModule { get; set; }
        public int DisplayOrder { get; set; }
        public string Name { get; set; }
        public string ActionName { get; set; }
        public string Icon { get; set; }        
    }


    public enum enmDocumentType : byte
    {
        Create = 1,
        Update = 2,
        Approval = 4,
        Delete = 8,
        Report = 16,
        DisplayMenu = 32
    }
    public enum enmDocumentPartitionType : byte
    {
        None,
        FiscalYear,
        Yearly,
        Quaterly,
        Monthly,
    }
}
