namespace ConsoleOfTheDamned.Terminal.Models;

public class Player
{
    public string Name { get; }
    public string ClassName { get; }
    public int MaxHealthPoints { get; private set; }
    public int HealthPoints { get; private set; }
    public int Damage { get; private set; }
    public int Gold { get; private set; }
    public int? ManaPoints { get; private set; }
    public int BloodVials { get; private set; } // limited heals

    public bool IsAlive => HealthPoints > 0;

    public Player(
        string name,
        string className,
        int maxHealthPoints,
        int damage,
        int gold,
        int? manaPoints = null,
        int bloodVials = 1)
    {
        Name = name;
        ClassName = className;
        MaxHealthPoints = maxHealthPoints;
        HealthPoints = maxHealthPoints;
        Damage = damage;
        Gold = gold;
        ManaPoints = manaPoints;
        BloodVials = bloodVials;
    }

    public int AttackRoll(Random rng) => Damage + rng.Next(0, 3); // tiny variance

    public void HealSmall()
    {
        if (BloodVials <= 0)
        {
            Console.WriteLine("✖ Your blood vials are empty—the console hums disapprovingly.");
            return;
        }

        int heal = Math.Max(3, MaxHealthPoints / 5);
        HealthPoints = Math.Min(MaxHealthPoints, HealthPoints + heal);
        BloodVials--;
        Console.WriteLine(
            $"🧪 You drink a blood vial and recover {heal} HP. ({HealthPoints}/{MaxHealthPoints} HP, {BloodVials} left)");
    }

    public void Rest()
    {
        int heal = Math.Max(2, MaxHealthPoints / 10);
        HealthPoints = Math.Min(MaxHealthPoints, HealthPoints + heal);
        Console.WriteLine($"🛌 You doze beneath the monitor’s glow. +{heal} HP ({HealthPoints}/{MaxHealthPoints}).");
    }

    public void AddGold(int amount)
    {
        Gold += amount;
        Console.WriteLine($"🪙 Looted {amount} gold. Total: {Gold}.");
    }

    public void TakeDamage(int amount)
    {
        HealthPoints = Math.Max(0, HealthPoints - amount);
    }

    public void PrintStatus()
    {
        Console.WriteLine(new string('─', 42));
        Console.WriteLine($"🔮 {Name} the {ClassName}");
        Console.WriteLine($"❤️ HP: {HealthPoints}/{MaxHealthPoints}   🗡️ Damage: {Damage}   🪙 Gold: {Gold}" +
                          (ManaPoints.HasValue ? $"   ✨ Mana: {ManaPoints}" : ""));
        Console.WriteLine($"🧪 Blood Vials: {BloodVials}");
        Console.WriteLine(new string('─', 42));
    }
}