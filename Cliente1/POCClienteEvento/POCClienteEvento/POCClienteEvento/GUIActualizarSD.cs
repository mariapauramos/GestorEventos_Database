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
using static POCClienteEvento.GUIBuscarSD;

namespace POCClienteEvento
{
    public partial class GUIActualizarSD : Form
    {
        private List<EventoDeportivoDto> eventos = new List<EventoDeportivoDto>();
        public GUIActualizarSD()
        {
            InitializeComponent();
        }


        private async void GUIActualizarSD_Load(object sender, EventArgs e)
        {
            comboBoxCubierta.Items.Clear();
            comboBoxCubierta.Items.Add("Sí");
            comboBoxCubierta.Items.Add("No");
            comboBoxCubierta.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxCubierta.Enabled = false;

            comboBoxEvento.DataSource = null; // inicial vacío
            comboBoxEvento.SelectedIndex = -1; // no seleccionar nada
            comboBoxEvento.Enabled = false;


            // Bloquear campos
            BloquearCampos();

            // Cargar eventos desde backend
            await CargarEventosAsync();
        }


        private void buttonCerrar_Click(object sender, EventArgs e)
        {
            this.Close();

        }

        // DTO para eventos
        public class EventoDeportivoDto
        {
            public string idEvento { get; set; }
            public string nombre { get; set; }

            public override string ToString() => nombre;
        }

        // DTO para sede
        public class SedeBuscarDto
        {
            public int idSede { get; set; }
            public string nombre { get; set; }
            public int capacidad { get; set; }
            public string direccion { get; set; }
            public double costoMantenimiento { get; set; }
            public bool cubierta { get; set; }
            public DateTime fechaCreacion { get; set; }
            public string idEventoAsociado { get; set; }
        }


