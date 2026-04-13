using System;

namespace Juegov4;

class Equipo
{
    private int _modificadorAtaque;
    private int _modificadorArmadura;

    public Equipo(int modificadorAtaque, int modificadorArmadura)
    {
        _modificadorAtaque = modificadorAtaque;   // sin ambigüedad, sin this
        _modificadorArmadura = modificadorArmadura;
    }
    public int GetModificadorAtaque() => _modificadorAtaque;
    public int GetModificadorArmadura() => _modificadorArmadura;
}
class Arma : Equipo { public Arma(int mod) : base(mod, 0) { } }
class Armadura : Equipo { public Armadura(int mod) : base(0, mod) { } }
class Personaje
{
    private string _nombre;
    private int _vida;
    private int _ataque;
    private Equipo? _equipo;
    public Personaje(string nombre, int vida, int ataque)
    {
        _nombre = nombre;
        _vida = vida;
        _ataque = ataque;
        _equipo = null;
    }
    public string GetNombre() => _nombre;
    public int GetVida() => _vida;
    public virtual int GetAtaque()
    {
        int bono = (_equipo != null) ? _equipo.GetModificadorAtaque() : 0;
        return _ataque + bono;
    }
    public virtual int GetArmadura()
    {
        return (_equipo != null) ? _equipo.GetModificadorArmadura() : 0;
    }
    public virtual int CalcularDanio() => GetAtaque();
    public virtual void AplicarDanio(int danio) => RecibirDanio(danio);
    public virtual void RecibirDanio(int danio)
    {
        int danioTotal = danio - GetArmadura();
        if (danioTotal < 1) danioTotal = 1;
        _vida -= danioTotal;
        if (_vida < 0) _vida = 0;
        if (_vida == 0)
            Console.WriteLine($"{_nombre} recibe {danioTotal} de daño. ¡Ha muerto!");
        else
            Console.WriteLine($"{_nombre} recibe {danioTotal} de daño. (Vida: {_vida})");
    }
    public void Equipar(Equipo equipo) => _equipo = equipo;
    public void QuitarEquipo() => _equipo = null;
    public bool EstaVivo() => _vida > 0;
}
class Sacerdote : Personaje
{
    public Sacerdote(string n, int v, int a) : base(n, v, a) { }
    public override void RecibirDanio(int danio)
    {
        if (Random.Shared.Next(1, 5) == 1)
        {
            Console.WriteLine($"¡Las plegarias de {GetNombre()} reducen el daño a la mitad!");
            danio /= 2;
        }
        base.RecibirDanio(danio);
    }
}
class Barbaro : Personaje
{
    private int _furia;
    public Barbaro(string n, int v, int a, int f) : base(n, v, a)
    {
        _furia = f;
    }
    public override int CalcularDanio()
    {
        int danioBase = GetAtaque();
        if (_furia >= 3)
        {
            Console.WriteLine($"{GetNombre()} ataca furioso (+15% daño)");
            _furia -= 3;
            return (int)(danioBase * 1.15);
        }
        Console.WriteLine($"{GetNombre()} está cansado (50% daño)");
        return (int)(danioBase * 0.5);
    }
}
class Zamuray : Personaje
{
    private bool _desarmar = false;
    public Zamuray(string n, int v, int a) : base(n, v, a) { }
    public override int CalcularDanio()
    {
        _desarmar = (Random.Shared.Next(1, 5) == 1);
        if (_desarmar) Console.WriteLine($"{GetNombre()} prepara un intento de desarme...");
        return GetAtaque();
    }
    public void AplicarHabilidad(Personaje objetivo)
    {
        if (_desarmar && objetivo.EstaVivo()) // debe poder desarmar Y el rival debe estar vivo
        {
            Console.WriteLine($"{GetNombre()} ha desarmado a {objetivo.GetNombre()}!");
            objetivo.QuitarEquipo();
        }
    }
    public override int GetArmadura() => 0;
}

class Juego
{
    static void Main()
    {
        //primera batalla

            //si empatan los jugadores se reinicia la primera batalla hasta que gane uno para que el samurai pueda luchar con alguien
        Personaje ganador = null;
        bool primerIntento = true;

        while(ganador == null)
        {
            if (!primerIntento)
            {
                Console.WriteLine("La batalla se repite ya que no hubo ningún ganador");
            }


            //hasta acá
            primerIntento = false;
        
            Personaje p1 = new Barbaro("Dave", 30, 8, 11);
            Personaje p2 = new Sacerdote("Samson", 30, 7);

            p1.Equipar(new Arma(3));
            p2.Equipar(new Armadura(2));

            ganador = Batalla(p1, p2);
        }

        //hasta acá

        //segunda batalla
        if (ganador != null)
        {
            Console.WriteLine($"\n--- {ganador.GetNombre()} avanza a la Final ---");
            Personaje zamuray = new Zamuray("Musashi", 10, 5);
            zamuray.Equipar(new Arma(2));

            Personaje? ganadorFinal = Batalla(zamuray, ganador);
            if (ganadorFinal != null)
                Console.WriteLine($"\nEL CAMPEÓN ES: {ganadorFinal.GetNombre()}");
            else
                Console.WriteLine("\nLa final terminó en un empate trágico.");
        }
        else
        {
            Console.WriteLine("\nAmbos murieron. Musashi no tiene oponente.");
        }
        //hasta acá
    }
    
    public static Personaje? Batalla(Personaje p1, Personaje p2)
    {
        Console.WriteLine($"\n--- INICIO: {p1.GetNombre()} VS {p2.GetNombre()} ---");
        int contadorRonda = 0;

        while (p1.EstaVivo() && p2.EstaVivo())
        {
            contadorRonda++;
            Console.WriteLine($"------Ronda No {contadorRonda}--------------");
            Console.WriteLine($"{p1.GetNombre()} Y {p2.GetNombre()} Luchan ..");
            int d1 = p1.CalcularDanio();
            int d2 = p2.CalcularDanio();
            p2.AplicarDanio(d1);
            p1.AplicarDanio(d2);

            if (p1 is Zamuray m1) m1.AplicarHabilidad(p2); // 
            if (p2 is Zamuray m2) m2.AplicarHabilidad(p1); // 
        }

        if (p1.EstaVivo()) return p1;
        if (p2.EstaVivo()) return p2;
        return null;
    }
}
