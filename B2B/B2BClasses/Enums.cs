using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;

namespace B2BClasses.Services.Enums
{

    public enum enmBookingStatus
    {
        Pending=0,
        Booked=1,
        Refund=2,
        PartialBooked=3,
        Failed=4,
        All=100,
    }
    public enum enmFlightBookingServiceType
    {
        Baggage=1,
        Meal=2,
        Seat=3,
        Extra=4
    }

    public enum enmDirectFlight
    {
        DirectFlight=1,
        ConnectFlight=2,
        ALL = 3
    }

    public enum enmMessageType
    {
        Success = 1,
        Error = 2,
        Warning = 3,
        Info = 4,
    }

    public enum enmMarkupApplicability
    {
        OnTicket=1,
        OnPassenger=2,
        OnBaggageServices=4,
        OnMealServices = 8,
        OnSeatServices=16,
        OnExtraService=32

    }

    public enum enmMessage
    {
        NoFlightDataFound,
        PassengerLimitExceed,
        FlightPriceChanged,
        InsufficientWalletBalance,
        SaveSuccessfully,
        UpdateSuccessfully,
        DeleteSuccessfully,
        RecordAlreadyExists,
        InvalidData
    }


    public enum enmCustomerType
    {
        Admin,
        MLM,
        B2B,
        B2C,
        InHouse
    }


    public enum enmCreditDebit
    {
        Credit = 0,
        Debit = 1
    }

    public enum enmTransactionType
    {
        TicketBook,
        WalletAmountUpdate,
        PaymentGatewayAmountUpdate,
        OnCreditUpdate,
    }

    public enum enmJourneyType
    {
        OneWay = 1, Return = 2, MultiStop = 3, AdvanceSearch = 4, SpecialReturn = 5
    }

    public enum enmGender
    {
        Male = 1,
        Female = 2,
        Other = 4,
        All = Male| Female| Other
    }

    public enum enmServiceProvider
    {
        None=0,
        TBO = 1,
        TripJack = 2,
        Kafila = 3
    }

    public enum enmTaxandFeeType
    {
        OtherCharges,//OT
        ManagementFee,//MF
        ManagementFeeTax,//MFT
        AirLineGSTComponent,//AGST
        FuelSurcharge,//YQ
        CarrierMiscFee,//YR

    }

    public enum enmPassengerType
    {
        Adult = 1,
        Child = 2,
        Infant = 3,
    }

    public enum enmRefundableType
    {
        NonRefundable = 0,
        Refundable = 1,
        PartialRefundable = 2
    }

    public enum enmCabinClass
    {
        //ALL=1,
        ECONOMY = 2,
        PREMIUM_ECONOMY = 3,
        BUSINESS = 4,
        //PremiumBusiness=5,
        FIRST = 6
    }

    public enum enmPreferredDepartureTime
    {
        AnyTime = 1,
        Morning = 2,
        AfterNoon = 3,
        Evening = 4,
        Night = 5
    }


    public enum enmApplication : int
    {
        [Application(IsArea: false, DisplayOrder: 0, Name: "Basic", Description: "Basic", Icon: "nav-icon fas fa-tree", AreaName: "")]
        B2B = 1,
    }

    public enum enmModule : int
    {   
        [Module(EnmApplication: enmApplication.B2B, IsArea: false, DisplayOrder: 2, Name: "Booking", Description: "", Icon: "menu-icon fa fa-suitcase", AreaName: "", CntrlName: "Booking")]
        Booking = 2,
        [Module(EnmApplication: enmApplication.B2B, IsArea: false, DisplayOrder: 3, Name: "Wallet", Description: "", Icon: "menu-icon fa fa-credit-card", AreaName: "", CntrlName: "Wallet")]
        Wallet = 3,
        [Module(EnmApplication: enmApplication.B2B, IsArea: false, DisplayOrder: 4, Name: "Incentive", Description: "", Icon: "menu-icon fa fa-rupee", AreaName: "", CntrlName: "Incentive")]
        Incentive = 4,
        [Module(EnmApplication: enmApplication.B2B, IsArea: false, DisplayOrder: 5, Name: "Report", Description: "", Icon: "menu-icon fa fa-chart-bar", AreaName: "", CntrlName: "Report")]
        Report = 5,
        [Module(EnmApplication: enmApplication.B2B, IsArea: false, DisplayOrder: 10, Name: "Setting", Description: "", Icon: "menu-icon fa fa-cog", AreaName: "", CntrlName: "Setting")]
        Setting = 10,
        [Module(EnmApplication: enmApplication.B2B, IsArea: false, DisplayOrder: 10, Name: "Master", Description: "", Icon: "menu-icon fa fa-user", AreaName: "", CntrlName: "Master")]
        Master = 11,
    }
    public enum enmSubModule : int
    {
        [SubModule(EnmModule: enmModule.Wallet, DisplayOrder: 1, Name: "Credit", Description: "Add Credit", Icon: "nav-icon fas fa-file", CntrlName: "Profile")]
        Credit = 1,

