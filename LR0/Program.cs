
using System.Linq;
using System;
using System.Collections.Generic;

class Product
{
    public string Name { get; }
    public decimal Price { get; }
    public int Quantity { get; set; }

    public Product(string name, decimal price, int quantity)
    {
        Name = name;
        Price = price;
        Quantity = quantity;
    }

    public override string ToString() => $"{Name} — {Price}₽ (осталось: {Quantity})";
}

static class Coin
{
    public static readonly decimal[] Nominals = { 1, 2, 5, 10, 50 };
}

class VendingMachineService
{
    private List<Product> products;
    private decimal insertedMoney = 0;
    private decimal collectedMoney = 0;

    public VendingMachineService()
    {
        products = new List<Product>
        {
            new Product("Кофе", 50m, 5),
            new Product("Чипсы", 80m, 3),
            new Product("Шоколад", 60m, 4),
            new Product("Вода", 40m, 6)
        };
    }

    public void Run()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== ВЕНДИНГОВЫЙ АВТОМАТ ===");
            Console.WriteLine($"Внесено: {insertedMoney}₽");
            Console.WriteLine("1 — Показать товары");
            Console.WriteLine("2 — Вставить монету");
            Console.WriteLine("3 — Купить товар");
            Console.WriteLine("4 — Отмена и возврат сдачи");
            Console.WriteLine("9 — Админ режим");
            Console.Write("Выберите: ");

            switch(Console.ReadLine())
            {
                case "1": ShowProducts(); break;
                case "2": InsertCoin(); break;
                case "3": BuyProduct(); break;
                case "4": CancelOperation(); break;
                case "9": AdminMode(); break;
            }
        }
    }

    private void ShowProducts()
    {
        Console.Clear();
        Console.WriteLine("=== СПИСОК ТОВАРОВ ===");
        for(int i = 0; i < products.Count; i++)
            Console.WriteLine($"{i+1}. {products[i]}");
        Console.ReadKey();
    }

    private void InsertCoin()
    {
        Console.Clear();
        Console.WriteLine("=== ДОСТУПНЫЕ МОНЕТЫ ===");
        for (int i = 0; i < Coin.Nominals.Length; i++)
            Console.WriteLine($"{i+1}. {Coin.Nominals[i]}₽");

        Console.Write("Выберите: ");
        if (int.TryParse(Console.ReadLine(), out int choice)
            && choice >= 1 && choice <= Coin.Nominals.Length)
        {
            insertedMoney += Coin.Nominals[choice - 1];
        }
    }

    private void BuyProduct()
    {
        Console.Clear();
        ShowProducts();
        Console.Write("Введите номер товара: ");
        if (!int.TryParse(Console.ReadLine(), out int index) ||
            index < 1 || index > products.Count)
            return;

        var product = products[index-1];

        if (product.Quantity <= 0)
        {
            Console.WriteLine("Нет в наличии.");
            Console.ReadKey();
            return;
        }

        if (insertedMoney < product.Price)
        {
            Console.WriteLine($"Недостаточно средств! Нужно ещё {product.Price - insertedMoney}₽");
            Console.ReadKey();
            return;
        }

        product.Quantity--;
        insertedMoney -= product.Price;
        collectedMoney += product.Price;

        Console.WriteLine($"Вы получили: {product.Name}");
        Console.ReadKey();
    }

    private void CancelOperation()
    {
        Console.WriteLine($"Возврат {insertedMoney}₽");
        insertedMoney = 0;
        Console.ReadKey();
    }

    private void AdminMode()
    {
        Console.Clear();
        Console.Write("Введите пароль администратора: ");
        string input = Console.ReadLine() ?? string.Empty;
        if (input != "admin")
        {
            Console.WriteLine("Неверный пароль.");
            Console.ReadKey();
            return;
        }

        while(true)
        {
            Console.Clear();
            Console.WriteLine("=== АДМИН РЕЖИМ ===");
            Console.WriteLine($"Собрано денег: {collectedMoney}₽");
            Console.WriteLine("1 — Пополнить товар");
            Console.WriteLine("2 — Обнулить кассу");
            Console.WriteLine("0 — Выход");
            Console.Write("Выберите: ");

            string cmd = Console.ReadLine();
            if (cmd == "0") break;
            if (cmd == "2") { collectedMoney = 0; continue; }
            if (cmd == "1") RestockProduct();
        }
    }

    private void RestockProduct()
    {
        Console.Clear();
        ShowProducts();
        Console.Write("Выберите товар для пополнения: ");

        if (int.TryParse(Console.ReadLine(), out int index)
            && index >= 1 && index <= products.Count)
        {
            Console.Write("На сколько пополнить? ");
            if (int.TryParse(Console.ReadLine(), out int addQty) && addQty > 0)
            {
                products[index - 1].Quantity += addQty;
            }
        }
    }
}

class Program
{
    static void Main()
    {
        VendingMachineService vm = new VendingMachineService();
        vm.Run();
    }
}
