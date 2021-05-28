using B2BClasses.Database;
using B2BClasses.Services.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B2BClasses
{
    
    public interface ICustomerWallet
    {
        int CustomerId { get; set; }

        Task AddBalenceAsync(DateTime TransactionDt, double Amount, enmTransactionType TransactionType, string TransactionDetails, string Remarks = "",int requestid=0);
        Task DeductBalenceAsync(DateTime TransactionDt, double Amount, enmTransactionType TransactionType, string TransactionDetails, string Remarks = "", int requestid = 0);
        Task<double> GetBalenceAsync();
    }

    public class CustomerWallet : ICustomerWallet
    {
        public int CustomerId { get { return _CustomerId; } set { _CustomerId = value; } }
        private readonly DBContext _context;
        private int _CustomerId;
        private IConfiguration _config;
        public CustomerWallet(DBContext context, IConfiguration config)
        {
            _config = config;
            _context = context;
        }
        private List<int> GetFixWalletBalanceCustomer()
        {
            var FixWalletBalanceCustomer = _config.GetSection("FixWalletBalanceCustomer")?.GetChildren()?.Select(x => int.Parse(x.Value))?.ToList();
            if (FixWalletBalanceCustomer == null)
            {
                FixWalletBalanceCustomer = new List<int>();
            }
            return FixWalletBalanceCustomer;
        }
        private double GetWalletBalanceAmt()
        {
            double WalletBalanceAmt = 1000000;
            double.TryParse(_config["WalletBalanceAmt"], out WalletBalanceAmt);
            return WalletBalanceAmt;
        }

        private double GetCustomerBalanceAmt(double Exitingbalance)
        {
            List<int> UnlimitedWalletBalance = GetFixWalletBalanceCustomer();
            if (UnlimitedWalletBalance.Any(p => p == _CustomerId))
            {
                return Exitingbalance = GetWalletBalanceAmt();
            }
            return Exitingbalance;
        }

        public async Task<double> GetBalenceAsync()
        {
            double CustomerBalance = 0;
            CustomerBalance = (await _context.tblCustomerBalence.Where(p => p.Id == _CustomerId).FirstOrDefaultAsync())?.WalletBalence ?? 0.0;
            return GetCustomerBalanceAmt(CustomerBalance);
        }

        public async Task DeductBalenceAsync(DateTime TransactionDt, double Amount, enmTransactionType TransactionType, string TransactionDetails, string Remarks = "",int requestid = 0)
        {
            bool CustomerFound = false;
            List<int> UnlimitedWalletBalance = GetFixWalletBalanceCustomer();
            if (UnlimitedWalletBalance.Any(p => p == _CustomerId))
            {
                CustomerFound = true;
            }
            try
            {
                var customer = _context.tblCustomerBalence.FirstOrDefault(p => p.Id == _CustomerId);
                if (customer == null)
                {
                    throw new Exception("Invalid Customer");
                }
                if (!CustomerFound)
                {
                    if (Amount > customer.WalletBalence)
                    {
                        throw new Exception("Insufficient fund");
                    }
                }
                _context.tblWalletDetailLedger.Add(new tblWalletDetailLedger()
                {
                    TransactionDt = TransactionDt,
                    Credit = 0,
                    Debit = Amount,
                    CustomerId = _CustomerId,
                    Remarks = Remarks,
                    TransactionDetails = TransactionDetails,
                    TransactionType = TransactionType,
                    PaymentRequestId=requestid,
                });
                if (!CustomerFound)
                {
                    customer.WalletBalence = customer.WalletBalence - Amount;
                }
                _context.tblCustomerBalence.Update(customer);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task AddBalenceAsync(DateTime TransactionDt, double Amount, enmTransactionType TransactionType, string TransactionDetails, string Remarks = "", int requestid = 0)
        {
            bool CustomerFound = false;
            List<int> UnlimitedWalletBalance = GetFixWalletBalanceCustomer();
            if (UnlimitedWalletBalance.Any(p => p == _CustomerId))
            {
                CustomerFound = true;
            }
            try
            {
                var customer = _context.tblCustomerBalence.FirstOrDefault(p => p.Id == _CustomerId);
                if (customer == null)
                {
                    throw new Exception("Invalid Customer");
                }
                _context.tblWalletDetailLedger.Add(new tblWalletDetailLedger()
                {
                    TransactionDt = TransactionDt,
                    Credit = Amount,
                    Debit = 0,
                    CustomerId = _CustomerId,
                    Remarks = Remarks,
                    TransactionDetails = TransactionDetails,
                    TransactionType = TransactionType,
                    PaymentRequestId = requestid,
                });
                if (!CustomerFound)
                {
                    customer.WalletBalence = customer.WalletBalence + Amount;
                }
                _context.tblCustomerBalence.Update(customer);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