        //[SubModule(EnmModule: enmModule.Gateway_Incentive, DisplayOrder: 2, Name: "Wallet", Description: "Add Wallet Money,See Ledger", Icon: "nav-icon fas fa-file", CntrlName: "Wallet")]
        //Gateway_Incentive_wallet = 2,
    }


    public enum enmDocumentMaster : int
    {
        [Document(enmDocumentType.Report | enmDocumentType.DisplayMenu, 1, "Dashboard", "Dashboard", "menu-icon fa fa-tachometer", "/Home/Index")]
        Dashboard = 1,
        [Document(EnmModule: enmModule.Booking, DocumentType: enmDocumentType.Create | enmDocumentType.Update | enmDocumentType.DisplayMenu,
            DisplayOrder: 1, Name: "Flight", Description: "Flight", Icon: "fa fa-plane", ActionName: "/Home/FlightSearch")]
        Flight = 10,
        [Document(EnmModule: enmModule.Booking, DocumentType: enmDocumentType.Create | enmDocumentType.Update | enmDocumentType.DisplayMenu,
            DisplayOrder: 1, Name: "Hotel", Description: "Hotel", Icon: "fa fa-building", ActionName: "/Home/Hotel")]
        Hotel = 11,
        [Document(EnmModule: enmModule.Booking, DocumentType: enmDocumentType.Create | enmDocumentType.Update | enmDocumentType.DisplayMenu,
            DisplayOrder: 1, Name: "Buses Train", Description: "Buses", Icon: "fa fa-bus", ActionName: "/Home/TrainBuses")]
        BusesTrain = 12,
        [Document(EnmModule: enmModule.Booking, DocumentType: enmDocumentType.Create | enmDocumentType.Update | enmDocumentType.DisplayMenu,
            DisplayOrder: 1, Name: "HolidayPackage", Description: "Train", Icon: "fa fa-train", ActionName: "/Home/HolidayPackage")]
        HolidayPackage = 13,

        [Document(EnmModule: enmModule.Wallet, DocumentType: enmDocumentType.Report | enmDocumentType.DisplayMenu,
            DisplayOrder: 1, Name: "Statement", Description: "Statement", Icon: "far fa-circle nav-icon", ActionName: "/Customer/CustomerwalletReport")]
        Statement = 21,
        [Document(EnmModule: enmModule.Wallet, DocumentType: enmDocumentType.Create | enmDocumentType.Update | enmDocumentType.DisplayMenu,
            DisplayOrder: 1, Name: "Add Wallet", Description: "Add Wallet", Icon: "far fa-circle nav-icon", ActionName: "/Customer/CustomerWallet")]
        Add_Wallet = 22,
        [Document(EnmSubModule: enmSubModule.Credit, DocumentType: enmDocumentType.Create | enmDocumentType.Update | enmDocumentType.DisplayMenu,
            DisplayOrder: 1, Name: "Credit Request", Description: "Credit Request", Icon: "far fa-circle nav-icon", ActionName: "/Customer/CreditRequest")]
        CreditRequest = 23,
        [Document(EnmSubModule: enmSubModule.Credit, DocumentType: enmDocumentType.Approval | enmDocumentType.DisplayMenu,
            DisplayOrder: 1, Name: "Credit Approval", Description: "Credit Approval", Icon: "far fa-circle nav-icon", ActionName: "/Customer/CreditApproval")]
        CreditApproval = 24,
        [Document(EnmSubModule: enmSubModule.Credit, DocumentType: enmDocumentType.Report | enmDocumentType.DisplayMenu,
            DisplayOrder: 1, Name: "Credit Report", Description: "Credit Report", Icon: "far fa-circle nav-icon", ActionName: "/Customer/CreditReport")]
        CreditReport = 25,

        [Document(EnmModule: enmModule.Incentive, DocumentType: enmDocumentType.Create | enmDocumentType.Update | enmDocumentType.DisplayMenu,
            DisplayOrder: 1, Name: "Incentive Statement", Description: "Incentive Statement", Icon: "far fa-circle nav-icon", ActionName: "/Wing/AddWallet")]
        Incentive_Statement = 20,


