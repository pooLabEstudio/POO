using System;

namespace EntendiendoHerencia;

// ===================== EQUIPO =====================
// ENCAPSULACIÓN: campos private, solo accesibles via getters públicos
class Equipo
{
    private int _modificadorAtaque;    // ENCAPSULACIÓN: acceso controlado
    private int _modificadorArmadura;  // ENCAPSULACIÓN: acceso controlado
    
    // convención anterior: private int modificadorAtaque;
    // convención anterior: private int modificadorArmadura;

    // Constructor de instancia — inicializa los modificadores del equipo
    // sin ambigüedad gracias a la convención _campo
    public Equipo(int modificadorAtaque, int modificadorArmadura)
    {
        _modificadorAtaque  = modificadorAtaque;   // sin ambigüedad, sin this
        _modificadorArmadura = modificadorArmadura;
        // anterior: this.modificadorAtaque = modificadorAtaque;
        // anterior: this.modificadorArmadura = modificadorArmadura;
    }

    // Getters públicos — única forma de leer los campos privados desde fuera
    public int GetModificadorAtaque()  => _modificadorAtaque;
    public int GetModificadorArmadura() => _modificadorArmadura;
}

// HERENCIA: Arma y Armadura heredan de Equipo, reutilizan su constructor via base()
// Arma solo aporta modificador de ataque — modificadorArmadura siempre es 0
// Armadura solo aporta modificador de armadura — modificadorAtaque siempre es 0
class Arma : Equipo { public Arma(int mod) : base(mod, 0) { } }
class Armadura : Equipo { public Armadura(int mod) : base(0, mod) { } }

// ===================== PERSONAJE =====================

// CANDIDATO A CLASE ABSTRACTA: Personaje nunca se instancia directamente en Main,
// siempre se usa a través de Barbaro, Sacerdote o Zamuray.
// Podría declararse abstract con CalcularDanio() como método abstracto.
class Personaje
{
    // ENCAPSULACIÓN: todos los campos son private, el estado interno está protegido
    private string _nombre;  // convención anterior: private string nombre;
    private int    _vida;    // convención anterior: private int vida;
    private int    _ataque;  // convención anterior: private int ataque;
    private Equipo? _equipo; // convención anterior: private Equipo? equipo;
                             // Equipo? nullable — un personaje puede no tener equipo

    // Constructor de Instancia
    // se ejecuta cada vez que se hace new Barbaro(...), new Sacerdote(...), etc.
    // las subclases lo invocan via base(n, v, a)
    public Personaje(string nombre, int vida, int ataque)
    {
        _nombre = nombre;   // anterior: this.nombre = nombre;
        _vida   = vida;     // anterior: this.vida   = vida;
        _ataque = ataque;   // anterior: this.ataque = ataque;
        _equipo = null;     // anterior: this.equipo = null;
                            // null = sin equipo al momento de crear el personaje
    }

    // METODOS DE ACCESO (getters)
    // permiten leer _nombre y _vida desde fuera sin exponer los campos directamente
    public string GetNombre() => _nombre;
    public int    GetVida()   => _vida;

    // METODOS ADICIONALES

    // POLIMORFISMO: virtual permite que subclases sobreescriban este comportamiento
    // hereda este metodo con sobreescritura
    public virtual int GetAtaque()
    {
        // si tiene equipo arma o armadura, obtiene el bono de ataque, sino 0
        // operador ternario: condicion ? valorSiTrue : valorSiFalse
        int bono = (_equipo != null) ? _equipo.GetModificadorAtaque() : 0;
        return _ataque + bono;
    }

    // POLIMORFISMO: virtual — Zamuray sobreescribe este método para retornar 0
    // virtual permite que subclases sobreescriban este comportamiento
    public virtual int GetArmadura()
    {
        // si tiene equipo (arma o armadura), obtiene el bono de armadura, sino 0
        return (_equipo != null) ? _equipo.GetModificadorArmadura() : 0;
    }

