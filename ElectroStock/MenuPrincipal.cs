using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Studium
{
 
    public partial class MenuPrincipal : Form
    {
        string _rol;
        string usuarioiniciado;
        public Panel PanelContainer => panelContainer;
        public MenuPrincipal(string rol, string username)
        {
            InitializeComponent();
            _rol = rol;
            usuarioiniciado = username;
            ConfigurarMenuPorRol();
            this.Layout += new LayoutEventHandler(MenuPrincipal_Layout);
        }
        private void ConfigurarMenuPorRol()
        {
            if (_rol == "Administrador")
            {
                // Habilitar todos los botones
                iconButton1.Enabled = true;
                iconButton2.Enabled = true;
                iconButton3.Enabled = true;
                iconButton4.Enabled = true;
                iconButton5.Enabled = true;
                iconButton6.Enabled = true;
                iconButton7.Enabled = true;
            }
            else if (_rol == "Empleado")
            {
                // Deshabilitar algunos botones
                iconButton1.Enabled = true;
                iconButton2.Enabled = false;
                iconButton3.Enabled = false;
                iconButton4.Enabled = false;
                iconButton5.Enabled = true;
                iconButton6.Enabled = true;
                iconButton7.Enabled = true;
            }
        }
        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void bunifuLabel1_Click(object sender, EventArgs e)
        {

        }

        private void bunifuLabel2_Click(object sender, EventArgs e)
        {

        }

        private void MenuPrincipal_Load(object sender, EventArgs e)
        {
            lblUsuario.Text = UserClass.Username;
            frmDashBoard frmdashboard = new frmDashBoard();
            LoadFormInPanel(new frmDashBoard());

        }

        private void bunifuPanel2_Click(object sender, EventArgs e)
        {

        }

        private void bunifuLabel1_Click_1(object sender, EventArgs e)
        {

        }

        private void bunifuLabel1_Click_2(object sender, EventArgs e)
        {

        }

        private void bunifuLabel3_Click(object sender, EventArgs e)
        {

        }

        private void iconButton2_Click(object sender, EventArgs e)
        { 
            
            frmUsuarios frmusuarios = new frmUsuarios(usuarioiniciado);
            LoadFormInPanel(frmusuarios); 

        }

        private Form currentForm; // Variable para rastrear el formulario actual

        public void LoadFormInPanel(Form form)
        {
            // Verifica si hay un formulario cargado actualmente y si es frmVentas
            if (currentForm != null && currentForm is frmVentas ventasForm)
            {
                // Verifica si la DataGridView tiene filas con datos reales
                bool tieneDatos = ventasForm.dgvVentas.Rows
                    .Cast<DataGridViewRow>()
                    .Any(row => !row.IsNewRow && row.Cells.Cast<DataGridViewCell>().Any(cell => cell.Value != null));

                if (tieneDatos)
                {
                    DialogResult confirmacion = MessageBox.Show(
                        "El pedido contiene información. ¿Está seguro de que desea abandonarlo?",
                        "Confirmación",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question
                    );

                    if (confirmacion == DialogResult.No)
                    {
                        return; // Cancela la operación si el usuario selecciona "No"
                    }
                }
            }

            // Limpiar el panel (si ya hay algo cargado)
            panelContainer.Controls.Clear();

            // Configurar el nuevo formulario
            form.TopLevel = false; // Indicar que no es de nivel superior
            form.FormBorderStyle = FormBorderStyle.None; // Sin bordes
            form.Dock = DockStyle.Fill; // Ajustar al tamaño del panel

            // Agregar el formulario al panel
            panelContainer.Controls.Add(form);

            // Mostrar el formulario
            form.Show();

            // Actualizar la referencia del formulario actual
            currentForm = form;
        }

        private void panelContainer_Paint(object sender, PaintEventArgs e)
        {

        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            frmDashBoard frmdashboard = new frmDashBoard();
            LoadFormInPanel(new frmDashBoard());
        }

        private void bunifuFormControlBox1_HelpClicked(object sender, EventArgs e)
        {
           
        }

        private void iconButton8_Click(object sender, EventArgs e)
        {
            frmSalir frmSalir = new frmSalir();
            frmSalir.Show();
        }

        private void MenuPrincipal_Resize(object sender, EventArgs e)
        {
         
        }

        private void MenuPrincipal_Layout(object sender, LayoutEventArgs e)
        {
            if (this.WindowState == FormWindowState.Maximized)
            {

                // Ajusta la posición del botón a la esquina superior derecha
                iconButton8.Left = this.ClientSize.Width - iconButton8.Width - 10; // 10 es el margen
                iconButton8.Top = 10; // 10 es el margen superior
            }
        }

        private void iconButton3_Click(object sender, EventArgs e)
        {
            frmProveedores frmproveedor = new frmProveedores();
            LoadFormInPanel(frmproveedor);
        }

        private void iconButton4_Click(object sender, EventArgs e)
        {
            LoadFormInPanel(new frmProductos());
        }

        private void iconButton9_Click(object sender, EventArgs e)
        {
            LoginFrm login = new LoginFrm();
            login.Show();
            this.Close();
        }

        private void bunifuPanel1_Click(object sender, EventArgs e)
        {

        }

        private void iconButton6_Click(object sender, EventArgs e)
        {
            frmClientes frmCliente = new frmClientes();
         
            LoadFormInPanel(frmCliente);
        }

        private void iconButton5_Click(object sender, EventArgs e)
        {

            FormUtils.LoadFormInPanel(panelContainer, new frmVentas());
        }

        private void iconButton7_Click(object sender, EventArgs e)
        {
            frmReportes frmReporte = new frmReportes();
            LoadFormInPanel(frmReporte);
        }
    }
}
