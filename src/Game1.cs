using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Project2
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        Texture2D debugTexture;
        private Texture2D texturaOverlayPausa;

        private Texture2D zombie;//f898f8
        private Texture2D superviviente;
        private Texture2D background;
        private Texture2D Zombiesas;//00a0f8
        private Texture2D ZombieTank;
        private ZombieSpawn zombieSpawn;
        private Texture2D Items;
        private Texture2D boost;
        private Texture2D boostatk;
        private Texture2D nivel1;
        private Texture2D nivel2;
        private Texture2D nivel1texto;
        private Texture2D nivel2texto;
        private Texture2D pool;
        // private Texture2D inventario;
        // Imagenes del menu //
        private Texture2D fondoMenu;
        private Texture2D logo;
        private Texture2D botonJugar;
        private Texture2D exitGame;
        private Texture2D opciones;
        private Texture2D tecladoZ;
        private Texture2D tecladoX;
        private Texture2D tecladoC;
        private Texture2D teclado1;
        private Texture2D teclado2;
        private Texture2D tecladoEspacio;
        private Texture2D arma;
        private Texture2D correr;
        private Texture2D comer;
        private Texture2D inventario;
        private Texture2D atacar;
        private JuegoBase juegoBase;
        private UIGame uiGame;
        //musica
        private Song backgroundMusic;
        private Song musicaNivelDos;
        private Song MusicaMenu;
        //letra
        private SpriteFont TipoLetra;

        bool slowfiolina;

        // Estados del juego
        enum EstadoJuego
        {
            Menu,
            Opciones,
            MenuNuevo,
            Jugando
        }

        EstadoJuego estadoActual = EstadoJuego.Menu;
        bool nivelDosSeleccionado;
        private bool aguaNivelDosActiva;
        private bool juegoPausado;
        private readonly Random randomBoost = new Random();
        private bool boostDisponible;
        private Rectangle hitboxBoost;
        private float tiempoHastaBoost;
        private float tiempoBoostActivo;
        private float tiempoAcumuladoPuntaje;
        private int puntajeTiempo;

        // Control del mouse
        MouseState mouseAnterior;

        // Control del teclado
        KeyboardState tecladoAnterior;

        // HITBOXES
        Rectangle hitboxJugador;
        Rectangle hitboxAgua = new Rectangle(199, 203, 580, 150);

        // Limites del pasto
        Rectangle limitesPasto = new Rectangle(160, 30, 625, 420);

        // Movimiento de Fiolina
        Vector2 posicionFiolina = new Vector2(220, 100);
        float velocidadFiolina = 150f;

        // Estado de movimiento
        bool moviendo = false;


        // Sprite de Fiolina quieta

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);

            Content.RootDirectory = "Content";

            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            debugTexture = new Texture2D(GraphicsDevice, 1, 1);

            debugTexture.SetData(new[] { Color.Red });

            texturaOverlayPausa = new Texture2D(GraphicsDevice, 1, 1);
            texturaOverlayPausa.SetData(new[] { Color.White });

            // CARGAR IMAGENES DEL JUEGO
            zombie = Content.Load<Texture2D>("imagenes/Zombie");
            superviviente = Content.Load<Texture2D>("imagenes/FiolinaSprite");
            background = Content.Load<Texture2D>("imagenes/Background");
            Zombiesas = Content.Load<Texture2D>("imagenes/ZombieSalvaje");
            ZombieTank = Content.Load<Texture2D>("imagenes/ZombieTanque");
            zombieSpawn = new ZombieSpawn(
                GraphicsDevice, zombie, Zombiesas, ZombieTank, debugTexture, limitesPasto
            );
            Items = Content.Load<Texture2D>("imagenes/Items");
            boost = Content.Load<Texture2D>("imagenes/boost");
            boostatk = Content.Load<Texture2D>("imagenes/boostatk");
            nivel1 = Content.Load<Texture2D>("imagenes/nivel1");
            nivel2 = Content.Load<Texture2D>("imagenes/nivel2");
            nivel1texto = Content.Load<Texture2D>("imagenes/nivel1texto");
            nivel2texto = Content.Load<Texture2D>("imagenes/nivel2texto");
            pool = Content.Load<Texture2D>("imagenes/pool");
            // CARGAR IMAGENES DEL MENU
            fondoMenu = Content.Load<Texture2D>("imagenes/FondoMenu");
            logo = Content.Load<Texture2D>("imagenes/logo");
            botonJugar = Content.Load<Texture2D>("imagenes/seleccionar");
            opciones = Content.Load<Texture2D>("imagenes/opciones"); //FF000000

            exitGame = Content.Load<Texture2D>("imagenes/ExitGame");
            tecladoZ = Content.Load<Texture2D>("imagenes/tecladoZ");
            tecladoX = Content.Load<Texture2D>("imagenes/tecladoX");
            tecladoC = Content.Load<Texture2D>("imagenes/tecladoC");
            teclado1 = Content.Load<Texture2D>("imagenes/teclado1");
            teclado2 = Content.Load<Texture2D>("imagenes/teclado2");
            tecladoEspacio = Content.Load<Texture2D>("imagenes/tecladoEspacio");
            arma = Content.Load<Texture2D>("imagenes/arma");
            correr = Content.Load<Texture2D>("imagenes/correr"); // FFED1C24
            comer = Content.Load<Texture2D>("imagenes/comer");
            inventario = Content.Load<Texture2D>("imagenes/inventario"); // FFED1C24
            Texture2D panelInventario = Content.Load<Texture2D>("imagenes/inventariosprite");
            Texture2D spriteManzana = Content.Load<Texture2D>("imagenes/manzana");
            Texture2D spriteEspada = Content.Load<Texture2D>("imagenes/Espada2");
            atacar = Content.Load<Texture2D>("imagenes/atacar"); // FFED1C24
            Texture2D pistola = Content.Load<Texture2D>("imagenes/pistola");

            // MUSICA
            backgroundMusic = Content.Load<Song>("musica/04. Grasswalk");
            musicaNivelDos = Content.Load<Song>("musica/24. Watery Graves");
            MusicaMenu = Content.Load<Song>("musica/MusicaMenu");


            MediaPlayer.IsRepeating = false;

            MediaPlayer.Play(MusicaMenu);


            //LETRA
            TipoLetra = Content.Load<SpriteFont>("fuente/TipoLetra");

            juegoBase = new JuegoBase(
                GraphicsDevice,
                _spriteBatch,
                superviviente,
                Content.Load<Texture2D>("imagenes/HealthBar"),
                Content.Load<Texture2D>("imagenes/StaminaMeter"),
                panelInventario,
                spriteManzana,
                spriteEspada,
                pistola,
                debugTexture,
                TipoLetra
            );

            // Crear la UI después de cargar todos los recursos que recibe.
            uiGame = new UIGame(
                GraphicsDevice, _spriteBatch,
                fondoMenu, logo, botonJugar, exitGame, opciones,
                texturaOverlayPausa, nivel1, nivel2, nivel1texto, nivel2texto,
                Items, boost, boostatk, tecladoZ, tecladoX, tecladoC, teclado1,
                teclado2, tecladoEspacio, correr, inventario, atacar, arma, comer,
                TipoLetra
            );
        }

        // REINICIAR POSICIONES
        private void ReiniciarPosiciones()
        {
            posicionFiolina = new Vector2(220, 100);

            boostDisponible = false;
            hitboxBoost = Rectangle.Empty;
            tiempoHastaBoost = randomBoost.Next(5, 16);
            tiempoBoostActivo = 0f;
            tiempoAcumuladoPuntaje = 0f;
            puntajeTiempo = 0;

            zombieSpawn.Reset();

            hitboxJugador = new Rectangle(220, 100, 30, 45);

            moviendo = false;
            juegoBase.Reiniciar();
            slowfiolina = false;
            juegoPausado = false;
        }

        private void ActualizarBoost(float tiempo)
        {
            tiempoBoostActivo = Math.Max(0f, tiempoBoostActivo - tiempo);

            if (!boostDisponible)
            {
                tiempoHastaBoost -= tiempo;
                if (tiempoHastaBoost <= 0f)
                {
                    const int tamanoBoost = 48;
                    int xMaximo = limitesPasto.Right - tamanoBoost;
                    int yMaximo = limitesPasto.Bottom - tamanoBoost;
                    int x = randomBoost.Next(limitesPasto.Left, xMaximo + 1);
                    int y = randomBoost.Next(limitesPasto.Top, yMaximo + 1);
                    hitboxBoost = new Rectangle(x, y, tamanoBoost, tamanoBoost);
                    boostDisponible = true;
                }

                return;
            }

            if (hitboxJugador.Intersects(hitboxBoost))
            {
                boostDisponible = false;
                hitboxBoost = Rectangle.Empty;
                tiempoBoostActivo = 15f;
                tiempoHastaBoost = 45f;
            }
        }

        private void IniciarJuego(bool usarFondoPool)
        {
            nivelDosSeleccionado = usarFondoPool;
            aguaNivelDosActiva = usarFondoPool;
            zombieSpawn.MultiplicadorPuntaje = usarFondoPool ? 1.5f : 1f;
            estadoActual = EstadoJuego.Jugando;
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(nivelDosSeleccionado ? musicaNivelDos : backgroundMusic);
            ReiniciarPosiciones();
        }

        private Rectangle ObtenerRectanguloAguaNivelDos()
        {
            const float escalaFondo = 0.8f;
            Rectangle fondoPoolOrigen = new Rectangle(0, 37, pool.Width, 600);
            Rectangle fondoPoolDestino = new Rectangle(
                -8,
                0,
                (int)(pool.Width * escalaFondo),
                (int)(fondoPoolOrigen.Height * escalaFondo)
            );
            Rectangle aguaPoolOrigen = new Rectangle(11, 912, 710, 158);

            return new Rectangle(
                fondoPoolDestino.X + (int)(254 * escalaFondo),
                fondoPoolDestino.Y + (int)((302 - fondoPoolOrigen.Y) * escalaFondo),
                (int)(727 * escalaFondo),
                (int)(176 * escalaFondo)
            );
        }

        protected override void Update(GameTime gameTime)
        {
            KeyboardState teclado = Keyboard.GetState();

            MouseState mouseActual = Mouse.GetState();

            // SALIR DEL JUEGO
            if (teclado.IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            bool clicNuevo = mouseActual.LeftButton == ButtonState.Pressed &&
                mouseAnterior.LeftButton == ButtonState.Released;

            // La pantalla de derrota solo permite volver al menu.
            if (juegoBase.Vida <= 0)
            {
                uiGame.ObtenerRectanguloVolverAlMenu(out Rectangle rectVolverMenu);
                bool volverConTeclado = teclado.IsKeyDown(Keys.Back) &&
                    tecladoAnterior.IsKeyUp(Keys.Back);

                if (volverConTeclado ||
                    clicNuevo && rectVolverMenu.Contains(mouseActual.Position))
                {
                    estadoActual = EstadoJuego.Menu;
                    MediaPlayer.IsRepeating = false;
                    MediaPlayer.Play(MusicaMenu);
                    ReiniciarPosiciones();
                }

                mouseAnterior = mouseActual;
                tecladoAnterior = teclado;
                base.Update(gameTime);
                return;
            }

            // Espera la elección del jugador en la pantalla de victoria.
            if (estadoActual == EstadoJuego.Jugando && zombieSpawn.PartidaGanada)
            {
                uiGame.ObtenerRectangulosBotonesVictoria(
                    out Rectangle rectVolver,
                    out Rectangle rectContinuar
                );

                if (clicNuevo && rectVolver.Contains(mouseActual.Position))
                {
                    estadoActual = EstadoJuego.Menu;
                    MediaPlayer.IsRepeating = false;
                    MediaPlayer.Play(MusicaMenu);
                    ReiniciarPosiciones();
                }
                else if (clicNuevo && rectContinuar.Contains(mouseActual.Position))
                {
                    zombieSpawn.ContinuarPartida();
                }

                mouseAnterior = mouseActual;
                tecladoAnterior = teclado;

                base.Update(gameTime);
                return;
            }

            // Los menús gestionan aquí únicamente la entrada; se dibujan en Draw.
            if (estadoActual == EstadoJuego.Menu)
            {
                if (clicNuevo)
                {
                    if (uiGame.ObtenerRectBotonJugar().Contains(mouseActual.Position))
                    {
                        estadoActual = EstadoJuego.MenuNuevo;
                    }
                    else if (uiGame.ObtenerRectOpciones().Contains(mouseActual.Position))
                    {
                        estadoActual = EstadoJuego.Opciones;
                    }
                    else if (uiGame.ContieneBotonSalir(mouseActual.Position))
                    {
                        Exit();
                    }
                }

                mouseAnterior = mouseActual;
                tecladoAnterior = teclado;
                base.Update(gameTime);
                return;
            }

            if (estadoActual == EstadoJuego.MenuNuevo)
            {
                uiGame.ObtenerRectangulosMenuNuevo(
                    out Rectangle rectNivel1,
                    out Rectangle rectNivel2,
                    out _,
                    out _
                );

                if (teclado.IsKeyDown(Keys.Back) && tecladoAnterior.IsKeyUp(Keys.Back) ||
                    clicNuevo && uiGame.ContieneBotonSalir(mouseActual.Position))
                {
                    estadoActual = EstadoJuego.Menu;
                }
                else if (clicNuevo && rectNivel1.Contains(mouseActual.Position))
                {
                    IniciarJuego(false);
                }
                else if (clicNuevo && rectNivel2.Contains(mouseActual.Position))
                {
                    IniciarJuego(true);
                }

                mouseAnterior = mouseActual;
                tecladoAnterior = teclado;
                base.Update(gameTime);
                return;
            }

            if (estadoActual == EstadoJuego.Opciones)
            {
                if (teclado.IsKeyDown(Keys.Back) && tecladoAnterior.IsKeyUp(Keys.Back) ||
                    clicNuevo && uiGame.ContieneBotonSalir(mouseActual.Position))
                {
                    estadoActual = EstadoJuego.Menu;
                }

                mouseAnterior = mouseActual;
                tecladoAnterior = teclado;
                base.Update(gameTime);
                return;
            }

            // Lentitud del agua para Fiolina; los zombis lo gestionan ZombieSpawn.
            velocidadFiolina = slowfiolina ? 150f * 0.56f : 150f;

            // VOLVER AL MENU
            if (teclado.IsKeyDown(Keys.Back) &&
                tecladoAnterior.IsKeyUp(Keys.Back))
            {
                estadoActual = EstadoJuego.Menu;
                MediaPlayer.IsRepeating = false;
                MediaPlayer.Play(MusicaMenu);

                ReiniciarPosiciones();

                mouseAnterior = mouseActual;

                tecladoAnterior = teclado;

                base.Update(gameTime);

                return;
            }

            if (estadoActual == EstadoJuego.Jugando &&
                juegoBase.Vida > 0 &&
                teclado.IsKeyDown(Keys.P) &&
                tecladoAnterior.IsKeyUp(Keys.P))
            {
                juegoPausado = !juegoPausado;
            }

            if (juegoPausado)
            {
                tecladoAnterior = teclado;
                mouseAnterior = mouseActual;
                base.Update(gameTime);
                return;
            }

            float tiempo = (float)gameTime.ElapsedGameTime.TotalSeconds;
            bool ataqueEspadaIniciado = juegoBase.ActualizarArmas(
                teclado,
                tecladoAnterior,
                tiempo,
                posicionFiolina,
                GraphicsDevice.Viewport.Width
            );

            tecladoAnterior = teclado;

            mouseAnterior = mouseActual;



            // Detener la simulación cuando ya no quedan vidas.
            if (juegoBase.Vida <= 0)
            {
                base.Update(gameTime);
                return;
            }

            tiempoAcumuladoPuntaje += tiempo;
            while (tiempoAcumuladoPuntaje >= 1f)
            {
                puntajeTiempo += 100;
                tiempoAcumuladoPuntaje -= 1f;
            }

            // MOVIMIENTO DEL JUEGO
            moviendo = false;
            juegoBase.ActualizarOrientacionHorizontal(
                teclado.IsKeyDown(Keys.Left),
                teclado.IsKeyDown(Keys.Right)
            );

            bool hayMovimientoSolicitado = teclado.IsKeyDown(Keys.Right) ||
                teclado.IsKeyDown(Keys.Left) ||
                teclado.IsKeyDown(Keys.Up) ||
                teclado.IsKeyDown(Keys.Down);
            bool correr = juegoBase.ActualizarStamina(
                teclado.IsKeyDown(Keys.Z) && hayMovimientoSolicitado,
                tiempo
            );

            // Velocidad de movimiento
            float velocidadActual = velocidadFiolina;

            // Correr con Z consume stamina durante 15 segundos.
            if (correr)
            {
                velocidadActual *= 1.33f;
            }

            // MOVIMIENTO HORIZONTAL
            float movimientoX = 0;

            if (teclado.IsKeyDown(Keys.Right))
            {
                movimientoX = velocidadActual * tiempo;

                moviendo = true;
            }

            if (teclado.IsKeyDown(Keys.Left))
            {
                movimientoX = -velocidadActual * tiempo;

                moviendo = true;
            }



            // Aplicar movimiento horizontal
            posicionFiolina.X += movimientoX;

            hitboxJugador = new Rectangle(
                (int)posicionFiolina.X,
                (int)posicionFiolina.Y,
                30,
                45
            );

            // Colision con los limites del pasto
            if (!limitesPasto.Contains(hitboxJugador))
            {
                posicionFiolina.X -= movimientoX;
            }

            // MOVIMIENTO VERTICAL
            float movimientoY = 0;

            if (teclado.IsKeyDown(Keys.Up))
            {
                movimientoY = -velocidadActual * tiempo;

                moviendo = true;
            }

            if (teclado.IsKeyDown(Keys.Down))
            {
                movimientoY = velocidadActual * tiempo;

                moviendo = true;
            }

            // Aplicar movimiento vertical
            posicionFiolina.Y += movimientoY;

            hitboxJugador = new Rectangle(
                (int)posicionFiolina.X,
                (int)posicionFiolina.Y,
                30,
                45
            );

            // Colision con los limites del pasto
            if (!limitesPasto.Contains(hitboxJugador))
            {
                posicionFiolina.Y -= movimientoY;
            }

            juegoBase.ActualizarAnimacion(
                moviendo,
                (float)gameTime.ElapsedGameTime.TotalMilliseconds
            );

            // HITBOX DE FIOLINA
            hitboxJugador = new Rectangle(
                (int)posicionFiolina.X,
                (int)posicionFiolina.Y,
                30,
                45
            );

            ActualizarBoost(tiempo);
            int multiplicadorDano = tiempoBoostActivo > 0f ? 2 : 1;

            if (aguaNivelDosActiva)
            {
                hitboxAgua = ObtenerRectanguloAguaNivelDos();
            }

            zombieSpawn.Update(tiempo, posicionFiolina, hitboxAgua, aguaNivelDosActiva);

            juegoBase.AplicarColisionesProyectiles(
                hitbox => zombieSpawn.AplicarDanoPorProyectil(hitbox, multiplicadorDano)
            );
            if (ataqueEspadaIniciado)
            {
                zombieSpawn.AplicarDanoPorEspada(
                    juegoBase.ObtenerHitboxAtaqueEspada(posicionFiolina),
                    multiplicadorDano
                );
            }

            if (zombieSpawn.IntentarColisionConJugador(hitboxJugador, out int danoZombie))
            {
                juegoBase.RecibirDano(danoZombie);
                posicionFiolina = new Vector2(220, 100);
                hitboxJugador = new Rectangle(220, 100, 30, 45);
            }

            if (aguaNivelDosActiva && hitboxJugador.Intersects(hitboxAgua))
            {
                slowfiolina = true;

            }
            else
            {
                slowfiolina = false;
            }

            base.Update(gameTime);
        }

        // DIBUJAR JUEGO
        protected override void Draw(GameTime gameTime)
        {
            // Dibuja la pantalla de derrota de forma independiente para que ningún sprite la tape.
            if (juegoBase.Vida <= 0)
            {
                uiGame.DibujarDerrota(
                    zombieSpawn.Puntaje + puntajeTiempo,
                    zombieSpawn.ZombiesEliminadosTotal,
                    zombieSpawn.ZombiesNormalesEliminados,
                    zombieSpawn.ZombiesSalvajesEliminados,
                    zombieSpawn.ZombiesTanqueEliminados
                );
                base.Draw(gameTime);
                return;
            }

            // Si estamos en el menu
            if (estadoActual == EstadoJuego.Menu)
            {
                uiGame.DibujarMenu();

                base.Draw(gameTime);

                return;
            }

            if (estadoActual == EstadoJuego.Opciones)
            {
                uiGame.DibujarOpciones();

                base.Draw(gameTime);

                return;
            }

            if (estadoActual == EstadoJuego.MenuNuevo)
            {
                uiGame.DibujarMenuNuevo();

                base.Draw(gameTime);

                return;
            }

            GraphicsDevice.Clear(Color.Orange);

            _spriteBatch.Begin();

            // FONDO DEL NIVEL
            if (nivelDosSeleccionado)
            {
                const float escalaFondo = 0.8f;
                Rectangle fondoPoolOrigen = new Rectangle(0, 37, pool.Width, 600);
                Rectangle fondoPoolDestino = new Rectangle(
                    -8,
                    0,
                    (int)(pool.Width * escalaFondo),
                    (int)(fondoPoolOrigen.Height * escalaFondo)
                );
                _spriteBatch.Draw(pool, fondoPoolDestino, fondoPoolOrigen, Color.White);

                Rectangle aguaPoolOrigen = new Rectangle(11, 912, 710, 158);
                Rectangle aguaPoolDestino = ObtenerRectanguloAguaNivelDos();
                _spriteBatch.Draw(pool, aguaPoolDestino, aguaPoolOrigen, Color.White);
            }
            else
            {
                _spriteBatch.Draw(
                    background,
                    new Vector2(0, -10),
                    null,
                    Color.White,
                    0f,
                    new Vector2(10, 25),
                    0.8f,
                    SpriteEffects.None,
                    0f
                );
            }

            if (boostDisponible)
            {
                _spriteBatch.Draw(Items, hitboxBoost, Color.White);
            }

            // FIOLINA
            juegoBase.DibujarFiolina(posicionFiolina, moviendo);

            zombieSpawn.Draw(_spriteBatch);
            juegoBase.DibujarInterfaz(posicionFiolina);

            string textoPuntaje = $"Puntaje: {zombieSpawn.Puntaje:N0}";
            string textoPuntajeTiempo = $"Puntaje de tiempo: {puntajeTiempo:N0}";
            string textoBoost = tiempoBoostActivo > 0f
                ? $"Boost x2: {Math.Ceiling(tiempoBoostActivo):0}s"
                : string.Empty;

            float anchoTextoPanel = Math.Max(
                TipoLetra.MeasureString(textoPuntaje).X,
                TipoLetra.MeasureString(textoPuntajeTiempo).X
            );
            if (!string.IsNullOrEmpty(textoBoost))
            {
                anchoTextoPanel = Math.Max(
                    anchoTextoPanel,
                    TipoLetra.MeasureString(textoBoost).X
                );
            }

            const int margenPanelX = 10;
            const int margenPanelY = 8;
            const int separacionLineasPanel = 6;
            int cantidadLineasPanel = string.IsNullOrEmpty(textoBoost) ? 2 : 3;
            int altoPanel = margenPanelY * 2
                + cantidadLineasPanel * TipoLetra.LineSpacing
                + (cantidadLineasPanel - 1) * separacionLineasPanel;
            _spriteBatch.Draw(
                texturaOverlayPausa,
                new Rectangle(
                    10,
                    90,
                    (int)Math.Ceiling(anchoTextoPanel) + margenPanelX * 2,
                    altoPanel
                ),
                Color.Black
            );

            float yTextoPanel = 98f;
            _spriteBatch.DrawString(
                TipoLetra,
                textoPuntaje,
                new Vector2(20, yTextoPanel),
                Color.White
            );
            yTextoPanel += TipoLetra.LineSpacing + separacionLineasPanel;
            _spriteBatch.DrawString(
                TipoLetra,
                textoPuntajeTiempo,
                new Vector2(20, yTextoPanel),
                Color.White
            );

            if (!string.IsNullOrEmpty(textoBoost))
            {
                yTextoPanel += TipoLetra.LineSpacing + separacionLineasPanel;
                _spriteBatch.DrawString(
                    TipoLetra,
                    textoBoost,
                    new Vector2(20, yTextoPanel),
                    Color.Gold
                );
            }

            if (juegoPausado)
            {
                _spriteBatch.Draw(
                    texturaOverlayPausa,
                    new Rectangle(
                        0,
                        0,
                        GraphicsDevice.Viewport.Width,
                        GraphicsDevice.Viewport.Height
                    ),
                    Color.Black * 0.6f
                );

                const string textoPausa = "PAUSA";
                Vector2 medidaPausa = TipoLetra.MeasureString(textoPausa);
                _spriteBatch.DrawString(
                    TipoLetra,
                    textoPausa,
                    new Vector2(
                        (GraphicsDevice.Viewport.Width - medidaPausa.X) / 2,
                        (GraphicsDevice.Viewport.Height - medidaPausa.Y) / 2
                    ),
                    Color.White
                );
            }

            if (zombieSpawn.PartidaGanada)
            {
                uiGame.DibujarVictoria(zombieSpawn.Puntaje + puntajeTiempo);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
