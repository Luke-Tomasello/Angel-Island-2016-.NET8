/***************************************************************************
 *
 *   RunUO                   : May 1, 2002
 *   portions copyright      : (C) The RunUO Software Team
 *   email                   : info@runuo.com
 *   
 *   Angel Island UO Shard   : March 25, 2004
 *   portions copyright      : (C) 2004-2024 Tomasello Software LLC.
 *   email                   : luke@tomasello.com
 *
 ***************************************************************************/

/***************************************************************************
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 2 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

/* Scripts\Misc\AccountPrompt.cs
 * ChangeLog:
 *  8/10/2024, Adam
 *      first 'admin account' created is made is Owner
 */

using Server.Accounting;

namespace Server.Misc
{
    public class AccountPrompt
    {
        // This script prompts the console for a username and password when 0 accounts have been loaded
        public static void Initialize()
        {
            if (Accounts.Table.Count == 0 && !Core.Service)
            {
                Console.WriteLine("This server has no accounts.");
                Console.WriteLine("Do you want to create an administrator account now? (y/n)");

                if (Console.ReadLine().StartsWith("y"))
                {
                    Console.Write("Username: ");
                    string username = Console.ReadLine();

                    Console.Write("Password: ");
                    string password = Console.ReadLine();

                    Account a = Accounts.AddAccount(username, password);

                    // 8/10/2024: Adam, first account is Owner
                    //a.AccessLevel = AccessLevel.Administrator;
                    a.AccessLevel = AccessLevel.Owner;
                    if (a.AccessLevel == AccessLevel.Owner)
                        Console.WriteLine("Owner account created, continuing");
                    else
                        Console.WriteLine("Administrator account created, continuing");
                }
                else
                {
                    Console.WriteLine("Account not created.");
                }
            }
        }
    }
}