using ConsoleOfTheDamned.Terminal.Models;

namespace ConsoleOfTheDamned.Terminal.Core;

public class Game
{
    private static readonly string[] EnemyNames =
    [
        "Graveling",
        "Crypt Mote",
        "Fetid Ghoul",
        "Bone Knight",
        "Woe Maggot",
        "Cultist Adept",
        "Hollow Priest"
    ];

    private static readonly string[] LootLines =
    [
        "You pry a blinking coin from the dust—ominous… but valid tender.",
        "The pockets hold mostly lint and a little gold. Lint discarded.",
        "A ring from a cold finger; the console approves your bling.",
        "A cracked sigil—useless, but the scattered gold is not."
    ];

    private static readonly string[] BattleActions =
    [
        "1) Attack",
        "2) Drink blood vial (heal)",
        "3) Run"
    ];

    private static readonly Random Rng = new();

    public static void Run()
    {
        PrintTitle();

        Console.Write("State your name, lost soul: ");
        var name = NonEmptyInput();

        var player = CreatePlayerViaClassChoice(name);

        bool running = true;
        while (running && player.IsAlive)
        {
            Console.WriteLine();
            Console.WriteLine("=== Console of the Damned ===");
            Console.WriteLine("1) Venture forth");
            Console.WriteLine("2) Rest");
            Console.WriteLine("3) Status");
            Console.WriteLine("4) Quit");
            Console.Write("Damned> ");

            string choice = Console.ReadLine()?.Trim() ?? "";
            switch (choice)
            {
                case "1":
                    Adventure(player);
                    break;
                case "2":
                    player.Rest();
                    break;
                case "3":
                    player.PrintStatus();
                    break;
                case "4":
                    running = false;
                    Console.WriteLine("The console fades… the whispers do not.");
                    break;
                default:
                    Console.WriteLine("Unrecognized command. The terminal chuckles without a voice.");
                    break;
            }
        }

        if (!player.IsAlive)
            Console.WriteLine("\n💀 GAME OVER: The console etches a new curse in the log.");

        Console.WriteLine("\n(Press Enter to sever the connection…)");
        Console.ReadLine();
    }

    private static void Adventure(Player player)
    {
        var enemy = CreateRandomEnemy(player);
        Console.WriteLine($"\n👁️ The screen flickers. A {enemy.Name} materializes!");

        // Combat loop
        while (player.IsAlive && enemy.IsAlive)
        {
            Console.WriteLine();
            Console.WriteLine(string.Join("  |  ", BattleActions));
            Console.Write("Damned/Battle> ");
            var pick = Console.ReadLine()?.Trim();

            if (pick == "1")
            {
                PlayerAttack(player, enemy);
                if (!enemy.IsAlive) break;
                EnemyAttack(player, enemy);
            }
            else
            {
                switch (pick)
                {
                    case "2":
                    {
                        player.HealSmall();
                        if (enemy.IsAlive) EnemyAttack(player, enemy);
                        break;
                    }
                    case "3" when TryRun():
                        Console.WriteLine("🏃 You slip between lines of code and vanish.");
                        return;
                    case "3":
                        Console.WriteLine("The shadows hang. Escape failed!");
                        EnemyAttack(player, enemy);
                        break;
                    default:
                        Console.WriteLine("Hesitation invites pain. The foe strikes!");
                        EnemyAttack(player, enemy);
                        break;
                }
            }
        }

        if (!player.IsAlive || enemy.IsAlive)
            return;

        Console.WriteLine($"\n✔ {enemy.Name} collapses into static and dust.");
        int reward = enemy.GoldReward + Rng.Next(0, 3);
        player.AddGold(reward);
        Console.WriteLine("— " + LootLines[Rng.Next(LootLines.Length)]);
    }

    private static void PlayerAttack(Player player, Enemy enemy)
    {
        int dmg = player.AttackRoll(Rng);
        enemy.TakeDamage(dmg);
        Console.WriteLine($"🗡️ {player.Name} hits {enemy.Name} for {dmg} damage (Enemy HP: {enemy.HealthPoints}).");
    }

    private static void EnemyAttack(Player player, Enemy enemy)
    {
        if (!enemy.IsAlive) return;
        int dmg = enemy.AttackRoll(Rng);
        player.TakeDamage(dmg);
        Console.WriteLine(
            $"👹 {enemy.Name} strikes you for {dmg} damage! ({player.HealthPoints}/{player.MaxHealthPoints} HP)");
    }

    private static bool TryRun() => Rng.NextDouble() < 0.5;

    // Light enemy scaling by player's accumulated gold to add tension
    private static Enemy CreateRandomEnemy(Player player)
    {
        var name = EnemyNames[Rng.Next(EnemyNames.Length)];
        int baseHp = 8 + (player.Gold / 5);
        int baseDmg = 2 + (player.Gold / 10);
        int reward = 3 + Rng.Next(0, 4);

        baseHp = Math.Clamp(baseHp, 8, 30);
        baseDmg = Math.Clamp(baseDmg, 2, 8);

        return new Enemy(name, baseHp, baseDmg, reward);
    }

    private static Player CreatePlayerViaClassChoice(string name)
    {
        Console.WriteLine("\nChoose your curse (class):");
        Console.WriteLine("1) Hellwalker — sturdy, consistent damage");
        Console.WriteLine("2) Bone Weaver — fragile, higher damage, a touch of mana");
        Console.WriteLine("3) Shadow Bard — in-between, dangerously stylish");

        while (true)
        {
            Console.Write("Damned/Class> ");
            var pick = Console.ReadLine()?.Trim();

            switch (pick)
            {
                case "1":
                    Console.WriteLine("You don spiked sabatons. The console nods.");
                    return new Player(name, "Hellwalker", maxHealthPoints: 28, damage: 5, gold: 3, manaPoints: null,
                        bloodVials: 1);
                case "2":
                    Console.WriteLine("Runes and bone glimmer—power at the cost of softness.");
                    return new Player(name, "Bone Weaver", maxHealthPoints: 20, damage: 7, gold: 2, manaPoints: 5,
                        bloodVials: 1);
                case "3":
                    Console.WriteLine("A chord in shadow; even the cursor winces.");
                    return new Player(name, "Shadow Bard", maxHealthPoints: 24, damage: 6, gold: 3, manaPoints: 2,
                        bloodVials: 1);
                default:
                    Console.WriteLine("Invalid choice. The terminal hums a funeral march.");
                    break;
            }
        }
    }

    private static string NonEmptyInput()
    {
        while (true)
        {
            var input = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(input)) return input.Trim();
            Console.Write("Nameless heroes are deleted first. Try a name: ");
        }
    }

    private static void PrintTitle()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.Clear();

        Console.WriteLine("╔══════════════════════════════════════════════════════════╗");
        Console.WriteLine("║                                                          ║");
        Console.WriteLine("║               C O N S O L E   O F   T H E                ║");
        Console.WriteLine("║                      D  A  M  N  E  D                    ║");
        Console.WriteLine("║                                                          ║");
        Console.WriteLine("╚══════════════════════════════════════════════════════════╝");
        Console.WriteLine("A text-based crawler where the terminal stares back.\n");
    }
}