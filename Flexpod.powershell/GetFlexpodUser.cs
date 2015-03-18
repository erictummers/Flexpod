using System;
using System.Management.Automation;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Flexpod.powershell
{
    /// <summary>
    /// Get-FlexpodUser commandlet for reading existing Users
    /// in Flexpod application
    /// </summary>
    [Cmdlet(
        VerbsCommon.Get,
        "FlexpodUser",
        ConfirmImpact = ConfirmImpact.Low
    )]
    public class GetFlexpodUser : Cmdlet
    {
        /// <summary>Gets the User(s).</summary>
        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            // get data from web API
#if DEBUG
            var URI = "http://localhost:49321/api/FlexpodUser";
#else
                var machine = Environment.GetEnvironmentVariable("WEBSITE_HOSTNAME");
                if (string.IsNullOrEmpty(machine)) machine = Environment.MachineName;
                var URI = string.Format("http://{0}/api/FlexpodUser", machine);
#endif
            WriteVerbose(string.Format("Calling API on {0}", URI));
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                // force use of JSON format
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                // make the call
                var response = Await<HttpResponseMessage>(() => client.GetAsync(URI));
                if (response.IsSuccessStatusCode)
                {
                    // deserialize the response
                    var users = Await<FlexpodUser[]>(
                        () => response.Content.ReadAsAsync<FlexpodUser[]>());
                    WriteVerbose(string.Format("Returned {0} users", users.Length));
                    // pass the object to the pipeline
                    foreach (var user in users)
                    {
                        WriteObject(user);
                    }
                }
            }
        }

        /// <summary>
        /// Helper function to async without await construction
        /// </summary>
        /// <typeparam name="T">Return type</typeparam>
        /// <param name="action">Action to await</param>
        /// <returns>Result from async action after completion</returns>
        private T Await<T>(Func<System.Threading.Tasks.Task<T>> action)
        {
            var t = action();
            t.Wait();
            return t.Result;
        }
    }

    /// <summary>
    /// Information from the user we're interested in
    /// </summary>
    public class FlexpodUser
    {
        public string UserName { get; set; }
        public string EmailAddress { get; set; }
        public DateTime LastPasswordChangedDate { get; set; }
    }
}
