using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Studium
{
   
        public static class FormUtils
        {
        private static Form currentForm = null;

        /// <summary>
        /// Carga un formulario en un panel, cerrando el formulario actual si existe.
        /// </summary>
        /// <param name="panel">El panel donde se cargará el nuevo formulario.</param>
        /// <param name="newForm">El formulario que se desea cargar.</param>
        public static void LoadFormInPanel(Panel panel, Form newForm)
        {
            // Verifica si hay un formulario actual y lo cierra
            currentForm?.Close();

            // Limpiar el panel
            panel.Controls.Clear();

            // Configurar el nuevo formulario
            newForm.TopLevel = false;
            newForm.FormBorderStyle = FormBorderStyle.None;
            newForm.Dock = DockStyle.Fill;

            // Agregar el formulario al panel
            panel.Controls.Add(newForm);

            // Mostrar el formulario
            newForm.Show();

            // Actualizar el formulario actual
            currentForm = newForm;
        }
    }
}