    // POLIMORFISMO: virtual — cada subclase define cómo calcula su daño
    // virtual permite que subclases sobreescriban este método para modificar el cálculo de daño
    // Barbaro y Zamuray lo sobreescriben — Sacerdote usa esta versión directamente
    public virtual int CalcularDanio() => GetAtaque();

    // POLIMORFISMO: virtual — permite interceptar el daño antes de aplicarlo
    // AplicarDanio es la puerta — RecibirDanio es lo que ocurre adentro
    // una subclase podría sobreescribirlo para bloquear o redirigir el daño
    public virtual void AplicarDanio(int danio) => RecibirDanio(danio);

    // POLIMORFISMO: virtual — Permite override de una clase hija para modificar 
    // cómo se recibe el daño (ej: sacerdote con reducción aleatoria)
    public virtual void RecibirDanio(int danio)
    {
        int danioTotal = danio - GetArmadura(); // Al daño que llegó le resta la armadura del personaje
        if (danioTotal < 1) danioTotal = 1;     // Garantiza daño mínimo de 1
                                                // sin esto podría recibir 0 o daño negativo (sanaría vida)

        _vida -= danioTotal;                    // Resta el daño a la vida
        if (_vida < 0) _vida = 0;              // evita vida negativa — mínimo 0

        // Si _vida llegó a 0 muestra "Ha muerto", si no muestra la vida restante
        if (_vida == 0)
            Console.WriteLine($"{_nombre} recibe {danioTotal} de daño. ¡Ha muerto!");
        else
            Console.WriteLine($"{_nombre} recibe {danioTotal} de daño. (Vida: {_vida})");
    }

    // métodos públicos NO virtual — comportamiento igual para todas las subclases
    public void Equipar(Equipo equipo) => _equipo = equipo; // Asigna un objeto Equipo al personaje
    public void QuitarEquipo()         => _equipo = null;   // Vuelve _equipo a null — lo usa Zamuray cuando desarma al rival
    public bool EstaVivo()             => _vida > 0;        // Retorna true si _vida es mayor a 0, false si es 0
}

// ===================== SUBCLASES =====================

// HERENCIA: Sacerdote hereda de Personaje
// En C# una clase solo puede heredar de una sola clase padre — herencia simple
// Sacerdote es una clase que hereda de otra clase (Personaje)
class Sacerdote : Personaje
{
    // Constructor — cuerpo vacío porque Sacerdote no tiene campos propios
    // todo lo inicializa el constructor de Personaje via base()
    public Sacerdote(string n, int v, int a) : base(n, v, a) { }

    // POLIMORFISMO: override — modifica cómo recibe el daño (reducción aleatoria)
    // sobreescribe RecibirDanio de Personaje — sin este override usaría la versión base
    public override void RecibirDanio(int danio)
    {
        // 25% de probabilidad (1 de cada 4) de reducir el daño a la mitad
        if (Random.Shared.Next(1, 5) == 1)
        {
            Console.WriteLine($"¡Las plegarias de {GetNombre()} reducen el daño a la mitad!");
            danio /= 2; // equivale a danio = danio / 2
        }
        base.RecibirDanio(danio); // llama al comportamiento base luego del ajuste
                                  // base ejecuta la lógica real de descontar vida
    }
}

// HERENCIA: Barbaro hereda de Personaje
class Barbaro : Personaje
{
    // ENCAPSULACIÓN: furia es estado interno privado de Bárbaro
    // ninguna otra clase puede leer ni modificar _furia directamente
    private int _furia;
    // convención anterior: private int furia;

    // string n, int v, int a → los 3 parámetros que necesita Personaje, se pasan via base()
    // int f → parámetro extra exclusivo de Barbaro, base() no lo conoce
    public Barbaro(string n, int v, int a, int f) : base(n, v, a)
    {
        _furia = f;
        // anterior: this.furia = f;
    }

