using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace VideoCardCatalogCourseWork
{
    // Базовий клас. Містить спільну інформацію про відеокарти.
    abstract class VideoCard
    {
        public int Code { get; set; }
        public string Manufacturer { get; set; }
        public string Model { get; set; }
        public string CardClass { get; set; }
        public int MemoryMb { get; set; }
        public string MemoryType { get; set; }
        public int MemoryFrequencyMHz { get; set; }
        public string Resolution { get; set; }
        public decimal Price { get; set; }

        public abstract string Type { get; }

        protected VideoCard()
        {
            Code = 0;
            Manufacturer = "Невідомо";
            Model = "Невідомо";
            CardClass = "Невідомо";
            MemoryMb = 0;
            MemoryType = "Невідомо";
            MemoryFrequencyMHz = 0;
            Resolution = "Невідомо";
            Price = 0;
        }

        protected VideoCard(int code, string manufacturer, string model, string cardClass,
            int memoryMb, string memoryType, int memoryFrequencyMHz, string resolution, decimal price)
        {
            Code = code;
            Manufacturer = manufacturer;
            Model = model;
            CardClass = cardClass;
            MemoryMb = memoryMb;
            MemoryType = memoryType;
            MemoryFrequencyMHz = memoryFrequencyMHz;
            Resolution = resolution;
            Price = price;
        }

        // Віртуальний метод, який перевизначається у похідних класах.
        public virtual decimal CalculatePurchasePrice(int quantity)
        {
            return Price * quantity;
        }

        public string ToFileLine()
        {
            return string.Join(";",
                Type,
                Code,
                Manufacturer,
                Model,
                CardClass,
                MemoryMb,
                MemoryType,
                MemoryFrequencyMHz,
                Resolution,
                Price.ToString(CultureInfo.InvariantCulture));
        }

        public override string ToString()
        {
            return $"Код: {Code}\n" +
                   $"Виробник: {Manufacturer}\n" +
                   $"Модель: {Model}\n" +
                   $"Клас: {CardClass}\n" +
                   $"Обсяг пам'яті: {MemoryMb} МБ\n" +
                   $"Тип пам'яті: {MemoryType}\n" +
                   $"Частота пам'яті: {MemoryFrequencyMHz} МГц\n" +
                   $"Роздільна здатність: {Resolution}\n" +
                   $"Ціна: {Price:F2} грн";
        }
    }

    // Похідний клас для геймерських відеокарт.
    class GamingVideoCard : VideoCard
    {
        public override string Type => "Gaming";

        public GamingVideoCard()
        {
            CardClass = "Геймерська";
        }

        public GamingVideoCard(int code, string manufacturer, string model, int memoryMb,
            string memoryType, int memoryFrequencyMHz, string resolution, decimal price)
            : base(code, manufacturer, model, "Геймерська", memoryMb, memoryType, memoryFrequencyMHz, resolution, price)
        {
        }

        public override decimal CalculatePurchasePrice(int quantity)
        {
            decimal total = Price * quantity;
            decimal discount = 0;

            // За умовою: для геймерської відеокарти дорожче 30000 грн знижка 5%.
            if (Price > 30000)
            {
                discount += 0.05m;
            }

            // За умовою: при купівлі 3 і більше геймерських відеокарт додаткова знижка 10%.
            if (quantity >= 3)
            {
                discount += 0.10m;
            }

            return total * (1 - discount);
        }
    }

    // Похідний клас для мультимедійних та інших відеокарт.
    class MultimediaVideoCard : VideoCard
    {
        public override string Type => "Multimedia";

        public MultimediaVideoCard()
        {
            CardClass = "Мультимедійна";
        }

        public MultimediaVideoCard(int code, string manufacturer, string model, string cardClass,
            int memoryMb, string memoryType, int memoryFrequencyMHz, string resolution, decimal price)
            : base(code, manufacturer, model, cardClass, memoryMb, memoryType, memoryFrequencyMHz, resolution, price)
        {
        }

        public override decimal CalculatePurchasePrice(int quantity)
        {
            decimal total = Price * quantity;
            decimal discount = 0;

            // За умовою: для інших відеокарт дорожче 10000 грн знижка 3%.
            if (Price > 10000)
            {
                discount = 0.03m;
            }

            return total * (1 - discount);
        }
    }

    class Program
    {
        private const string FileName = "videocards.txt";
        private static readonly List<VideoCard> VideoCards = new List<VideoCard>();

        static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            EnsureFileExists();
            LoadFromFile();

            bool isRunning = true;
            while (isRunning)
            {
                ShowMenu();
                Console.Write("Оберіть пункт меню: ");
                string choice = Console.ReadLine() ?? "";
                Console.Clear();

                switch (choice)
                {
                    case "1":
                        ShowAll(VideoCards);
                        break;
                    case "2":
                        AddVideoCard();
                        break;
                    case "3":
                        DeleteVideoCard();
                        break;
                    case "4":
                        EditVideoCard();
                        break;
                    case "5":
                        ShowSortedByPrice();
                        break;
                    case "6":
                        ShowGamingCards();
                        break;
                    case "7":
                        SearchByManufacturer();
                        break;
                    case "8":
                        FindCheapestGamingCard();
                        break;
                    case "9":
                        CalculatePriceWithDiscount();
                        break;
                    case "10":
                        SaveToFile();
                        Console.WriteLine("Дані збережено у файл.");
                        break;
                    case "0":
                        SaveToFile();
                        isRunning = false;
                        break;
                    default:
                        Console.WriteLine("Невірний пункт меню.");
                        break;
                }

                if (isRunning)
                {
                    Console.WriteLine("\nНатисніть Enter для продовження...");
                    Console.ReadLine();
                    Console.Clear();
                }
            }
        }

        static void ShowMenu()
        {
            Console.WriteLine("=== Каталог відеокарт ===");
            Console.WriteLine("1. Переглянути всі відеокарти");
            Console.WriteLine("2. Додати відеокарту");
            Console.WriteLine("3. Видалити відеокарту");
            Console.WriteLine("4. Редагувати відеокарту");
            Console.WriteLine("5. Показати відеокарти, впорядковані за ціною");
            Console.WriteLine("6. Відібрати тільки геймерські відеокарти");
            Console.WriteLine("7. Пошук відеокарт за виробником");
            Console.WriteLine("8. Знайти найдешевшу геймерську відеокарту");
            Console.WriteLine("9. Обчислити вартість купівлі зі знижкою");
            Console.WriteLine("10. Зберегти дані у файл");
            Console.WriteLine("0. Вихід");
            Console.WriteLine();
        }

        static void EnsureFileExists()
        {
            if (File.Exists(FileName))
            {
                return;
            }

            string[] defaultData =
            {
                "Gaming;1;GigaByte;RTX 4070 Gaming OC;Геймерська;12288;GDDR6X;21000;7680x4320;36500",
                "Multimedia;2;ASUS;GT 1030 Silent;Мультимедійна;2048;GDDR5;6008;4096x2160;4800",
                "Gaming;3;MSI;RTX 3060 Ventus;Геймерська;12288;GDDR6;15000;7680x4320;31000",
                "Multimedia;4;Sapphire;RX 6400 Pulse;Мультимедійна;4096;GDDR6;16000;7680x4320;11500",
                "Gaming;5;PowerColor;RX 6700 XT Fighter;Геймерська;12288;GDDR6;16000;7680x4320;28500"
            };

            File.WriteAllLines(FileName, defaultData, Encoding.UTF8);
        }

        static void LoadFromFile()
        {
            VideoCards.Clear();

            string[] lines = File.ReadAllLines(FileName, Encoding.UTF8);
            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                string[] parts = line.Split(';');
                if (parts.Length != 10)
                {
                    continue;
                }

                string type = parts[0];
                int code = int.Parse(parts[1]);
                string manufacturer = parts[2];
                string model = parts[3];
                string cardClass = parts[4];
                int memoryMb = int.Parse(parts[5]);
                string memoryType = parts[6];
                int memoryFrequency = int.Parse(parts[7]);
                string resolution = parts[8];
                decimal price = decimal.Parse(parts[9], CultureInfo.InvariantCulture);

                if (type == "Gaming")
                {
                    VideoCards.Add(new GamingVideoCard(code, manufacturer, model, memoryMb, memoryType, memoryFrequency, resolution, price));
                }
                else
                {
                    VideoCards.Add(new MultimediaVideoCard(code, manufacturer, model, cardClass, memoryMb, memoryType, memoryFrequency, resolution, price));
                }
            }
        }

        static void SaveToFile()
        {
            List<string> lines = VideoCards.Select(card => card.ToFileLine()).ToList();
            File.WriteAllLines(FileName, lines, Encoding.UTF8);
        }

        static void ShowAll(IEnumerable<VideoCard> cards)
        {
            // Action використовується для виконання дії над кожним елементом колекції.
            Action<VideoCard> printCard = card =>
            {
                Console.WriteLine(card);
                Console.WriteLine(new string('-', 45));
            };

            List<VideoCard> list = cards.ToList();
            if (list.Count == 0)
            {
                Console.WriteLine("Записів не знайдено.");
                return;
            }

            list.ForEach(printCard);
        }

        static void AddVideoCard()
        {
            Console.WriteLine("Додавання відеокарти");
            Console.WriteLine("1. Геймерська");
            Console.WriteLine("2. Мультимедійна / інша");
            Console.Write("Оберіть тип: ");
            string typeChoice = Console.ReadLine() ?? "";

            int code = ReadInt("Код товару: ");
            string manufacturer = ReadString("Виробник: ");
            string model = ReadString("Модель: ");
            int memoryMb = ReadInt("Обсяг пам'яті, МБ: ");
            string memoryType = ReadString("Тип пам'яті: ");
            int memoryFrequency = ReadInt("Частота пам'яті, МГц: ");
            string resolution = ReadString("Роздільна здатність: ");
            decimal price = ReadDecimal("Вартість, грн: ");

            if (typeChoice == "1")
            {
                VideoCards.Add(new GamingVideoCard(code, manufacturer, model, memoryMb, memoryType, memoryFrequency, resolution, price));
            }
            else
            {
                string cardClass = ReadString("Клас відеокарти: ");
                VideoCards.Add(new MultimediaVideoCard(code, manufacturer, model, cardClass, memoryMb, memoryType, memoryFrequency, resolution, price));
            }

            SaveToFile();
            Console.WriteLine("Відеокарту додано.");
        }

        static void DeleteVideoCard()
        {
            int code = ReadInt("Введіть код відеокарти для видалення: ");
            VideoCard? card = VideoCards.FirstOrDefault(x => x.Code == code);

            if (card == null)
            {
                Console.WriteLine("Відеокарту з таким кодом не знайдено.");
                return;
            }

            VideoCards.Remove(card);
            SaveToFile();
            Console.WriteLine("Відеокарту видалено.");
        }

        static void EditVideoCard()
        {
            int code = ReadInt("Введіть код відеокарти для редагування: ");
            VideoCard? card = VideoCards.FirstOrDefault(x => x.Code == code);

            if (card == null)
            {
                Console.WriteLine("Відеокарту з таким кодом не знайдено.");
                return;
            }

            Console.WriteLine("Поточні дані:");
            Console.WriteLine(card);
            Console.WriteLine();

            card.Manufacturer = ReadString("Новий виробник: ");
            card.Model = ReadString("Нова модель: ");
            card.MemoryMb = ReadInt("Новий обсяг пам'яті, МБ: ");
            card.MemoryType = ReadString("Новий тип пам'яті: ");
            card.MemoryFrequencyMHz = ReadInt("Нова частота пам'яті, МГц: ");
            card.Resolution = ReadString("Нова роздільна здатність: ");
            card.Price = ReadDecimal("Нова вартість, грн: ");

            if (card is MultimediaVideoCard)
            {
                card.CardClass = ReadString("Новий клас відеокарти: ");
            }

            SaveToFile();
            Console.WriteLine("Дані оновлено.");
        }

        static void ShowSortedByPrice()
        {
            List<VideoCard> sorted = VideoCards
                .OrderBy(card => card.Price)
                .ToList();

            ShowAll(sorted);
        }

        static void ShowGamingCards()
        {
            // Predicate використовується для перевірки умови відбору.
            Predicate<VideoCard> isGaming = card => card is GamingVideoCard;

            List<VideoCard> result = VideoCards
                .Where(card => isGaming(card))
                .ToList();

            ShowAll(result);
        }

        static void SearchByManufacturer()
        {
            string manufacturer = ReadString("Введіть виробника: ");

            Predicate<VideoCard> hasManufacturer = card =>
                card.Manufacturer.Contains(manufacturer, StringComparison.OrdinalIgnoreCase);

            List<VideoCard> result = VideoCards
                .Where(card => hasManufacturer(card))
                .ToList();

            ShowAll(result);
        }

        static void FindCheapestGamingCard()
        {
            VideoCard? cheapest = VideoCards
                .Where(card => card is GamingVideoCard)
                .OrderBy(card => card.Price)
                .FirstOrDefault();

            if (cheapest == null)
            {
                Console.WriteLine("Геймерські відеокарти не знайдено.");
                return;
            }

            Console.WriteLine("Найдешевша геймерська відеокарта:");
            Console.WriteLine(cheapest);
        }

        static void CalculatePriceWithDiscount()
        {
            int code = ReadInt("Введіть код відеокарти: ");
            VideoCard? card = VideoCards.FirstOrDefault(x => x.Code == code);

            if (card == null)
            {
                Console.WriteLine("Відеокарту з таким кодом не знайдено.");
                return;
            }

            int quantity = ReadInt("Введіть кількість: ");
            decimal total = card.Price * quantity;
            decimal result = card.CalculatePurchasePrice(quantity);
            decimal discount = total - result;

            Console.WriteLine("Обрана відеокарта:");
            Console.WriteLine(card);
            Console.WriteLine();
            Console.WriteLine($"Кількість: {quantity}");
            Console.WriteLine($"Вартість без знижки: {total:F2} грн");
            Console.WriteLine($"Сума знижки: {discount:F2} грн");
            Console.WriteLine($"Вартість до сплати: {result:F2} грн");
        }

        static string ReadString(string message)
        {
            Console.Write(message);
            string? value = Console.ReadLine();
            return string.IsNullOrWhiteSpace(value) ? "Невідомо" : value.Trim();
        }

        static int ReadInt(string message)
        {
            while (true)
            {
                Console.Write(message);
                if (int.TryParse(Console.ReadLine(), out int value) && value >= 0)
                {
                    return value;
                }

                Console.WriteLine("Помилка. Введіть невід'ємне ціле число.");
            }
        }

        static decimal ReadDecimal(string message)
        {
            while (true)
            {
                Console.Write(message);
                string input = (Console.ReadLine() ?? "").Replace(',', '.');

                if (decimal.TryParse(input, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal value) && value >= 0)
                {
                    return value;
                }

                Console.WriteLine("Помилка. Введіть число, наприклад 12500 або 12500.50.");
            }
        }
    }
}
