using System;
using System.Collections.Generic;
using System.Linq;
using VismaResourceShortage.Models;
using VismaResourceShortage.Services;

namespace VismaResourceShortage
{
    internal class Program
    {
        internal static JsonService jsonService = new JsonService();
        internal static List<Shortage> shortages = new List<Shortage>();
        internal static string currentUser;
        internal static bool isAdmin;

        static void Main(string[] args)
        {
            shortages = jsonService.LoadShortages();

            Console.Write("Enter your name: ");
            currentUser = Console.ReadLine();
            Console.Write("Are you an administrator? (yes/no): ");
            isAdmin = Console.ReadLine().ToLower() == "yes";

            while (true)
            {
                Console.WriteLine("1. Register a new shortage");
                Console.WriteLine("2. Delete a request");
                Console.WriteLine("3. List all requests");
                Console.WriteLine("4. Exit");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        RegisterShortage();
                        break;
                    case "2":
                        DeleteRequest();
                        break;
                    case "3":
                        ListRequests();
                        break;
                    case "4":
                        return;
                }
            }
        }

        internal static void RegisterShortage()
        {
            Console.Write("Enter title: ");
            var title = Console.ReadLine();

            var room = GetValidInput("Enter room (Meeting room / kitchen / bathroom): ", new[] { "meeting room", "kitchen", "bathroom" });
            var category = GetValidInput("Enter category (Electronics / Food / Other): ", new[] { "electronics", "food", "other" });
            var priority = GetValidPriority();

            var existingShortage = shortages.FirstOrDefault(s => s.Title == title && s.Room == room);
            if (existingShortage != null)
            {
                if (priority > existingShortage.Priority)
                {
                    existingShortage.Name = currentUser;
                    existingShortage.Category = category;
                    existingShortage.Priority = priority;
                    existingShortage.CreatedOn = DateTime.Now;
                    Console.WriteLine("Existing shortage updated with higher priority.");
                }
                else
                {
                    Console.WriteLine("A shortage with the same title and room already exists with equal or higher priority.");
                }
            }
            else
            {
                var newShortage = new Shortage
                {
                    Title = title,
                    Name = currentUser,
                    Room = room,
                    Category = category,
                    Priority = priority,
                    CreatedOn = DateTime.Now
                };

                shortages.Add(newShortage);
                Console.WriteLine("New shortage registered.");
            }

            jsonService.SaveShortages(shortages);
        }

        internal static void DeleteRequest()
        {
            Console.Write("Enter title: ");
            var title = Console.ReadLine();

            var shortage = shortages.FirstOrDefault(s => s.Title == title && (s.Name == currentUser || isAdmin));
            if (shortage != null)
            {
                shortages.Remove(shortage);
                jsonService.SaveShortages(shortages);
                Console.WriteLine("Shortage deleted.");
            }
            else
            {
                Console.WriteLine("Shortage not found or you do not have permission to delete it.");
            }
        }

        internal static void ListRequests()
        {
            IEnumerable<Shortage> filteredShortages = shortages;

            if (!isAdmin)
            {
                filteredShortages = filteredShortages.Where(s => s.Name == currentUser);
            }

            Console.WriteLine("1. Filter by title");
            Console.WriteLine("2. Filter by date range");
            Console.WriteLine("3. Filter by category");
            Console.WriteLine("4. Filter by room");
            Console.WriteLine("5. List all");

            var filterChoice = Console.ReadLine();

            switch (filterChoice)
            {
                case "1":
                    Console.Write("Enter title: ");
                    var title = Console.ReadLine().ToLower();
                    filteredShortages = filteredShortages.Where(s => s.Title.ToLower().Contains(title));
                    break;
                case "2":
                    Console.Write("Enter start date (yyyy-MM-dd): ");
                    var startDate = DateTime.Parse(Console.ReadLine());
                    Console.Write("Enter end date (yyyy-MM-dd): ");
                    var endDate = DateTime.Parse(Console.ReadLine());
                    filteredShortages = filteredShortages.Where(s => s.CreatedOn >= startDate && s.CreatedOn <= endDate);
                    break;
                case "3":
                    Console.Write("Enter category: ");
                    var category = Console.ReadLine();
                    filteredShortages = filteredShortages.Where(s => s.Category == category);
                    break;
                case "4":
                    Console.Write("Enter room: ");
                    var room = Console.ReadLine();
                    filteredShortages = filteredShortages.Where(s => s.Room == room);
                    break;
                case "5":
                    break;
            }

            foreach (var shortage in filteredShortages.OrderByDescending(s => s.Priority))
            {
                Console.WriteLine($"Title: {shortage.Title}, Name: {shortage.Name}, Room: {shortage.Room}, Category: {shortage.Category}, Priority: {shortage.Priority}, CreatedOn: {shortage.CreatedOn}");
            }
        }

        internal static string GetValidInput(string prompt, string[] validValues)
        {
            while (true)
            {
                Console.Write(prompt);
                var input = Console.ReadLine().ToLower();
                if (validValues.Contains(input))
                {
                    return input;
                }
                Console.WriteLine($"Invalid input. Valid values are: {string.Join(", ", validValues)}");
            }
        }

        internal static int GetValidPriority()
        {
            while (true)
            {
                Console.Write("Enter priority (1-10): ");
                if (int.TryParse(Console.ReadLine(), out int priority) && priority >= 1 && priority <= 10)
                {
                    return priority;
                }
                Console.WriteLine("Invalid input. Priority must be a number between 1 and 10.");
            }
        }
    }
}