    // POLIMORFISMO: override — lógica de daño propia con gestión de furia
    // override = sobreescritura del método
    public override int CalcularDanio()
    {
        int danioBase = GetAtaque(); // Obtiene el ataque base incluyendo el bono del arma equipada
        if (_furia >= 3)             // si tiene suficiente furia para atacar furioso — mínimo 3
        {
            Console.WriteLine($"{GetNombre()} ataca furioso (+15% daño)");
            _furia -= 3;                    // Le resta 3 a _furia — costo de atacar furioso
            return (int)(danioBase * 1.15); // multiplica por 1.15 = +15% de daño
                                            // (int) descarta decimales: 12.65 → 12
        }
        // si llegó hasta aquí _furia < 3 — el if no se cumplió
        // cansado para siempre: sin furia no hay recuperación en este diseño
        Console.WriteLine($"{GetNombre()} está cansado (50% daño)");
        return (int)(danioBase * 0.5); // daño reducido a la mitad — 0.5 = 50%
    }
}

// HERENCIA: Zamuray hereda de Personaje
class Zamuray : Personaje
{
    // ENCAPSULACIÓN: estado de desarme encapsulado, solo se expone por AplicarHabilidad()
    // private solo accesible dentro de la clase Zamuray
    // Flag interno — arranca en false, se actualiza cada ronda en CalcularDanio
    private bool _desarmar = false;
    // convención anterior: private bool desarmar = false;
    // _desarmar tiene valor por defecto en la declaración → constructor puede quedar vacío

    // Constructor — pasa los 3 parámetros al padre Personaje
    // cuerpo vacío porque _desarmar ya tiene valor por defecto false
    public Zamuray(string n, int v, int a) : base(n, v, a) { }

    // POLIMORFISMO: override — calcula daño y activa flag de desarme aleatoriamente
    public override int CalcularDanio()
    {
        // Genera un número aleatorio entre 1 y 4
        // y pregunta si ese número es igual a 1 — resultado es true o false
        // probabilidad de desarme: 1 de cada 4 = 25%
        _desarmar = (Random.Shared.Next(1, 5) == 1);
        if (_desarmar) Console.WriteLine($"{GetNombre()} prepara un intento de desarme...");
        return GetAtaque(); // Retorna el daño normal sin modificador
                            // Zamuray no tiene bonus de daño — su habilidad es el desarme
    }

    // Método público exclusivo de Zamuray — no existe en Personaje
    // Recibe como parámetro el Personaje al que va a intentar desarmar — el rival
    // se llama explícitamente en Batalla() después de aplicar el daño
    public void AplicarHabilidad(Personaje objetivo)
    {
        // dos condiciones con && — ambas deben ser true para desarmar
        if (_desarmar && objetivo.EstaVivo()) // debe poder desarmar Y el rival debe estar vivo
        {
            Console.WriteLine($"{GetNombre()} ha desarmado a {objetivo.GetNombre()}!");
            objetivo.QuitarEquipo(); // el rival pierde su bono de arma o armadura para el resto de la batalla
        }
    }

    // POLIMORFISMO: override — Zamuray ignora armadura completamente
    // Zamuray no usa armadura — recibe daño completo siempre
    // sobreescribe GetArmadura para retornar siempre 0
    public override int GetArmadura() => 0;
}

