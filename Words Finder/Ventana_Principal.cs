using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Words_Finder.Properties;

namespace Words_Finder
{
    public partial class Ventana_Principal : Form
    {
        public Ventana_Principal()
        {
            InitializeComponent();
        }

        internal readonly string Texto_Título = "Words Finder by Jupisoft for " + Program.Texto_Usuario;
        internal bool Variable_Excepción = false;
        internal bool Variable_Excepción_Imagen = false;
        internal int Variable_Excepción_Total = 0;
        internal bool Variable_Memoria = false;
        internal static Stopwatch FPS_Cronómetro = Stopwatch.StartNew();
        internal long FPS_Segundo_Anterior = 0L;
        internal long FPS_Temporal = 0L;
        internal long FPS_Real = 0L;
        /// <summary>
        /// List used to see the actual time spacing between the FPS. It can only store a full second before it resets itself.
        /// </summary>
        internal List<int> Lista_FPS_Milisegundos = new List<int>();
        /// <summary>
        /// Variable that if it's true will always show the main window on top of others.
        /// </summary>
        internal static bool Variable_Siempre_Visible = false;
        internal static int Cargar_Texto = 2;
        internal static string[] Matriz_Líneas = null;
        //internal BackgroundWorker Subproceso = null;
        internal string Ruta_Original = null;
        internal static readonly string Ruta_Diccionario_Español = Application.StartupPath + "\\Spanish Dictionary.txt";

        // Tests of filters to be ignored.
        /*Longitud mínima
         Longitud máxima
         //Reordenar?
         Exclusiones de palabras
         Consonantes
         Vocales
         //Caracteres?
         Empieza
         No empieza
         //No posiciones?
         Contiene
         No contiene
         //Repeticiones?
         Termina
         No termina*/

        private void Ventana_Principal_Load(object sender, EventArgs e)
        {
            try
            {
                if (Program.Icono_Jupisoft == null) Program.Icono_Jupisoft = this.Icon.Clone() as Icon;
                this.Text = Texto_Título + " - [Drag and drop any file containing a list of words to load it]";
                Menú_Contextual_Acerca.Text = "About " + Program.Texto_Programa + " " + Program.Texto_Versión + "...";
                this.WindowState = FormWindowState.Maximized;
            }
            catch (Exception Excepción) { Depurador.Escribir_Excepción(Excepción != null ? Excepción.ToString() : null); Variable_Excepción_Total++; Variable_Excepción = true; }
        }

        private void Ventana_Principal_Shown(object sender, EventArgs e)
        {
            try
            {
                this.Activate();
                Temporizador_Principal.Start();
                if (!string.IsNullOrEmpty(Ruta_Diccionario_Español) && File.Exists(Ruta_Diccionario_Español))
                {
                    Cargar_Ruta(Ruta_Diccionario_Español);
                }
            }
            catch (Exception Excepción) { Depurador.Escribir_Excepción(Excepción != null ? Excepción.ToString() : null); Variable_Excepción_Total++; Variable_Excepción = true; }
        }

