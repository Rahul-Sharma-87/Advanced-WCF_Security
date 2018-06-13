using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Security.Principal;
using System.ServiceModel;

namespace WCFImpersonateService
{
    internal class SecuredServiceAuthorizationManager : ServiceAuthorizationManager {

        private readonly IList<string> authorizedUsers;
        private readonly IList<string> authorizedGroupsList;

        internal SecuredServiceAuthorizationManager(
            IList<string> configUserGroup,
            IList<string> configUsers
        ) {
            authorizedUsers = configUsers;
            authorizedGroupsList = configUserGroup;
        }

        /// <summary>
        /// Override check for logged user is part of configured user group
        /// </summary>
        /// <param name="operationContext"></param>
        /// <returns></returns>
        protected override bool CheckAccessCore(
            OperationContext operationContext
        ) {
            bool identityIsAuthorized;
            var identity = 
                operationContext.ServiceSecurityContext.WindowsIdentity;
            var userName = identity.Name;

            //Code for user based access check
            if (authorizedUsers != null && authorizedUsers.Any()) {
                identityIsAuthorized = authorizedUsers.Contains(
                    userName,
                    StringComparer.OrdinalIgnoreCase
                );
                if (identityIsAuthorized) {
                    return true;
                }
            }

            var principal = new WindowsPrincipal(identity);
            foreach (var group in authorizedGroupsList) {
                if (principal.IsInRole(group)) {
                    return true;
                }
            }
            return false;
        }
    }
}