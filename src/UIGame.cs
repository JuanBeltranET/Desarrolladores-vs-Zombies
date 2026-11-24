using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Project2
{
    public class UIGame
    {
        private GraphicsDevice graphicsDevice;
        private SpriteBatch spriteBatch;

        // Texturas de las pantallas
        private Texture2D fondoMenu;
        private Texture2D logo;
        private Texture2D botonJugar;
        private Texture2D exitGame;
        private Texture2D opciones;
        private Texture2D gameover;
        private Texture2D overlay;

        // Texturas del menu de niveles
        private Texture2D nivel1;
        private Texture2D nivel2;
        private Texture2D nivel1texto;
        private Texture2D nivel2texto;

        // Texturas de las opciones
        private Texture2D Items;
        private Texture2D boost;
        private Texture2D boostatk;
        private Texture2D tecladoZ;
        private Texture2D tecladoX;
        private Texture2D tecladoC;
        private Texture2D teclado1;
        private Texture2D teclado2;
        private Texture2D tecladoEspacio;
        private Texture2D correr;
        private Texture2D inventario;
        private Texture2D atacar;
        private Texture2D arma;
        private Texture2D comer;

        private SpriteFont TipoLetra;

        private Rectangle rectBotonJugar;
        private Rectangle rectOpciones;
        private Rectangle spriteBotonJugarNormal;
        private Rectangle spriteBotonJugarHover;

        public UIGame(
            GraphicsDevice graphicsDevice,
            SpriteBatch spriteBatch,
            Texture2D fondoMenu,
            Texture2D logo,
            Texture2D botonJugar,
            Texture2D exitGame,
            Texture2D opciones,
            Texture2D gameover,
            Texture2D overlay,
            Texture2D nivel1,
            Texture2D nivel2,
            Texture2D nivel1texto,
            Texture2D nivel2texto,
            Texture2D Items,
            Texture2D boost,
            Texture2D boostatk,
            Texture2D tecladoZ,
            Texture2D tecladoX,
            Texture2D tecladoC,
            Texture2D teclado1,
            Texture2D teclado2,
            Texture2D tecladoEspacio,
            Texture2D correr,
            Texture2D inventario,
            Texture2D atacar,
            Texture2D arma,
            Texture2D comer,
            SpriteFont TipoLetra
        )
        {
            this.graphicsDevice = graphicsDevice;
            this.spriteBatch = spriteBatch;

            this.fondoMenu = fondoMenu;
            this.logo = logo;
            this.botonJugar = botonJugar;
            this.exitGame = exitGame;
            this.opciones = opciones;
            this.gameover = gameover;
            this.overlay = overlay;

            this.nivel1 = nivel1;
            this.nivel2 = nivel2;
            this.nivel1texto = nivel1texto;
            this.nivel2texto = nivel2texto;

            this.Items = Items;
            this.boost = boost;
            this.boostatk = boostatk;
            this.tecladoZ = tecladoZ;
            this.tecladoX = tecladoX;
            this.tecladoC = tecladoC;
            this.teclado1 = teclado1;
            this.teclado2 = teclado2;
            this.tecladoEspacio = tecladoEspacio;
            this.correr = correr;
            this.inventario = inventario;
            this.atacar = atacar;
            this.arma = arma;
            this.comer = comer;

            this.TipoLetra = TipoLetra;

            // Configurar el boton Jugar
            int altoFrameBotonJugar = botonJugar.Width;

            spriteBotonJugarHover = new Rectangle(
                0, 0,
                botonJugar.Width,
                altoFrameBotonJugar
            );

            spriteBotonJugarNormal = new Rectangle(
                0,
                botonJugar.Height / 2,
                botonJugar.Width,
                altoFrameBotonJugar
            );

            rectBotonJugar = new Rectangle(
                (graphicsDevice.Viewport.Width - botonJugar.Width * 2) / 2,
                (graphicsDevice.Viewport.Height - altoFrameBotonJugar * 2) / 2,
                botonJugar.Width * 2,
                altoFrameBotonJugar * 2
            );

            rectOpciones = new Rectangle(
                (graphicsDevice.Viewport.Width - opciones.Width * 2) / 2,
                rectBotonJugar.Bottom + 10,
                opciones.Width * 2,
                opciones.Height * 2
            );
        }

        // MENU PRINCIPAL
        public void DibujarMenu()
        {
            graphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            spriteBatch.Draw(
                fondoMenu,
                new Rectangle(
                    0, 0,
                    graphicsDevice.Viewport.Width,
                    graphicsDevice.Viewport.Height
                ),
                Color.White
            );

            const int anchoLogo = 360;
            int altoLogo = logo.Height * anchoLogo / logo.Width;

            spriteBatch.Draw(
                logo,
                new Rectangle(24, 24, anchoLogo, altoLogo),
                Color.White
            );

            Rectangle spriteActual =
                rectBotonJugar.Contains(Mouse.GetState().Position)
                ? spriteBotonJugarHover
                : spriteBotonJugarNormal;

            spriteBatch.Draw(
                botonJugar,
                rectBotonJugar,
                spriteActual,
                Color.White
            );

            spriteBatch.Draw(
                opciones,
                rectOpciones,
                Color.White
            );

            spriteBatch.Draw(
                exitGame,
                new Vector2(
                    0,
                    graphicsDevice.Viewport.Height - exitGame.Height
                ),
                Color.White
            );

            spriteBatch.End();
        }

        // MENU DE OPCIONES
        public void DibujarOpciones()
        {
            graphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            spriteBatch.Draw(
                fondoMenu,
                new Rectangle(
                    0, 0,
                    graphicsDevice.Viewport.Width,
                    graphicsDevice.Viewport.Height
                ),
                Color.White
            );

            int escalaSprites = 2;
            int margenDerecho = 20;
            int separacionSprites = 8;

            int xItems = graphicsDevice.Viewport.Width
                - Items.Width * escalaSprites - margenDerecho;

            int yItems = 20;

            spriteBatch.Draw(
                Items,
                new Rectangle(
                    xItems, yItems,
                    Items.Width * escalaSprites,
                    Items.Height * escalaSprites
                ),
                Color.White
            );

            int yBoost = yItems + Items.Height * escalaSprites
                + separacionSprites;

            spriteBatch.Draw(
                boost,
                new Rectangle(
                    graphicsDevice.Viewport.Width
                    - boost.Width * escalaSprites - margenDerecho,
                    yBoost,
                    boost.Width * escalaSprites,
                    boost.Height * escalaSprites
                ),
                Color.White
            );

            int yBoostAtk = yBoost + boost.Height * escalaSprites
                + separacionSprites;

            spriteBatch.Draw(
                boostatk,
                new Rectangle(
                    graphicsDevice.Viewport.Width
                    - boostatk.Width * escalaSprites - margenDerecho,
                    yBoostAtk,
                    boostatk.Width * escalaSprites,
                    boostatk.Height * escalaSprites
                ),
                Color.White
            );

            int xTeclado = 20;
            int yTeclado = 20;
            const int separacionTeclados = 8;

            int anchoMaximoTeclado = Math.Max(
                tecladoZ.Width,
                Math.Max(tecladoX.Width, tecladoC.Width)
            );

            int xAccion = xTeclado
                + anchoMaximoTeclado * 2
                + separacionTeclados;

            spriteBatch.Draw(
                tecladoZ,
                new Rectangle(
                    xTeclado, yTeclado,
                    tecladoZ.Width * 2,
                    tecladoZ.Height * 2
                ),
                Color.White
            );

            spriteBatch.Draw(
                correr,
                new Rectangle(
                    xAccion, yTeclado,
                    correr.Width * 2,
                    correr.Height * 2
                ),
                Color.White
            );

            yTeclado += tecladoZ.Height * 2 + separacionTeclados;

            spriteBatch.Draw(
                tecladoX,
                new Rectangle(
                    xTeclado, yTeclado,
                    tecladoX.Width * 2,
                    tecladoX.Height * 2
                ),
                Color.White
            );

            spriteBatch.Draw(
                inventario,
                new Rectangle(
                    xAccion, yTeclado,
                    inventario.Width * 2,
                    inventario.Height * 2
                ),
                Color.White
            );

            yTeclado += tecladoX.Height * 2 + separacionTeclados;

            spriteBatch.Draw(
                tecladoC,
                new Rectangle(
                    xTeclado, yTeclado,
                    tecladoC.Width * 2,
                    tecladoC.Height * 2
                ),
                Color.White
            );

            spriteBatch.Draw(
                atacar,
                new Rectangle(
                    xAccion, yTeclado,
                    atacar.Width * 2,
                    atacar.Height * 2
                ),
                Color.White
            );

            yTeclado += tecladoC.Height * 2 + separacionTeclados;

            spriteBatch.Draw(
                teclado1,
                new Rectangle(
                    xTeclado, yTeclado,
                    teclado1.Width * 2,
                    teclado1.Height * 2
                ),
                Color.White
            );

            spriteBatch.Draw(
                arma,
                new Rectangle(
                    xAccion, yTeclado,
                    arma.Width * 2,
                    arma.Height * 2
                ),
                Color.White
            );

            yTeclado += teclado1.Height * 2 + separacionTeclados;

            spriteBatch.Draw(
                teclado2,
                new Rectangle(
                    xTeclado, yTeclado,
                    teclado2.Width * 2,
                    teclado2.Height * 2
                ),
                Color.White
            );

            spriteBatch.Draw(
                arma,
                new Rectangle(
                    xAccion, yTeclado,
                    arma.Width * 2,
                    arma.Height * 2
                ),
                Color.White
            );

            yTeclado += teclado2.Height * 2 + separacionTeclados;

            spriteBatch.Draw(
                tecladoEspacio,
                new Rectangle(
                    xTeclado, yTeclado,
                    tecladoEspacio.Width * 2,
                    tecladoEspacio.Height * 2
                ),
                Color.White
            );

            spriteBatch.Draw(
                comer,
                new Rectangle(
                    xAccion, yTeclado,
                    comer.Width * 2,
                    comer.Height * 2
                ),
                Color.White
            );

            spriteBatch.End();
        }

        // CALCULAR POSICIONES DE LOS NIVELES
        public void ObtenerRectangulosMenuNuevo(
            out Rectangle rectNivel1,
            out Rectangle rectNivel2,
            out Rectangle rectTextoNivel1,
            out Rectangle rectTextoNivel2
        )
        {
            const int escalaSpritesNivel = 2;
            const int separacionNiveles = 40;
            const int separacionVerticalTexto = 10;

            int anchoNivel1 = nivel1.Width * escalaSpritesNivel;
            int altoNivel1 = nivel1.Height * escalaSpritesNivel;

            int anchoNivel2 = nivel2.Width * escalaSpritesNivel;
            int altoNivel2 = nivel2.Height * escalaSpritesNivel;

            int altoMaximo = Math.Max(altoNivel1, altoNivel2);

            int anchoTextoNivel1 = nivel1texto.Width * escalaSpritesNivel;
            int altoTextoNivel1 = nivel1texto.Height * escalaSpritesNivel;

            int anchoTextoNivel2 = nivel2texto.Width * escalaSpritesNivel;
            int altoTextoNivel2 = nivel2texto.Height * escalaSpritesNivel;

            int altoMaximoTexto = Math.Max(
                altoTextoNivel1,
                altoTextoNivel2
            );

            int anchoTotal = anchoNivel1 + separacionNiveles + anchoNivel2;

            int xInicial = (
                graphicsDevice.Viewport.Width - anchoTotal
            ) / 2;

            int altoTotal = altoMaximo
                + separacionVerticalTexto
                + altoMaximoTexto;

            int yInicial = (
                graphicsDevice.Viewport.Height - altoTotal
            ) / 2;

            int yTexto = yInicial
                + altoMaximo
                + separacionVerticalTexto;

            rectNivel1 = new Rectangle(
                xInicial,
                yInicial + (altoMaximo - altoNivel1) / 2,
                anchoNivel1,
                altoNivel1
            );

            rectNivel2 = new Rectangle(
                xInicial + anchoNivel1 + separacionNiveles,
                yInicial + (altoMaximo - altoNivel2) / 2,
                anchoNivel2,
                altoNivel2
            );

            rectTextoNivel1 = new Rectangle(
                xInicial + (anchoNivel1 - anchoTextoNivel1) / 2,
                yTexto + (altoMaximoTexto - altoTextoNivel1) / 2,
                anchoTextoNivel1,
                altoTextoNivel1
            );

            rectTextoNivel2 = new Rectangle(
                xInicial + anchoNivel1 + separacionNiveles
                + (anchoNivel2 - anchoTextoNivel2) / 2,
                yTexto + (altoMaximoTexto - altoTextoNivel2) / 2,
                anchoTextoNivel2,
                altoTextoNivel2
            );
        }

        // MENU DE SELECCION DE NIVEL
        public void DibujarMenuNuevo()
        {
            graphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            spriteBatch.Draw(
                fondoMenu,
                new Rectangle(
                    0, 0,
                    graphicsDevice.Viewport.Width,
                    graphicsDevice.Viewport.Height
                ),
                Color.White
            );

            ObtenerRectangulosMenuNuevo(
                out Rectangle rectNivel1,
                out Rectangle rectNivel2,
                out Rectangle rectTextoNivel1,
                out Rectangle rectTextoNivel2
            );

            spriteBatch.Draw(nivel1, rectNivel1, Color.White);
            spriteBatch.Draw(nivel2, rectNivel2, Color.White);

            spriteBatch.Draw(nivel1texto, rectTextoNivel1, Color.White);
            spriteBatch.Draw(nivel2texto, rectTextoNivel2, Color.White);

            spriteBatch.Draw(
                exitGame,
                new Vector2(
                    0,
                    graphicsDevice.Viewport.Height - exitGame.Height
                ),
                Color.White
            );

            spriteBatch.End();
        }

        // PANTALLA DE DERROTA
        public void DibujarDerrota()
        {
            graphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            int anchoGameOver = Math.Min(
                gameover.Width,
                graphicsDevice.Viewport.Width
            );

            int altoGameOver = gameover.Height
                * anchoGameOver / gameover.Width;

            spriteBatch.Draw(
                gameover,
                new Rectangle(
                    (graphicsDevice.Viewport.Width - anchoGameOver) / 2,
                    (graphicsDevice.Viewport.Height - altoGameOver) / 2,
                    anchoGameOver,
                    altoGameOver
                ),
                Color.White
            );

            string textoGameOver = "perdiste we intentalo otra vez";

            spriteBatch.DrawString(
                TipoLetra,
                textoGameOver,
                new Vector2(
                    20,
                    graphicsDevice.Viewport.Height - 40
                ),
                Color.White
            );

            spriteBatch.End();
        }

        // PANTALLA DE VICTORIA
        // Este metodo se ejecuta dentro del Begin y End del juego.
        public void DibujarVictoria()
        {
            spriteBatch.Draw(
                overlay,
                new Rectangle(
                    0, 0,
                    graphicsDevice.Viewport.Width,
                    graphicsDevice.Viewport.Height
                ),
                Color.Black * 0.7f
            );

            const string textoVictoria = "Ganaste!";
            const float escalaVictoria = 3f;

            Vector2 medidaVictoria =
                TipoLetra.MeasureString(textoVictoria) * escalaVictoria;

            spriteBatch.DrawString(
                TipoLetra,
                textoVictoria,
                new Vector2(
                    (graphicsDevice.Viewport.Width - medidaVictoria.X) / 2f,
                    (graphicsDevice.Viewport.Height - medidaVictoria.Y) / 2f
                ),
                Color.White,
                0f,
                Vector2.Zero,
                escalaVictoria,
                SpriteEffects.None,
                0f
            );
        }

        // RECTANGULOS DE LOS BOTONES
        public Rectangle ObtenerRectBotonJugar()
        {
            return rectBotonJugar;
        }

        public Rectangle ObtenerRectOpciones()
        {
            return rectOpciones;
        }

        public bool ContieneBotonSalir(Point posicion)
        {
            Rectangle rectBotonSalir = new Rectangle(
                0,
                graphicsDevice.Viewport.Height - exitGame.Height,
                exitGame.Width,
                exitGame.Height
            );

            return rectBotonSalir.Contains(posicion);
        }
    }
}
