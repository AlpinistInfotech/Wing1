using B2BClasses.Database;
using B2BClasses.Services.Enums;
using Microsoft.EntityFrameworkCore;
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

        Task DeductBalenceAsync(DateTime TransactionDt, double Amount, enmTransactionType TransactionType, string TransactionDetails);
        Task<double> GetBalenceAsync();
    }

    public class CustomerWallet : ICustomerWallet
    {
        public int CustomerId { get { return _CustomerId; } set { _CustomerId = value; } }
        private readonly DBContext _context;
        private int _CustomerId;
        public CustomerWallet(DBContext context)
        {
            _context = context;
        }
        public async Task<double> GetBalenceAsync()
        {
            return (await _context.tblCustomerMaster.Where(p => p.Id == _CustomerId).FirstOrDefaultAsync())?.WalletBalence ?? 0.0;

        }
        public async Task DeductBalenceAsync(DateTime TransactionDt, double Amount, enmTransactionType TransactionType, string TransactionDetails)
        {
            try
            {
                var customer = _context.tblCustomerMaster.FirstOrDefault(p => p.Id == _CustomerId);
                if (customer == null)
                {
                    throw new Exception("Invalid Customer");
                }
                if (Amount > customer.WalletBalence)
                {
                    throw new Exception("Insufficient fund");
                }
                _context.tblWalletDetailLedger.Add(new tblWalletDetailLedger()
                {
                    TransactionDt = TransactionDt,
                    Credit = 0,
                    Debit = Amount,
                    CustomerId = _CustomerId,
                    Remarks = string.Empty,
                    TransactionDetails = TransactionDetails,
                    TransactionType = TransactionType,
                });
                customer.WalletBalence = customer.WalletBalence - Amount;
                _context.tblCustomerMaster.Update(customer);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
