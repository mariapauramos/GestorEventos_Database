using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POCClienteEvento
{
    public partial class GUICrearEQ : Form
    {
        public GUICrearEQ()
        {
            InitializeComponent();
        }
        public class EventoDeportivoDto
        {
            public string idEvento { get; set; }
            public string nombre { get; set; }

            public override string ToString()
            {
                return nombre; 
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void dateTimePickerFecha_ValueChanged(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private async Task CargarEventosAsync()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes("admin:admin"));
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Basic", credentials);

                    HttpResponseMessage response = await client.GetAsync("http://localhost:8091/eventos/");

                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();

                        var eventos = JsonSerializer.Deserialize<List<EventoDeportivoDto>>(
                            json,
                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                        // 👉 Insertamos la opción "Sin Evento" al inicio
                        eventos.Insert(0, new EventoDeportivoDto
                        {
                            idEvento = null,
                            nombre = "Sin Evento"
                        });

                        comboBoxEvento.DataSource = eventos;
                        comboBoxEvento.DisplayMember = "nombre";
                        comboBoxEvento.ValueMember = "idEvento";

                        comboBoxEvento.SelectedIndex = 0; // que arranque en "Sin Evento"
                    }
                    else
                    {
                        MessageBox.Show("No se pudieron cargar los eventos.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cargando eventos: " + ex.Message);
            }
        }

        private async void buttonCrear_Click(object sender, EventArgs e)
        {

            // Obtenemos el evento seleccionado
            string idEventoSeleccionado = null;

            if (comboBoxEvento.SelectedItem is EventoDeportivoDto ev)
            {
                idEventoSeleccionado = ev.idEvento; // será null si es "Sin Evento"
            }

            int idEquipo = int.TryParse(txtIdEquipo.Text.Trim(), out int idEq) ? idEq : 0;
            int numeroJugadores = int.TryParse(txtJugadores.Text.Trim(), out int nj) ? nj : 0;
            double puntaje = double.TryParse(
                txtPuntaje.Text.Trim(),
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture,
                out double p) ? p : 0.0;

            object cuerpo;

            if (string.IsNullOrEmpty(idEventoSeleccionado))
            {
                // 👉 Opción "Sin Evento" seleccionada
                cuerpo = new
                {
                    idEquipo = idEquipo,
                    nombre = txtNombre.Text.Trim(),
                    ciudadOrigen = txtCiudadO.Text.Trim(),
                    numeroJugadores = numeroJugadores,
                    puntaje = puntaje,
                    eventoDeportivo = (object)null   // se envía null al backend
                };
            }
            else
            {
                // 👉 Evento real seleccionado
                cuerpo = new
                {
                    idEquipo = idEquipo,
                    nombre = txtNombre.Text.Trim(),
                    ciudadOrigen = txtCiudadO.Text.Trim(),
                    numeroJugadores = numeroJugadores,
                    puntaje = puntaje,
                    eventoDeportivo = new
                    {
                        idEvento = idEventoSeleccionado
                    }
                };
            }

            try
            {
                using (var client = new HttpClient())
                {
                    var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes("admin:admin"));
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Basic", credentials);

                    var json = JsonSerializer.Serialize(cuerpo);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync("http://localhost:8091/equipos/", content);

                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Equipo creado correctamente", "Éxito",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

                        txtIdEquipo.Clear();
                        txtNombre.Clear();
                        txtCiudadO.Clear();
                        txtJugadores.Clear();
                        txtPuntaje.Clear();
                        comboBoxEvento.SelectedIndex = 0; // volver a "Sin Evento"
                    }
                    else
                    {
                        string errorMsg = await response.Content.ReadAsStringAsync();
                        MessageBox.Show($"Código: {response.StatusCode}\n\nError:\n{errorMsg}",
                            "Error del servidor", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrió un error: " + ex.Message);
            }

        }

        private void buttonCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private async void GUICrearEQ_Load(object sender, EventArgs e)
        {
            await CargarEventosAsync();
        }
    }
}
