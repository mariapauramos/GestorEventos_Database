using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//Solo prueba de que este es el archivo final
namespace POCClienteEvento
{
    public partial class GUIPrincipal : Form
    {
        public GUIPrincipal()
        {
            InitializeComponent();
        }

        private void jMenuItemCrear_Click(object sender, EventArgs e)
        {
            GUICrearED gui = new GUICrearED();
            gui.Show();

        }

        private void jMenuItemSalir_Click(object sender, EventArgs e)
        {
            Application.Exit();

        }

        private void jMenuItemBuscar_Click(object sender, EventArgs e)
        {
            GUIBuscarED gui = new GUIBuscarED();
            gui.Show();
        }

        private void jMenuItemEliminar_Click(object sender, EventArgs e)
        {
            GUIEliminarED gui = new GUIEliminarED(); 
            gui.Show();
        }

        private void listarPorParametroToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GUIListarParametroED gui = new GUIListarParametroED();
            gui.Show();
        }

        private void jMenuItemListar_Click(object sender, EventArgs e)
        {
            GUIListarED gui = new GUIListarED();
            gui.Show();
        }

        private void jMenuItemActualizar_Click(object sender, EventArgs e)
        {
            GUIActualizarED gui = new GUIActualizarED();
            gui.Show();
        }

        private void jMenuItemAcerca_Click(object sender, EventArgs e)
        {
            GUIAcercaDe gui = new GUIAcercaDe();
            gui.Show();
        }

        private void jMenuEvento_Click(object sender, EventArgs e)
        {

        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            GUICrearEQ gui = new GUICrearEQ();
            gui.Show();
        }

        private void jMenuBuscarEquipo_Click(object sender, EventArgs e)
        {
            GUIBuscarEQ gui = new GUIBuscarEQ();
            gui.Show();
        }

        private void jMenuListarEquipo_Click(object sender, EventArgs e)
        {
            GUIListarEQ gui = new GUIListarEQ();
            gui.Show();
        }

        private void jMenuFiltroEquipo_Click(object sender, EventArgs e)
        {
            GUIListarParametroEQ gui = new GUIListarParametroEQ();
            gui.Show();
        }

        private void jMenuActualizarEquipo_Click(object sender, EventArgs e)
        {
            GUIActualizarEQ gui = new GUIActualizarEQ();
            gui.Show();
        }

        private void jMenuEliminarEquipo_Click(object sender, EventArgs e)
        {
            GUIEliminarEQ gui = new GUIEliminarEQ();
            gui.Show();
        }

        private void jMenuCrearSede_Click(object sender, EventArgs e)
        {
            GUICrearSD gui = new GUICrearSD();
            gui.Show();
        }

        private void jMenuBuscarSede_Click(object sender, EventArgs e)
        {
            GUIBuscarSD gui = new GUIBuscarSD();
            gui.Show();
        }

        private void jMenuListarSede_Click(object sender, EventArgs e)
        {
            GUIListarSD gui = new GUIListarSD();
            gui.Show();
        }

        private void jMenuFiltroSede_Click(object sender, EventArgs e)
        {
            GUIListarParametroSD gui = new GUIListarParametroSD();
            gui.Show();
        }

        private void jMenuActualizarSede_Click(object sender, EventArgs e)
        {
            GUIActualizarSD gui = new GUIActualizarSD();
            gui.Show();
        }

        private void jMenuEliminarSede_Click(object sender, EventArgs e)
        {
            GUIEliminarSD gui = new GUIEliminarSD();
            gui.Show();

        }
    }
}
