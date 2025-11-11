using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using static POCClienteEvento.GUICrearED;

namespace POCClienteEvento
{
    public partial class GUIBuscarED : Form
    {
        public GUIBuscarED()
        {
            InitializeComponent();
        }

        private void buttonCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private async void buttonBuscar_Click(object sender, EventArgs e)
        {
            try
            {
                
                string idEvento = txtIdEvento.Text.Trim();
                if (string.IsNullOrEmpty(idEvento))
                {
                    MessageBox.Show("Por favor ingresa un IdEvento.");
                    return;
                }

                using (HttpClient client = new HttpClient())
                {
                    // Autenticación
                    var credentials = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes("admin:admin"));
                    client.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", credentials);

                    // URL con el ID
                    string url = $"http://localhost:8091/eventos/{idEvento}";

                    // Petición GET
                    HttpResponseMessage response = await client.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();

                        // Deserializar usando DTO
                        var evento = JsonSerializer.Deserialize<EventoD>(json);

                        // imprimir datos
                        txtNombre.Text = evento.nombre;
                        txtCiudad.Text = evento.ciudad;
                        txtAsistentes.Text = evento.asistentes.ToString();
                        txtTipoDeporte.Text = evento.tipoDeporte ?? "";
                        txtFecha.Text = evento.fecha;

                        
                        txtNombre.ReadOnly = true;
                        txtCiudad.ReadOnly = true;
                        txtAsistentes.ReadOnly = true;
                        txtTipoDeporte.ReadOnly = true;
                        txtFecha.ReadOnly = true;

                        //para listbox en equipos jeje
                        listaEquipos.Items.Clear();

                        if (evento.equipos != null && evento.equipos.Count > 0)
                        {
                            foreach (var eq in evento.equipos)
                            {
                                listaEquipos.Items.Add(eq.nombre);
                            }
                        }
                        else
                        {
                            listaEquipos.Items.Add("Sin equipos registrados.");
                        }
                    }
                    else
                    {
                        MessageBox.Show($"No se encontró el evento con el Id {idEvento}");
                        // Limpiar campos si no estan
                        txtNombre.Clear();
                        txtCiudad.Clear();
                        txtAsistentes.Clear();
                        txtTipoDeporte.Clear();
                        txtFecha.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar evento: " + ex.Message);
            }


        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void listaEquipos_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }

    public class EventoD
    {
        public String idEvento { get; set; }
        public string nombre { get; set; }
        public string ciudad { get; set; }
        public int asistentes { get; set; }
        public string fecha { get; set; }
        public string tipoDeporte { get; set; }

        public List<EquipoD> equipos { get; set; }
    }

    public class EquipoD
    {
        public int idEquipo { get; set; }
        public string nombre { get; set; }
    }
}