        [Document(EnmModule: enmModule.Report, DocumentType: enmDocumentType.Create | enmDocumentType.Update | enmDocumentType.DisplayMenu,
            DisplayOrder: 1, Name: "Flight Booking", Description: "Flight", Icon: "fa fa-plane", ActionName: "/Customer/FlightBookingReport")]
        Flight_Booking_Report = 51,

        [Document(EnmModule: enmModule.Setting, DocumentType: enmDocumentType.Create | enmDocumentType.Update | enmDocumentType.Report | enmDocumentType.DisplayMenu,
            DisplayOrder: 1, Name: "MarkUp", Description: "MarkUp", Icon: "far fa-circle nav-icon", ActionName: "/Customer/CustomerMarkUp")]
        Markup = 101,
        [Document(EnmModule: enmModule.Setting, DocumentType: enmDocumentType.Create | enmDocumentType.Update | enmDocumentType.DisplayMenu,
            DisplayOrder: 2, Name: "Convenience", Description: "Convenience", Icon: "far fa-circle nav-icon", ActionName: "/Home/Convenience")]
        convenience_fee = 102,
        [Document(EnmModule: enmModule.Setting, DocumentType: enmDocumentType.Create | enmDocumentType.Update | enmDocumentType.DisplayMenu,
            DisplayOrder: 3, Name: "IP Filter", Description: "Filter The IP of Customer", Icon: "far fa-circle nav-icon", ActionName: "/Customer/CustomerIPFilter")]
        IP_Filter = 103,
        [Document(EnmModule: enmModule.Setting, DocumentType: enmDocumentType.Create | enmDocumentType.Update | enmDocumentType.Report | enmDocumentType.DisplayMenu,
            DisplayOrder: 1, Name: "Wing MarkUp", Description: "Wing MarkUp", Icon: "far fa-circle nav-icon", ActionName: "/Home/WingMarkup")]
        WingMarkup = 104,


        [Document(EnmModule: enmModule.Master, DocumentType: enmDocumentType.Create | enmDocumentType.DisplayMenu,
        DisplayOrder: 1, Name: "Customer", Description: "Customer Master", Icon: "far fa-circle nav-icon", ActionName: "/Customer/AddCustomer")]
        CustomerMaster = 111,
        [Document(EnmModule: enmModule.Master, DocumentType: enmDocumentType.Create | enmDocumentType.DisplayMenu,
        DisplayOrder: 1, Name: "Customer Report", Description: "Customer Report", Icon: "far fa-circle nav-icon", ActionName: "/Customer/CustomerDetails")]
        CustomerReport = 112,        
        [Document(EnmModule: enmModule.Master, DocumentType: enmDocumentType.Create | enmDocumentType.DisplayMenu,
        DisplayOrder: 1, Name: "User", Description: "User Master", Icon: "far fa-circle nav-icon", ActionName: "/Customer/AddUser")]
        UserMaster = 113,
        

    }


