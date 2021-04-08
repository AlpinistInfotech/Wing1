using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace B2BClasses.Database
{
    class DefaultData
    {
        private readonly ModelBuilder _modelBuilder;
        private readonly DateTime _CurrentDt;
        public DefaultData(ModelBuilder modelBuilder)
        {
            _modelBuilder = modelBuilder;
            _CurrentDt = new DateTime(2021, 1, 1);
        }

        public void InsertCustomerMaster()
        {
            

            tblCustomerMaster CustomerMaster = new tblCustomerMaster()
            {
                Id = 1,
                Code = "ADM",
                CustomerName = "Admin",
                Email = "Prabhakarks90@gmail.com",
                Address = "New Delhi",
                ContactNo = "9873404402",
                AlternateNo = "9873404402",
                WalletBalence = 0,
                CreditBalence = 0,
                CustomerType = Services.Enums.enmCustomerType.Admin,
                IsActive = true,
                CreatedBy = 1,
                CreatedDt = _CurrentDt
            };
            _modelBuilder.Entity<tblCustomerMaster>().HasData(CustomerMaster);
            //IP Filteration
            tblCustomerIPFilter CustomerIPFilter = new tblCustomerIPFilter()
            {
                Id = 1,
                CreatedBy = 1,
                CreatedDt = _CurrentDt,
                AllowedAllIp = true,
                CustomerId = 1,
                IsDeleted = false,
                ModifiedBy = 1,
                ModifiedDt = _CurrentDt,
            };
            _modelBuilder.Entity<tblCustomerIPFilter>().HasData(CustomerIPFilter);
        }


        public void InsertUser()
        {
            tblUserMaster UserMaster = new tblUserMaster()
            {
                Id = 1,
                IsActive = true,
                CreatedBy = 1,
                CreatedDt = _CurrentDt,
                CustomerId = 1,
                Password = "123456",
                UserName = "admin",
            };
            _modelBuilder.Entity<tblUserMaster>().HasData(UserMaster);

            int index = 1;
            var datas=Enum.GetValues(typeof(B2BClasses.Services.Enums.enmDocumentMaster));
            List<tblUserRole> UserRoles = new List<tblUserRole>();
            foreach (var d in datas)
            {
                UserRoles.Add(new tblUserRole()
                {
                    Id= index,
                    Role= (B2BClasses.Services.Enums.enmDocumentMaster)d,
                    UserId=1,
                    CreatedBy=1,
                    CreatedDt=_CurrentDt,
                    IsDeleted=false,
                    ModifiedBy= 1,
                    ModifiedDt= _CurrentDt,                    
                });
                index++;
            }
            _modelBuilder.Entity<tblUserRole>().HasData(UserRoles);
        }

    }
}