        private void Ventana_Principal_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                Temporizador_Principal.Stop();
            }
            catch (Exception Excepción) { Depurador.Escribir_Excepción(Excepción != null ? Excepción.ToString() : null); Variable_Excepción_Total++; Variable_Excepción = true; }
        }

        private void Ventana_Principal_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {

            }
            catch (Exception Excepción) { Depurador.Escribir_Excepción(Excepción != null ? Excepción.ToString() : null); Variable_Excepción_Total++; Variable_Excepción = true; }
        }

        private void Ventana_Principal_SizeChanged(object sender, EventArgs e)
        {
            try
            {
                //if (this.WindowState == FormWindowState.Maximized) this.WindowState = FormWindowState.Normal;
            }
            catch (Exception Excepción) { Depurador.Escribir_Excepción(Excepción != null ? Excepción.ToString() : null); Variable_Excepción_Total++; Variable_Excepción = true; }
        }

        private void Ventana_Principal_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (!e.Alt && !e.Control && !e.Shift)
                {
                    if (e.KeyCode == Keys.Escape || e.KeyCode == Keys.Delete)
                    {
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                        this.Close();
                    }
                    else if (e.KeyCode == Keys.Enter)
                    {
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                        Buscar_Palabras();
                    }
                }
            }
            catch (Exception Excepción) { Depurador.Escribir_Excepción(Excepción != null ? Excepción.ToString() : null); Variable_Excepción_Total++; Variable_Excepción = true; }
        }

        private void Ventana_Principal_DragEnter(object sender, DragEventArgs e)
        {
            try
            {
                e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop, true) ? DragDropEffects.Copy : DragDropEffects.None;
            }
            catch (Exception Excepción) { Depurador.Escribir_Excepción(Excepción != null ? Excepción.ToString() : null); Variable_Excepción_Total++; Variable_Excepción = true; }
        }

        private void Ventana_Principal_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;
                if (e.Data.GetDataPresent(DataFormats.FileDrop, true))
                {
                    string[] Matriz_Rutas = e.Data.GetData(DataFormats.FileDrop, true) as string[];
                    if (Matriz_Rutas != null && Matriz_Rutas.Length > 0)
                    {
                        foreach (string Ruta in Matriz_Rutas)
                        {
                            try
                            {
                                if (Cargar_Ruta(Ruta)) break;
                            }
                            catch (Exception Excepción) { Depurador.Escribir_Excepción(Excepción != null ? Excepción.ToString() : null); continue; }
                        }
                        Matriz_Rutas = null;
                    }
                }
            }
            catch (Exception Excepción) { Depurador.Escribir_Excepción(Excepción != null ? Excepción.ToString() : null); Variable_Excepción_Total++; Variable_Excepción = true; }
            finally { this.Cursor = Cursors.Default; }
        }

        private void Barra_Estado_Botón_Excepción_Click(object sender, EventArgs e)
        {
            try
            {
                Variable_Excepción = false;
                Variable_Excepción_Imagen = false;
                Variable_Excepción_Total = 0;
                Barra_Estado_Botón_Excepción.Visible = false;
                Barra_Estado_Separador_1.Visible = false;
                Barra_Estado_Botón_Excepción.Image = Resources.Excepción_Gris;
                Barra_Estado_Botón_Excepción.ForeColor = Color.Black;
                Barra_Estado_Botón_Excepción.Text = "Exceptions: 0";
                Ventana_Depurador_Excepciones Ventana = new Ventana_Depurador_Excepciones();
                Ventana.ShowDialog(this);
                Ventana.Dispose();
                Ventana = null;
            }
            catch (Exception Excepción) { Depurador.Escribir_Excepción(Excepción != null ? Excepción.ToString() : null); Variable_Excepción_Total++; Variable_Excepción = true; }
        }

        private void Menú_Contextual_Opening(object sender, CancelEventArgs e)
        {
            try
            {
                Menú_Contextual_Depurador_Excepciones.Text = "Exception debugger - [" + Program.Traducir_Número(Variable_Excepción_Total) + (Variable_Excepción_Total != 1 ? " exceptions" : " exception") + "]...";
            }
            catch (Exception Excepción) { Depurador.Escribir_Excepción(Excepción != null ? Excepción.ToString() : null); Variable_Excepción_Total++; Variable_Excepción = true; }
        }

        private void Menú_Contextual_Donar_Click(object sender, EventArgs e)
        {
            try
            {
                Program.Ejecutar_Ruta("https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=KSMZ3XNG2R9P6", ProcessWindowStyle.Normal);
            }
            catch (Exception Excepción) { Depurador.Escribir_Excepción(Excepción != null ? Excepción.ToString() : null); Variable_Excepción_Total++; Variable_Excepción = true; }
        }

        private void Menú_Contextual_Visor_Ayuda_Click(object sender, EventArgs e)
        {
            try
            {
                MessageBox.Show(this, "The help file is not available yet... sorry.", Program.Texto_Título_Versión, MessageBoxButtons.OK, MessageBoxIcon.Question);
            }
            catch (Exception Excepción) { Depurador.Escribir_Excepción(Excepción != null ? Excepción.ToString() : null); Variable_Excepción_Total++; Variable_Excepción = true; }
        }

        private void Menú_Contextual_Acerca_Click(object sender, EventArgs e)
        {
            try
            {
                Ventana_Acerca Ventana = new Ventana_Acerca();
                Ventana.ShowDialog(this);
                Ventana.Dispose();
                Ventana = null;
            }
            catch (Exception Excepción) { Depurador.Escribir_Excepción(Excepción != null ? Excepción.ToString() : null); Variable_Excepción_Total++; Variable_Excepción = true; }
        }

        private void Menú_Contextual_Depurador_Excepciones_Click(object sender, EventArgs e)
        {
            try
            {
                Variable_Excepción = false;
                Variable_Excepción_Imagen = false;
                Variable_Excepción_Total = 0;
                Barra_Estado_Botón_Excepción.Visible = false;
                Barra_Estado_Separador_1.Visible = false;
                Barra_Estado_Botón_Excepción.Image = Resources.Excepción_Gris;
                Barra_Estado_Botón_Excepción.ForeColor = Color.Black;
                Barra_Estado_Botón_Excepción.Text = "Exceptions: 0";
                Ventana_Depurador_Excepciones Ventana = new Ventana_Depurador_Excepciones();
                Ventana.ShowDialog(this);
                Ventana.Dispose();
                Ventana = null;
            }
            catch (Exception Excepción) { Depurador.Escribir_Excepción(Excepción != null ? Excepción.ToString() : null); Variable_Excepción_Total++; Variable_Excepción = true; }
        }

        private void Menú_Contextual_Siempre_Visible_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                Variable_Siempre_Visible = Menú_Contextual_Siempre_Visible.Checked;
                this.TopMost = Variable_Siempre_Visible;
            }
            catch (Exception Excepción) { Depurador.Escribir_Excepción(Excepción != null ? Excepción.ToString() : null); Variable_Excepción_Total++; Variable_Excepción = true; }
        }

        private void Menú_Contextual_Cargar_Texto_Click(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;
                for (int Índice = 0; Índice < Menú_Contextual.Items.Count; Índice++)
                {
                    //if (Menú_Contextual.Items[Índice].GetType() == typeof(ToolStripMenuItem))
                    if (Menú_Contextual.Items[Índice].Name.StartsWith("Menú_Contextual_Cargar_Texto_"))
                    {
                        ((ToolStripMenuItem)Menú_Contextual.Items[Índice]).Checked = false;
                    }
                }
                ToolStripMenuItem Menú = (ToolStripMenuItem)sender;
                if (Menú != null)
                {
                    Menú.Checked = true;
                    Cargar_Texto = int.Parse(Menú.Name.Replace("Menú_Contextual_Cargar_Texto_", null));
                }
                Cargar_Ruta(Ruta_Original);
            }
            catch (Exception Excepción) { Depurador.Escribir_Excepción(Excepción != null ? Excepción.ToString() : null); Variable_Excepción_Total++; Variable_Excepción = true; }
            finally { this.Cursor = Cursors.Default; }
        }

        private void Menú_Contextual_Ignorar_Mayúsculas_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;
                Cargar_Ruta(Ruta_Original);
            }
            catch (Exception Excepción) { Depurador.Escribir_Excepción(Excepción != null ? Excepción.ToString() : null); Variable_Excepción_Total++; Variable_Excepción = true; }
            finally { this.Cursor = Cursors.Default; }
        }

        private void Menú_Contextual_Quitar_Palabras_Vacías_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;
                Cargar_Ruta(Ruta_Original);
            }
            catch (Exception Excepción) { Depurador.Escribir_Excepción(Excepción != null ? Excepción.ToString() : null); Variable_Excepción_Total++; Variable_Excepción = true; }
            finally { this.Cursor = Cursors.Default; }
        }

        private void Menú_Contextual_Ordenar_Lista_Palabras_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;
                Cargar_Ruta(Ruta_Original);
            }
            catch (Exception Excepción) { Depurador.Escribir_Excepción(Excepción != null ? Excepción.ToString() : null); Variable_Excepción_Total++; Variable_Excepción = true; }
            finally { this.Cursor = Cursors.Default; }
        }

        private void Menú_Contextual_Ajustar_Palabras_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;
                if (!Menú_Contextual_Ajustar_Palabras.Checked)
                {
                    TextBox_Lista_Original.ScrollBars = ScrollBars.Both;
                    TextBox_Lista_Original.WordWrap = false;
                    TextBox_Lista_Resultados.ScrollBars = ScrollBars.Both;
                    TextBox_Lista_Resultados.WordWrap = false;
                }
                else
                {
                    TextBox_Lista_Original.ScrollBars = ScrollBars.Vertical;
                    TextBox_Lista_Original.WordWrap = true;
                    TextBox_Lista_Resultados.ScrollBars = ScrollBars.Vertical;
                    TextBox_Lista_Resultados.WordWrap = true;
                }
            }
            catch (Exception Excepción) { Depurador.Escribir_Excepción(Excepción != null ? Excepción.ToString() : null); Variable_Excepción_Total++; Variable_Excepción = true; }
            finally { this.Cursor = Cursors.Default; }
        }

        private void Temporizador_Principal_Tick(object sender, EventArgs e)
        {
            try
            {
                int Tick = Environment.TickCount; // Used in the next calculations.

                try // If there are new exceptions, flash in red text every 500 milliseconds.
                {
                    if (Variable_Excepción)
                    {
                        if ((Tick / 500) % 2 == 0)
                        {
                            if (!Variable_Excepción_Imagen)
                            {
                                Variable_Excepción_Imagen = true;
                                Barra_Estado_Botón_Excepción.Image = Resources.Excepción;
                                Barra_Estado_Botón_Excepción.ForeColor = Color.Red;
                                Barra_Estado_Botón_Excepción.Text = "Exceptions: " + Program.Traducir_Número(Variable_Excepción_Total);
                            }
                        }
                        else
                        {
                            if (Variable_Excepción_Imagen)
                            {
                                Variable_Excepción_Imagen = false;
                                Barra_Estado_Botón_Excepción.Image = Resources.Excepción_Gris;
                                Barra_Estado_Botón_Excepción.ForeColor = Color.Black;
                                Barra_Estado_Botón_Excepción.Text = "Exceptions: " + Program.Traducir_Número(Variable_Excepción_Total);
                            }
                        }
                        if (!Barra_Estado_Botón_Excepción.Visible) Barra_Estado_Botón_Excepción.Visible = true;
                        if (!Barra_Estado_Separador_1.Visible) Barra_Estado_Separador_1.Visible = true;
                    }
                }
                catch (Exception Excepción) { Depurador.Escribir_Excepción(Excepción != null ? Excepción.ToString() : null); Variable_Excepción_Total++; Variable_Excepción = true; }

                try // CPU and RAM use calculations.
                {
                    try
                    {
                        if (Tick % 250 == 0) // Update every 250 milliseconds.
                        {
                            if (Program.Rendimiento_Procesador != null)
                            {
                                double CPU = (double)Program.Rendimiento_Procesador.NextValue();
                                if (CPU < 0d) CPU = 0d;
                                else if (CPU > 100d) CPU = 100d;
                                Barra_Estado_Etiqueta_CPU.Text = "CPU: " + Program.Traducir_Número_Decimales_Redondear(CPU, 2) + " %";
                            }
                            Program.Proceso.Refresh();
                            long Memoria_Bytes = Program.Proceso.PagedMemorySize64;
                            Barra_Estado_Etiqueta_Memoria.Text = "RAM: " + Program.Traducir_Tamaño_Bytes_Automático(Memoria_Bytes, 2, true);
                            if (Memoria_Bytes < 4294967296L) // < 4 GB, default black text.
                            {
                                if (Variable_Memoria)
                                {
                                    Variable_Memoria = false;
                                    Barra_Estado_Etiqueta_Memoria.ForeColor = Color.Black;
                                }
                            }
                            else // >= 4 GB, flash in red text every 500 milliseconds.
                            {
                                if ((Tick / 500) % 2 == 0)
                                {
                                    if (!Variable_Memoria)
                                    {
                                        Variable_Memoria = true;
                                        Barra_Estado_Etiqueta_Memoria.ForeColor = Color.Red;
                                    }
                                }
                                else
                                {
                                    if (Variable_Memoria)
                                    {
                                        Variable_Memoria = false;
                                        Barra_Estado_Etiqueta_Memoria.ForeColor = Color.Black;
                                    }
                                }
                            }
                        }
                    }
                    catch { Barra_Estado_Etiqueta_Memoria.Text = "RAM: ? MB (? GB)"; }
                }
                catch (Exception Excepción) { Depurador.Escribir_Excepción(Excepción != null ? Excepción.ToString() : null); Variable_Excepción_Total++; Variable_Excepción = true; }

                try // FPS calculation and drawing.
                {
                    long FPS_Milisegundo = FPS_Cronómetro.ElapsedMilliseconds;
                    long FPS_Segundo = FPS_Milisegundo / 1000L;
                    int Milisegundo_Actual = FPS_Cronómetro.Elapsed.Milliseconds;
                    if (FPS_Segundo != FPS_Segundo_Anterior)
                    {
                        FPS_Segundo_Anterior = FPS_Segundo;
                        FPS_Real = FPS_Temporal;
                        Barra_Estado_Etiqueta_FPS.Text = FPS_Real.ToString() + " FPS";
                        FPS_Temporal = 0L;
                        Lista_FPS_Milisegundos.Clear(); // Reset.
                    }
                    Lista_FPS_Milisegundos.Add(Milisegundo_Actual); // Add the current millisecond.
                    FPS_Temporal++;

                    //if (Variable_Dibujar_Espaciado_FPS)
                    {
                        // Draw the FPS spacing in real time.
                        int Ancho_FPS = Picture_FPS.ClientSize.Width;
                        if (Ancho_FPS > 0) // Don't draw if the window is minimized.
                        {
                            Bitmap Imagen_FPS = new Bitmap(Ancho_FPS, 8, PixelFormat.Format32bppArgb);
                            Graphics Pintar_FPS = Graphics.FromImage(Imagen_FPS);
                            Pintar_FPS.CompositingMode = CompositingMode.SourceOver;
                            Pintar_FPS.CompositingQuality = CompositingQuality.HighQuality;
                            Pintar_FPS.InterpolationMode = InterpolationMode.NearestNeighbor;
                            Pintar_FPS.PixelOffsetMode = PixelOffsetMode.HighQuality;
                            Pintar_FPS.SmoothingMode = SmoothingMode.None;
                            Pintar_FPS.TextRenderingHint = TextRenderingHint.AntiAlias;
                            Ancho_FPS -= 8; // Subtract 8 pixels to draw the full FPS icons on the image borders.
                            foreach (int Milisegundo in Lista_FPS_Milisegundos)
                            {
                                SolidBrush Pincel = new SolidBrush(Program.Obtener_Color_Puro_1530((Milisegundo * 1529) / 999));
                                Pintar_FPS.FillEllipse(Pincel, ((Milisegundo * Ancho_FPS) / 999), 0, 8, 8);
                                Pincel.Dispose();
                                Pincel = null;
                            }
                            Pintar_FPS.Dispose();
                            Pintar_FPS = null;
                            Picture_FPS.BackgroundImage = Imagen_FPS;
                        }
                    }
                }
                catch (Exception Excepción) { Depurador.Escribir_Excepción(Excepción != null ? Excepción.ToString() : null); Variable_Excepción_Total++; Variable_Excepción = true; }
            }
            catch (Exception Excepción) { Depurador.Escribir_Excepción(Excepción != null ? Excepción.ToString() : null); Variable_Excepción_Total++; Variable_Excepción = true; }
        }

        private void Botón_Longitud_Mínima_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                bool Habilitar = Botón_Longitud_Mínima.Checked;
                TextBox_Longitud_Mínima.Enabled = Habilitar;
                Botón_Longitud_Mínima_Restablecer.Enabled = Habilitar;
                TextBox_Longitud_Mínima.Select();
                TextBox_Longitud_Mínima.Focus();
            }
            catch (Exception Excepción) { Depurador.Escribir_Excepción(Excepción != null ? Excepción.ToString() : null); Variable_Excepción_Total++; Variable_Excepción = true; }
        }

        private void Botón_Longitud_Mínima_Restablecer_Click(object sender, EventArgs e)
        {
            try
            {
                TextBox_Longitud_Mínima.Text = null;
                Buscar_Palabras();
                TextBox_Longitud_Mínima.Select();
                TextBox_Longitud_Mínima.Focus();
            }
            catch (Exception Excepción) { Depurador.Escribir_Excepción(Excepción != null ? Excepción.ToString() : null); Variable_Excepción_Total++; Variable_Excepción = true; }
        }

        private void Botón_Longitud_Máxima_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                bool Habilitar = Botón_Longitud_Máxima.Checked;
                TextBox_Longitud_Máxima.Enabled = Habilitar;
                Botón_Longitud_Máxima_Restablecer.Enabled = Habilitar;
                TextBox_Longitud_Máxima.Select();
                TextBox_Longitud_Máxima.Focus();
            }
            catch (Exception Excepción) { Depurador.Escribir_Excepción(Excepción != null ? Excepción.ToString() : null); Variable_Excepción_Total++; Variable_Excepción = true; }
        }

        private void Botón_Longitud_Máxima_Restablecer_Click(object sender, EventArgs e)
        {
            try
            {
                TextBox_Longitud_Máxima.Text = null;
                Buscar_Palabras();
                TextBox_Longitud_Máxima.Select();
                TextBox_Longitud_Máxima.Focus();
            }
            catch (Exception Excepción) { Depurador.Escribir_Excepción(Excepción != null ? Excepción.ToString() : null); Variable_Excepción_Total++; Variable_Excepción = true; }
        }

        private void Botón_Empieza_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                bool Habilitar = Botón_Empieza.Checked;
                ComboBox_Empieza.Enabled = Habilitar;
                Botón_Empieza_Agregar.Enabled = Habilitar;
                Botón_Empieza_Quitar.Enabled = Habilitar;
                Botón_Empieza_Restablecer.Enabled = Habilitar;
                ComboBox_Empieza.Select();
                ComboBox_Empieza.Focus();
            }
            catch (Exception Excepción) { Depurador.Escribir_Excepción(Excepción != null ? Excepción.ToString() : null); Variable_Excepción_Total++; Variable_Excepción = true; }
        }

        private void Botón_Empieza_Agregar_Click(object sender, EventArgs e)
        {
            try
            {
                string Texto = ComboBox_Empieza.Text;
                if (!string.IsNullOrEmpty(Texto))
                {
                    Texto = Texto.ToUpperInvariant();
                    if (!ComboBox_Empieza.Items.Contains(Texto))
                    {
                        ComboBox_Empieza.Items.Add(Texto);
                        Buscar_Palabras();
                    }
                    Texto = null;
                    ComboBox_Empieza.Text = null;
                }
                ComboBox_Empieza.Select();
                ComboBox_Empieza.Focus();
            }
            catch (Exception Excepción) { Depurador.Escribir_Excepción(Excepción != null ? Excepción.ToString() : null); Variable_Excepción_Total++; Variable_Excepción = true; }
        }

        private void Botón_Empieza_Quitar_Click(object sender, EventArgs e)
        {
            try
            {
                string Texto = ComboBox_Empieza.Text;
                if (!string.IsNullOrEmpty(Texto))
                {
                    Texto = Texto.ToUpperInvariant();
                    if (ComboBox_Empieza.Items.Contains(Texto))
                    {
                        ComboBox_Empieza.Items.Remove(Texto);
                        Buscar_Palabras();
                    }
                    Texto = null;
                    ComboBox_Empieza.Text = null;
                }
                else if (ComboBox_Empieza.Items.Count > 0)
                {
                    ComboBox_Empieza.SelectedIndex = 0;
                }
                ComboBox_Empieza.Select();
                ComboBox_Empieza.Focus();
            }
            catch (Exception Excepción) { Depurador.Escribir_Excepción(Excepción != null ? Excepción.ToString() : null); Variable_Excepción_Total++; Variable_Excepción = true; }
        }

        private void Botón_Empieza_Restablecer_Click(object sender, EventArgs e)
        {
            try
            {
                if (ComboBox_Empieza.Items.Count > 0)
                {
                    ComboBox_Empieza.Items.Clear();
                    ComboBox_Empieza.Text = null;
                    Buscar_Palabras();
                }
                ComboBox_Empieza.Select();
                ComboBox_Empieza.Focus();
            }
            catch (Exception Excepción) { Depurador.Escribir_Excepción(Excepción != null ? Excepción.ToString() : null); Variable_Excepción_Total++; Variable_Excepción = true; }
        }

        private void Botón_No_Empieza_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                bool Habilitar = Botón_No_Empieza.Checked;
                ComboBox_No_Empieza.Enabled = Habilitar;
                Botón_No_Empieza_Agregar.Enabled = Habilitar;
                Botón_No_Empieza_Quitar.Enabled = Habilitar;
                Botón_No_Empieza_Restablecer.Enabled = Habilitar;
                ComboBox_No_Empieza.Select();
                ComboBox_No_Empieza.Focus();
            }
            catch (Exception Excepción) { Depurador.Escribir_Excepción(Excepción != null ? Excepción.ToString() : null); Variable_Excepción_Total++; Variable_Excepción = true; }
        }

        private void Botón_No_Empieza_Agregar_Click(object sender, EventArgs e)
        {
            try
            {
                string Texto = ComboBox_No_Empieza.Text;
                if (!string.IsNullOrEmpty(Texto))
                {
                    Texto = Texto.ToUpperInvariant();
                    if (!ComboBox_No_Empieza.Items.Contains(Texto))
                    {
                        ComboBox_No_Empieza.Items.Add(Texto);
                        Buscar_Palabras();
                    }
                    Texto = null;
                    ComboBox_No_Empieza.Text = null;
                }
                ComboBox_No_Empieza.Select();
                ComboBox_No_Empieza.Focus();
            }
            catch (Exception Excepción) { Depurador.Escribir_Excepción(Excepción != null ? Excepción.ToString() : null); Variable_Excepción_Total++; Variable_Excepción = true; }
        }

        private void Botón_No_Empieza_Quitar_Click(object sender, EventArgs e)
        {
            try
            {
                string Texto = ComboBox_No_Empieza.Text;
                if (!string.IsNullOrEmpty(Texto))
                {
                    Texto = Texto.ToUpperInvariant();
                    if (ComboBox_No_Empieza.Items.Contains(Texto))
                    {
                        ComboBox_No_Empieza.Items.Remove(Texto);
                        Buscar_Palabras();
                    }
                    Texto = null;
                    ComboBox_No_Empieza.Text = null;
                }
                else if (ComboBox_No_Empieza.Items.Count > 0)
                {
                    ComboBox_No_Empieza.SelectedIndex = 0;
                }
                ComboBox_No_Empieza.Select();
                ComboBox_No_Empieza.Focus();
            }
            catch (Exception Excepción) { Depurador.Escribir_Excepción(Excepción != null ? Excepción.ToString() : null); Variable_Excepción_Total++; Variable_Excepción = true; }
        }

        private void Botón_No_Empieza_Restablecer_Click(object sender, EventArgs e)
        {
            try
            {
                if (ComboBox_No_Empieza.Items.Count > 0)
                {
                    ComboBox_No_Empieza.Items.Clear();
                    ComboBox_No_Empieza.Text = null;
                    Buscar_Palabras();
                }
                ComboBox_No_Empieza.Select();
                ComboBox_No_Empieza.Focus();
            }
            catch (Exception Excepción) { Depurador.Escribir_Excepción(Excepción != null ? Excepción.ToString() : null); Variable_Excepción_Total++; Variable_Excepción = true; }
        }

        private void Botón_Contiene_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                bool Habilitar = Botón_Contiene.Checked;
                ComboBox_Contiene.Enabled = Habilitar;
                Botón_Contiene_Agregar.Enabled = Habilitar;
                Botón_Contiene_Quitar.Enabled = Habilitar;
                Botón_Contiene_Restablecer.Enabled = Habilitar;
                ComboBox_Contiene.Select();
                ComboBox_Contiene.Focus();
            }
            catch (Exception Excepción) { Depurador.Escribir_Excepción(Excepción != null ? Excepción.ToString() : null); Variable_Excepción_Total++; Variable_Excepción = true; }
        }

        private void Botón_Contiene_Agregar_Click(object sender, EventArgs e)
        {
            try
            {
                string Texto = ComboBox_Contiene.Text;
                if (!string.IsNullOrEmpty(Texto))
                {
                    Texto = Texto.ToUpperInvariant();
                    if (!ComboBox_Contiene.Items.Contains(Texto))
                    {
                        ComboBox_Contiene.Items.Add(Texto);
                        Buscar_Palabras();
                    }
                    Texto = null;
                    ComboBox_Contiene.Text = null;
                }
                ComboBox_Contiene.Select();
                ComboBox_Contiene.Focus();
            }
            catch (Exception Excepción) { Depurador.Escribir_Excepción(Excepción != null ? Excepción.ToString() : null); Variable_Excepción_Total++; Variable_Excepción = true; }
        }

        private void Botón_Contiene_Quitar_Click(object sender, EventArgs e)
        {
            try
            {
                string Texto = ComboBox_Contiene.Text;
                if (!string.IsNullOrEmpty(Texto))
                {
                    Texto = Texto.ToUpperInvariant();
                    if (ComboBox_Contiene.Items.Contains(Texto))
                    {
                        ComboBox_Contiene.Items.Remove(Texto);
                        Buscar_Palabras();
                    }
                    Texto = null;
                    ComboBox_Contiene.Text = null;
                }
                else if (ComboBox_Contiene.Items.Count > 0)
                {
                    ComboBox_Contiene.SelectedIndex = 0;
                }
                ComboBox_Contiene.Select();
                ComboBox_Contiene.Focus();
            }
            catch (Exception Excepción) { Depurador.Escribir_Excepción(Excepción != null ? Excepción.ToString() : null); Variable_Excepción_Total++; Variable_Excepción = true; }
        }

        private void Botón_Contiene_Restablecer_Click(object sender, EventArgs e)
        {
            try
            {
                if (ComboBox_Contiene.Items.Count > 0)
                {
                    ComboBox_Contiene.Items.Clear();
                    ComboBox_Contiene.Text = null;
                    Buscar_Palabras();
                }
                ComboBox_Contiene.Select();
                ComboBox_Contiene.Focus();
            }
            catch (Exception Excepción) { Depurador.Escribir_Excepción(Excepción != null ? Excepción.ToString() : null); Variable_Excepción_Total++; Variable_Excepción = true; }
        }

        private void Botón_No_Contiene_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                bool Habilitar = Botón_No_Contiene.Checked;
                ComboBox_No_Contiene.Enabled = Habilitar;
                Botón_No_Contiene_Agregar.Enabled = Habilitar;
                Botón_No_Contiene_Quitar.Enabled = Habilitar;
                Botón_No_Contiene_Restablecer.Enabled = Habilitar;
                ComboBox_No_Contiene.Select();
                ComboBox_No_Contiene.Focus();
            }
            catch (Exception Excepción) { Depurador.Escribir_Excepción(Excepción != null ? Excepción.ToString() : null); Variable_Excepción_Total++; Variable_Excepción = true; }
        }

        private void Botón_No_Contiene_Agregar_Click(object sender, EventArgs e)
        {
            try
            {
                string Texto = ComboBox_No_Contiene.Text;
                if (!string.IsNullOrEmpty(Texto))
                {
                    Texto = Texto.ToUpperInvariant();
                    if (!ComboBox_No_Contiene.Items.Contains(Texto))
                    {
                        ComboBox_No_Contiene.Items.Add(Texto);
                        Buscar_Palabras();
                    }
                    Texto = null;
                    ComboBox_No_Contiene.Text = null;
                }
                ComboBox_No_Contiene.Select();
                ComboBox_No_Contiene.Focus();
            }
            catch (Exception Excepción) { Depurador.Escribir_Excepción(Excepción != null ? Excepción.ToString() : null); Variable_Excepción_Total++; Variable_Excepción = true; }
        }

        private void Botón_No_Contiene_Quitar_Click(object sender, EventArgs e)
        {
            try
            {
                string Texto = ComboBox_No_Contiene.Text;
                if (!string.IsNullOrEmpty(Texto))
                {
                    Texto = Texto.ToUpperInvariant();
                    if (ComboBox_No_Contiene.Items.Contains(Texto))
                    {
                        ComboBox_No_Contiene.Items.Remove(Texto);
                        Buscar_Palabras();
                    }
                    Texto = null;
                    ComboBox_No_Contiene.Text = null;
                }
                else if (ComboBox_No_Contiene.Items.Count > 0)
                {
                    ComboBox_No_Contiene.SelectedIndex = 0;
                }
                ComboBox_No_Contiene.Select();
                ComboBox_No_Contiene.Focus();
            }
            catch (Exception Excepción) { Depurador.Escribir_Excepción(Excepción != null ? Excepción.ToString() : null); Variable_Excepción_Total++; Variable_Excepción = true; }
        }

        private void Botón_No_Contiene_Restablecer_Click(object sender, EventArgs e)
        {
            try
            {
                if (ComboBox_No_Contiene.Items.Count > 0)
                {
                    ComboBox_No_Contiene.Items.Clear();
                    ComboBox_No_Contiene.Text = null;
                    Buscar_Palabras();
                }
                ComboBox_No_Contiene.Select();
                ComboBox_No_Contiene.Focus();
            }
            catch (Exception Excepción) { Depurador.Escribir_Excepción(Excepción != null ? Excepción.ToString() : null); Variable_Excepción_Total++; Variable_Excepción = true; }
        }

        private void Botón_Termina_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                bool Habilitar = Botón_Termina.Checked;
                ComboBox_Termina.Enabled = Habilitar;
                Botón_Termina_Agregar.Enabled = Habilitar;
                Botón_Termina_Quitar.Enabled = Habilitar;
                Botón_Termina_Restablecer.Enabled = Habilitar;
                ComboBox_Termina.Select();
                ComboBox_Termina.Focus();
            }
            catch (Exception Excepción) { Depurador.Escribir_Excepción(Excepción != null ? Excepción.ToString() : null); Variable_Excepción_Total++; Variable_Excepción = true; }
        }

        private void Botón_Termina_Agregar_Click(object sender, EventArgs e)
        {
            try
            {
                string Texto = ComboBox_Termina.Text;
                if (!string.IsNullOrEmpty(Texto))
                {
                    Texto = Texto.ToUpperInvariant();
                    if (!ComboBox_Termina.Items.Contains(Texto))
                    {
                        ComboBox_Termina.Items.Add(Texto);
                        Buscar_Palabras();
                    }
                    Texto = null;
                    ComboBox_Termina.Text = null;
                }
                ComboBox_Termina.Select();
                ComboBox_Termina.Focus();
            }
            catch (Exception Excepción) { Depurador.Escribir_Excepción(Excepción != null ? Excepción.ToString() : null); Variable_Excepción_Total++; Variable_Excepción = true; }
        }

        private void Botón_Termina_Quitar_Click(object sender, EventArgs e)
        {
            try
            {
                string Texto = ComboBox_Termina.Text;
                if (!string.IsNullOrEmpty(Texto))
                {
                    Texto = Texto.ToUpperInvariant();
                    if (ComboBox_Termina.Items.Contains(Texto))
                    {
                        ComboBox_Termina.Items.Remove(Texto);
                        Buscar_Palabras();
                    }
                    Texto = null;
                    ComboBox_Termina.Text = null;
                }
                else if (ComboBox_Termina.Items.Count > 0)
                {
                    ComboBox_Termina.SelectedIndex = 0;
                }
                ComboBox_Termina.Select();
                ComboBox_Termina.Focus();
            }
            catch (Exception Excepción) { Depurador.Escribir_Excepción(Excepción != null ? Excepción.ToString() : null); Variable_Excepción_Total++; Variable_Excepción = true; }
        }

        private void Botón_Termina_Restablecer_Click(object sender, EventArgs e)
        {
            try
            {
                if (ComboBox_Termina.Items.Count > 0)
                {
                    ComboBox_Termina.Items.Clear();
                    ComboBox_Termina.Text = null;
                    Buscar_Palabras();
                }
                ComboBox_Termina.Select();
                ComboBox_Termina.Focus();
            }
            catch (Exception Excepción) { Depurador.Escribir_Excepción(Excepción != null ? Excepción.ToString() : null); Variable_Excepción_Total++; Variable_Excepción = true; }
        }

        private void Botón_No_Termina_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                bool Habilitar = Botón_No_Termina.Checked;
                ComboBox_No_Termina.Enabled = Habilitar;
                Botón_No_Termina_Agregar.Enabled = Habilitar;
                Botón_No_Termina_Quitar.Enabled = Habilitar;
                Botón_No_Termina_Restablecer.Enabled = Habilitar;
                ComboBox_No_Termina.Select();
                ComboBox_No_Termina.Focus();
            }
            catch (Exception Excepción) { Depurador.Escribir_Excepción(Excepción != null ? Excepción.ToString() : null); Variable_Excepción_Total++; Variable_Excepción = true; }
        }

        private void Botón_No_Termina_Agregar_Click(object sender, EventArgs e)
        {
            try
            {
                string Texto = ComboBox_No_Termina.Text;
                if (!string.IsNullOrEmpty(Texto))
                {
                    Texto = Texto.ToUpperInvariant();
                    if (!ComboBox_No_Termina.Items.Contains(Texto))
                    {
                        ComboBox_No_Termina.Items.Add(Texto);
                        Buscar_Palabras();
                    }
                    Texto = null;
                    ComboBox_No_Termina.Text = null;
                }
                ComboBox_No_Termina.Select();
                ComboBox_No_Termina.Focus();
            }
            catch (Exception Excepción) { Depurador.Escribir_Excepción(Excepción != null ? Excepción.ToString() : null); Variable_Excepción_Total++; Variable_Excepción = true; }
        }

        private void Botón_No_Termina_Quitar_Click(object sender, EventArgs e)
        {
            try
            {
                string Texto = ComboBox_No_Termina.Text;
                if (!string.IsNullOrEmpty(Texto))
                {
                    Texto = Texto.ToUpperInvariant();
                    if (ComboBox_No_Termina.Items.Contains(Texto))
                    {
                        ComboBox_No_Termina.Items.Remove(Texto);
                        Buscar_Palabras();
                    }
                    Texto = null;
                    ComboBox_No_Termina.Text = null;
                }
                else if (ComboBox_No_Termina.Items.Count > 0)
                {
                    ComboBox_No_Termina.SelectedIndex = 0;
                }
                ComboBox_No_Termina.Select();
                ComboBox_No_Termina.Focus();
            }
            catch (Exception Excepción) { Depurador.Escribir_Excepción(Excepción != null ? Excepción.ToString() : null); Variable_Excepción_Total++; Variable_Excepción = true; }
        }

        private void Botón_No_Termina_Restablecer_Click(object sender, EventArgs e)
        {
            try
            {
                if (ComboBox_No_Termina.Items.Count > 0)
                {
                    ComboBox_No_Termina.Items.Clear();
                    ComboBox_No_Termina.Text = null;
                    Buscar_Palabras();
                }
                ComboBox_No_Termina.Select();
                ComboBox_No_Termina.Focus();
            }
            catch (Exception Excepción) { Depurador.Escribir_Excepción(Excepción != null ? Excepción.ToString() : null); Variable_Excepción_Total++; Variable_Excepción = true; }
        }

        private void Subproceso_DoWork()//object sender, DoWorkEventArgs e)
        {
            try
            {
                //Temporizador_Principal.Stop();
                this.Cursor = Cursors.WaitCursor;
                TextBox_Lista_Resultados.Lines = null;
                if (Matriz_Líneas != null && Matriz_Líneas.Length > 0)
                {
                    //Picture_FPS.Image = Program.Obtener_Imagen_Barra_Progreso(Picture_FPS.ClientSize, 0);
                    int Filtro_Longitud_Mínima = 0;
                    int Filtro_Longitud_Máxima = 0;
                    List<string> Lista_Filtros_Empieza = new List<string>();
                    List<string> Lista_Filtros_No_Empieza = new List<string>();
                    List<string> Lista_Filtros_Contiene = new List<string>();
                    List<string> Lista_Filtros_No_Contiene = new List<string>();
                    List<string> Lista_Filtros_Termina = new List<string>();
                    List<string> Lista_Filtros_No_Termina = new List<string>();

                    try
                    {
                        if (Botón_Longitud_Mínima.Checked)
                        {
                            if (!string.IsNullOrEmpty(TextBox_Longitud_Mínima.Text))
                            {
                                Filtro_Longitud_Mínima = int.Parse(TextBox_Longitud_Mínima.Text);
                                if (Filtro_Longitud_Mínima < 0) Filtro_Longitud_Mínima = 0;
                            }
                        }
                    }
                    catch (Exception Excepción) { Depurador.Escribir_Excepción(Excepción != null ? Excepción.ToString() : null); Variable_Excepción_Total++; Variable_Excepción = true; Filtro_Longitud_Mínima = 0; }

                    try
                    {
                        if (Botón_Longitud_Máxima.Checked)
                        {
                            if (!string.IsNullOrEmpty(TextBox_Longitud_Máxima.Text))
                            {
                                Filtro_Longitud_Máxima = int.Parse(TextBox_Longitud_Máxima.Text);
                                if (Filtro_Longitud_Máxima < 0) Filtro_Longitud_Máxima = 0;
                            }
                        }
                    }
                    catch (Exception Excepción) { Depurador.Escribir_Excepción(Excepción != null ? Excepción.ToString() : null); Variable_Excepción_Total++; Variable_Excepción = true; Filtro_Longitud_Máxima = 0; }

                    try
                    {
                        if (Botón_Empieza.Checked)
                        {
                            if (ComboBox_Empieza.Items.Count > 0)
                            {
                                foreach (string Filtro in ComboBox_Empieza.Items)
                                {
                                    Lista_Filtros_Empieza.Add(Filtro);
                                }
                            }
                        }
                    }
                    catch (Exception Excepción) { Depurador.Escribir_Excepción(Excepción != null ? Excepción.ToString() : null); Variable_Excepción_Total++; Variable_Excepción = true; }

                    try
                    {
                        if (Botón_No_Empieza.Checked)
                        {
                            if (ComboBox_No_Empieza.Items.Count > 0)
                            {
                                foreach (string Filtro in ComboBox_No_Empieza.Items)
                                {
                                    Lista_Filtros_No_Empieza.Add(Filtro);
                                }
                            }
                        }
                    }
                    catch (Exception Excepción) { Depurador.Escribir_Excepción(Excepción != null ? Excepción.ToString() : null); Variable_Excepción_Total++; Variable_Excepción = true; }

                    try
                    {
                        if (Botón_Contiene.Checked)
                        {
                            if (ComboBox_Contiene.Items.Count > 0)
                            {
                                foreach (string Filtro in ComboBox_Contiene.Items)
                                {
                                    Lista_Filtros_Contiene.Add(Filtro);
                                }
                            }
                        }
                    }
                    catch (Exception Excepción) { Depurador.Escribir_Excepción(Excepción != null ? Excepción.ToString() : null); Variable_Excepción_Total++; Variable_Excepción = true; }

                    try
                    {
                        if (Botón_No_Contiene.Checked)
                        {
                            if (ComboBox_No_Contiene.Items.Count > 0)
                            {
                                foreach (string Filtro in ComboBox_No_Contiene.Items)
                                {
                                    Lista_Filtros_No_Contiene.Add(Filtro);
                                }
                            }
                        }
                    }
                    catch (Exception Excepción) { Depurador.Escribir_Excepción(Excepción != null ? Excepción.ToString() : null); Variable_Excepción_Total++; Variable_Excepción = true; }

                    try
                    {
                        if (Botón_Termina.Checked)
                        {
                            if (ComboBox_Termina.Items.Count > 0)
                            {
                                foreach (string Filtro in ComboBox_Termina.Items)
                                {
                                    Lista_Filtros_Termina.Add(Filtro);
                                }
                            }
                        }
                    }
                    catch (Exception Excepción) { Depurador.Escribir_Excepción(Excepción != null ? Excepción.ToString() : null); Variable_Excepción_Total++; Variable_Excepción = true; }

                    try
                    {
                        if (Botón_No_Termina.Checked)
                        {
                            if (ComboBox_No_Termina.Items.Count > 0)
                            {
                                foreach (string Filtro in ComboBox_No_Termina.Items)
                                {
                                    Lista_Filtros_No_Termina.Add(Filtro);
                                }
                            }
                        }
                    }
                    catch (Exception Excepción) { Depurador.Escribir_Excepción(Excepción != null ? Excepción.ToString() : null); Variable_Excepción_Total++; Variable_Excepción = true; }

                    if (Matriz_Líneas != null && Matriz_Líneas.Length > 0 &&
                        (Filtro_Longitud_Mínima > 0 ||
                        Filtro_Longitud_Máxima > 0 ||
                        Lista_Filtros_Empieza.Count > 0 ||
                        Lista_Filtros_No_Empieza.Count > 0 ||
                        Lista_Filtros_Contiene.Count > 0 ||
                        Lista_Filtros_No_Contiene.Count > 0 ||
                        Lista_Filtros_Termina.Count > 0 ||
                        Lista_Filtros_No_Termina.Count > 0)) // We are going to filter the words.
                    {
                        /*// Make sure that the minimum is actually the minimum value and vice versa...
                        int Mínimo = Math.Min(Filtro_Longitud_Mínima, Filtro_Longitud_Máxima);
                        int Máximo = Math.Max(Filtro_Longitud_Mínima, Filtro_Longitud_Máxima);
                        // So even if the user inputs the values swapped they will be used wisely.
                        Filtro_Longitud_Mínima = Mínimo;
                        Filtro_Longitud_Máxima = Máximo;*/

                        Dictionary<string, object> Diccionario_Resultados = new Dictionary<string, object>();

                        //int Índice = 1; // Just for progress report.
                        foreach (string Línea in Matriz_Líneas)
                        {
                            try
                            {
                                //Size Dimensiones = Picture_FPS.ClientSize;
                                //Picture_FPS.Image = Program.Obtener_Imagen_Barra_Progreso(Dimensiones, (Índice * Dimensiones.Width) / Matriz_Líneas.Length);
                                //Índice++;

                                // Now just filter each word and if they reach the end they will be valid.
                                if (!string.IsNullOrEmpty(Línea))
                                {
                                    if (Filtro_Longitud_Mínima > 0) // This filter is enabled.
                                    {
                                        if (Línea.Length < Filtro_Longitud_Mínima)
                                        {
                                            continue; // It's not a valid word.
                                        }
                                    }

                                    if (Filtro_Longitud_Máxima > 0) // This filter is enabled.
                                    {
                                        if (Línea.Length > Filtro_Longitud_Máxima)
                                        {
                                            continue; // It's not a valid word.
                                        }
                                    }

                                    if (Lista_Filtros_Empieza.Count > 0) // This filter is enabled.
                                    {
                                        bool Válido = false;
                                        foreach (string Filtro in Lista_Filtros_Empieza)
                                        {
                                            try
                                            {
                                                if (Línea.StartsWith(Filtro, StringComparison.InvariantCultureIgnoreCase))
                                                {
                                                    Válido = true;
                                                    break; // If it passes at least one filter, then it's valid.
                                                }
                                            }
                                            catch (Exception Excepción) { Depurador.Escribir_Excepción(Excepción != null ? Excepción.ToString() : null); Variable_Excepción_Total++; Variable_Excepción = true; continue; }
                                        }
                                        if (!Válido) continue; // It's not a valid word.
                                    }

                                    if (Lista_Filtros_No_Empieza.Count > 0) // This filter is enabled.
                                    {
                                        bool Válido = true;
                                        foreach (string Filtro in Lista_Filtros_No_Empieza)
                                        {
                                            try
                                            {
                                                if (Línea.StartsWith(Filtro, StringComparison.InvariantCultureIgnoreCase))
                                                {
                                                    Válido = false;
                                                    break; // If it fails at least one filter, then it's not valid.
                                                }
                                            }
                                            catch (Exception Excepción) { Depurador.Escribir_Excepción(Excepción != null ? Excepción.ToString() : null); Variable_Excepción_Total++; Variable_Excepción = true; continue; }
                                        }
                                        if (!Válido) continue; // It's not a valid word.
                                    }

                                    if (Lista_Filtros_Contiene.Count > 0) // This filter is enabled.
                                    {
                                        bool Válido = false;
                                        foreach (string Filtro in Lista_Filtros_Contiene)
                                        {
                                            try
                                            {
                                                if (Línea.Contains(Filtro))
                                                {
                                                    Válido = true;
                                                    break; // If it passes at least one filter, then it's valid.
                                                }
                                            }
                                            catch (Exception Excepción) { Depurador.Escribir_Excepción(Excepción != null ? Excepción.ToString() : null); Variable_Excepción_Total++; Variable_Excepción = true; continue; }
                                        }
                                        if (!Válido) continue; // It's not a valid word.
                                    }

                                    if (Lista_Filtros_No_Contiene.Count > 0) // This filter is enabled.
                                    {
                                        bool Válido = true;
                                        foreach (string Filtro in Lista_Filtros_No_Contiene)
                                        {
                                            try
                                            {
                                                if (Línea.Contains(Filtro))
                                                {
                                                    Válido = false;
                                                    break; // If it fails at least one filter, then it's not valid.
                                                }
                                            }
                                            catch (Exception Excepción) { Depurador.Escribir_Excepción(Excepción != null ? Excepción.ToString() : null); Variable_Excepción_Total++; Variable_Excepción = true; continue; }
                                        }
                                        if (!Válido) continue; // It's not a valid word.
                                    }

                                    if (Lista_Filtros_Termina.Count > 0) // This filter is enabled.
                                    {
                                        bool Válido = false;
                                        foreach (string Filtro in Lista_Filtros_Termina)
                                        {
                                            try
                                            {
                                                if (Línea.EndsWith(Filtro, StringComparison.InvariantCultureIgnoreCase))
                                                {
                                                    Válido = true;
                                                    break; // If it passes at least one filter, then it's valid.
                                                }
                                            }
                                            catch (Exception Excepción) { Depurador.Escribir_Excepción(Excepción != null ? Excepción.ToString() : null); Variable_Excepción_Total++; Variable_Excepción = true; continue; }
                                        }
                                        if (!Válido) continue; // It's not a valid word.
                                    }

                                    if (Lista_Filtros_No_Termina.Count > 0) // This filter is enabled.
                                    {
                                        bool Válido = true;
                                        foreach (string Filtro in Lista_Filtros_No_Termina)
                                        {
                                            try
                                            {
                                                if (Línea.EndsWith(Filtro, StringComparison.InvariantCultureIgnoreCase))
                                                {
                                                    Válido = false;
                                                    break; // If it fails at least one filter, then it's not valid.
                                                }
                                            }
                                            catch (Exception Excepción) { Depurador.Escribir_Excepción(Excepción != null ? Excepción.ToString() : null); Variable_Excepción_Total++; Variable_Excepción = true; continue; }
                                        }
                                        if (!Válido) continue; // It's not a valid word.
                                    }

                                    if (!Diccionario_Resultados.ContainsKey(Línea))
                                    {
                                        Diccionario_Resultados.Add(Línea, null);
                                    }
                                }
                            }
                            catch (Exception Excepción) { Depurador.Escribir_Excepción(Excepción != null ? Excepción.ToString() : null); Variable_Excepción_Total++; Variable_Excepción = true; continue; }
                        }

                        if (Diccionario_Resultados != null && Diccionario_Resultados.Count > 0)
                        {
                            string[] Matriz_Resultados = new string[Diccionario_Resultados.Count];
                            Diccionario_Resultados.Keys.CopyTo(Matriz_Resultados, 0);
                            TextBox_Lista_Resultados.Lines = Matriz_Resultados;
                            Diccionario_Resultados = null;
                        }
                    }
                    else // No filter was added yet, so just copy all the available words.
                    {
                        TextBox_Lista_Resultados.Lines = Matriz_Líneas;
                        /*if (Matriz_Líneas.Length > 0) // Super slow!
                        {
                            string Texto = null;
                            foreach (string Línea in Matriz_Líneas)
                            {
                                try
                                {
                                    Texto += Línea + "\r\n";
                                }
                                catch (Exception Excepción) { Depurador.Escribir_Excepción(Excepción != null ? Excepción.ToString() : null); Variable_Excepción_Total++; Variable_Excepción = true; continue; }
                            }
                            TextBox_Lista_Resultados.Text = Texto;
                            Texto = null;
                        }*/
                    }
                }
                this.Text = Texto_Título + " - [Original words: " + Program.Traducir_Número(TextBox_Lista_Original.Lines.Length) + ", Filtered words: " + Program.Traducir_Número(TextBox_Lista_Resultados.Lines.Length) + "]";
            }
            catch (Exception Excepción) { Depurador.Escribir_Excepción(Excepción != null ? Excepción.ToString() : null); Variable_Excepción_Total++; Variable_Excepción = true; }
            finally
            {
                this.Cursor = Cursors.Default;
                //Temporizador_Principal.Start();
            }
        }

        /// <summary>
        /// Loads a list of words form the selected file.
        /// </summary>
        /// <param name="Ruta">Any valid and existing file path.</param>
        /// <returns>Returns true if at least a word was loaded from the file. Returns false on any error.</returns>
        internal bool Cargar_Ruta(string Ruta)
        {
            try
            {
                if (!string.IsNullOrEmpty(Ruta) && File.Exists(Ruta))
                {
                    //FileStream Lector = new FileStream(Ruta, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    //if (Lector != null && Lector.Length > 0L)
                    if (new FileInfo(Ruta).Length > 0L)
                    {
                        Matriz_Líneas = null;
                        TextBox_Lista_Original.Lines = null;
                        TextBox_Lista_Resultados.Lines = null;
                        /*Lector.Seek(0L, SeekOrigin.Begin);
                        StreamReader Lector_Texto = null;
                        if (Cargar_Texto == 0)
                        {
                            Lector_Texto = new StreamReader(Lector, true);
                        }
                        else if (Cargar_Texto == 1)
                        {
                            Lector_Texto = new StreamReader(Lector, Encoding.ASCII);
                        }
                        else if (Cargar_Texto == 2)
                        {
                            Lector_Texto = new StreamReader(Lector, Encoding.Default);
                        }
                        else if (Cargar_Texto == 3)
                        {
                            Lector_Texto = new StreamReader(Lector, Encoding.UTF7);
                        }
                        else if (Cargar_Texto == 4)
                        {
                            Lector_Texto = new StreamReader(Lector, Encoding.UTF8);
                        }
                        else if (Cargar_Texto == 5)
                        {
                            Lector_Texto = new StreamReader(Lector, Encoding.Unicode);
                        }
                        else if (Cargar_Texto == 6)
                        {
                            Lector_Texto = new StreamReader(Lector, Encoding.BigEndianUnicode);
                        }
                        else if (Cargar_Texto == 7)
                        {
                            Lector_Texto = new StreamReader(Lector, Encoding.UTF32);
                        }
                        if (Lector_Texto != null)
                        {
                            //Lector_Texto.re
                            while (!Lector_Texto.EndOfStream)
                            {
                                string Línea = Lector_Texto.ReadLine();

                                if (!string.IsNullOrEmpty(Línea))
                                {
                                    //Línea = Línea.ToUpperInvariant();

                                    // TODO: extra line processing, like removing control characters?
                                }

                                //Lista_Líneas.Add(Línea);
                            }
                            Lector_Texto.Close();
                            Lector_Texto.Dispose();
                            Lector_Texto = null;
                        }
                        Lector.Close();
                        Lector.Dispose();
                        Lector = null;*/

                        if (Cargar_Texto == 0)
                        {
                            Matriz_Líneas = File.ReadAllLines(Ruta);
                        }
                        else if (Cargar_Texto == 1)
                        {
                            Matriz_Líneas = File.ReadAllLines(Ruta, Encoding.ASCII);
                        }
                        else if (Cargar_Texto == 2)
                        {
                            Matriz_Líneas = File.ReadAllLines(Ruta, Encoding.Default);
                        }
                        else if (Cargar_Texto == 3)
                        {
                            Matriz_Líneas = File.ReadAllLines(Ruta, Encoding.UTF7);
                        }
                        else if (Cargar_Texto == 4)
                        {
                            Matriz_Líneas = File.ReadAllLines(Ruta, Encoding.UTF8);
                        }
                        else if (Cargar_Texto == 5)
                        {
                            Matriz_Líneas = File.ReadAllLines(Ruta, Encoding.Unicode);
                        }
                        else if (Cargar_Texto == 6)
                        {
                            Matriz_Líneas = File.ReadAllLines(Ruta, Encoding.BigEndianUnicode);
                        }
                        else if (Cargar_Texto == 7)
                        {
                            Matriz_Líneas = File.ReadAllLines(Ruta, Encoding.UTF32);
                        }

                        if (Matriz_Líneas != null && Matriz_Líneas.Length > 0)
                        {
                            if (Menú_Contextual_Ignorar_Mayúsculas.Checked)
                            {
                                List<string> Lista_Líneas = new List<string>();
                                foreach (string Línea in Matriz_Líneas)
                                {
                                    try
                                    {
                                        if (!string.IsNullOrEmpty(Línea))
                                        {
                                            Lista_Líneas.Add(Línea.ToUpperInvariant());
                                        }
                                        else Lista_Líneas.Add(Línea);
                                    }
                                    catch (Exception Excepción) { Depurador.Escribir_Excepción(Excepción != null ? Excepción.ToString() : null); continue; }
                                }
                                Matriz_Líneas = Lista_Líneas.ToArray();
                                Lista_Líneas = null;
                            }
                            if (Menú_Contextual_Quitar_Palabras_Vacías.Checked)
                            {
                                List<string> Lista_Líneas = new List<string>();
                                foreach (string Línea in Matriz_Líneas)
                                {
                                    try
                                    {
                                        if (!string.IsNullOrEmpty(Línea))
                                        {
                                            Lista_Líneas.Add(Línea);
                                        }
                                    }
                                    catch (Exception Excepción) { Depurador.Escribir_Excepción(Excepción != null ? Excepción.ToString() : null); continue; }
                                }
                                Matriz_Líneas = Lista_Líneas.ToArray();
                                Lista_Líneas = null;
                            }
                            if (Menú_Contextual_Ordenar_Lista_Palabras.Checked)
                            {
                                if (Matriz_Líneas.Length > 1) Array.Sort(Matriz_Líneas);
                            }
                            TextBox_Lista_Original.Lines = Matriz_Líneas;
                            Buscar_Palabras();
                            Ruta_Original = Ruta;
                            return true;
                        }
                    }
                }
            }
            catch (Exception Excepción) { Depurador.Escribir_Excepción(Excepción != null ? Excepción.ToString() : null); Variable_Excepción_Total++; Variable_Excepción = true; }
            return false;
        }

        /// <summary>
        /// Filters the loaded list of words and shows the resulting words.
        /// </summary>
        internal void Buscar_Palabras()
        {
            try
            {
                Subproceso_DoWork();
                //Subproceso = new BackgroundWorker();
                //Subproceso.DoWork += Subproceso_DoWork;
                //Subproceso.RunWorkerAsync();
            }
            catch (Exception Excepción) { Depurador.Escribir_Excepción(Excepción != null ? Excepción.ToString() : null); Variable_Excepción_Total++; Variable_Excepción = true; }
        }
    }
}
