using System;
using System.Drawing;
using System.Windows.Forms;
using FontAwesome.Sharp;

namespace P1
{
    public partial class FrmAcercaDe : Form
    {
        private Label lblTitulo, lblVersion, lblDescripcion, lblGuia, lblCreditos;
        private IconPictureBox picIcono;
        private IconButton btnCerrar;
        private Panel panelHeader, panelContenido;

        public FrmAcercaDe()
        {
            ConstruirInterfazManual();
        }

        private void ConstruirInterfazManual()
        {
            // ============ FORMULARIO ============
            this.Text = "Acerca de - Librería Morita";
            this.Size = new Size(600, 620);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.WhiteSmoke;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // ============ HEADER ============
            panelHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 90,
                BackColor = Color.FromArgb(41, 128, 185)
            };

            picIcono = new IconPictureBox
            {
                IconChar = IconChar.BookOpen,
                IconColor = Color.White,
                IconSize = 48,
                Location = new Point(25, 20),
                Size = new Size(48, 48),
                BackColor = Color.Transparent
            };
            panelHeader.Controls.Add(picIcono);

            lblTitulo = new Label
            {
                Text = "Punto de Venta",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(90, 18),
                BackColor = Color.Transparent
            };
            panelHeader.Controls.Add(lblTitulo);

            lblVersion = new Label
            {
                Text = "Librería Morita — Versión 1.1",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(220, 230, 240),
                AutoSize = true,
                Location = new Point(92, 50),
                BackColor = Color.Transparent
            };
            panelHeader.Controls.Add(lblVersion);

            this.Controls.Add(panelHeader);

            // ============ CONTENIDO ============
            panelContenido = new Panel
            {
                Location = new Point(20, 110),
                Size = new Size(545, 400),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                AutoScroll = true
            };

            lblDescripcion = new Label
            {
                Text = "Sistema de gestión para punto de venta de librería.\n" +
                       "Administra ventas, compras, inventario, clientes, proveedores, caja y promociones.",
                Font = new Font("Segoe UI", 10, FontStyle.Italic),
                ForeColor = Color.FromArgb(80, 80, 80),
                Location = new Point(20, 15),
                Size = new Size(505, 45)
            };
            panelContenido.Controls.Add(lblDescripcion);

            lblGuia = new Label
            {
                Text =
        "📖  GUÍA RÁPIDA DE USO\r\n" +
        "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\r\n\r\n" +
        "🛒  PUNTO DE VENTA\r\n" +
        "• Busca el libro en el catálogo.\r\n" +
        "• Doble clic para agregarlo al carrito.\r\n" +
        "• Usa el botón azul \"👤 Público General\" para buscar un cliente por su DNI (necesario para el descuento de cumpleaños).\r\n" +
        "• Ingresa el efectivo y presiona \"COBRAR\".\r\n\r\n" +
        "🎁  PROMOCIONES AUTOMÁTICAS\r\n" +
        "• Aniversario: 20% de descuento el 09/11/2026.\r\n" +
        "• Cumpleaños: 10% de descuento si el cliente está registrado y es su día.\r\n" +
        "• Categorías: 5% de descuento en las categorías seleccionadas.\r\n" +
        "• El sistema suma los descuentos y lo muestra antes de cobrar.\r\n\r\n" +
        "📦  COMPRAS\r\n" +
        "• Registra compras a proveedores.\r\n" +
        "• Aumenta el stock y actualiza precios automáticamente.\r\n\r\n" +
        "📚  PRODUCTOS\r\n" +
        "• Crea, edita y desactiva libros del catálogo.\r\n" +
        "• Usa el botón verde \"+\" al lado del autor para registrar escritores nuevos rápidamente.\r\n\r\n" +
        "🚚  PROVEEDORES Y 👥 CLIENTES\r\n" +
        "• Gestiona el directorio del negocio.\r\n" +
        "• No olvides registrar la fecha de nacimiento del cliente para sus promociones.\r\n\r\n" +
        "💰  CAJA\r\n" +
        "• Abre caja al iniciar turno con el monto inicial.\r\n" +
        "• Registra ingresos y egresos del turno.\r\n" +
        "• Cierra caja al final con el conteo real y verifica sobrantes o faltantes.\r\n\r\n" +
        "🔒  PERMISOS POR ROL\r\n" +
        "• Administrador: acceso total.\r\n" +
        "• Cajero: Punto de Venta y Caja.\r\n" +
        "• Almacenero: Productos, Proveedores y Compras.\r\n\r\n" +
        "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\r\n\r\n" +
        "💡  Consejo: Usa el botón \"Cerrar Sesión\" para\r\n" +
        "cambiar de usuario sin cerrar el programa.",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(60, 60, 60),
                Location = new Point(20, 70),

                AutoSize = true, // Permite que crezca en altura
                MaximumSize = new Size(505, 0) // Fija el ancho máximo para que el texto haga salto de línea

            };
            panelContenido.Controls.Add(lblGuia);

            this.Controls.Add(panelContenido);

            // ============ CRÉDITOS ============
            lblCreditos = new Label
            {
                Text = "© 2026 Librería Morita · Todos los derechos reservados",
                Font = new Font("Segoe UI", 8, FontStyle.Italic),
                ForeColor = Color.Gray,
                AutoSize = true,
                Location = new Point(20, 525)
            };
            this.Controls.Add(lblCreditos);

            // ============ BOTÓN CERRAR ============
            btnCerrar = new IconButton
            {
                Text = "Cerrar",
                IconChar = IconChar.Times,
                IconColor = Color.White,
                IconSize = 16,
                Location = new Point(460, 520),
                Size = new Size(105, 40),
                BackColor = Color.FromArgb(41, 128, 185),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                ImageAlign = ContentAlignment.MiddleLeft,
                TextAlign = ContentAlignment.MiddleRight,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnCerrar.FlatAppearance.BorderSize = 0;
            btnCerrar.Click += (s, e) => this.Close();
            this.Controls.Add(btnCerrar);

            this.CancelButton = btnCerrar;
        }
    }
}