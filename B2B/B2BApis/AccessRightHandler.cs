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
            if (_currentUser.HaveClaim(requirement.accessRight))
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }


    }
}