// ===================== JUEGO =====================
class Juego
{
    // Main es el punto de entrada del programa
    // Es el primer método que ejecuta el runtime al correr dotnet run
    // static — no necesita crear un objeto new Juego() para ejecutarse
    static void Main()
    {
        // Crea dos objetos. p1 y p2 son de tipo Personaje pero apuntan a instancias reales
        // de Barbaro y Sacerdote — aquí ya está el polimorfismo
        // tipo declarado: Personaje | tipo real en runtime: Barbaro / Sacerdote
        Personaje p1 = new Barbaro("Dave", 30, 8, 11);
        Personaje p2 = new Sacerdote("Samson", 30, 7);

        // p1=Dave recibe un arma con modificador +3 → ataque total 8+3 = 11
        // p2=Samson recibe armadura con modificador +2 → absorbe 2 de daño por ronda
        p1.Equipar(new Arma(3));
        p2.Equipar(new Armadura(2));

        // El resultado puede ser un Personaje o null si empatan, por eso el ?
        Personaje? ganador = Batalla(p1, p2);

        // Si hubo ganador en la semifinal, crea a Musashi y lo enfrenta al ganador en la final
        if (ganador != null)
        {
            // Anuncia quién ganó la semifinal — ganador ya tiene el objeto del sobreviviente
            Console.WriteLine($"\n--- {ganador.GetNombre()} avanza a la Final ---");

            // Crea al rival final Musashi con vida 10, ataque 5
            // le equipa un arma +2 → ataque total 5+2 = 7
            // solo aparece si hubo ganador en la semifinal
            Personaje zamuray = new Zamuray("Musashi", 10, 5);
            zamuray.Equipar(new Arma(2)); // Llama al método Equipar de Personaje pasándole un objeto Arma con modificador 2

            // declaración de variable con asignación del resultado de Batalla
            // Personaje? porque Batalla puede retornar null si empatan
            Personaje? ganadorFinal = Batalla(zamuray, ganador);
            if (ganadorFinal != null)
                Console.WriteLine($"\nEL CAMPEÓN ES: {ganadorFinal.GetNombre()}");
            else
                Console.WriteLine("\nLa final terminó en un empate trágico.");
        }
        else
        {
            // ganador es null — Dave y Samson murieron en la misma ronda
            Console.WriteLine("\nAmbos murieron. Musashi no tiene oponente.");
        }
    }

    // POLIMORFISMO EN ACCIÓN: p1 y p2 son tipo Personaje, pero en runtime
    // cada llamada a CalcularDanio/AplicarDanio ejecuta el override de la subclase real
    // Esto es dispatch dinámico (late binding)
    // static — se llama directamente desde Main sin necesitar new Juego()
    // Personaje? — retorna el ganador o null si empatan
    public static Personaje? Batalla(Personaje p1, Personaje p2)
    {
        Console.WriteLine($"\n--- INICIO: {p1.GetNombre()} VS {p2.GetNombre()} ---");
        // Muestra el encabezado de la batalla e inicializa el contador de rondas en 0
        int contadorRonda = 0;

        // termina el bucle cuando alguien muere
        // RECIÉN AQUÍ el while verifica si alguien murió — al inicio de cada nueva ronda
        while (p1.EstaVivo() && p2.EstaVivo())
        {
            contadorRonda++;
            Console.WriteLine($"------Ronda No {contadorRonda}--------------");
            Console.WriteLine($"{p1.GetNombre()} Y {p2.GetNombre()} Luchan ..");

            // Primero se calculan ambos daños y luego se aplican ambos
            // ambos personajes golpean simultáneamente — permite empate si mueren en la misma ronda
            int d1 = p1.CalcularDanio(); // dispatch dinámico → ejecuta override de su tipo real
            int d2 = p2.CalcularDanio(); // d1 d2 variables de daño calculado para cada personaje en esta ronda

            p2.AplicarDanio(d1); // p2 recibe el daño de p1
            p1.AplicarDanio(d2); // p1 recibe el daño de p2

            // POLIMORFISMO con pattern matching: verifica tipo real en runtime
            // Pattern matching — verifica en runtime si alguno es Zamuray
            // si lo es, intenta desarmar al rival
            // en la semifinal ninguno es Zamuray — en la final p1 sí lo es
            if (p1 is Zamuray m1) m1.AplicarHabilidad(p2); // p1 is Zamuray → pregunta: ¿el tipo real de p1 es Zamuray?
            if (p2 is Zamuray m2) m2.AplicarHabilidad(p1); // m1/m2 → variable creada casteada a Zamuray si la condición es true
                                                            // necesaria porque AplicarHabilidad no existe en Personaje
        }

        // determina el resultado al salir del bucle
        if (p1.EstaVivo()) return p1;  // solo p1 sobrevivió
        if (p2.EstaVivo()) return p2;  // solo p2 sobrevivió
        return null;                   // ambos murieron en la misma ronda → empate
    }
}