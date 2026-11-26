using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Project2
{
    public sealed class ZombieSpawn
    {
        private enum TipoZombie
        {
            Normal,
            Salvaje,
            Tanque
        }

        private sealed class ZombieInstancia
        {
            public Vector2 Posicion;
            public Rectangle Hitbox;
            public int Vida;
            public int GolpesEspada;
            public float TimerAnimacion;
            public int Frame;
            public bool EsOleadaEspecial;

            public ZombieInstancia(int x, int y, int ancho, int alto, int vida, bool esOleadaEspecial = false)
            {
                Posicion = new Vector2(x, y);
                Hitbox = new Rectangle(x, y, ancho, alto);
                Vida = vida;
                EsOleadaEspecial = esOleadaEspecial;
            }
        }

        private const int VidaMaximaZombie = 2;
        private const int VidaMaximaZombieSalvaje = 3;
        private const int VidaMaximaZombieTanque = 5;
        private const int MaxZombiesSimultaneos = 8;
        private const int MaxZombiesAparecidosPorPartida = 8;
        private const int MuertesParaOleadaEspecial = 8;
        private const int ZombiesSalvajesOleada = 3;
        private const int ZombiesTanqueOleada = 3;
        private const int IntervaloMinimoNormal = 4;
        private const int IntervaloMaximoNormal = 6;
        private const int IntervaloMinimoSalvaje = 10;
        private const int IntervaloMaximoSalvaje = 14;
        private const int IntervaloMinimoTanque = 16;
        private const int IntervaloMaximoTanque = 22;
        private const float IntervaloOleadaInfinita = 75f;
        private const int ZombiesSalvajesOleadaInfinita = 3;
        private const int ZombiesTanqueOleadaInfinita = 4;
        private const float DuracionFrameZombie = 0.12f;

        private readonly GraphicsDevice graphicsDevice;
        private readonly Texture2D texturaZombie;
        private readonly Texture2D texturaZombieSalvaje;
        private readonly Texture2D texturaZombieTanque;
        private readonly Texture2D texturaBarraVida;
        private readonly Rectangle limitesPasto;
        private readonly Random random = new Random();
        private readonly List<ZombieInstancia> zombiesNormales = new List<ZombieInstancia>();
        private readonly List<ZombieInstancia> zombiesSalvajes = new List<ZombieInstancia>();
        private readonly List<ZombieInstancia> zombiesTanque = new List<ZombieInstancia>();
        private readonly Rectangle[] framesZombie = new Rectangle[7];
        private readonly Rectangle[] framesZombieSalvaje = new Rectangle[7];
        private readonly Rectangle[] framesZombieTanque = new Rectangle[7];

        private float tiempoAparicionZombie;
        private float tiempoAparicionSalvaje;
        private float tiempoAparicionTanque;
        private float tiempoHastaOleadaInfinita;
        private int zombiesEliminadosFase;
        private int zombiesOleadaRestantes;
        private int zombiesAparecidosPorPartida;
        private bool oleadaEspecialActiva;
        private bool oleadaEspecialPendiente;
        private bool partidaGanada;
        private bool continuacionActiva;
        private bool zombieNormalEnAgua;
        private bool zombieSalvajeEnAgua;
        private readonly Dictionary<ZombieInstancia, float> velocidadOriginal = new Dictionary<ZombieInstancia, float>();

        public bool ZombieNormalEnAgua => zombieNormalEnAgua;
        public bool ZombieSalvajeEnAgua => zombieSalvajeEnAgua;
        public bool PartidaGanada => partidaGanada;
        public int Puntaje { get; private set; }
        public float MultiplicadorPuntaje { get; set; } = 1f;
        public int ZombiesNormalesEliminados { get; private set; }
        public int ZombiesSalvajesEliminados { get; private set; }
        public int ZombiesTanqueEliminados { get; private set; }
        public int ZombiesEliminadosTotal =>
            ZombiesNormalesEliminados + ZombiesSalvajesEliminados + ZombiesTanqueEliminados;

        public ZombieSpawn(
            GraphicsDevice graphicsDevice,
            Texture2D texturaZombie,
            Texture2D texturaZombieSalvaje,
            Texture2D texturaZombieTanque,
            Texture2D texturaBarraVida,
            Rectangle limitesPasto)
        {
            this.graphicsDevice = graphicsDevice;
            this.texturaZombie = texturaZombie;
            this.texturaZombieSalvaje = texturaZombieSalvaje;
            this.texturaZombieTanque = texturaZombieTanque;
            this.texturaBarraVida = texturaBarraVida;
            this.limitesPasto = limitesPasto;

            for (int i = 0; i < framesZombie.Length; i++)
            {
                framesZombie[i] = new Rectangle(i * 50, 58, 45, 55);
                framesZombieSalvaje[i] = new Rectangle(i * 58, 0, 50, 55);
                framesZombieTanque[i] = new Rectangle(i * 51, 78, 42, 54);
            }
            framesZombieTanque[framesZombieTanque.Length - 1] = new Rectangle(306, 76, 42, 54);

            Reset();
        }

        public void Reset()
        {
            zombiesNormales.Clear();
            zombiesSalvajes.Clear();
            zombiesTanque.Clear();
            zombiesEliminadosFase = 0;
            zombiesOleadaRestantes = 0;
            zombiesAparecidosPorPartida = 0;
            oleadaEspecialActiva = false;
            oleadaEspecialPendiente = false;
            partidaGanada = false;
            continuacionActiva = false;
            Puntaje = 0;
            ZombiesNormalesEliminados = 0;
            ZombiesSalvajesEliminados = 0;
            ZombiesTanqueEliminados = 0;
            zombieNormalEnAgua = false;
            zombieSalvajeEnAgua = false;
            velocidadOriginal.Clear();
            tiempoAparicionZombie = TiempoHastaProximaAparicion(0, 1);
            tiempoAparicionSalvaje = TiempoHastaProximaAparicion(8, 12);
            tiempoAparicionTanque = TiempoHastaProximaAparicion(14, 18);
            tiempoHastaOleadaInfinita = 0f;
        }

        public void ContinuarPartida()
        {
            partidaGanada = false;
            oleadaEspecialActiva = false;
            oleadaEspecialPendiente = false;
            zombiesOleadaRestantes = 0;
            zombiesEliminadosFase = 0;
            zombiesAparecidosPorPartida = 0;
            zombiesNormales.Clear();
            zombiesSalvajes.Clear();
            zombiesTanque.Clear();
            velocidadOriginal.Clear();

            continuacionActiva = true;
            IniciarOleadaInfinita();
            tiempoHastaOleadaInfinita = IntervaloOleadaInfinita;

            tiempoAparicionSalvaje = TiempoHastaProximaAparicion(3, 5);
            tiempoAparicionTanque = TiempoHastaProximaAparicion(20, 28);
        }

        public void Update(float tiempo, Vector2 posicionJugador, Rectangle hitboxAgua, bool aguaActiva)
        {
            if (!continuacionActiva)
            {
                ActualizarAparicionZombie(
                    zombiesNormales, ref tiempoAparicionZombie,
                    VidaMaximaZombie, 35, 50,
                    IntervaloMinimoNormal, IntervaloMaximoNormal, tiempo
                );
            }
            ActualizarAparicionZombie(
                zombiesSalvajes, ref tiempoAparicionSalvaje,
                VidaMaximaZombieSalvaje, 45, 57,
                continuacionActiva ? 3 : IntervaloMinimoSalvaje,
                continuacionActiva ? 5 : IntervaloMaximoSalvaje,
                tiempo
            );

            if (continuacionActiva)
            {
                tiempoHastaOleadaInfinita -= tiempo;
                if (tiempoHastaOleadaInfinita <= 0f)
                {
                    IniciarOleadaInfinita();
                    tiempoHastaOleadaInfinita = IntervaloOleadaInfinita;
                }
            }
            ActualizarAparicionZombie(
                zombiesTanque, ref tiempoAparicionTanque,
                VidaMaximaZombieTanque, 35, 57,
                continuacionActiva ? 20 : IntervaloMinimoTanque,
                continuacionActiva ? 28 : IntervaloMaximoTanque,
                tiempo
            );

            foreach (ZombieInstancia zombie in zombiesNormales)
            {
                PerseguirJugador(zombie, posicionJugador, velocidadOriginal.TryGetValue(zombie, out float velocidadNormal) ? velocidadNormal : 65f, tiempo);
                ActualizarAnimacion(zombie, tiempo, framesZombie.Length);
            }

            foreach (ZombieInstancia zombie in zombiesSalvajes)
            {
                PerseguirJugador(zombie, posicionJugador, velocidadOriginal.TryGetValue(zombie, out float velocidadSalvaje) ? velocidadSalvaje : 90f, tiempo);
                ActualizarAnimacion(zombie, tiempo, framesZombieSalvaje.Length);
            }
            foreach (ZombieInstancia zombie in zombiesTanque)
            {
                PerseguirJugador(zombie, posicionJugador, 35f, tiempo);
                ActualizarAnimacion(zombie, tiempo, framesZombieTanque.Length);
            }

            zombieNormalEnAgua = aguaActiva && HayZombieEnAgua(zombiesNormales, hitboxAgua);
            zombieSalvajeEnAgua = aguaActiva && HayZombieEnAgua(zombiesSalvajes, hitboxAgua);

            AjustarVelocidad(zombiesNormales, 65f, zombieNormalEnAgua);
            AjustarVelocidad(zombiesSalvajes, 90f, zombieSalvajeEnAgua);
            AjustarVelocidad(zombiesTanque, 35f, false);
        }

        private void IniciarOleadaInfinita()
        {
            for (int i = 0; i < ZombiesSalvajesOleadaInfinita; i++)
            {
                zombiesSalvajes.Add(CrearZombie(45, 57, VidaMaximaZombieSalvaje));
                velocidadOriginal[zombiesSalvajes[zombiesSalvajes.Count - 1]] = 90f;
            }

            for (int i = 0; i < ZombiesTanqueOleadaInfinita; i++)
            {
                zombiesTanque.Add(CrearZombie(35, 57, VidaMaximaZombieTanque));
                velocidadOriginal[zombiesTanque[zombiesTanque.Count - 1]] = 35f;
            }
        }

        public bool AplicarDanoPorProyectil(Rectangle hitboxProyectil, int multiplicadorDano)
        {
            bool golpeAplicado = AplicarDanoPorProyectil(
                    hitboxProyectil, zombiesNormales, 1000, TipoZombie.Normal, multiplicadorDano
                ) ||
                AplicarDanoPorProyectil(
                    hitboxProyectil, zombiesSalvajes, 2000, TipoZombie.Salvaje, multiplicadorDano
                ) ||
                AplicarDanoPorProyectil(
                    hitboxProyectil, zombiesTanque, 5000, TipoZombie.Tanque, multiplicadorDano
                );

            ProcesarOleadaEspecialPendiente();
            return golpeAplicado;
        }

        public bool AplicarDanoPorEspada(Rectangle hitboxEspada, int multiplicadorDano)
        {
            bool golpeAplicado = false;
            golpeAplicado |= AplicarDanoPorEspada(
                hitboxEspada, zombiesNormales, 1, 1000, TipoZombie.Normal, multiplicadorDano
            );
            golpeAplicado |= AplicarDanoPorEspada(
                hitboxEspada, zombiesSalvajes, 2, 2000, TipoZombie.Salvaje, multiplicadorDano
            );
            golpeAplicado |= AplicarDanoPorEspada(
                hitboxEspada, zombiesTanque, 4, 5000, TipoZombie.Tanque, multiplicadorDano
            );

            ProcesarOleadaEspecialPendiente();
            return golpeAplicado;
        }

        public bool IntentarColisionConJugador(Rectangle hitboxJugador, out int dano)
        {
            if (IntentarColisionConLista(zombiesNormales, hitboxJugador, 2, out dano))
            {
                return true;
            }
            if (IntentarColisionConLista(zombiesSalvajes, hitboxJugador, 1, out dano))
            {
                return true;
            }
            return IntentarColisionConLista(zombiesTanque, hitboxJugador, 3, out dano);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            DibujarLista(spriteBatch, zombiesNormales, texturaZombie, framesZombie, VidaMaximaZombie);
            DibujarLista(spriteBatch, zombiesSalvajes, texturaZombieSalvaje, framesZombieSalvaje, VidaMaximaZombieSalvaje);
            DibujarLista(spriteBatch, zombiesTanque, texturaZombieTanque, framesZombieTanque, VidaMaximaZombieTanque);
        }

        private void ActualizarAparicionZombie(
            List<ZombieInstancia> zombies,
            ref float tiempoAparicion,
            int vidaMaxima,
            int ancho,
            int alto,
            int intervaloMinimo,
            int intervaloMaximo,
            float tiempo)
        {
            if (oleadaEspecialActiva ||
                (!continuacionActiva && zombiesAparecidosPorPartida >= MaxZombiesAparecidosPorPartida))
            {
                return;
            }

            tiempoAparicion -= tiempo;
            if (tiempoAparicion > 0f || ContarZombiesActivos() >= MaxZombiesSimultaneos)
            {
                return;
            }

            zombies.Add(CrearZombie(ancho, alto, vidaMaxima));
            velocidadOriginal[zombies[zombies.Count - 1]] = VelocidadBase(vidaMaxima);
            tiempoAparicion = TiempoHastaProximaAparicion(intervaloMinimo, intervaloMaximo);
        }

        private ZombieInstancia CrearZombie(
            int ancho,
            int alto,
            int vidaMaxima,
            bool esOleadaEspecial = false)
        {
            int yMaximo = Math.Max(limitesPasto.Y + 1, limitesPasto.Bottom - alto + 1);
            int x = graphicsDevice.Viewport.Width + random.Next(20, 121);
            int y = random.Next(limitesPasto.Y, yMaximo);
            zombiesAparecidosPorPartida++;
            return new ZombieInstancia(x, y, ancho, alto, vidaMaxima, esOleadaEspecial);
        }

        private int ContarZombiesActivos()
        {
            return zombiesNormales.Count + zombiesSalvajes.Count + zombiesTanque.Count;
        }

        private void IniciarOleadaEspecial()
        {
            oleadaEspecialActiva = true;
            oleadaEspecialPendiente = false;
            zombiesOleadaRestantes = ZombiesSalvajesOleada + ZombiesTanqueOleada;
            zombiesNormales.Clear();
            zombiesSalvajes.Clear();
            zombiesTanque.Clear();

            for (int i = 0; i < ZombiesSalvajesOleada; i++)
            {
                zombiesSalvajes.Add(CrearZombie(45, 57, VidaMaximaZombieSalvaje, true));
                velocidadOriginal[zombiesSalvajes[zombiesSalvajes.Count - 1]] = 90f;
            }
            for (int i = 0; i < ZombiesTanqueOleada; i++)
            {
                zombiesTanque.Add(CrearZombie(35, 57, VidaMaximaZombieTanque, true));
                velocidadOriginal[zombiesTanque[zombiesTanque.Count - 1]] = 35f;
            }
        }

        private void FinalizarOleadaEspecial()
        {
            oleadaEspecialActiva = false;
            partidaGanada = true;
            zombiesOleadaRestantes = 0;
            zombiesEliminadosFase = 0;
            tiempoAparicionZombie = TiempoHastaProximaAparicion(IntervaloMinimoNormal, IntervaloMaximoNormal);
            tiempoAparicionSalvaje = TiempoHastaProximaAparicion(IntervaloMinimoSalvaje, IntervaloMaximoSalvaje);
            tiempoAparicionTanque = TiempoHastaProximaAparicion(IntervaloMinimoTanque, IntervaloMaximoTanque);
        }

        private void RegistrarZombieEliminado(ZombieInstancia zombie, bool eliminadoPorJugador)
        {
            if (zombie.EsOleadaEspecial)
            {
                zombiesOleadaRestantes--;
                if (zombiesOleadaRestantes <= 0)
                {
                    FinalizarOleadaEspecial();
                }
                return;
            }

            if (!eliminadoPorJugador || oleadaEspecialActiva || continuacionActiva ||
                oleadaEspecialPendiente)
            {
                return;
            }

            zombiesEliminadosFase++;
            if (zombiesEliminadosFase >= MuertesParaOleadaEspecial)
            {
                oleadaEspecialPendiente = true;
            }
        }

        private void ProcesarOleadaEspecialPendiente()
        {
            if (oleadaEspecialPendiente)
            {
                IniciarOleadaEspecial();
            }
        }

        private bool AplicarDanoPorProyectil(
            Rectangle hitboxProyectil,
            List<ZombieInstancia> zombies,
            int puntos,
            TipoZombie tipo,
            int multiplicadorDano)
        {
            for (int i = zombies.Count - 1; i >= 0; i--)
            {
                ZombieInstancia zombie = zombies[i];
                if (!hitboxProyectil.Intersects(zombie.Hitbox))
                {
                    continue;
                }

                zombie.Vida = Math.Max(0, zombie.Vida - Math.Max(1, multiplicadorDano));
                Debug.WriteLine($"Zombie alcanzado; vida restante: {zombie.Vida}");
                if (zombie.Vida == 0)
                {
                    zombies.RemoveAt(i);
                    velocidadOriginal.Remove(zombie);
                    AgregarPuntaje(puntos);
                    RegistrarEliminacionPorJugador(tipo);
                    RegistrarZombieEliminado(zombie, true);
                    Debug.WriteLine("Zombie eliminado");
                }
                return true;
            }

            return false;
        }

        private bool AplicarDanoPorEspada(
            Rectangle hitboxEspada,
            List<ZombieInstancia> zombies,
            int golpesNecesarios,
            int puntos,
            TipoZombie tipo,
            int multiplicadorDano)
        {
            bool golpeAplicado = false;
            for (int i = zombies.Count - 1; i >= 0; i--)
            {
                ZombieInstancia zombie = zombies[i];
                if (!hitboxEspada.Intersects(zombie.Hitbox))
                {
                    continue;
                }

                int dano = Math.Max(1, multiplicadorDano);
                zombie.GolpesEspada += dano;
                zombie.Vida = Math.Max(0, zombie.Vida - dano);
                Debug.WriteLine($"Impacto de espada: {zombie.GolpesEspada}/{golpesNecesarios}");

                if (zombie.GolpesEspada >= golpesNecesarios || zombie.Vida == 0)
                {
                    zombies.RemoveAt(i);
                    velocidadOriginal.Remove(zombie);
                    AgregarPuntaje(puntos);
                    RegistrarEliminacionPorJugador(tipo);
                    RegistrarZombieEliminado(zombie, true);
                    Debug.WriteLine("Zombie eliminado por espada");
                }

                golpeAplicado = true;
            }

            return golpeAplicado;
        }

        private void AgregarPuntaje(int puntos)
        {
            Puntaje += (int)Math.Round(
                puntos * MultiplicadorPuntaje,
                MidpointRounding.AwayFromZero
            );
        }

        private void RegistrarEliminacionPorJugador(TipoZombie tipo)
        {
            switch (tipo)
            {
                case TipoZombie.Normal:
                    ZombiesNormalesEliminados++;
                    break;
                case TipoZombie.Salvaje:
                    ZombiesSalvajesEliminados++;
                    break;
                case TipoZombie.Tanque:
                    ZombiesTanqueEliminados++;
                    break;
            }
        }

        private bool IntentarColisionConLista(
            List<ZombieInstancia> zombies,
            Rectangle hitboxJugador,
            int danoZombie,
            out int dano)
        {
            for (int i = zombies.Count - 1; i >= 0; i--)
            {
                ZombieInstancia zombie = zombies[i];
                if (!hitboxJugador.Intersects(zombie.Hitbox))
                {
                    continue;
                }

                dano = danoZombie;
                zombies.RemoveAt(i);
                velocidadOriginal.Remove(zombie);
                RegistrarZombieEliminado(zombie, false);
                return true;
            }

            dano = 0;
            return false;
        }

        private void PerseguirJugador(ZombieInstancia zombie, Vector2 posicionJugador, float velocidad, float tiempo)
        {
            Vector2 centroJugador = posicionJugador + new Vector2(15, 22);
            Vector2 centroZombie = new Vector2(
                zombie.Hitbox.X + zombie.Hitbox.Width / 2,
                zombie.Hitbox.Y + zombie.Hitbox.Height / 2
            );
            Vector2 direccion = centroJugador - centroZombie;
            if (direccion.Length() > 0f)
            {
                direccion.Normalize();
                zombie.Posicion += direccion * velocidad * tiempo;
            }

            zombie.Hitbox = new Rectangle(
                (int)zombie.Posicion.X,
                (int)zombie.Posicion.Y,
                zombie.Hitbox.Width,
                zombie.Hitbox.Height
            );
        }

        private static void ActualizarAnimacion(ZombieInstancia zombie, float tiempo, int cantidadFrames)
        {
            zombie.TimerAnimacion += tiempo;
            while (zombie.TimerAnimacion >= DuracionFrameZombie)
            {
                zombie.TimerAnimacion -= DuracionFrameZombie;
                zombie.Frame = (zombie.Frame + 1) % cantidadFrames;
            }
        }

        private void DibujarLista(
            SpriteBatch spriteBatch,
            List<ZombieInstancia> zombies,
            Texture2D textura,
            Rectangle[] frames,
            int vidaMaxima)
        {
            foreach (ZombieInstancia zombie in zombies)
            {
                spriteBatch.Draw(textura, zombie.Posicion, frames[zombie.Frame], Color.White);
                DibujarBarraVida(spriteBatch, zombie.Hitbox, zombie.Vida, vidaMaxima);
            }
        }

        private void DibujarBarraVida(SpriteBatch spriteBatch, Rectangle hitbox, int vida, int vidaMaxima)
        {
            const int altoBarra = 5;
            Rectangle fondo = new Rectangle(hitbox.X, hitbox.Y - altoBarra - 2, hitbox.Width, altoBarra);
            spriteBatch.Draw(texturaBarraVida, fondo, Color.Black);

            int anchoVida = hitbox.Width * vida / vidaMaxima;
            if (anchoVida > 0)
            {
                spriteBatch.Draw(
                    texturaBarraVida,
                    new Rectangle(hitbox.X, hitbox.Y - altoBarra - 2, anchoVida, altoBarra),
                    Color.White
                );
            }
        }

        private static bool HayZombieEnAgua(List<ZombieInstancia> zombies, Rectangle agua)
        {
            foreach (ZombieInstancia zombie in zombies)
            {
                if (zombie.Hitbox.Intersects(agua))
                {
                    return true;
                }
            }
            return false;
        }

        private float TiempoHastaProximaAparicion(int minimo, int maximo)
        {
            return minimo + (float)random.NextDouble() * (maximo - minimo);
        }

        private static float VelocidadBase(int vida)
        {
            if (vida == VidaMaximaZombieSalvaje)
            {
                return 90f;
            }
            return vida == VidaMaximaZombieTanque ? 35f : 65f;
        }

        private void AjustarVelocidad(List<ZombieInstancia> zombies, float velocidadBase, bool enAgua)
        {
            foreach (ZombieInstancia zombie in zombies)
            {
                float velocidad = velocidadBase * (enAgua ? 0.7f : 1f);
                velocidadOriginal[zombie] = velocidad;
            }
        }
    }
}
