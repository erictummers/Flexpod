using Flexpod.MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Security;
using WebMatrix.WebData;

namespace Flexpod.MVC.Controllers
{
    public class FlexpodUserController : ApiController
    {
        public HttpResponseMessage PostFlexpodUser(FlexpodUserModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                try
                {
                    WebSecurity.CreateUserAndAccount(
                        model.UserName,
                        model.Password,
                        propertyValues: new
                        {
                            EmailAddress = model.EmailAddress
                        });
                    return Request.CreateResponse<FlexpodUserModel>(HttpStatusCode.Created, model);
                }
                catch (MembershipCreateUserException e)
                {
                    var errorMessage = ErrorCodeToString(e.StatusCode);
                    System.Diagnostics.Trace.TraceError(errorMessage);
                    ModelState.AddModelError("", errorMessage);
                    return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
                }
                catch (Exception e)
                {
                    var errorMessage = e.Message;
                    System.Diagnostics.Trace.TraceError(errorMessage);
                    ModelState.AddModelError("", errorMessage);
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, ModelState);
                }
            }            
            return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
        }

        public HttpResponseMessage GetFlexpodUser()
        {
            var allUsers = new UsersContext().UserProfiles
                .Select(x => new FlexpodUserModel { 
                    UserName = x.UserName, 
                    EmailAddress = x.EmailAddress,
                }).ToArray();
            foreach (var user in allUsers)
            {
                user.LastPasswordChangedDate = WebSecurity.GetPasswordChangedDate(user.UserName);
            }
            return Request.CreateResponse<FlexpodUserModel[]>(HttpStatusCode.OK, allUsers);
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
    }
}
