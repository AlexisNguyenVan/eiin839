public class Programm {
    static readonly HttpClient client = new HttpClient();

    static async Task Main()
    {
        while (true)
        {
            // Call asynchronous network methods in a try/catch block to handle exceptions.
            string a = Console.ReadLine();
            try
            {
                HttpResponseMessage response = await client.GetAsync($"http://localhost:8080/incr?param1={a}");
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                // Above three lines can be replaced with new helper method below
                // string responseBody = await client.GetStringAsync(uri);

                Console.WriteLine(responseBody);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }
        }
    }
}

