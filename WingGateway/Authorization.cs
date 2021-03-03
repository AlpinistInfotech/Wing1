using Database;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WingGateway
{
    //public class AccessRightRequirement : IAuthorizationRequirement
    //{

    //    public enmDocumentMaster documentId;
    //    public enmDocumentType policyType;
    //    public AccessRightRequirement(enmDocumentMaster documentId, enmDocumentType policyType)
    //    {
    //        this.documentId = documentId;
    //        this.policyType = policyType;
    //    }
    //}
    //public class AccessRightHandler : AuthorizationHandler<AccessRightRequirement>
    //{
    //    private readonly RolePermission _rolePermission;
    //    private readonly ICurrentUsers _currentUsers;
    //    public AccessRightHandler(RolePermission rolePermission, ICurrentUsers currentUsers)
    //    {
    //        _rolePermission = rolePermission;
    //        _currentUsers = currentUsers;
    //    }


    //    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, AccessRightRequirement requirement)
    //    {
    //        _currentUsers.EnmDocumentMaster = requirement.documentId;
    //        //if Document does not have those policy
    //        if (!_currentUsers.currentDocument.DocumentType.HasFlag(requirement.policyType))
    //        {
    //            return;
    //        }
    //        var allpermission = await _rolePermission.GetPermissionAsync(requirement.documentId, _currentUsers.Roles, _currentUsers.OrgId);
    //        if (allpermission.Any(p => p.HasFlag(requirement.policyType)))
    //        {
    //            _currentUsers.currentPermission = requirement.policyType;
    //            context.Succeed(requirement);

    //            foreach (enmDocumentType perm in Enum.GetValues(typeof(enmDocumentType)))
    //            {
    //                if (!_currentUsers.currentDocument.DocumentType.HasFlag(perm))
    //                {
    //                    continue;
    //                }

    //                if (allpermission.Any(p => p.HasFlag(perm)))
    //                {
    //                    if (!_currentUsers.tabPermission.HasValue)
    //                    {
    //                        _currentUsers.tabPermission = perm;
    //                    }
    //                    else if (!_currentUsers.tabPermission.Value.HasFlag(perm))
    //                    {
    //                        _currentUsers.tabPermission = _currentUsers.tabPermission.Value | perm;
    //                    }
    //                }

    //            }
    //        }
    //        //return  Task.CompletedTask;
    //    }
    //}
}
