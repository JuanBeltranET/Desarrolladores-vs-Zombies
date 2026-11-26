using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Project2
{
    /// <summary>
    /// Gestiona el estado y la representación del personaje y sus sistemas de juego.
    /// Game1 conserva la coordinación general del nivel.
    /// </summary>
    public sealed class JuegoBase
    {
        private const int VidaMaxima = 5;
        private const float StaminaMaximaSegundos = 7.5f;
        private const float RecargaStaminaSegundos = 5f;
        private const float IntervaloDisparoPistola = 0.8f;
        private const float IntervaloAtaqueEspada = 1f;
        private const float VelocidadProyectilPistola = 500f;
        private const int TamanoIconoInventario = 32;
        private const int UmbralAnimacionMs = 100;
        private const float TiempoRecargaManzana = 30f;

        private readonly GraphicsDevice _graphicsDevice;
        private readonly SpriteBatch _spriteBatch;
        private readonly Texture2D _spriteFiolina;
        private readonly Texture2D _barraVida;
        private readonly Texture2D _barraStamina;
        private readonly Texture2D _panelInventario;
        private readonly Texture2D _spriteManzana;
        private readonly Texture2D _spriteEspada;
        private readonly Texture2D _pistola;
        private readonly Texture2D _proyectilPistola;
        private readonly Texture2D _debugTexture;
        private readonly SpriteFont _tipoLetra;
        private readonly Rectangle[] _fotogramasMovimiento;
        private readonly Rectangle _fotogramaQuieto = new Rectangle(71, 3, 29, 50);
        private readonly List<(Vector2 Posicion, int Direccion)> _proyectiles =
            new List<(Vector2 Posicion, int Direccion)>();

        private float _stamina = StaminaMaximaSegundos;
        private float _tiempoAtaqueEspada;
        private float _enfriamientoAtaqueEspada;
        private float _tiempoHastaDisparo;
        private float _tiempoRecargaManzana;
        private float _tiempoAnimacion;
        private int _fotogramaActual;
        private bool _recuperandoStamina;
        private bool _inventarioVisible;
        private bool _pistolaSeleccionada;
        private bool _espadaSeleccionada;
        private bool _ataqueEspadaActivo;
        private bool _miraIzquierda;

        public int Vida { get; private set; } = VidaMaxima;

        public JuegoBase(
            GraphicsDevice graphicsDevice,
            SpriteBatch spriteBatch,
            Texture2D spriteFiolina,
            Texture2D barraVida,
            Texture2D barraStamina,
            Texture2D panelInventario,
            Texture2D spriteManzana,
            Texture2D spriteEspada,
            Texture2D pistola,
            Texture2D debugTexture,
            SpriteFont tipoLetra)
        {
            _graphicsDevice = graphicsDevice;
            _spriteBatch = spriteBatch;
            _spriteFiolina = spriteFiolina;
            _barraVida = barraVida;
            _barraStamina = barraStamina;
            _panelInventario = panelInventario;
            _spriteManzana = spriteManzana;
            _spriteEspada = spriteEspada;
            _pistola = pistola;
            _debugTexture = debugTexture;
            _tipoLetra = tipoLetra;
            _proyectilPistola = CrearTexturaProyectil(graphicsDevice);

            _fotogramasMovimiento = new[]
            {
                new Rectangle(8, 56, 33, 41),
                new Rectangle(39, 56, 33, 41),
                new Rectangle(70, 56, 33, 41),
                new Rectangle(103, 56, 34, 41),
                new Rectangle(137, 56, 33, 40),
                new Rectangle(170, 56, 34, 41),
                new Rectangle(205, 56, 34, 41),
                new Rectangle(244, 56, 34, 41)
            };

            Reiniciar();
        }

        public void Reiniciar()
        {
            Vida = VidaMaxima;
            _stamina = StaminaMaximaSegundos;
            _recuperandoStamina = false;
            _inventarioVisible = false;
            _pistolaSeleccionada = false;
            _espadaSeleccionada = false;
            _ataqueEspadaActivo = false;
            _tiempoAtaqueEspada = 0f;
            _enfriamientoAtaqueEspada = 0f;
            _tiempoHastaDisparo = 0f;
            _tiempoRecargaManzana = 0f;
            _tiempoAnimacion = 0f;
            _fotogramaActual = 0;
            _miraIzquierda = false;
            _proyectiles.Clear();
        }

        public void ActualizarOrientacionHorizontal(bool izquierda, bool derecha)
        {
            if (izquierda)
            {
                _miraIzquierda = true;
            }
            else if (derecha)
            {
                _miraIzquierda = false;
            }
        }

        public void RecibirDano(int dano)
        {
            Vida = Math.Max(0, Vida - Math.Max(0, dano));
        }

        public bool ActualizarStamina(bool quiereCorrer, float tiempo)
        {
            bool correr = quiereCorrer && !_recuperandoStamina && _stamina > 0f;
            if (correr)
            {
                _stamina = Math.Max(0f, _stamina - tiempo);
                if (_stamina <= 0f)
                {
                    _recuperandoStamina = true;
                }
            }
            else if (_stamina < StaminaMaximaSegundos)
            {
                float velocidadRecuperacion = StaminaMaximaSegundos / RecargaStaminaSegundos;
                _stamina = Math.Min(StaminaMaximaSegundos, _stamina + velocidadRecuperacion * tiempo);
                if (_stamina >= StaminaMaximaSegundos)
                {
                    _recuperandoStamina = false;
                }
            }

            return correr;
        }

        public bool ActualizarArmas(
            KeyboardState teclado,
            KeyboardState tecladoAnterior,
            float tiempo,
            Vector2 posicionFiolina,
            int anchoPantalla)
        {
            _enfriamientoAtaqueEspada = Math.Max(0f, _enfriamientoAtaqueEspada - tiempo);
            _tiempoRecargaManzana = Math.Max(0f, _tiempoRecargaManzana - tiempo);
            bool ataqueEspadaIniciado = false;

            if (teclado.IsKeyDown(Keys.X) && tecladoAnterior.IsKeyUp(Keys.X))
            {
                _inventarioVisible = !_inventarioVisible;
            }

            if (_inventarioVisible &&
                teclado.IsKeyDown(Keys.Space) && tecladoAnterior.IsKeyUp(Keys.Space) &&
                _tiempoRecargaManzana <= 0f && Vida < VidaMaxima)
            {
                Vida = Math.Min(VidaMaxima, Vida + 1);
                _tiempoRecargaManzana = TiempoRecargaManzana;
            }

            if (_inventarioVisible && teclado.IsKeyDown(Keys.D1) && tecladoAnterior.IsKeyUp(Keys.D1))
            {
                _pistolaSeleccionada = true;
                _espadaSeleccionada = false;
            }

            if (_inventarioVisible && teclado.IsKeyDown(Keys.D2) && tecladoAnterior.IsKeyUp(Keys.D2))
            {
                _espadaSeleccionada = true;
                _pistolaSeleccionada = false;
            }

            if (_ataqueEspadaActivo)
            {
                _tiempoAtaqueEspada -= tiempo;
                if (_tiempoAtaqueEspada <= 0f)
                {
                    _ataqueEspadaActivo = false;
                }
            }

            if (_espadaSeleccionada && teclado.IsKeyDown(Keys.C) && _enfriamientoAtaqueEspada <= 0f)
            {
                _ataqueEspadaActivo = true;
                _tiempoAtaqueEspada = 0.25f;
                _enfriamientoAtaqueEspada = IntervaloAtaqueEspada;
                ataqueEspadaIniciado = true;
            }

            _tiempoHastaDisparo = Math.Max(0f, _tiempoHastaDisparo - tiempo);
            if (_pistolaSeleccionada && Vida > 0 && teclado.IsKeyDown(Keys.C) && _tiempoHastaDisparo <= 0f)
            {
                int direccion = _miraIzquierda ? -1 : 1;
                float posicionX = direccion < 0 ? posicionFiolina.X - 10 : posicionFiolina.X + 30;
                _proyectiles.Add((new Vector2(posicionX, posicionFiolina.Y + 18), direccion));
                _tiempoHastaDisparo = IntervaloDisparoPistola;
            }

            for (int i = _proyectiles.Count - 1; i >= 0; i--)
            {
                (Vector2 posicion, int direccion) = _proyectiles[i];
                posicion.X += VelocidadProyectilPistola * tiempo * direccion;
                if (direccion < 0 ? posicion.X + 20 < 0 : posicion.X > anchoPantalla)
                {
                    _proyectiles.RemoveAt(i);
                }
                else
                {
                    _proyectiles[i] = (posicion, direccion);
                }
            }

            return ataqueEspadaIniciado;
        }

        public Rectangle ObtenerHitboxAtaqueEspada(Vector2 posicionFiolina)
        {
            return new Rectangle(
                (int)posicionFiolina.X + (_miraIzquierda ? -45 : 20),
                (int)posicionFiolina.Y - 8,
                65,
                64
            );
        }

        public void AplicarColisionesProyectiles(Func<Rectangle, bool> aplicarDano)
        {
            for (int i = _proyectiles.Count - 1; i >= 0; i--)
            {
                Vector2 posicion = _proyectiles[i].Posicion;
                Rectangle hitbox = new Rectangle((int)posicion.X, (int)posicion.Y, 20, 12);
                if (aplicarDano(hitbox))
                {
                    _proyectiles.RemoveAt(i);
                }
            }
        }

        public void ActualizarAnimacion(bool moviendo, float tiempoMs)
        {
            if (!moviendo)
            {
                _fotogramaActual = 0;
                _tiempoAnimacion = 0f;
                return;
            }

            _tiempoAnimacion += tiempoMs;
            if (_tiempoAnimacion >= UmbralAnimacionMs)
            {
                _fotogramaActual = (_fotogramaActual + 1) % _fotogramasMovimiento.Length;
                _tiempoAnimacion = 0f;
            }
        }

        public void DibujarFiolina(Vector2 posicion, bool moviendo)
        {
            Rectangle fotograma = moviendo ? _fotogramasMovimiento[_fotogramaActual] : _fotogramaQuieto;
            _spriteBatch.Draw(
                _spriteFiolina,
                posicion,
                fotograma,
                Color.White,
                0f,
                Vector2.Zero,
                1f,
                _miraIzquierda ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                0f
            );
        }

        public void DibujarInterfaz(Vector2 posicionFiolina)
        {
            Rectangle origenStamina = new Rectangle(3, 4, 164, 35);
            _spriteBatch.Draw(_barraStamina, new Vector2(20, 425), origenStamina, Color.DarkGray);

            int anchoStamina = (int)(origenStamina.Width * MathHelper.Clamp(_stamina / StaminaMaximaSegundos, 0f, 1f));
            if (anchoStamina > 0)
            {
                _spriteBatch.Draw(
                    _barraStamina,
                    new Vector2(20, 425),
                    new Rectangle(origenStamina.X, origenStamina.Y, anchoStamina, origenStamina.Height),
                    Color.White
                );
            }

            Rectangle origenVida = new Rectangle(0, 0, 48, 45);
            const int anchoVida = 72;
            const int separacionVidas = 64;
            int anchoTotalVidas = anchoVida + (VidaMaxima - 1) * separacionVidas;
            int xInicialVidas = (_graphicsDevice.Viewport.Width - anchoTotalVidas) / 2;

            for (int i = 0; i < VidaMaxima; i++)
            {
                _spriteBatch.Draw(
                    _barraVida,
                    new Rectangle(xInicialVidas + i * separacionVidas, 12, anchoVida, 68),
                    origenVida,
                    i < Vida ? Color.White : Color.Black
                );
            }

            if (_inventarioVisible)
            {
                DibujarInventario();
            }

            if (_espadaSeleccionada)
            {
                Rectangle destinoEspada = new Rectangle(
                    (int)posicionFiolina.X + (_miraIzquierda ? -14 : 24),
                    (int)posicionFiolina.Y + 16,
                    20,
                    20
                );
                _spriteBatch.Draw(
                    _spriteEspada,
                    destinoEspada,
                    null,
                    Color.White,
                    0f,
                    Vector2.Zero,
                    _miraIzquierda ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                    0f
                );
            }

            if (_pistolaSeleccionada)
            {
                Rectangle destinoPistola = new Rectangle(
                    (int)posicionFiolina.X + (_miraIzquierda ? -20 : 20),
                    (int)posicionFiolina.Y + 18,
                    30,
                    15
                );
                _spriteBatch.Draw(
                    _pistola,
                    destinoPistola,
                    new Rectangle(0, 0, _pistola.Width / 2, _pistola.Height),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    _miraIzquierda ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                    0f
                );
            }

            foreach ((Vector2 posicion, int _) in _proyectiles)
            {
                _spriteBatch.Draw(_proyectilPistola, new Rectangle((int)posicion.X, (int)posicion.Y, 20, 12), Color.White);
            }

            if (_ataqueEspadaActivo)
            {
                Rectangle hitboxEspada = ObtenerHitboxAtaqueEspada(posicionFiolina);
                _spriteBatch.Draw(_debugTexture, hitboxEspada, Color.Red * 0.45f);
            }
        }

        private void DibujarInventario()
        {
            int panelX = (_graphicsDevice.Viewport.Width - _panelInventario.Width) / 2;
            int panelY = (_graphicsDevice.Viewport.Height - _panelInventario.Height) / 2;
            _spriteBatch.Draw(_panelInventario, new Vector2(panelX, panelY), Color.White);

            int centroY = panelY + _panelInventario.Height / 2;
            for (int i = 0; i < 3; i++)
            {
                int centroX = panelX + _panelInventario.Width * (2 * i + 1) / 6;
                int iconoX = centroX - TamanoIconoInventario / 2 + (i == 0 ? 5 : 0);
                Rectangle destino = new Rectangle(iconoX, centroY - TamanoIconoInventario / 2,
                    TamanoIconoInventario, TamanoIconoInventario);

                if (i == 0)
                {
                    _spriteBatch.Draw(_spriteEspada, destino, Color.White);
                }
                else if (i == 1)
                {
                    Color colorManzana = _tiempoRecargaManzana > 0f
                        ? Color.Black * 0.65f
                        : Color.White;
                    _spriteBatch.Draw(_spriteManzana, destino, colorManzana);

                    if (_tiempoRecargaManzana > 0f)
                    {
                        string cuentaAtras = Math.Ceiling(_tiempoRecargaManzana).ToString("0");
                        Vector2 medidaCuentaAtras = _tipoLetra.MeasureString(cuentaAtras);
                        _spriteBatch.DrawString(
                            _tipoLetra,
                            cuentaAtras,
                            new Vector2(
                                destino.Center.X - medidaCuentaAtras.X / 2f,
                                destino.Center.Y - medidaCuentaAtras.Y / 2f
                            ),
                            Color.White
                        );
                    }
                }
                else
                {
                    Rectangle recortePistola = new Rectangle(0, 0, _pistola.Width / 2, _pistola.Height);
                    _spriteBatch.Draw(_pistola, destino, recortePistola, Color.White);
                }
            }
        }

        private static Texture2D CrearTexturaProyectil(GraphicsDevice graphicsDevice)
        {
            const int ancho = 20;
            const int alto = 12;
            Texture2D textura = new Texture2D(graphicsDevice, ancho, alto);
            Color[] pixeles = new Color[ancho * alto];
            for (int y = 0; y < alto; y++)
            {
                for (int x = 0; x < ancho; x++)
                {
                    float dx = (x - 9.5f) / 9.5f;
                    float dy = (y - 5.5f) / 5.5f;
                    pixeles[y * ancho + x] = dx * dx + dy * dy <= 1f ? Color.Yellow : Color.Transparent;
                }
            }
            textura.SetData(pixeles);
            return textura;
        }
    }
}
