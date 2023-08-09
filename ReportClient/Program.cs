using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TwitchLib.Api;

namespace ReportClient
{
    internal class Program
    {
        public static List<string> GenerateTwitchUsernames(int length, int count)
        {
            List<string> usernames = new List<string>();
            StringBuilder usernameBuilder = new StringBuilder();
            Random random = new Random();

            // Valid Twitch username characters
            char[] chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_".ToCharArray();
            for (int i = 0; i < count; i++)
            {
                usernameBuilder.Clear();

                for (int j = 0; j < length; j++)
                {
                    usernameBuilder.Append(chars[random.Next(chars.Length)]);
                }

                usernames.Add(usernameBuilder.ToString());
            }

            return usernames;
        }

        static async Task Main(string[] args)
        {
            var client = new TwitchAPI();

            Console.WriteLine("Enter nicknames length you want to parse. (Snussed negr i pidor)");
            int nicklength = Convert.ToInt32(Console.ReadLine());
            Console.Clear();

            Console.WriteLine($"Enter nicknames amount for generation and checking. (Maximum nicknames with length {nicklength} symbols: '{Math.Pow(63,nicklength)}'");
            int nicknamescount = Convert.ToInt32(Console.ReadLine());
            Console.Clear();
            
            List<string> usernames = GenerateTwitchUsernames(nicklength, nicknamescount);

            int count = 0;

            foreach (string username in usernames)
            {
                try
                {
                    bool result = await client.Undocumented.IsUsernameAvailableAsync(username);
                    if (result == true)
                    {
                        Console.WriteLine($"[+] {username} - Available");
                        using (StreamWriter sw = new StreamWriter("Avaiables.txt", append: true))
                        {
                            sw.WriteLine(username);
                        }
                    }
                    else
                    {
                        Console.WriteLine($"[-] {username} - Not Available");
                    }

                    count++;
                    Console.Title = $"Checking nicknames... | Left: {count}/{usernames.Count}";
                }
                catch
                {
                    Console.WriteLine($"[-] Cant check nickanmes {username} (Too many requests, stopping checking for 3 seconds)");
                    Console.Title = $"Checking nicknames... | Left: {count}/{usernames.Count}";
                    Thread.Sleep(3000);
                }
            }
        }
    }
}
