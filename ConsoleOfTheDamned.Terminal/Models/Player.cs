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
            Console.WriteLine(Colors.Warning("✖ Your blood vials are empty—the console hums disapprovingly."));
            return;
        }

        int heal = Math.Max(3, MaxHealthPoints / 5);
        HealthPoints = Math.Min(MaxHealthPoints, HealthPoints + heal);
        BloodVials--;
        Console.WriteLine(
            Colors.Flavor("🧪 You drink a blood vial and recover ") + Colors.Highlight($"{heal}") + Colors.Flavor(" HP. (") +
            Colors.Highlight($"{HealthPoints}/{MaxHealthPoints}") + Colors.Flavor(" HP, ") + Colors.Highlight($"{BloodVials}") + Colors.Flavor(" left)")
        );
    }

    public void Rest()
    {
        int heal = Math.Max(2, MaxHealthPoints / 10);
        HealthPoints = Math.Min(MaxHealthPoints, HealthPoints + heal);
        Console.WriteLine(Colors.Flavor("🛌 You doze beneath the monitor’s glow. ") + Colors.Highlight($"+{heal}") + Colors.Flavor(" HP (") + Colors.Highlight($"{HealthPoints}/{MaxHealthPoints}") + Colors.Flavor(")."));
    }

    public void AddGold(int amount)
    {
        Gold += amount;
        Console.WriteLine(Colors.Flavor("🪙 Looted ") + Colors.Highlight($"{amount}") + Colors.Flavor(" gold. Total: ") + Colors.Highlight($"{Gold}") + Colors.Flavor("."));
    }

    public void TakeDamage(int amount)
    {
        HealthPoints = Math.Max(0, HealthPoints - amount);
    }

    public void PrintStatus()
    {
        Console.WriteLine(Colors.Gray + new string('─', 42) + Colors.Reset);
        Console.WriteLine("🔮 " + Colors.Highlight(Name) + Colors.Flavor(" the ") + Colors.Arcane(ClassName));
        Console.WriteLine(
            Colors.Flavor("❤️ HP: ") + Colors.Highlight($"{HealthPoints}/{MaxHealthPoints}") +
            Colors.Flavor("   🗡️ Damage: ") + Colors.Highlight($"{Damage}") +
            Colors.Flavor("   🪙 Gold: ") + Colors.Highlight($"{Gold}") +
            (ManaPoints.HasValue ? Colors.Flavor("   ✨ Mana: ") + Colors.Highlight($"{ManaPoints}") : string.Empty)
        );
        Console.WriteLine(Colors.Flavor("🧪 Blood Vials: ") + Colors.Highlight($"{BloodVials}"));
        Console.WriteLine(Colors.Gray + new string('─', 42) + Colors.Reset);
    }
}