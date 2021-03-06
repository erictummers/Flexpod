﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Automation;
using System.Net;
using System.IO;

namespace Flexpod.powershell
{
    /// <summary>
    /// Add-FlexpodUser commandlet for creating new User
    /// in Flexpod application
    /// </summary>
    [Cmdlet(
        VerbsCommon.Add, 
        "FlexpodUser", 
        ConfirmImpact = ConfirmImpact.Low,
        SupportsShouldProcess = true
    )]
    public class AddFlexpodUser : Cmdlet
    {
        /// <summary>Gets or sets the name of the user.</summary>
        /// <value>The name of the user.</value>
        [Parameter(
            Mandatory = true,
            ValueFromPipelineByPropertyName = true,
            ValueFromPipeline = true,
            Position = 0,
            HelpMessage = "UserName for the new User."
        )]
        [Alias("Name")]
        public string UserName { get; set; }

        /// <summary>Gets or sets the email address.</summary>
        /// <value>The email address.</value>
        [Parameter(
            Mandatory = true,
            ValueFromPipelineByPropertyName = true,
            ValueFromPipeline = true,
            Position = 1,
            HelpMessage = "E-mail address for the new User."
        )]
        [Alias("Email")]
        public string EmailAddress { get; set; }

        /// <summary>Gets or sets the password.</summary>
        /// <value>The password.</value>
        [Parameter(
            Mandatory = false,
            ValueFromPipelineByPropertyName = true,
            ValueFromPipeline = true,
            Position = 2,
            HelpMessage = "Password for the new User. If omitted a password is generated."
        )]
        public string Password { get; set; }

        /// <summary>Pre-processing.</summary>
        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            GeneratePasswordIfOmitted();
        }

        /// <summary>Creates the new User.</summary>
        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            // messages for verbose, what-if and confirm
            var verboseDescription = string.Format("Creating {0} {1} {2}", UserName, EmailAddress, Password);
            var verboseWarning = "Are you sure?";
            var caption = string.Format("Creating {0} {1} {2}", UserName, EmailAddress, Password);
            if (ShouldProcess(verboseDescription, verboseWarning, caption))
            {
                // post data to web API
#if DEBUG
                var URI = "http://localhost:49321/api/FlexpodUser";
#else
                var machine = Environment.GetEnvironmentVariable("WEBSITE_HOSTNAME");
                if (string.IsNullOrEmpty(machine)) machine = Environment.MachineName;
                var URI = string.Format("http://{0}/api/FlexpodUser", machine);
#endif
                WriteVerbose(string.Format("Calling API on {0}", URI));

                // actual creation of user
                var myParameters = string.Format("Username={0}&Password={1}&EmailAddress={2}",
                    UserName, Password, EmailAddress);
                try
                {
                    using (var webClient = new WebClient())
                    {
                        webClient.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                        var htmlResult = webClient.UploadString(URI, myParameters);
                    }
                    WriteVerbose(string.Format("User {0} created", UserName));

                    // pass properties to the pipeline
                    WriteObject(new { UserName, EmailAddress, Password });
                } 
                catch (WebException ex)
                {
                    var exceptionResponse = (ex.Response as HttpWebResponse);
                    var reader = new StreamReader(exceptionResponse.GetResponseStream());
                    var message = reader.ReadToEnd();
                    WriteError(new ErrorRecord(ex, exceptionResponse.StatusCode.ToString(),
                        ErrorCategory.ConnectionError, null) { ErrorDetails = new ErrorDetails(message) });
                }
            }
        }

        /// <summary>Generates a password if omitted.</summary>
        private void GeneratePasswordIfOmitted()
        {
            if (string.IsNullOrEmpty(Password))
            {
                Password = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 6);
            }
        }
    }
}
