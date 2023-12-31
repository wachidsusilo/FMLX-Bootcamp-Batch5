﻿namespace Project1.Animal;

public class Cat
{
    public string Name;
    public string Color;
    public string Gender;
    public int Age;
    public string Race;
    public bool IsStripe;

    public Cat() : this("", "", "", 0, "", false)
    {
    }

    public Cat(string name, string color, string gender, int age, string race, bool isStripe)
    {
        Name = name;
        Color = color;
        Gender = gender;
        Age = age;
        Race = race;
        IsStripe = isStripe;
    }

    public void Meow()
    {
        Console.WriteLine("Meow");
    }

    public void Meow(string text)
    {
        Console.WriteLine($"Meow {text}");
    }

    public bool Meow(int secondsOfDay)
    {
        if (secondsOfDay is <= 6 * 3600 or >= 18 * 3600)
        {
            return false;
        }
        
        Meow();
        return true;
    }

    public void Eat()
    {
        Console.WriteLine("Eat");
    }

    public void Jump()
    {
        Console.WriteLine("Jump");
    }
}