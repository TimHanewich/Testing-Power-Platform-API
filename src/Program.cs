using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Spectre.Console;

namespace TestPowerAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            DemoAsync().Wait();
        }

        public static async Task DemoAsync()
        {
            
            /// SETTINGS ///
            string ClientID = "537c9fd5-1887-4786-9fc2-05863149de86";
            ////////////////

            HttpClient hc = new HttpClient();

            //Set up Request for device code from Microsoft that we can give to the user
            HttpRequestMessage req = new HttpRequestMessage();
            req.Method = HttpMethod.Post;
            req.RequestUri = new Uri("https://login.microsoftonline.com/common/oauth2/v2.0/devicecode");
            Dictionary<string, string> body = new Dictionary<string, string>();
            body.Add("client_id", ClientID);
            body.Add("scope", "https://api.powerplatform.com/.default offline_access"); //SCOPE
            req.Content = new FormUrlEncodedContent(body);

            //Request Device Code
            Console.Write("Requesting device code... ");
            HttpResponseMessage resp = await hc.SendAsync(req);
            string response = await resp.Content.ReadAsStringAsync();
            if (resp.StatusCode != HttpStatusCode.OK)
            {
                Console.WriteLine("Returned code '" + resp.StatusCode.ToString() + "': " + response);
            }
            Console.WriteLine("Success!");

            //Parse device code
            JObject root = JObject.Parse(response);
            JToken? user_code = root.SelectToken("user_code");
            JToken? device_code = root.SelectToken("device_code");
            JToken? verification_uri = root.SelectToken("verification_uri");
            if (user_code == null || device_code == null || verification_uri == null)
            {
                Console.WriteLine("Unable to pull necessary and expected properties out of response for device code.");
                return;
            }
            Console.WriteLine();
            Console.WriteLine("Please visit: " + verification_uri.ToString());
            Console.WriteLine("And use code '" + user_code.ToString() + "'");
            Console.WriteLine();
            

            //Collect the token
            string? token = null;
            string? refresh_token = null;
            while (token == null)
            {
                //Have them press enter once they authenticate
                Console.Write("Press enter after you have done that please.");
                Console.ReadLine();
                Console.WriteLine();

                //Poll for token
                req = new HttpRequestMessage();
                req.Method = HttpMethod.Post;
                req.RequestUri = new Uri("https://login.microsoftonline.com/common/oauth2/v2.0/token");
                body = new Dictionary<string, string>();
                body.Add("grant_type", "urn:ietf:params:oauth:grant-type:device_code");
                body.Add("client_id", ClientID);
                body.Add("device_code", device_code.ToString()); //from the first call
                req.Content = new FormUrlEncodedContent(body);

                //Request
                Console.Write("Requesting token... ");
                resp = await hc.SendAsync(req);
                response = await resp.Content.ReadAsStringAsync();
                if (resp.StatusCode == HttpStatusCode.OK) //success! there will be a bearer token in there
                {
                    root = JObject.Parse(response);
                    JToken? access_token = root.SelectToken("access_token");
                    if (access_token == null)
                    {
                        Console.WriteLine("Request for token after authentication was successfull (200 OK) but unable to find access_token in body.");
                        return;
                    }
                    token = access_token.ToString(); //will break out of the infinite loop we are in.
                    Console.WriteLine("got it!");

                    //Was there a refresh token too? If so, get that
                    JToken? jtrefresh_token = root.SelectToken("refresh_token"); //it will be there as lone as offline_access was requested as the scope in step 1
                    if (jtrefresh_token != null)
                    {
                        refresh_token = jtrefresh_token.ToString();
                    }
                }
                else if (resp.StatusCode == HttpStatusCode.BadRequest)
                {
                    root = JObject.Parse(response);
                    JToken? error = root.SelectToken("error");
                    if (error != null)
                    {
                        if (error.ToString() == "authorization_pending")
                        {
                            Console.WriteLine("You haven't authenticated yet.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Received 400 Bad Request, but it isn't due to authorization being pending. This is the body:");
                        Console.WriteLine(response);
                        return;
                    }
                }
                else
                {
                    Console.WriteLine("Request for token returned status code '" + resp.StatusCode.ToString() + "': " + response);
                    return;
                }
            }

            // Set up query power platform API (as an example)
            // https://learn.microsoft.com/en-us/rest/api/power-platform/environmentmanagement/environments/list-environments-for-user
            req = new HttpRequestMessage();
            req.Method = HttpMethod.Get;
            req.RequestUri = new Uri("https://api.powerplatform.com/environmentmanagement/environments?api-version=2024-10-01");
            req.Headers.Add("Authorization", "Bearer " + token); //add bearer token we received previously

            //Make the call for the power platform environments
            Console.Write("Querying Power Platform (Dataverse) Environments...");
            resp = await hc.SendAsync(req);
            response = await resp.Content.ReadAsStringAsync();
            if (resp.StatusCode != HttpStatusCode.OK)
            {
                Console.WriteLine("Query for Power Platfrom Environments returned code '" + resp.StatusCode.ToString() + "': " + response);
                return;
            }
            Console.WriteLine("Success!");
            Console.WriteLine();

            //Collect each environment
            List<DataverseEnvironment> environments = new List<DataverseEnvironment>();
            root = JObject.Parse(response);
            JProperty? prop_value = root.Property("value");
            if (prop_value == null)
            {
                Console.WriteLine("The response of environments did not include a 'value' array which is where I expect the environment data to be in.");
                return;
            }
            JArray envDatas = (JArray)prop_value.Value;
            foreach (JObject envData in envDatas)
            {
                JToken? tenantId = envData.SelectToken("tenantId");
                JToken? id = envData.SelectToken("id");
                JToken? displayName = envData.SelectToken("displayName");
                JToken? url = envData.SelectToken("url");
                if (id != null && displayName != null && url != null && tenantId != null)
                {
                    DataverseEnvironment env = new DataverseEnvironment();
                    env.ID = id.ToString();
                    env.DisplayName = displayName.ToString();
                    env.URL = url.ToString();
                    env.TenantID = tenantId.ToString();
                    environments.Add(env);
                }
            }

            //Construct a table with those
            Table envtable = new Table();
            envtable.Title("Power Platform Environments");
            envtable.AddColumns("Name", "ID", "URL");
            foreach (DataverseEnvironment env in environments)
            {
                envtable.AddRow(env.DisplayName, env.ID, env.URL);
            }

            //Print the table
            AnsiConsole.Write(envtable);

            //If refresh token we have, ask which one
            if (refresh_token != null)
            {

                //Ask to pick an enviornment
                SelectionPrompt<string> EnvSelect = new SelectionPrompt<string>();
                EnvSelect.Title("Which environment do you want to access data for?");
                foreach (DataverseEnvironment env in environments)
                {
                    EnvSelect.AddChoice(env.ToSelectionString());
                }
                string SelectedEnv = AnsiConsole.Prompt(EnvSelect);

                //Get the environment that they selected
                DataverseEnvironment? TargetEnv = null;
                foreach (DataverseEnvironment env in environments)
                {
                    if (SelectedEnv == env.ToSelectionString())
                    {
                        TargetEnv = env;
                    }
                }
                if (TargetEnv == null)
                {
                    Console.WriteLine("Failed to find selected environment. There is a problem with this code.");
                    return;
                }


                //Get token for that environment, using the refresh token
                req = new HttpRequestMessage();
                req.Method = HttpMethod.Post;
                req.RequestUri = new Uri("https://login.microsoftonline.com/" + TargetEnv.TenantID  + "/oauth2/v2.0/token");
                body = new Dictionary<string, string>();
                body.Add("grant_type", "refresh_token");
                body.Add("client_id", ClientID);
                body.Add("refresh_token", refresh_token);
                body.Add("scope", TargetEnv.URL + "/.default"); //e.g. https://contoso.crm.dynamics.com/.default
                req.Content = new FormUrlEncodedContent(body);

                //Make request
                Console.Write("Requesting access token to that environment using refresh token... ");
                resp = await hc.SendAsync(req);
                Console.WriteLine("Done.");
                response = await resp.Content.ReadAsStringAsync();
                if (resp.StatusCode != HttpStatusCode.OK)
                {
                    Console.WriteLine("Returned code '" + resp.StatusCode.ToString() + "': " + response);
                    return;
                }
                Console.WriteLine("Success! Token granted!");
                Console.WriteLine(response);
                
            
            }

        }

    }
}