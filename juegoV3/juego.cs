using System;

// ===================== EQUIPO =====================
class Equipo
{
    private int modificadorAtaque;
    private int modificadorArmadura;

    public Equipo(int modificadorAtaque, int modificadorArmadura)
    {
        this.modificadorAtaque = modificadorAtaque;
        this.modificadorArmadura = modificadorArmadura;
    }

    public int GetModificadorAtaque() => modificadorAtaque;
    public int GetModificadorArmadura() => modificadorArmadura;
}

class Arma : Equipo { public Arma(int mod) : base(mod, 0) { } }
class Armadura : Equipo { public Armadura(int mod) : base(0, mod) { } }

// ===================== PERSONAJE =====================
class Personaje
{
    private string nombre;
    private int vida;
    private int ataque;
    private Equipo equipo;

    public Personaje(string nombre, int vida, int ataque)
    {
        this.nombre = nombre;
        this.vida = vida;
        this.ataque = ataque;
        this.equipo = null;
    }

    public string GetNombre() => nombre;
    public int GetVida() => vida;

    public virtual int GetAtaque()
    {
        int bono = (equipo != null) ? equipo.GetModificadorAtaque() : 0;
        return ataque + bono;
    }

    public virtual int GetArmadura()
    {
        return (equipo != null) ? equipo.GetModificadorArmadura() : 0;
    }

    public virtual int CalcularDanio() => GetAtaque();

    public virtual void AplicarDanio(int danio) => RecibirDanio(danio);

    public virtual void RecibirDanio(int danio)
    {
        int danioTotal = danio - GetArmadura();
        if (danioTotal < 1) danioTotal = 1;

        vida -= danioTotal;
        if (vida < 0) vida = 0;

        if (vida == 0)
            Console.WriteLine($"{nombre} recibe {danioTotal} de daño. ¡Ha muerto!");
        else
            Console.WriteLine($"{nombre} recibe {danioTotal} de daño. (Vida: {vida})");
    }

    public void Equipar(Equipo equipo) => this.equipo = equipo;
    public void QuitarEquipo() => this.equipo = null;
    public bool EstaVivo() => vida > 0;
}

// ===================== SUBCLASES =====================
class Sacerdote : Personaje
{
    private static Random random = new Random();
    public Sacerdote(string n, int v, int a) : base(n, v, a) { }

    public override void RecibirDanio(int danio)
    {
        if (random.Next(1, 5) == 1)
        {
            Console.WriteLine($"¡Las plegarias de {GetNombre()} reducen el daño a la mitad!");
            danio /= 2;
        }
        base.RecibirDanio(danio);
    }
}

class Barbaro : Personaje
{
    private int furia;
    public Barbaro(string n, int v, int a, int f) : base(n, v, a) { this.furia = f; }

    public override int CalcularDanio()
    {
        int danioBase = GetAtaque();
        if (furia >= 3)
        {
            Console.WriteLine($"{GetNombre()} ataca furioso (+15% daño)");
            furia -= 3;
            return (int)(danioBase * 1.15);
        }
        Console.WriteLine($"{GetNombre()} está cansado (50% daño)");
        return (int)(danioBase * 0.5);
    }
}

class Musashi : Personaje
{
    private static Random random = new Random();
    private bool desarmar = false;

    public Musashi(string n, int v, int a) : base(n, v, a) { }

    public override int CalcularDanio()
    {
        desarmar = (random.Next(1, 5) == 1);
        if (desarmar) Console.WriteLine($"{GetNombre()} prepara un intento de desarme...");
        return GetAtaque();
    }

    public void AplicarHabilidad(Personaje objetivo)
    {
        if (desarmar && objetivo.EstaVivo())
        {
            Console.WriteLine($"{GetNombre()} ha desarmado a {objetivo.GetNombre()}!");
            objetivo.QuitarEquipo();
        }
    }

    public override int GetArmadura() => 0; // Musashi no confía en armaduras
}

// ===================== JUEGO =====================
class Juego
{
    static void Main() => ronda();

    public static void ronda() // Corregido: sin punto y coma extra
    {
        Personaje p1 = new Barbaro("Dave", 30, 8, 10);
        Personaje p2 = new Sacerdote("Samson", 30, 7);

        p1.Equipar(new Arma(3));
        p2.Equipar(new Armadura(2));

        Personaje ganador = Batalla(p1, p2);

        if (ganador != null)
        {
            Console.WriteLine($"\n--- {ganador.GetNombre()} avanza a la Final ---");
            Personaje musashi = new Musashi("Musashi", 20, 5);
            musashi.Equipar(new Arma(2));

            Personaje ganadorFinal = Batalla(musashi, ganador);
            if (ganadorFinal != null)
                Console.WriteLine($"\nEL CAMPEÓN ES: {ganadorFinal.GetNombre()}");
            else
                Console.WriteLine("\nLa final terminó en un empate trágico.");
        }
        else
        {
            Console.WriteLine("\nAmbos murieron. Musashi no tiene oponente.");
        }
    }

    public static Personaje Batalla(Personaje p1, Personaje p2)
    {
        Console.WriteLine($"\n--- INICIO: {p1.GetNombre()} vs {p2.GetNombre()} ---");

        while (p1.EstaVivo() && p2.EstaVivo())
        {
            int d1 = p1.CalcularDanio();
            int d2 = p2.CalcularDanio();

            p2.AplicarDanio(d1);
            p1.AplicarDanio(d2);

            if (p1 is Musashi m1) m1.AplicarHabilidad(p2);
            if (p2 is Musashi m2) m2.AplicarHabilidad(p1);
            Console.WriteLine("--------------------");
        }

        if (p1.EstaVivo()) return p1;
        if (p2.EstaVivo()) return p2;
        return null;
    }
}
