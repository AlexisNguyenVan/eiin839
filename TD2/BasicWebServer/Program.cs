﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Diagnostics;


namespace BasicServerHTTPlistener
{
    internal class Program
    {
        private static void Main(string[] args)
        {

            //if HttpListener is not supported by the Framework
            if (!HttpListener.IsSupported)
            {
                Console.WriteLine("A more recent Windows version is required to use the HttpListener class.");
                return;
            }
 
 
            // Create a listener.
            HttpListener listener = new HttpListener();

            // Add the prefixes.
            if (args.Length != 0)
            {
                foreach (string s in args)
                {
                    listener.Prefixes.Add(s);
                    // don't forget to authorize access to the TCP/IP addresses localhost:xxxx and localhost:yyyy 
                    // with netsh http add urlacl url=http://localhost:xxxx/ user="Tout le monde"
                    // and netsh http add urlacl url=http://localhost:yyyy/ user="Tout le monde"
                    // user="Tout le monde" is language dependent, use user=Everyone in english 

                }
            }
            else
            {
                Console.WriteLine("Syntax error: the call must contain at least one web server url as argument");
            }
            listener.Start();

            // get args 
            foreach (string s in args)
            {
                Console.WriteLine("Listening for connections on " + s);
            }

            // Trap Ctrl-C on console to exit 
            Console.CancelKeyPress += delegate {
                // call methods to close socket and exit
                listener.Stop();
                listener.Close();
                Environment.Exit(0);
            };


            while (true)
            {
                // Note: The GetContext method blocks while waiting for a request.
                HttpListenerContext context = listener.GetContext();
                HttpListenerRequest request = context.Request;

                string documentContents;
                using (Stream receiveStream = request.InputStream)
                {
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        documentContents = readStream.ReadToEnd();
                    }
                }
                
                // get url 
                Console.WriteLine($"Received request for {request.Url}");

                //get url protocol
                Console.WriteLine(request.Url.Scheme);
                //get user in url
                Console.WriteLine(request.Url.UserInfo);
                //get host in url
                Console.WriteLine(request.Url.Host);
                //get port in url
                Console.WriteLine(request.Url.Port);
                //get path in url 
                Console.WriteLine(request.Url.LocalPath);

                // parse path in url 
                string methodname = request.Url.LocalPath.Substring(1);

                foreach (string str in request.Url.Segments)
                {
                    Console.WriteLine(str);

                }

                //get params un url. After ? and between &

                Console.WriteLine(request.Url.Query);
                //parse params in url

                //Console.WriteLine(request.Url.Query.Split('&').Length) ;
                string[] paramsMethod = new string[request.Url.Query.Split('&').Length];
                //Console.WriteLine(request.Url.Query.Split('&')[0]);
                for(int i=1;i<= request.Url.Query.Split('&').Length;i++)
                {
                    Console.WriteLine($"param{i}= " + HttpUtility.ParseQueryString(request.Url.Query).Get($"param{i}"));
                    paramsMethod[i - 1] = HttpUtility.ParseQueryString(request.Url.Query).Get($"param{i}");
                }

                //Reflection
             
                Type type = typeof(MyReflectionClass);
                MethodInfo method = type.GetMethod(methodname);
                MyReflectionClass c = new MyReflectionClass();
                string responseString = "";
                if (c.GetType().GetMethod(methodname) != null)
                {
                     if(methodname=="MyMethod")
                        responseString = (string)method.Invoke(c, paramsMethod);
                    if (methodname == "incr")
                    {
                        responseString = ((int)method.Invoke(c, paramsMethod)).ToString();
                    }
                    else
                        responseString = (string)method.Invoke(c, null);
                     
                }


                
                //
                //Console.WriteLine(documentContents);

                // Obtain a response object.
                HttpListenerResponse response = context.Response;


                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);

                // Get a response stream and write the response to it.
                response.ContentLength64 = buffer.Length;
                System.IO.Stream output = response.OutputStream;
                output.Write(buffer, 0, buffer.Length);
                // You must close the output stream.
                output.Close();
            }
            // Httplistener neither stop ... But Ctrl-C do that ...
            // listener.Stop();
        }
    }
    public class MyReflectionClass
    {
        public string MyMethod(String param1,String param2)
        {
            string responseString = $"<HTML><BODY> Hello {param1} et {param2}</BODY></HTML>";
            return responseString;
        }

        public int incr(string a)
        {
            int x =Int32.Parse(a);
             x++;
            return x;
        }

        public string callExe()
        {
            //
            // Set up the process with the ProcessStartInfo class.
            // https://www.dotnetperls.com/process
            //
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = @"C:\Users\ngnva\source\repos\td2\TD2\ExecTest\bin\Debug\ExecTest.exe"; // Specify exe name.
            start.Arguments = "Utilisation executable"; // Specify arguments.
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;
            //
            // Start the process.
            //
            using (Process process = Process.Start(start))
            {
                //
                // Read in all the text from the process with the StreamReader.
                //
                using (StreamReader reader = process.StandardOutput)
                {
                    string result = reader.ReadToEnd();
                  
                    Console.WriteLine(result);
                    return result;
                }
            }
        }
    }
}