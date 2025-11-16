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
    public partial class GUICrearSD : Form
    {
        public GUICrearSD()
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

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void buttonCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private async void GUICrearSD_Load(object sender, EventArgs e)
        {
            comboBoxCubiera.Items.Clear();
            comboBoxCubiera.Items.Add("Sí");
            comboBoxCubiera.Items.Add("No");
            comboBoxCubiera.SelectedIndex = 1;

            await CargarEventosAsync();
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

            // Obtener el evento seleccionado
            string idEventoSeleccionado = null;

            if (comboBoxEvento.SelectedItem is EventoDeportivoDto ev)
            {
                idEventoSeleccionado = ev.idEvento; // será null si es "Sin Evento"
            }

            // Parsear valores
            int idSede = int.TryParse(txtIdSede.Text.Trim(), out int id) ? id : 0;
            int capacidad = int.TryParse(txtJCapacidad.Text.Trim(), out int cap) ? cap : 0;
            double costo = double.TryParse(
                txtCosto.Text.Trim(),
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture,
                out double cm) ? cm : 0.0;

            string cubiertaSeleccionada = comboBoxCubiera.SelectedItem?.ToString() ?? "No";

            bool cubierta = cubiertaSeleccionada.Equals("Sí", StringComparison.OrdinalIgnoreCase);

            // Mapea directamente al campo REAL que tu API usa
            string idEventoAsociado = idEventoSeleccionado ?? "SIN_EVENTO";

             string fechaIso = dateTimePickerFecha.Value.ToString("yyyy-MM-dd'T'HH:mm:ss");

            // Crear el cuerpo JSON EXACTO que tu API espera
            object cuerpo = new
            {
                idSede = idSede,
                nombre = txtNombre.Text.Trim(),
                capacidad = capacidad,
                direccion = txtDireccion.Text.Trim(),
                costoMantenimiento = costo,
                fechaCreacion = fechaIso,
                cubierta = cubierta,
                idEventoAsociado = idEventoAsociado
            };

            try
            {
                using (var client = new HttpClient())
                {
                    var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes("admin:admin"));
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Basic", credentials);

                     var json = JsonSerializer.Serialize(cuerpo);
                     var content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync("http://localhost:8092/sedes/", content);

                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Sede creada correctamente", "Éxito",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Limpiar campos
                        txtIdSede.Clear();
                        txtNombre.Clear();
                        txtJCapacidad.Clear();
                        txtDireccion.Clear();
                        txtCosto.Clear();
                        comboBoxCubiera.SelectedIndex = 0;
                        comboBoxEvento.SelectedIndex = 0;
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
    }
}
