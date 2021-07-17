using B2BClasses.Database;
using B2BClasses.Services.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace B2BApis
{
    public class AccessRightRequirement : IAuthorizationRequirement
    {
        public enmDocumentMaster accessRight;

        public AccessRightRequirement(enmDocumentMaster accessRight)
        {
            this.accessRight = accessRight;
        }
    }
    public class AccessRightHandler: AuthorizationHandler<AccessRightRequirement>
    {
        private readonly ICurrentUsers _currentUser;
        public AccessRightHandler(ICurrentUsers currentUser)
        {
            _currentUser = currentUser;
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AccessRightRequirement requirement)
        {

            var data=context.User.Claims.FirstOrDefault(c => c.Type == "_UserId")?.Value;

            int _UserId=0, _CustomerId=0;
            enmCustomerType _CustomerType = enmCustomerType.B2C;
            string tempUserId = context.User.Claims.FirstOrDefault(c=>c.Type== "_UserId")?.Value;
            int.TryParse(tempUserId, out _UserId);

            string _Name = context.User.Claims.FirstOrDefault(c => c.Type == "_Name")?.Value; 
            string tempCustomerId = context.User.Claims.FirstOrDefault(c => c.Type == "_CustomerId")?.Value; 
            int.TryParse(tempCustomerId, out _CustomerId);
            string tempCustomerType = context.User.Claims.FirstOrDefault(c => c.Type == "_CustomerType")?.Value; 
            
            if (!string.IsNullOrEmpty(tempCustomerType))
            {
                Enum.TryParse<enmCustomerType>(tempCustomerType, out _CustomerType);
            }

            _currentUser.CustomerId = _CustomerId;
            _currentUser.UserId = _UserId;
            _currentUser.CustomerType= _CustomerType;
            _currentUser.Name = _Name;

            if (_currentUser.HaveClaim(requirement.accessRight))
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }


    }
}