    #region ******************** Enum Description Atributes **********************
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
        public Application(bool IsArea, int DisplayOrder, string Name, string Description, string Icon, string AreaName)
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
        public string AreaName { get; set; }
        public virtual List<Module> Modules { get; set; }
    }



    [AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = false)]
    public class Module : Attribute, IDocuments
    {
        public Module(enmApplication EnmApplication, bool IsArea, int DisplayOrder, string Name, string Description, string Icon, string AreaName, string CntrlName)
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
        public SubModule(enmModule EnmModule, int DisplayOrder, string Name, string Description, string Icon, string CntrlName)
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

        public Document(enmDocumentType DocumentType, int DisplayOrder, string Name, string Description,
            string Icon, string ActionName)
        {

            this.DocumentType = DocumentType;
            this.DisplayOrder = DisplayOrder;
            this.Description = Description;
            this.Name = Name;
            this.Icon = Icon;
            this.ActionName = ActionName;
        }

        public Document(enmModule EnmModule, enmDocumentType DocumentType, int DisplayOrder, string Name, string Description,
            string Icon, string ActionName)
        {

            this.DocumentType = DocumentType;
            this.DisplayOrder = DisplayOrder;
            this.Description = Description;
            this.Name = Name;
            this.Icon = Icon;
            this.ActionName = ActionName;
            this.EnmModule = EnmModule;
        }

        public Document(enmModule EnmModule, enmSubModule EnmSubModule, enmDocumentType DocumentType, int DisplayOrder, string Name, string Description,
            string Icon, string ActionName)
        {

            this.DocumentType = DocumentType;
            this.DisplayOrder = DisplayOrder;
            this.Description = Description;
            this.Name = Name;
            this.Icon = Icon;
            this.ActionName = ActionName;
            this.EnmSubModule = EnmSubModule;
            this.EnmModule = EnmModule;
        }
        public Document(enmSubModule EnmSubModule, enmDocumentType DocumentType, int DisplayOrder, string Name, string Description,
            string Icon, string ActionName)
        {

            this.DocumentType = DocumentType;
            this.DisplayOrder = DisplayOrder;
            this.Description = Description;
            this.Name = Name;
            this.Icon = Icon;
            this.ActionName = ActionName;
            this.EnmSubModule = EnmSubModule;
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
        DisplayMenu = 32,        

    }

    public enum enmStatus 
    {
        Active = 1,
        DeActive = 2,
        
    }


    public enum enmApprovalStatus
    {
        Pending = 0,
        Approved = 1,
        Reject=2,
    }
    public enum enmDocumentPartitionType : byte
    {
        None,
        FiscalYear,
        Yearly,
        Quaterly,
        Monthly,
    }

    public enum enmSaveStatus : byte
    {
        none = 0,
        success = 1,
        danger = 2,
        warning = 3,
        info = 4,
        primary = 5,
        secondary = 6,
    }
    //public enum enmMessage
    //{
    //    [Description("Access Denied!!!")]
    //    AccessDenied,
    //    [Description("Pending for Approval!!!")]
    //    PendingApproval,
    //    [Description("Save Successfully!!!")]
    //    SaveSucessfully,
    //    [Description("Update Successfully!!!")]
    //    UpdateSucessfully,
    //    [Description("Approved Successfully!!!")]
    //    ApprovedSucessfully,
    //    [Description("Rejected Successfully!!!")]
    //    RejectSucessfully,
    //    [Description("Enter all Required inputs!!!")]
    //    RequiredField,
    //    [Description("Invalid Id !!!")]
    //    InvalidID,
    //    [Description("Invalid Organization !!!")]
    //    InvalidOrganization,
    //    [Description("Invalid UserId !!!")]
    //    InvalidUserID,
    //    [Description("Invalid Applicable date !!!")]
    //    InvalidApplicableDt,
    //    [Description("Invalid Data !!!")]
    //    InvalidData,
    //    [Description("Invalid User or Password !!!")]
    //    InvalidUserOrPassword,
    //    [Description("Invalid Operation !!!")]
    //    InvalidOperation,
    //    [Description("User Blocked !!!")]
    //    UserLocked,
    //    [Description("Data already exists !!!")]
    //    AlreadyExists,
    //    [Description("Request already in Processing !!!")]
    //    RequestAlreadyInProcessing,
    //    [Description("Request already Processed !!!")]
    //    RequestAlreadyProcessed,
    //    [Description("Data not exists !!!")]
    //    DataNotExists,
    //    [Description("Concurrency Error !!!")]
    //    ConcurrencyError,
    //    [Description("Database Error !!!")]
    //    DatabaseError,
    //    [Description("Undefined Exception!!!")]
    //    UndefinedException,
    //    [Description("Only one head office can be created!!!")]
    //    SingleHeadOfficeException
    //}

    public static class EnmDescription
    {
        public static string GetDescription<T>(this T e) where T : IConvertible
        {
            if (e is Enum)
            {
                Type type = e.GetType();
                Array values = System.Enum.GetValues(type);

                foreach (int val in values)
                {
                    if (val == e.ToInt32(CultureInfo.InvariantCulture))
                    {
                        var memInfo = type.GetMember(type.GetEnumName(val));
                        var descriptionAttribute = memInfo[0]
                            .GetCustomAttributes(typeof(DescriptionAttribute), false)
                            .FirstOrDefault() as DescriptionAttribute;

                        if (descriptionAttribute != null)
                        {
                            return descriptionAttribute.Description;
                        }
                    }
                }
            }
            return null; // could also return string.Empty
        }

        public static Document GetDocumentDetails(this enmDocumentMaster e)
        {
            var type = e.GetType();
            var name = Enum.GetName(type, e);
            var returnData = (Document)type.GetField(name).GetCustomAttributes(typeof(Document), false).FirstOrDefault();
            returnData.Id = (int)e;
            return returnData;
        }

        public static Module GetModuleDetails(this enmModule e)
        {
            var type = e.GetType();
            var name = Enum.GetName(type, e);
            var returnData = (Module)type.GetField(name).GetCustomAttributes(typeof(Module), false).FirstOrDefault();
            returnData.Id = (int)e;
            return returnData;
        }

        public static SubModule GetSubModuleDetails(this enmSubModule e)
        {
            var type = e.GetType();
            var name = Enum.GetName(type, e);
            var returnData = (SubModule)type.GetField(name).GetCustomAttributes(typeof(SubModule), false).FirstOrDefault();
            returnData.Id = (int)e;
            return returnData;
        }
    }

    #endregion


}
