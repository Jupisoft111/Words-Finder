using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Words_Finder
{
    internal static class Program
    {
        internal static readonly string Texto_Fecha = "2020_02_10_18_53_01_956"; // First created at: 2020_01_28_13_49_24_302.
        internal static string Texto_Usuario = Environment.UserName;
        internal static string Texto_Título = "Words Finder by Jupisoft";
        internal static string Texto_Programa = "Words Finder";
        internal static readonly string Texto_Versión = "1.0";
        internal static readonly string Texto_Versión_Fecha = Texto_Versión + " (" + Texto_Fecha/*.Replace("_", null)*/ + ")";
        internal static string Texto_Título_Versión = Texto_Título + " " + Texto_Versión;

        /// <summary>
        /// Using this icon instead of adding it to the designer of each form saved almost 11 MB of space in my Minecraft Tools application.
        /// </summary>
        internal static Icon Icono_Jupisoft = null;

        internal static Random Rand = new Random();
        internal static readonly char Caracter_Coma_Decimal = (0.5d).ToString()[1];
        internal static Process Proceso = Process.GetCurrentProcess();
        internal static PerformanceCounter Rendimiento_Procesador = null;

        /// <summary>
        /// Executes the specified file, directory or URL, with the specified window style.
        /// </summary>
        /// <param name="Ruta">Any valid file or directory path.</param>
        /// <param name="Estado">Any valid window style.</param>
        /// <returns>Returns true if the process can be executed. Returns false if it can't be executed.</returns>
        internal static bool Ejecutar_Ruta(string Ruta, ProcessWindowStyle Estado)
        {
            try
            {
                if (!string.IsNullOrEmpty(Ruta))
                {
                    Process Proceso = new Process();
                    Proceso.StartInfo.Arguments = null;
                    Proceso.StartInfo.ErrorDialog = false;
                    Proceso.StartInfo.FileName = Ruta;
                    Proceso.StartInfo.UseShellExecute = true;
                    Proceso.StartInfo.Verb = "open";
                    Proceso.StartInfo.WindowStyle = Estado;
                    if (File.Exists(Ruta)) Proceso.StartInfo.WorkingDirectory = Ruta;
                    else if (Directory.Exists(Ruta)) Proceso.StartInfo.WorkingDirectory = Ruta;
                    bool Resultado;
                    try { Resultado = Proceso.Start(); }
                    catch { Resultado = false; }
                    Proceso.Close();
                    Proceso.Dispose();
                    Proceso = null;
                    return Resultado;
                }
            }
            catch (Exception Excepción) { Depurador.Escribir_Excepción(Excepción != null ? Excepción.ToString() : null); }
            return false;
        }

        /// <summary>
        /// Function that returns one of the 1.530 possible 24 bits RGB colors with full saturation and middle brightness.
        /// </summary>
        /// <param name="Índice">Any value between 0 and 1529. Red = 0, Yellow = 255, Green = 510, Cyan = 765, blue = 1020, purple = 1275. If the value is below 0 or above 1529, pure white will be returned instead.</param>
        /// <returns>Returns an ARGB color based on the selected index, or white if out of bounds.</returns>
        internal static Color Obtener_Color_Puro_1530(int Índice)
        {
            try
            {
                if (Índice >= 0 && Índice <= 1529)
                {
                    if (Índice < 255) return Color.FromArgb(255, Índice, 0);
                    else if (Índice < 510) return Color.FromArgb(510 - Índice, 255, 0);
                    else if (Índice < 765) return Color.FromArgb(0, 255, 255 - (765 - Índice));
                    else if (Índice < 1020) return Color.FromArgb(0, 1020 - Índice, 255);
                    else if (Índice < 1275) return Color.FromArgb(255 - (1275 - Índice), 0, 255);
                    else return Color.FromArgb(255, 0, 1530 - Índice);
                }
            }
            catch (Exception Excepción) { Depurador.Escribir_Excepción(Excepción != null ? Excepción.ToString() : null); }
            return Color.FromArgb(255, 255, 255);
        }

        /// <summary>
        /// Brush used to emulate the progress bars green color, since those update with a delay are a bit
        /// useless in this application, so it's better to "simulate" them with a graphics object.
        /// </summary>
        internal static readonly SolidBrush Pincel_Progreso = new SolidBrush(Color.FromArgb(255, 6, 176, 37));

        /// <summary>
        /// Function used to obtain a new image with a "simulated" progress bar drawn on it.
        /// </summary>
        /// <param name="Dimensiones">The client size of the desired picture box or the size of the progress bar image.</param>
        /// <param name="Ancho_Progreso">Any value between zero and the width of the image.</param>
        /// <returns>Returns a new image with the "simulated" progress bar. Returns null on any error.</returns>
        internal static Bitmap Obtener_Imagen_Barra_Progreso(Size Dimensiones, int Ancho_Progreso)
        {
            try
            {
                if (Dimensiones.Width > 0 && Dimensiones.Height > 0)
                {
                    if (Ancho_Progreso < 0) Ancho_Progreso = 0;
                    else if (Ancho_Progreso > Dimensiones.Width) Ancho_Progreso = Dimensiones.Width;
                    Bitmap Imagen = new Bitmap(Dimensiones.Width, Dimensiones.Height, PixelFormat.Format32bppArgb);
                    Graphics Pintar = Graphics.FromImage(Imagen);
                    Pintar.CompositingMode = CompositingMode.SourceCopy;
                    Pintar.CompositingQuality = CompositingQuality.HighQuality;
                    Pintar.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    Pintar.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    Pintar.SmoothingMode = SmoothingMode.HighQuality;
                    Pintar.TextRenderingHint = TextRenderingHint.AntiAlias;
                    Pintar.FillRectangle(Pincel_Progreso, 0, 0, Ancho_Progreso, Dimensiones.Height);
                    Pintar.Dispose();
                    Pintar = null;
                    return Imagen;
                }
            }
            catch (Exception Excepción) { Depurador.Escribir_Excepción(Excepción != null ? Excepción.ToString() : null); }
            return null;
        }

        internal static string Traducir_Fecha_Hora(DateTime Fecha)
        {
            try
            {
                if (Fecha != null && Fecha >= DateTime.MinValue && Fecha <= DateTime.MaxValue)
                {
                    string Año = Fecha.Year.ToString();
                    string Mes = Fecha.Month.ToString();
                    string Día = Fecha.Day.ToString();
                    string Hora = Fecha.Hour.ToString();
                    string Minuto = Fecha.Minute.ToString();
                    string Segundo = Fecha.Second.ToString();
                    string Milisegundo = Fecha.Millisecond.ToString();
                    while (Año.Length < 4) Año = "0" + Año;
                    while (Mes.Length < 2) Mes = "0" + Mes;
                    while (Día.Length < 2) Día = "0" + Día;
                    while (Hora.Length < 2) Hora = "0" + Hora;
                    while (Minuto.Length < 2) Minuto = "0" + Minuto;
                    while (Segundo.Length < 2) Segundo = "0" + Segundo;
                    while (Milisegundo.Length < 3) Milisegundo = "0" + Milisegundo;
                    return Día + "-" + Mes + "-" + Año + ", " + Hora + ":" + Minuto + ":" + Segundo + "." + Milisegundo;
                }
            }
            catch (Exception Excepción) { Application.OnThreadException(Excepción); }
            return "??-??-????, ??:??:??.???";
        }

        internal static string Traducir_Número(sbyte Valor)
        {
            return Valor.ToString();
        }

        internal static string Traducir_Número(byte Valor)
        {
            return Valor.ToString();
        }

        internal static string Traducir_Número(short Valor)
        {
            return Valor > -1000 && Valor < 1000 ? Valor.ToString() : Traducir_Número(Valor.ToString());
        }

        internal static string Traducir_Número(ushort Valor)
        {
            return Valor < 1000 ? Valor.ToString() : Traducir_Número(Valor.ToString());
        }

        internal static string Traducir_Número(int Valor)
        {
            return Valor > -1000 && Valor < 1000 ? Valor.ToString() : Traducir_Número(Valor.ToString());
        }

        internal static string Traducir_Número(uint Valor)
        {
            return Valor < 1000 ? Valor.ToString() : Traducir_Número(Valor.ToString());
        }

        internal static string Traducir_Número(long Valor)
        {
            return Valor > -1000L && Valor < 1000L ? Valor.ToString() : Traducir_Número(Valor.ToString());
        }

        internal static string Traducir_Número(ulong Valor)
        {
            return Valor < 1000UL ? Valor.ToString() : Traducir_Número(Valor.ToString());
        }

        internal static string Traducir_Número(float Valor)
        {
            //if (Single.IsNegativeInfinity(Valor)) return "-?";
            //else if (Single.IsPositiveInfinity(Valor)) return "+?";
            //else if (Single.IsNaN(Valor)) return "?";
            if (float.IsInfinity(Valor) || float.IsNaN(Valor)) return "0";
            else return Valor > -1000f && Valor < 1000f ? Valor.ToString().Replace(Caracter_Coma_Decimal, ',') : Traducir_Número(Valor.ToString());
        }

        internal static string Traducir_Número(double Valor)
        {
            //if (Double.IsNegativeInfinity(Valor)) return "-?";
            //else if (Double.IsPositiveInfinity(Valor)) return "+?";
            //else if (Double.IsNaN(Valor)) return "?";
            if (double.IsInfinity(Valor) || double.IsNaN(Valor)) return "0";
            else return Valor > -1000d && Valor < 1000d ? Valor.ToString().Replace(Caracter_Coma_Decimal, ',') : Traducir_Número(Valor.ToString());
        }

        internal static string Traducir_Número(decimal Valor)
        {
            return Valor > -1000m && Valor < 1000m ? Valor.ToString().Replace(Caracter_Coma_Decimal, ',') : Traducir_Número(Valor.ToString());
        }

        internal static string Traducir_Número(string Texto)
        {
            Texto = Texto.Replace(Caracter_Coma_Decimal, ',').Replace(".", null);
            for (int Índice = !Texto.Contains(",") ? Texto.Length - 3 : Texto.IndexOf(',') - 3, Índice_Final = !Texto.StartsWith("-") ? 0 : 1; Índice > Índice_Final; Índice -= 3) Texto = Texto.Insert(Índice, ".");
            return Texto;
            /*Texto = Texto.Replace(Caracter_Coma_Decimal, ',');
            if (Texto.Contains(".")) Texto = Texto.Replace(".", null);
            int Índice = Texto.IndexOf(',');
            for (Índice = Índice < 0 ? Texto.Length - 3 : Índice - 3; Índice > (Texto[0] != '-' ? 0 : 1); Índice -= 3) Texto = Texto.Insert(Índice, ".");
            return Texto;*/
        }

        internal static string Traducir_Número_Decimales_Redondear(double Valor, int Decimales)
        {
            Valor = Math.Round(Valor, Decimales, MidpointRounding.AwayFromZero);
            string Texto = double.IsInfinity(Valor) || double.IsNaN(Valor) ? "0" : Valor > -1000d && Valor < 1000d ? Valor.ToString().Replace(Caracter_Coma_Decimal, ',') : Traducir_Número(Valor.ToString());
            if (Texto.Contains(",") == false) Texto += ',' + new string('0', Decimales);
            else
            {
                Decimales = Decimales - (Texto.Length - (Texto.IndexOf(',') + 1));
                if (Decimales > 0) Texto += new string('0', Decimales);
            }
            return Texto;
        }

        internal static string Traducir_Tamaño_Bytes_Automático(long Tamaño_Bytes, int Decimales, bool Decimales_Cero)
        {
            try
            {
                decimal Valor = (decimal)Tamaño_Bytes;
                int Índice = 0;
                for (; Índice < 7; Índice++)
                {
                    if (Valor < 1024m) break;
                    else Valor = Valor / 1024m;
                }
                string Texto = Traducir_Número(Math.Round(Valor, Decimales, MidpointRounding.AwayFromZero));
                if (Decimales_Cero)
                {
                    if (!Texto.Contains(Caracter_Coma_Decimal.ToString())) Texto += ',' + new string('0', Decimales);
                    else
                    {
                        Decimales = Decimales - (Texto.Length - (Texto.IndexOf(Caracter_Coma_Decimal) + 1));
                        if (Decimales > 0) Texto += new string('0', Decimales);
                    }
                }
                if (Índice == 0) Texto += Tamaño_Bytes == 1L ? " Byte" : " Bytes";
                else if (Índice == 1) Texto += " KB";
                else if (Índice == 2) Texto += " MB";
                else if (Índice == 3) Texto += " GB";
                else if (Índice == 4) Texto += " TB";
                else if (Índice == 5) Texto += " PB";
                else if (Índice == 6) Texto += " EB";
                return Texto;
            }
            catch (Exception Excepción) { Depurador.Escribir_Excepción(Excepción != null ? Excepción.ToString() : null); }
            return "? Bytes";
        }

        internal static class HSL
        {
            /// <summary>
            /// Convierte un color RGB en uno HSL.
            /// </summary>
            /// <param name="Rojo">Valor entre 0 y 255.</param>
            /// <param name="Verde">Valor entre 0 y 255.</param>
            /// <param name="Azul">Valor entre 0 y 255.</param>
            /// <param name="Matiz">Valor entre 0 y 360.</param>
            /// <param name="Saturación">Valor entre 0 y 100.</param>
            /// <param name="Luminosidad">Valor entre 0 y 100.</param>
            internal static void From_RGB(byte Rojo, byte Verde, byte Azul, out double Matiz, out double Saturación, out double Luminosidad)
            {
                Matiz = 0d;
                Saturación = 0d;
                Luminosidad = 0d;
                double Rojo_1 = Rojo / 255d;
                double Verde_1 = Verde / 255d;
                double Azul_1 = Azul / 255d;
                double Máximo, Mínimo, Diferencia;
                Máximo = Math.Max(Rojo_1, Math.Max(Verde_1, Azul_1));
                Mínimo = Math.Min(Rojo_1, Math.Min(Verde_1, Azul_1));
                Luminosidad = (Mínimo + Máximo) / 2d;
                if (Luminosidad <= 0d) return;
                Diferencia = Máximo - Mínimo;
                Saturación = Diferencia;
                if (Saturación > 0d) Saturación /= (Luminosidad <= 0.5d) ? (Máximo + Mínimo) : (2d - Máximo - Mínimo);
                else
                {
                    //Luminosidad = Math.Round(Luminosidad * 100d, 1, MidpointRounding.AwayFromZero);
                    Luminosidad *= Luminosidad * 100d;
                    return;
                }
                double Rojo_2 = (Máximo - Rojo_1) / Diferencia;
                double Verde_2 = (Máximo - Verde_1) / Diferencia;
                double Azul_2 = (Máximo - Azul_1) / Diferencia;
                if (Rojo_1 == Máximo) Matiz = (Verde_1 == Mínimo ? 5d + Azul_2 : 1d - Verde_2);
                else if (Verde_1 == Máximo) Matiz = (Azul_1 == Mínimo ? 1d + Rojo_2 : 3d - Azul_2);
                else Matiz = (Rojo_1 == Mínimo ? 3d + Verde_2 : 5d - Rojo_2);
                Matiz /= 6d;
                if (Matiz >= 1d) Matiz = 0d;
                Matiz *= 360d;
                Saturación *= 100d;
                Luminosidad *= 100d;
                //if (Matiz < 0d || Matiz >= 360d) MessageBox.Show("To Matiz", Matiz.ToString());
                //if (Saturación < 0d || Saturación > 100d) MessageBox.Show("To Saturación");
                //if (Luminosidad < 0d || Luminosidad > 100d) MessageBox.Show("To Luminosidad");
                //Matiz = Math.Round(Matiz * 360d, 1, MidpointRounding.AwayFromZero); // 0.0d ~ 360.0d
                //Saturación = Math.Round(Saturación * 100d, 1, MidpointRounding.AwayFromZero); // 0.0d ~ 100.0d
                //Luminosidad = Math.Round(Luminosidad * 100d, 1, MidpointRounding.AwayFromZero); // 0.0d ~ 100.0d
                //if (Matiz >= 360d) Matiz = 0d;
            }

            /// <summary>
            /// Convierte un color HSL en uno RGB.
            /// </summary>
            /// <param name="Matiz">Valor entre 0 y 360.</param>
            /// <param name="Saturación">Valor entre 0 y 100.</param>
            /// <param name="Luminosidad">Valor entre 0 y 100.</param>
            /// <param name="Rojo">Valor entre 0 y 255.</param>
            /// <param name="Verde">Valor entre 0 y 255.</param>
            /// <param name="Azul">Valor entre 0 y 255.</param>
            internal static void To_RGB(double Matiz, double Saturación, double Luminosidad, out byte Rojo, out byte Verde, out byte Azul)
            {
                if (Matiz >= 360d) Matiz = 0d;
                //Matiz = Math.Round(Matiz, 1, MidpointRounding.AwayFromZero);
                //Saturación = Math.Round(Saturación, 1, MidpointRounding.AwayFromZero);
                //Luminosidad = Math.Round(Luminosidad, 1, MidpointRounding.AwayFromZero);
                Matiz /= 360d; // 0.0d ~ 1.0d
                Saturación /= 100d; // 0.0d ~ 1.0d
                Luminosidad /= 100d; // 0.0d ~ 1.0d
                double Rojo_Temporal = Luminosidad; // Default to Gray
                double Verde_Temporal = Luminosidad;
                double Azul_Temporal = Luminosidad;
                double v = Luminosidad <= 0.5d ? (Luminosidad * (1d + Saturación)) : (Luminosidad + Saturación - Luminosidad * Saturación);
                if (v > 0d)
                {
                    double m, sv, Sextante, fract, vsf, mid1, mid2;
                    m = Luminosidad + Luminosidad - v;
                    sv = (v - m) / v;
                    Matiz *= 6d;
                    Sextante = Math.Floor(Matiz);
                    fract = Matiz - Sextante;
                    vsf = v * sv * fract;
                    mid1 = m + vsf;
                    mid2 = v - vsf;
                    if (Sextante == 0d)
                    {
                        Rojo_Temporal = v;
                        Verde_Temporal = mid1;
                        Azul_Temporal = m;
                    }
                    else if (Sextante == 1d)
                    {
                        Rojo_Temporal = mid2;
                        Verde_Temporal = v;
                        Azul_Temporal = m;
                    }
                    else if (Sextante == 2d)
                    {
                        Rojo_Temporal = m;
                        Verde_Temporal = v;
                        Azul_Temporal = mid1;
                    }
                    else if (Sextante == 3d)
                    {
                        Rojo_Temporal = m;
                        Verde_Temporal = mid2;
                        Azul_Temporal = v;
                    }
                    else if (Sextante == 4d)
                    {
                        Rojo_Temporal = mid1;
                        Verde_Temporal = m;
                        Azul_Temporal = v;
                    }
                    else if (Sextante == 5d)
                    {
                        Rojo_Temporal = v;
                        Verde_Temporal = m;
                        Azul_Temporal = mid2;
                    }
                }
                Rojo = (byte)Math.Round(Rojo_Temporal * 255d, MidpointRounding.AwayFromZero);
                Verde = (byte)Math.Round(Verde_Temporal * 255d, MidpointRounding.AwayFromZero);
                Azul = (byte)Math.Round(Azul_Temporal * 255d, MidpointRounding.AwayFromZero);
            }
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Depurador.Iniciar_Depurador();
                try { Rendimiento_Procesador = new PerformanceCounter("Processor", "% Processor Time", "_Total", true); }
                catch { Rendimiento_Procesador = null; }
                Application.Run(new Ventana_Principal());
            }
            catch (Exception Excepción) { Depurador.Escribir_Excepción(Excepción != null ? Excepción.ToString() : null); }
        }
    }
}