        private async Task CargarEventosAsync()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes("admin:admin"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

                    HttpResponseMessage response = await client.GetAsync("http://localhost:8091/eventos/");

                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();
                        var listaEventos = JsonSerializer.Deserialize<List<EventoDeportivoDto>>(json,
                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                        // Insertar opción "Sin evento"
                        listaEventos.Insert(0, new EventoDeportivoDto { idEvento = null, nombre = "Sin evento" });

                        eventos = listaEventos;

                        comboBoxEvento.DataSource = eventos;
                        comboBoxEvento.DisplayMember = "nombre";
                        comboBoxEvento.ValueMember = "idEvento";
                        comboBoxEvento.Enabled = false; // hasta buscar sede
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


        private async void buttonBuscar_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();

            if (!int.TryParse(txtIdSede.Text.Trim(), out int idSede))
            {
                MessageBox.Show("Ingresa un ID de sede válido.");
                return;
            }

            // Esperar a que los eventos estén cargados
            if (eventos == null || eventos.Count == 0)
                await CargarEventosAsync();

            try
            {
                using (var client = new HttpClient())
                {
                    var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes("admin:admin"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

                    string url = $"http://localhost:8092/sedes/{idSede}";
                    HttpResponseMessage response = await client.GetAsync(url);

                    if (!response.IsSuccessStatusCode)
                    {
                        MessageBox.Show($"No se encontró la sede con el ID {idSede}");
                        return;
                    }

                    string json = await response.Content.ReadAsStringAsync();
                    var sede = JsonSerializer.Deserialize<SedeBuscarDto>(json,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    // Llenar campos
                    txtNombre.Text = sede.nombre;
                    txtCapacidad.Text = sede.capacidad.ToString();
                    txtDireccion.Text = sede.direccion;
                    txtCosto.Text = sede.costoMantenimiento.ToString("0.00");

                    comboBoxCubierta.Items.Clear();
                    comboBoxCubierta.Items.Add("Sí"); 
                    comboBoxCubierta.Items.Add("No");
                    comboBoxCubierta.SelectedIndex = sede.cubierta ? 0 : 1;


                    dateTimePickerFecha.Value = sede.fechaCreacion;

                    if (comboBoxEvento.Items.Count > 0)
                    {
                        if (string.IsNullOrEmpty(sede.idEventoAsociado))
                            comboBoxEvento.SelectedIndex = 0; // Sin evento
                        else
                        {
                            int idx = eventos.FindIndex(ev => ev.idEvento == sede.idEventoAsociado);
                            comboBoxEvento.SelectedIndex = idx >= 0 ? idx : 0;
                            comboBoxEvento.Enabled = false; // bloquear hasta presionar Editar

                        }
                    }

                    BloquearCampos();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar sede: " + ex.Message);
            }

        }

        private void buttonEditar_Click(object sender, EventArgs e)
        {
            txtNombre.ReadOnly = false;
            txtCapacidad.ReadOnly = false;
            txtDireccion.ReadOnly = false;
            txtCosto.ReadOnly = false;
            comboBoxCubierta.Enabled = true;
            comboBoxEvento.Enabled = true;
            dateTimePickerFecha.Enabled = true;

        }

        private async void buttonGuardar_Click(object sender, EventArgs e)
        {

            if (!int.TryParse(txtIdSede.Text.Trim(), out int idSede))
            {
                MessageBox.Show("Debes buscar primero una sede válida.");
                return;
            }

            if (!int.TryParse(txtCapacidad.Text.Trim(), out int capacidad))
            {
                MessageBox.Show("La capacidad debe ser un número entero.");
                return;
            }

            if (!double.TryParse(txtCosto.Text.Trim(), out double costo))
            {
                MessageBox.Show("El costo debe ser un número válido.");
                return;
            }

            // Obtener el ID del evento seleccionado
            string idEventoSeleccionado = null;
            if (comboBoxEvento.SelectedItem is EventoDeportivoDto ev)
                idEventoSeleccionado = ev.idEvento;

            // Si no hay evento, usar "SIN_EVENTO" igual que en Crear
            string idEventoAsociado = idEventoSeleccionado ?? "SIN_EVENTO";

            // Formatear fecha igual que en Crear
            string fechaIso = dateTimePickerFecha.Value.ToString("yyyy-MM-dd'T'HH:mm:ss");

            // Crear el cuerpo IGUAL que en Crear
            var cuerpo = new
            {
                idSede = idSede,
                nombre = txtNombre.Text.Trim(),
                capacidad = capacidad,
                direccion = txtDireccion.Text.Trim(),
                costoMantenimiento = costo,
                fechaCreacion = fechaIso,
                cubierta = comboBoxCubierta.SelectedIndex == 0,
                idEventoAsociado = idEventoAsociado  // <-- String directo, no objeto
            };

            try
            {
                using (var client = new HttpClient())
                {
                    var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes("admin:admin"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

                    string url = $"http://localhost:8092/sedes/{idSede}";
                    string json = JsonSerializer.Serialize(cuerpo);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PutAsync(url, content);

                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Sede actualizada correctamente.");
                        LimpiarFormulario();
                        txtIdSede.Clear();
                        BloquearCampos();
                    }
                    else
                    {
                        string errorMsg = await response.Content.ReadAsStringAsync();
                        MessageBox.Show($"Error al actualizar sede: {errorMsg}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar cambios: " + ex.Message);
            }

        }

        private void LimpiarFormulario()
        {
            txtNombre.Clear();
            txtCapacidad.Clear();
            txtDireccion.Clear();
            txtCosto.Clear();
            dateTimePickerFecha.Value = DateTime.Today;

            if (comboBoxEvento.Items.Count > 0)
                comboBoxEvento.SelectedIndex = -1;
        }

        private void BloquearCampos()
        {
            txtNombre.ReadOnly = true;
            txtCapacidad.ReadOnly = true;
            txtDireccion.ReadOnly = true;
            txtCosto.ReadOnly = true;
            comboBoxCubierta.Enabled = false;
            comboBoxEvento.Enabled = false;
            dateTimePickerFecha.Enabled = false;
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {

        }
    }
}
