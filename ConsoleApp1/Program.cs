using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Birthday
    {
        List<Person> people = new List<Person>();
        string path = Path.Combine(Environment.CurrentDirectory, "listHB.txt");
        public Birthday() 
        {
            LoadFromFile(); 
        }
        public void RunMenu()
        {
            Console.WriteLine("Программа напоминание о днях рождениях.");
            while (true)
            {
                Console.WriteLine("\nЧто вы хотите сделать?");
                Console.WriteLine("1. Показать весь список.");
                Console.WriteLine("2. Показать список сегодняшних и ближайших дней рождений.");
                Console.WriteLine("3. Добавление в список.");
                Console.WriteLine("4. Удаление из списка.");
                Console.WriteLine("5. Редактрирование списка");
                Console.WriteLine("6. Выйти\n");

                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        ShowAllList();
                        break;
                    case "2":
                        ShowUpcomingBirthday();
                        break;
                    case "3":
                        AddPerson();
                        break;
                    case "4":
                        RemovePerson();
                        break;
                    case "5":
                        EditPerson();
                        break;;
                    case "6":
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Неккоректный ввод. Попробуйте ещё раз");
                        break;
                }
            }
        }

        void ShowAllList()
        {
            using (StreamReader reader = new StreamReader(path, true)) 
            {
                string text = reader.ReadToEnd();
                Console.WriteLine(text);
            }
        }

        void ShowUpcomingBirthday()
        {
            foreach (var person in people)
            {
                if (person.BirthdayTodayOrUpcoming())
                    Console.WriteLine($"{person.Birthday.ToShortDateString()}; {person.Surname}; {person.Name}");
            }
        }

        void AddPerson()
        {
            Console.Write("Введите фамилию: ");
            string surname = Console.ReadLine();
            Console.Write("Введите имя: ");
            string name = Console.ReadLine();
            Console.WriteLine("Введите дату рождения (в формате dd.mm.yyyy): ");
            DateTime birthday;
            
            while (!DateTime.TryParseExact(Console.ReadLine(), "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out birthday))
            {
                Console.WriteLine("Некорректный формат даты. Введите дату рождения ещё раз (в формате dd.MM.yyyy):");
            }

            birthday = birthday.Date;
            people.Add(new Person(birthday, surname, name));
            Console.WriteLine("Человек успешно добавлен!");
            Save();
        }
        void RemovePerson()
        {
            Console.WriteLine("Введите фамилию человека, которого хотите удалить:");
            string surname = Console.ReadLine();
            Person personRemove = SearchPerson(surname);
            if (personRemove != null)
            {
                people.Remove(personRemove);
                Console.WriteLine($"Человек с фамилией {surname} успешно удален из списка.");
                Save();
            }
            else Console.WriteLine($"Человек с фамилией {surname} не найден в списке.");
        }

        void EditPerson()
        {
            Console.WriteLine("Введите фамилию человека, которого хотите изменить");
            string surname = Console.ReadLine();
            Person personEdit = SearchPerson(surname);

            if (personEdit != null)
            {
                Console.WriteLine($"Текущие данные.");
                Console.WriteLine($"Фамилия: {surname}");
                Console.WriteLine($"Имя: {personEdit.Name}");
                Console.WriteLine($"Дата рождения: {personEdit.Birthday.ToString("dd.MM.yyyy")}"); 

                Console.WriteLine("\nВведите новое имя:");
                string name = Console.ReadLine();

                Console.WriteLine("Введите новую дату рождения (в формате dd.mm.yyyy):");
                DateTime birthday = CheckRightData();
                personEdit.Name = name;
                personEdit.Birthday = birthday;
                Console.WriteLine($"Данные успешно обновлены.");
                Save();
            }
            else Console.WriteLine($"Человек с фамилией {surname} не найден в списке.");
        }

        Person SearchPerson (string surname)
        {
            Person personEdit = null;
            foreach (var person in people)
            {
                if (person.Surname.Equals(surname, StringComparison.OrdinalIgnoreCase))
                {
                    personEdit = person;
                    break;
                }
            }
            return personEdit;
        }
        DateTime CheckRightData()
        {
            DateTime birthday;
            while (!DateTime.TryParseExact(Console.ReadLine(), "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out birthday))
                Console.WriteLine("Некорректный формат даты. Введите дату рождения ещё раз (в формате dd.MM.yyyy):");
            return birthday;
        }

        void LoadFromFile() 
        {
            if (File.Exists(path))
            {
                using (StreamReader reader = new StreamReader(path, true))
                {
                    string text;
                    while ((text = reader.ReadLine()) != null)
                    {
                        var parts = text.Split(';');
                        var birthday = DateTime.ParseExact(parts[0].Trim(), "dd.MM.yyyy", CultureInfo.InvariantCulture);
                        var surname = parts[1].Trim();
                        var name = parts[2].Trim();

                        people.Add(new Person(birthday, surname, name));
                    }
                }
            }
        }

        void Save()
        {
            using (StreamWriter stream = new StreamWriter(path))
            {
                foreach (var person in people)
                    stream.WriteLine($"{person.Birthday.ToShortDateString()}; {person.Surname}; {person.Name};");
            }
        }
    }

    class Person
    {
        public DateTime Birthday { get; set; }
        public string Surname { get; set; }
        public string Name { get; set; }

        public Person(DateTime birthday, string surname, string name) 
        { 
            Birthday = birthday.Date;
            Surname = surname;
            Name = name;
        }
        public bool BirthdayTodayOrUpcoming()
        {
            DateTime today = DateTime.Today;
            return (Birthday.Month > today.Month || (Birthday.Month == today.Month && Birthday.Day >= today.Day));
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {  
            Birthday birthday = new Birthday();
            birthday.RunMenu();
        }
    }
}