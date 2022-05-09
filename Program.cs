using System;
using System.Text;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace test
{
    class Program
    {
       
        private static readonly Encoding encoding = Encoding.UTF8; 
        const string secret = "!ChangeMe!";    
        static async Task Main(string[] args)
        {
            StringBuilder serverTargeted =new StringBuilder();
           
            serverTargeted.Append("http://127.0.0.1"/*config.http.domain_name*/)
                  .Append(":")
                  .Append("3000"/*config.http.port*/)
                  .Append("/.well-known/mercure");

            string headers = "{\"alg\":\"HS256\"}";
            string payload = "{\"mercure\":{\"publish\":[\"*\"],";

            payload+= "\"subscribe\":[\"https://example.com/my-private-topic\",\"{scheme}://{+host}/demo/books/{id}.jsonld\",\"/.well-known/mercure/subscriptions{/topic}{/subscriber}\"],";
            payload+="\"payload\":{\"user\":\"https://example.com/users/dunglas\",\"remoteAddr\":\"127.0.0.1\"}}}";

            Console.WriteLine(payload);

            string token= generate_jwt(headers,payload,secret);

            Console.WriteLine("\n My JWT Generated {0}",token);

            StringBuilder dataToPost=new StringBuilder();

            dataToPost.Append("topic="+Uri.EscapeDataString("http://172.16.0.124:3000/apply"));


            // Your message ...what ever you want to be displayed for your mercure customers ! :)

            dataToPost.Append("&data="+Uri.EscapeDataString("{\"hello\":\"csharp\"}"));
            
            // Post to mercure
            await PostJSONDataToMercureServer(serverTargeted, dataToPost, token);

            return;
    
        }

        static string generate_jwt(string headers, string payload, string secret) {

            string hp = EncodeTo64(headers)+"."+EncodeTo64(payload);
            
            byte[] keyByte = encoding.GetBytes(secret);
            string signature_encoded="";
            using (var alg = new HMACSHA256(keyByte))
            {
                byte[] hmacsha256=alg.ComputeHash(encoding.GetBytes(hp));
                
                signature_encoded = Convert.ToBase64String(hmacsha256);
                signature_encoded=signature_encoded.Replace('+', '-');
                signature_encoded=signature_encoded.Replace('/', '_');
                signature_encoded=signature_encoded.TrimEnd('=');
            }

            string jwt = hp+"."+signature_encoded;
            
            return jwt;
        }


        static string EncodeTo64(string toEncode)
        {
           
            byte[] bytes = encoding.GetBytes(toEncode);
                
            string str= Convert.ToBase64String(bytes);
            str=str.Replace('+', '-');
            str=str.Replace('/', '_');
            str=str.TrimEnd('=');
            return str;

        }
        
        public static async Task PostJSONDataToMercureServer(StringBuilder serverTargeted, StringBuilder serializedJSON,string token)
        {
           
                HttpClient httpClient = new HttpClient();

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                StringContent content = new StringContent(serializedJSON.ToString(),Encoding.UTF8, "application/x-www-form-urlencoded");

                try	
                {          
                    HttpResponseMessage response = await httpClient.PostAsync(new Uri(serverTargeted.ToString()),content);

                    response.EnsureSuccessStatusCode();

                    string responseBody = await response.Content.ReadAsStringAsync();
                  
                    Console.WriteLine(responseBody);
                }
                catch(WebException e)
                {
                    if(e.Status == WebExceptionStatus.ProtocolError)
                        Console.WriteLine("\r\nWebException Raised. The following error occurred : {0}",e.Status);
                    else
                        Console.WriteLine("\r\nUnable to reach the http server");
                }
        }    
    }
}
