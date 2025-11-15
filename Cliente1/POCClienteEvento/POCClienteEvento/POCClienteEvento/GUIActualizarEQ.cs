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
    public partial class GUIActualizarEQ : Form
    {
        public GUIActualizarEQ()
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

        public class EquipoDto
        {
            public int idEquipo { get; set; }
            public string nombre { get; set; }
            public string ciudadOrigen { get; set; }
            public int numeroJugadores { get; set; }
            public double puntaje { get; set; }

            // viene del @JsonProperty("nombreEvento") del backend
            public string nombreEvento { get; set; }
        }

        private List<EventoDeportivoDto> _eventosCombo = new List<EventoDeportivoDto>();

        private void DeshabilitarEdicion()
        {
            txtNombre.ReadOnly = true;
            txtCiudadO.ReadOnly = true;
            txtJugadores.ReadOnly = true;
            txtPuntaje.ReadOnly = true;
            comboBoxEvento.Enabled = false;
        }

        private void HabilitarEdicion()
        {
            txtNombre.ReadOnly = false;
            txtCiudadO.ReadOnly = false;
            txtJugadores.ReadOnly = false;
            txtPuntaje.ReadOnly = false;
            comboBoxEvento.Enabled = true;
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

                        // 👉 Insertamos "Sin Evento" (ID null)
                        eventos.Insert(0, new EventoDeportivoDto
                        {
                            idEvento = null,
                            nombre = "Sin Evento"
                        });

                        _eventosCombo = eventos;

                        comboBoxEvento.DataSource = _eventosCombo;
                        comboBoxEvento.DisplayMember = "nombre";  // Mostrar solo nombre
                        comboBoxEvento.ValueMember = "idEvento";  // Value es ID (o null)
                        comboBoxEvento.SelectedIndex = 0;
                    }
                    else
                    {
                        MessageBox.Show("No se pudieron cargar los eventos.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cargando eventos: " + ex.Message);
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void buttonCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private async void buttonBuscar_Click(object sender, EventArgs e)
        {

            try
            {
                if (!int.TryParse(txtIdEquipo.Text.Trim(), out int idEquipo))
                {
                    MessageBox.Show("Ingresa un ID de equipo válido (número entero).");
                    return;
                }

                using (var client = new HttpClient())
                {
                    var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes("admin:admin"));
                    client.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", credentials);

                    string url = $"http://localhost:8091/equipos/{idEquipo}";
                    HttpResponseMessage response = await client.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();

                        var equipo = JsonSerializer.Deserialize<EquipoDto>(
                            json,
                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                        // llenar cajas de texto
                        txtNombre.Text = equipo.nombre;
                        txtCiudadO.Text = equipo.ciudadOrigen;
                        txtJugadores.Text = equipo.numeroJugadores.ToString();
                        txtPuntaje.Text = equipo.puntaje.ToString();

                        // asegurarnos de que el combo ya está cargado
                        if (_eventosCombo == null || _eventosCombo.Count == 0)
                            await CargarEventosAsync();

                        // seleccionar el evento correspondiente
                        if (string.IsNullOrEmpty(equipo.nombreEvento))
                        {
                            comboBoxEvento.SelectedIndex = 0; // Sin Evento
                        }
                        else
                        {
                            int index = _eventosCombo.FindIndex(ev => ev.nombre == equipo.nombreEvento);

                            if (index >= 0)
                                comboBoxEvento.SelectedIndex = index;
                            else
                                comboBoxEvento.SelectedIndex = 0;
                        }

                        DeshabilitarEdicion();
                    }
                    else
                    {
                        MessageBox.Show($"No se encontró el equipo con ID {idEquipo}");
                        txtNombre.Clear();
                        txtCiudadO.Clear();
                        txtJugadores.Clear();
                        txtPuntaje.Clear();
                        comboBoxEvento.SelectedIndex = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar equipo: " + ex.Message);
            }

        }

        private async void GUIActualizarEQ_Load(object sender, EventArgs e)
        {
            await CargarEventosAsync();
            DeshabilitarEdicion();
        }

        private void buttonEditar_Click(object sender, EventArgs e)
        {
            HabilitarEdicion();
        }

        private async void buttonGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                if (!int.TryParse(txtIdEquipo.Text.Trim(), out int idEquipo))
                {
                    MessageBox.Show("Debes buscar primero un equipo válido.");
                    return;
                }

                if (!int.TryParse(txtJugadores.Text.Trim(), out int numeroJugadores))
                {
                    MessageBox.Show("Número de jugadores debe ser un entero.");
                    return;
                }

                if (!double.TryParse(
                        txtPuntaje.Text.Trim(),
                        System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture,
                        out double puntaje))
                {
                    MessageBox.Show("Puntaje debe ser un número.");
                    return;
                }

                // evento seleccionado
                string idEventoSeleccionado = null;
                if (comboBoxEvento.SelectedItem is EventoDeportivoDto ev)
                {
                    idEventoSeleccionado = ev.idEvento; // será null si es "Sin Evento"
                }

                object cuerpo;

                if (string.IsNullOrEmpty(idEventoSeleccionado))
                {
                    // sin evento
                    cuerpo = new
                    {
                        idEquipo = idEquipo,
                        nombre = txtNombre.Text.Trim(),
                        ciudadOrigen = txtCiudadO.Text.Trim(),
                        numeroJugadores = numeroJugadores,
                        puntaje = puntaje,
                        eventoDeportivo = (object)null
                    };
                }
                else
                {
                    // con evento
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

                using (var client = new HttpClient())
                {
                    var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes("admin:admin"));
                    client.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", credentials);

                    string url = $"http://localhost:8091/equipos/{idEquipo}";

                    string json = JsonSerializer.Serialize(cuerpo);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PutAsync(url, content);

                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Equipo actualizado correctamente", "Éxito",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        DeshabilitarEdicion();
                    }
                    else
                    {
                        string errorMsg = await response.Content.ReadAsStringAsync();
                        MessageBox.Show($"Error al actualizar:\n{errorMsg}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar cambios: " + ex.Message);
            }
        }
    }
}
