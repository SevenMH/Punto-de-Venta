using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using FontAwesome.Sharp;

namespace P1
{
    public partial class PantallaVenta : Form
    {
        // Controles
        private Label lblTitulo, lblUsuario, lblResultadosTitulo, lblCarritoTitulo;
        private TextBox txtBuscar;
        private IconPictureBox picBuscar;
        private DataGridView dgvResultados, dgvCarrito;
        private Label lblSubtotal, lblIva, lblTotal, lblPago, lblCambio;
        private NumericUpDown numPago, numCantidad;
        private IconButton btnAgregar, btnQuitar, btnCobrar, btnCancelar, btnCerrarSesion;
        private IconButton btnCompras;
        private IconButton btnProductos;
        private IconButton btnProveedores;
        private IconButton btnClientes;
        private IconButton btnCaja;
        private IconButton btnReportes;
        private IconButton btnAcercaDe;
        private Panel panelHeader;
        private int idClienteActual = 0;
        private string nombreClienteActual = "Público General";
        private string docClienteActual = "00000000";
        private DateTime? fechaNacimientoCliente = null;
        private Label lblDescuento; 
        private IconButton btnSeleccionarCliente; 

        // Datos
        private List<DetalleVentaItem> carrito = new List<DetalleVentaItem>();
        private readonly string connectionString =
            @"Server=DESKTOP-IQ00U68;Database=Libreria;Integrated Security=True;TrustServerCertificate=True;";

        private const decimal IVA = 0.16m;

        public PantallaVenta()
        {
            ConstruirInterfazManual();
            MostrarUsuarioActual();
            AplicarPermisosPorRol();
            ConfigurarBotonCerrarSesion();
        }

        private void ConstruirInterfazManual()
        {
            this.Text = "Punto de Venta - Librería Morita";
            this.Size = new Size(720, 720); // ancho 1500 , alto
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.WhiteSmoke;
            this.MinimumSize = new Size(1400, 650);

            // ENCABEZADO
            panelHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.FromArgb(41, 128, 185)
            };

            lblTitulo = new Label
            {
                Text = "🛒 Punto de Venta - Librería Morita",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(20, 15),
                BackColor = Color.Transparent
            };
            panelHeader.Controls.Add(lblTitulo);

            lblUsuario = new Label
            {
                Text = "",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.White,
                AutoSize = true,
                BackColor = Color.Transparent
            };
            panelHeader.Controls.Add(lblUsuario);

            btnCerrarSesion = new IconButton
            {
                Text = "Cerrar Sesión",
                IconChar = IconChar.SignOutAlt,
                IconColor = Color.White,
                IconSize = 18,
                Size = new Size(110, 35), // (ancho, alto)
                BackColor = Color.FromArgb(192, 57, 43),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                ImageAlign = ContentAlignment.MiddleLeft,
                TextAlign = ContentAlignment.MiddleRight,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };

            btnCompras = new IconButton
            {
                Text = "Compras",
                IconChar = IconChar.CartArrowDown,
                IconColor = Color.White,
                IconSize = 18,
                Size = new Size(110, 35), // (ancho, alto)
                BackColor = Color.FromArgb(39, 174, 96),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                ImageAlign = ContentAlignment.MiddleLeft,
                TextAlign = ContentAlignment.MiddleRight,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };

            btnCompras.FlatAppearance.BorderSize = 0;
            btnCompras.Click += (s, e) =>
            {
                var frm = new FrmCompras();
                frm.ShowDialog();
                CargarLibros(txtBuscar.Text.Trim()); // Refrescar catálogo al cerrar
            };

            btnProductos = new IconButton
            {
                Text = "Productos",
                IconChar = IconChar.Book,
                IconColor = Color.White,
                IconSize = 18,
                Size = new Size(110, 35), // (ancho, alto)
                BackColor = Color.FromArgb(155, 89, 182),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                ImageAlign = ContentAlignment.MiddleLeft,
                TextAlign = ContentAlignment.MiddleRight,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            btnProductos.FlatAppearance.BorderSize = 0;
            btnProductos.Click += (s, e) =>
            {
                var frm = new FrmProductos();
                frm.ShowDialog();
                CargarLibros(txtBuscar.Text.Trim()); // Refrescar catálogo al cerrar
            };

            btnProveedores = new IconButton
            {
                Text = "Proveedores",
                IconChar = IconChar.Truck,
                IconColor = Color.White,
                IconSize = 18,
                Size = new Size(110, 35), // (ancho, alto)
                BackColor = Color.FromArgb(155, 89, 182),  // Morado
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                ImageAlign = ContentAlignment.MiddleLeft,
                TextAlign = ContentAlignment.MiddleRight,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            btnProveedores.FlatAppearance.BorderSize = 0;
            btnProveedores.Click += (s, e) =>
            {
                var frm = new FrmProveedores();
                frm.ShowDialog();
            };

            btnClientes = new IconButton
            {
                Text = "Clientes",
                IconChar = IconChar.Users,
                IconColor = Color.White,
                IconSize = 18,
                Size = new Size(120, 35),
                BackColor = Color.FromArgb(230, 126, 34),  // Naranja
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                ImageAlign = ContentAlignment.MiddleLeft,
                TextAlign = ContentAlignment.MiddleRight,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            btnClientes.FlatAppearance.BorderSize = 0;
            btnClientes.Click += (s, e) =>
            {
                var frm = new FrmClientes();
                frm.ShowDialog();
            };

            btnCaja = new IconButton
            {
                Text = "Caja",
                IconChar = IconChar.CashRegister,
                IconColor = Color.White,
                IconSize = 18,
                Size = new Size(100, 35),
                BackColor = Color.FromArgb(39, 174, 96),  // Verde
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                ImageAlign = ContentAlignment.MiddleLeft,
                TextAlign = ContentAlignment.MiddleRight,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            btnCaja.FlatAppearance.BorderSize = 0;
            btnCaja.Click += (s, e) =>
            {
                var frm = new FrmCaja();
                frm.ShowDialog();
            };

            btnReportes = new IconButton
            {
                Text = "Reportes",
                IconChar = IconChar.ChartBar,
                IconColor = Color.White,
                IconSize = 18,
                Size = new Size(110, 35),
                BackColor = Color.FromArgb(155, 89, 182),  // Morado
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                ImageAlign = ContentAlignment.MiddleLeft,
                TextAlign = ContentAlignment.MiddleRight,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Location = new Point(panelHeader.Width - 920, 12)
            };
            btnReportes.FlatAppearance.BorderSize = 0;
            btnReportes.Click += (s, e) =>
            {
                var frm = new FrmReportes();
                frm.ShowDialog();
            };
            panelHeader.Controls.Add(btnReportes);

            panelHeader.Controls.Add(btnCaja);
            panelHeader.Controls.Add(btnClientes);
            panelHeader.Controls.Add(btnProveedores);
            panelHeader.Controls.Add(btnProductos);

            panelHeader.Controls.Add(btnCompras);
            btnCerrarSesion.FlatAppearance.BorderSize = 0;
            btnCerrarSesion.Click += BtnCerrarSesion_Click;
            panelHeader.Controls.Add(btnCerrarSesion);

            panelHeader.Resize += (s, e) => ReposicionarHeader();

            this.Controls.Add(panelHeader);

            // PANEL IZQUIERDO
            Panel panelIzquierdo = new Panel
            {
                Location = new Point(20, 80),
                Size = new Size(600, 600),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            lblResultadosTitulo = new Label
            {
                Text = "📚 Catálogo de Libros",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(41, 128, 185),
                Location = new Point(15, 15),
                AutoSize = true
            };
            panelIzquierdo.Controls.Add(lblResultadosTitulo);

            picBuscar = new IconPictureBox
            {
                IconChar = IconChar.Search,
                IconColor = Color.Gray,
                IconSize = 20,
                Location = new Point(15, 50),
                Size = new Size(20, 20),
                BackColor = Color.Transparent
            };
            panelIzquierdo.Controls.Add(picBuscar);

            txtBuscar = new TextBox
            {
                Location = new Point(45, 47),
                Size = new Size(540, 28),
                Font = new Font("Segoe UI", 11),
                PlaceholderText = "Buscar por título, autor o categoría..."
            };
            txtBuscar.TextChanged += TxtBuscar_TextChanged;
            panelIzquierdo.Controls.Add(txtBuscar);

            dgvResultados = new DataGridView
            {
                Location = new Point(15, 90),
                Size = new Size(570, 495),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                Font = new Font("Segoe UI", 10)
            };
            dgvResultados.Columns.Add("IdLibro", "ID");
            dgvResultados.Columns.Add("Titulo", "Título");
            dgvResultados.Columns.Add("Precio", "Precio");
            dgvResultados.Columns.Add("Stock", "Stock");
            dgvResultados.Columns.Add("IdCategoria", "Categoría");

            dgvResultados.Columns["IdLibro"].Visible = false;
            dgvResultados.Columns["IdCategoria"].Visible = false;
            dgvResultados.Columns["Precio"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvResultados.Columns["Stock"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvResultados.CellDoubleClick += DgvResultados_CellDoubleClick;
            panelIzquierdo.Controls.Add(dgvResultados);

            this.Controls.Add(panelIzquierdo);

            // PANEL DERECHO
            Panel panelDerecho = new Panel
            {
                Location = new Point(640, 80),
                Size = new Size(540, 600),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            lblCarritoTitulo = new Label
            {
                Text = "🛒 Carrito de Venta",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(41, 128, 185),
                Location = new Point(15, 15),
                AutoSize = true
            };
            panelDerecho.Controls.Add(lblCarritoTitulo);

            // BOTÓN PARA SELECCIONAR CLIENTE
            btnSeleccionarCliente = new IconButton
            {
                Text = "👤 Público General",
                IconChar = IconChar.User,
                IconColor = Color.White,
                IconSize = 16,
                Location = new Point(300, 10), // A la derecha del título "Carrito"
                Size = new Size(225, 30),
                BackColor = Color.FromArgb(52, 73, 94),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                ImageAlign = ContentAlignment.MiddleLeft,
                TextAlign = ContentAlignment.MiddleRight,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            btnSeleccionarCliente.FlatAppearance.BorderSize = 0;
            btnSeleccionarCliente.Click += BtnSeleccionarCliente_Click;
            panelDerecho.Controls.Add(btnSeleccionarCliente);

            dgvCarrito = new DataGridView
            {
                Location = new Point(15, 50),
                Size = new Size(510, 250),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                RowHeadersVisible = false,
                Font = new Font("Segoe UI", 10)
            };
            dgvCarrito.Columns.Add("IdLibro", "ID");
            dgvCarrito.Columns.Add("Titulo", "Título");
            dgvCarrito.Columns.Add("Cantidad", "Cant.");
            dgvCarrito.Columns.Add("Precio", "P. Unit.");
            dgvCarrito.Columns.Add("Subtotal", "Subtotal");
            dgvCarrito.Columns["IdLibro"].Visible = false;
            dgvCarrito.Columns["Cantidad"].Width = 50;
            dgvCarrito.Columns["Precio"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvCarrito.Columns["Subtotal"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            panelDerecho.Controls.Add(dgvCarrito);

            panelDerecho.Controls.Add(new Label
            {
                Text = "Cantidad:",
                Location = new Point(15, 315),
                AutoSize = true,
                Font = new Font("Segoe UI", 10)
            });

            numCantidad = new NumericUpDown
            {
                Location = new Point(90, 312),
                Size = new Size(60, 25),
                Minimum = 1,
                Maximum = 999,
                Value = 1,
                Font = new Font("Segoe UI", 10)
            };
            panelDerecho.Controls.Add(numCantidad);

            btnAgregar = new IconButton
            {
                Text = "Agregar",
                IconChar = IconChar.CartPlus,
                IconColor = Color.White,
                IconSize = 18,
                Location = new Point(165, 308),
                Size = new Size(120, 35),
                BackColor = Color.FromArgb(39, 174, 96),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                ImageAlign = ContentAlignment.MiddleLeft,
                TextAlign = ContentAlignment.MiddleRight,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            btnAgregar.FlatAppearance.BorderSize = 0;
            btnAgregar.Click += BtnAgregar_Click;
            panelDerecho.Controls.Add(btnAgregar);

            btnQuitar = new IconButton
            {
                Text = "Quitar",
                IconChar = IconChar.TrashAlt,
                IconColor = Color.White,
                IconSize = 16,
                Location = new Point(295, 308),
                Size = new Size(110, 35),
                BackColor = Color.FromArgb(192, 57, 43),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                ImageAlign = ContentAlignment.MiddleLeft,
                TextAlign = ContentAlignment.MiddleRight,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            btnQuitar.FlatAppearance.BorderSize = 0;
            btnQuitar.Click += BtnQuitar_Click;
            panelDerecho.Controls.Add(btnQuitar);

            // ================ Totales ================

            int y = 350; // 360
            panelDerecho.Controls.Add(new Label { Text = "Subtotal:", Location = new Point(300, y), AutoSize = true, Font = new Font("Segoe UI", 11) });
            lblSubtotal = new Label { Text = "$0.00", Location = new Point(400, y), AutoSize = true, Font = new Font("Segoe UI", 11, FontStyle.Bold) };
            panelDerecho.Controls.Add(lblSubtotal);

            y += 25; //30
            panelDerecho.Controls.Add(new Label { Text = "Descuento:", Location = new Point(300, y), AutoSize = true, Font = new Font("Segoe UI", 11), ForeColor = Color.FromArgb(192, 57, 43) });
            lblDescuento = new Label { Text = "-$0.00", Location = new Point(400, y), AutoSize = true, Font = new Font("Segoe UI", 11, FontStyle.Bold), ForeColor = Color.FromArgb(192, 57, 43) };
            panelDerecho.Controls.Add(lblDescuento);

            y += 25; // 30
            panelDerecho.Controls.Add(new Label { Text = "IVA (16%):", Location = new Point(300, y), AutoSize = true, Font = new Font("Segoe UI", 11) });
            lblIva = new Label { Text = "$0.00", Location = new Point(400, y), AutoSize = true, Font = new Font("Segoe UI", 11, FontStyle.Bold) };
            panelDerecho.Controls.Add(lblIva);

            y += 35;
            panelDerecho.Controls.Add(new Label { Text = "TOTAL:", Location = new Point(300, y), AutoSize = true, Font = new Font("Segoe UI", 14, FontStyle.Bold), ForeColor = Color.FromArgb(192, 57, 43) });
            lblTotal = new Label { Text = "$0.00", Location = new Point(400, y), AutoSize = true, Font = new Font("Segoe UI", 14, FontStyle.Bold), ForeColor = Color.FromArgb(192, 57, 43) };
            panelDerecho.Controls.Add(lblTotal);

            // =================== Pago y Cambio ===================
            y += 45; //50
            panelDerecho.Controls.Add(new Label { Text = "Pago:", Location = new Point(15, y), AutoSize = true, Font = new Font("Segoe UI", 11) });
            numPago = new NumericUpDown //NumericUpDown no permite estar vacio.
            {
                Location = new Point(80, y - 3),
                Size = new Size(120, 28),
                DecimalPlaces = 2,
                Maximum = 999999,
                Font = new Font("Segoe UI", 11)
            };
            numPago.ValueChanged += (s, e) => CalcularTotales();
            numPago.Enter += (s, e) => numPago.Select(0, numPago.Text.Length); // s
            panelDerecho.Controls.Add(numPago);

            y += 30; // 35
            panelDerecho.Controls.Add(new Label { Text = "Cambio:", Location = new Point(15, y), AutoSize = true, Font = new Font("Segoe UI", 11) });
            lblCambio = new Label { Text = "$0.00", Location = new Point(80, y), AutoSize = true, Font = new Font("Segoe UI", 11, FontStyle.Bold), ForeColor = Color.FromArgb(39, 174, 96) };
            panelDerecho.Controls.Add(lblCambio);


            // ============ BOTONES ============
            btnCobrar = new IconButton
            {
                Text = "COBRAR",
                IconChar = IconChar.MoneyBillWave,
                IconColor = Color.White,
                IconSize = 22,
                Location = new Point(15, 530),
                Size = new Size(250, 55),
                BackColor = Color.FromArgb(39, 174, 96),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                ImageAlign = ContentAlignment.MiddleLeft,
                TextAlign = ContentAlignment.MiddleRight,
                Font = new Font("Segoe UI", 13, FontStyle.Bold)
            };
            btnCobrar.FlatAppearance.BorderSize = 0;
            btnCobrar.Click += BtnCobrar_Click;
            panelDerecho.Controls.Add(btnCobrar);

            btnCancelar = new IconButton
            {
                Text = "Cancelar",
                IconChar = IconChar.TimesCircle,
                IconColor = Color.White,
                IconSize = 20,
                Location = new Point(275, 530),
                Size = new Size(250, 55),
                BackColor = Color.Gray,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                ImageAlign = ContentAlignment.MiddleLeft,
                TextAlign = ContentAlignment.MiddleRight,
                Font = new Font("Segoe UI", 11, FontStyle.Bold)
            };
            btnCancelar.FlatAppearance.BorderSize = 0;
            btnCancelar.Click += BtnCancelar_Click;
            panelDerecho.Controls.Add(btnCancelar);

            this.Controls.Add(panelDerecho);

            // ============ BOTÓN ACERCA DE ============
            btnAcercaDe = new IconButton
            {
                Text = "Acerca de",
                IconChar = IconChar.InfoCircle,
                IconColor = Color.White,
                IconSize = 18,
                Size = new Size(130, 45),
                BackColor = Color.FromArgb(52, 73, 94),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                ImageAlign = ContentAlignment.MiddleLeft,
                TextAlign = ContentAlignment.MiddleRight,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right
            };
            btnAcercaDe.FlatAppearance.BorderSize = 0;
            btnAcercaDe.Click += (s, e) =>
            {
                var frm = new FrmAcercaDe();
                frm.ShowDialog();
            };

            // Posición inicial (esquina inferior derecha)
            btnAcercaDe.Location = new Point(
                this.ClientSize.Width - btnAcercaDe.Width - 20,
                this.ClientSize.Height - btnAcercaDe.Height - 20
            );

            // Aplicar borde redondeado
            RedondearBoton(btnAcercaDe, 15);

            // Reajustar al redimensionar
            this.Resize += (s, e) =>
            {
                btnAcercaDe.Location = new Point(
                    this.ClientSize.Width - btnAcercaDe.Width - 20,
                    this.ClientSize.Height - btnAcercaDe.Height - 20
                );

                if (Sesion.IdRol == 1 && btnCerrarSesion.Parent == this)
                {
                    btnCerrarSesion.Location = new Point(
                        this.ClientSize.Width - btnCerrarSesion.Width - 20,
                        this.ClientSize.Height - btnAcercaDe.Height - btnCerrarSesion.Height - 30
                    );
                }
            };

            this.Controls.Add(btnAcercaDe);
            btnAcercaDe.BringToFront();   // Asegura que quede encima

            this.Load += (s, e) =>
            {
                CargarLibros("");
                txtBuscar.Focus();
                ReposicionarHeader();
            };
        }

        private void MostrarUsuarioActual()
        {
            lblUsuario.Text = $"👤 {Sesion.NombreUsuario}  |  {Sesion.NombreRol}";

            // Color según rol (bonus visual)
            switch (Sesion.IdRol)
            {
                case 1:
                    lblUsuario.ForeColor = Color.FromArgb(241, 196, 15);   // Dorado 
                    lblUsuario.Text = $"👤 {Sesion.NombreRol}\r\n  {Sesion.NombreUsuario}";
                    lblUsuario.TextAlign = ContentAlignment.MiddleLeft; // MiddleRight
                    break;

                case 2: lblUsuario.ForeColor = Color.White; break;                     // Blanco - 
                case 3: lblUsuario.ForeColor = Color.FromArgb(189, 195, 199); break; // Gris claro - Almacenero
                default: lblUsuario.ForeColor = Color.White; break;
            }
        }

        // ============ PERMISOS POR ROL ============
        private void AplicarPermisosPorRol()
        {
            bool esAdmin = Sesion.IdRol == 1;
            bool esCajero = Sesion.IdRol == 2;
            bool esAlmacenero = Sesion.IdRol == 3;

            btnClientes.Visible = esAdmin;
            btnProveedores.Visible = esAdmin || esAlmacenero;
            btnProductos.Visible = esAdmin || esAlmacenero;
            btnCompras.Visible = esAdmin || esAlmacenero;
            btnCaja.Visible = esAdmin || esCajero;   
            btnReportes.Visible = esAdmin;  

            ReposicionarHeader();
        }

        // ============ CONFIGURAR BOTÓN CERRAR SESIÓN SEGÚN ROL ============
        private void ConfigurarBotonCerrarSesion()
        {
            if (Sesion.IdRol == 1) // Si es Administrador
            {
                // 1. Quitarlo del header
                if (panelHeader.Controls.Contains(btnCerrarSesion))
                    panelHeader.Controls.Remove(btnCerrarSesion);

                // 2. Agregarlo al formulario principal
                if (!this.Controls.Contains(btnCerrarSesion))
                {
                    this.Controls.Add(btnCerrarSesion);
                    btnCerrarSesion.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
                    btnCerrarSesion.Size = new Size(130, 45); // (ancho, alto)
                    btnCerrarSesion.BringToFront(); // Para que se vea por encima de todo
                }

                // 3. Posicionarlo justo encima de btnAcercaDe
                btnCerrarSesion.Location = new Point(
                    this.ClientSize.Width - btnCerrarSesion.Width - 20,
                    this.ClientSize.Height - btnAcercaDe.Height - btnCerrarSesion.Height - 30
                );
            }
            else // Para Cajero y Almacenero
            {
                // Asegurarse de que esté en el header
                if (!panelHeader.Controls.Contains(btnCerrarSesion))
                {
                    if (this.Controls.Contains(btnCerrarSesion))
                        this.Controls.Remove(btnCerrarSesion);

                    panelHeader.Controls.Add(btnCerrarSesion);
                    btnCerrarSesion.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                }
                ReposicionarHeader();
            }
        }

        // ============ REPOSICIONAR BOTONES DEL HEADER ============

        private void ReposicionarHeader()
        {
            // Si el panel aún no tiene un ancho válido, no hacer nada para evitar cálculos erróneos
            if (panelHeader == null || panelHeader.Width <= 0) return;

            const int margenDerecho = 20; // Distancia al borde derecho de la pantalla
            const int gap = 15;           // Separación entre botones

            // El orden del arreglo importa (del más a la derecha al más a la izquierda)
            var botones = new IconButton[]
            {
        btnCerrarSesion,
        btnCompras,
        btnProductos,
        btnProveedores,
        btnClientes,
        btnCaja,
        btnReportes
            };

            int posFromRight = margenDerecho;

            foreach (var btn in botones)
            {
                if (btn != null && btn.Visible && btn.Parent == panelHeader) // && btn.Parent == panelHeader  #evita mover un botón sacado del header
                { 
                    // Calcular la posición X desde la derecha
                    btn.Location = new Point(panelHeader.Width - posFromRight - btn.Width, 12);
                    posFromRight += btn.Width + gap;
                }
            }


            // Posicionar el nombre del usuario dinámicamente al lado del título
            if (lblUsuario != null && lblTitulo != null)
            {
                if (Sesion.IdRol == 1)
                {
                    lblUsuario.Location = new Point(lblTitulo.Right + 25, 10);
                }
                else
                {
                    lblUsuario.Location = new Point(lblTitulo.Right + 20, 20);
                }
            }

        }

        // ============ BÚSQUEDA ============
        private void TxtBuscar_TextChanged(object sender, EventArgs e)
        {
            CargarLibros(txtBuscar.Text.Trim());
        }


        private void CargarLibros(string filtro)
        {
            try
            {
                using var con = new SqlConnection(connectionString);
                con.Open();

                string query = @"
            SELECT l.id_libro, l.titulo, l.precio_venta, l.stock, l.idCategoria 
            FROM Libro l
            WHERE l.estado = 1 AND l.stock > 0
              AND (l.titulo LIKE @filtro OR @filtro = '')
            ORDER BY l.titulo";

                using var cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@filtro", "%" + filtro + "%");

                using var reader = cmd.ExecuteReader();
                dgvResultados.Rows.Clear();
                while (reader.Read())
                {
                    dgvResultados.Rows.Add(
                        reader["id_libro"],
                        reader["titulo"].ToString(),
                        Convert.ToDecimal(reader["precio_venta"]).ToString("N2"),
                        reader["stock"],
                        reader["idCategoria"]
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar libros: " + ex.Message);
            }
        }

        // ============ CARRITO ============
        private void DgvResultados_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            BtnAgregar_Click(sender, e);
        }

        private void BtnAgregar_Click(object sender, EventArgs e)
        {
            if (dgvResultados.CurrentRow == null)
            {
                MessageBox.Show("Selecciona un libro del catálogo primero.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int idLibro = Convert.ToInt32(dgvResultados.CurrentRow.Cells["IdLibro"].Value);
            string titulo = dgvResultados.CurrentRow.Cells["Titulo"].Value.ToString();
            decimal precio = Convert.ToDecimal(dgvResultados.CurrentRow.Cells["Precio"].Value);
            int stockDisponible = Convert.ToInt32(dgvResultados.CurrentRow.Cells["Stock"].Value);
            int idCategoria = Convert.ToInt32(dgvResultados.CurrentRow.Cells["IdCategoria"].Value);

            int cantidad = (int)numCantidad.Value;

            var existente = carrito.FirstOrDefault(c => c.IdLibro == idLibro);
            int cantidadActual = existente?.Cantidad ?? 0;

            if (cantidadActual + cantidad > stockDisponible)
            {
                MessageBox.Show($"Stock insuficiente. Solo hay {stockDisponible} unidades disponibles.",
                    "Stock insuficiente", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (existente != null)
            {
                existente.Cantidad += cantidad;
            }
            else
            {
                carrito.Add(new DetalleVentaItem
                {
                    IdLibro = idLibro,
                    IdCategoria = idCategoria,
                    Titulo = titulo,
                    Cantidad = cantidad,
                    PrecioUnitario = precio,
                    StockDisponible = stockDisponible
                });
            }

            RefrescarCarrito();
            numCantidad.Value = 1;
        }

        private void BtnQuitar_Click(object sender, EventArgs e)
        {
            if (dgvCarrito.CurrentRow == null) return;
            int index = dgvCarrito.CurrentRow.Index;
            carrito.RemoveAt(index);
            RefrescarCarrito();
        }

        private void RefrescarCarrito()
        {
            dgvCarrito.Rows.Clear();
            foreach (var item in carrito)
            {
                dgvCarrito.Rows.Add(
                    item.IdLibro,
                    item.Titulo,
                    item.Cantidad,
                    item.PrecioUnitario.ToString("N2"),
                    item.Subtotal.ToString("N2")
                );
            }
            CalcularTotales();
        }

        private void CalcularTotales()
        {
            decimal subtotal = carrito.Sum(c => c.Subtotal);

            var gestor = new GestorPromociones();

            // Evaluamos descuentos globales (tomamos el mayor entre aniversario y cumpleaños)
            decimal descAniversario = gestor.EvaluarAniversario();
            decimal descCumple = gestor.EvaluarCumpleanos(fechaNacimientoCliente);
            decimal porcentajeDescuento = Math.Max(descAniversario, descCumple);

            decimal montoDescuentoGlobal = subtotal * porcentajeDescuento;
            decimal montoDescuentoCategoria = gestor.EvaluarPromocionCategoria(carrito);

            decimal descuentoTotal = montoDescuentoGlobal + montoDescuentoCategoria;

            // Calculamos totales finales (el IVA se calcula sobre el subtotal ya descontado)
            decimal subtotalConDescuento = subtotal - descuentoTotal;
            if (subtotalConDescuento < 0) subtotalConDescuento = 0; // Evitar negativos

            decimal iva = subtotalConDescuento * IVA;
            decimal total = subtotalConDescuento + iva;

            // Actualizamos la UI
            lblSubtotal.Text = subtotal.ToString("C2");
            lblDescuento.Text = "-" + descuentoTotal.ToString("C2");
            lblIva.Text = iva.ToString("C2");
            lblTotal.Text = total.ToString("C2");

            decimal pago = numPago.Value;
            decimal cambio = pago - total;
            lblCambio.Text = cambio >= 0 ? cambio.ToString("C2") : "Falta " + (-cambio).ToString("C2");
            lblCambio.ForeColor = cambio >= 0 ? Color.FromArgb(39, 174, 96) : Color.Red;
        }

        private int? ObtenerCajaAbierta(SqlConnection con, SqlTransaction trans)
        {
            var cmd = new SqlCommand(@"
        SELECT TOP 1 id_caja FROM Caja 
        WHERE idEmpleado = @idEmpleado AND estado = 'Abierta' 
        ORDER BY fecha_apertura DESC", con, trans);
            cmd.Parameters.AddWithValue("@idEmpleado", Sesion.IdEmpleado);
            var result = cmd.ExecuteScalar();
            return result != null ? Convert.ToInt32(result) : (int?)null;
        }

        private void BtnSeleccionarCliente_Click(object sender, EventArgs e)
        {
            string dni = Microsoft.VisualBasic.Interaction.InputBox("Ingrese el DNI del cliente (o deje vacío para Público General):", "Buscar Cliente", "");

            if (string.IsNullOrWhiteSpace(dni))
            {
                idClienteActual = 0;
                nombreClienteActual = "Público General";
                docClienteActual = "00000000";
                fechaNacimientoCliente = null;
                btnSeleccionarCliente.Text = "👤 Público General";
                CalcularTotales();
                return;
            }

            try
            {
                using var con = new SqlConnection(connectionString);
                con.Open();
                var cmd = new SqlCommand("SELECT id_cliente, nombreCliente, fechaNacimiento FROM Cliente WHERE documento = @doc", con);
                cmd.Parameters.AddWithValue("@doc", dni);
                using var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    idClienteActual = Convert.ToInt32(reader["id_cliente"]);
                    nombreClienteActual = reader["nombreCliente"].ToString();
                    docClienteActual = dni;
                    fechaNacimientoCliente = reader["fechaNacimiento"] != DBNull.Value ? Convert.ToDateTime(reader["fechaNacimiento"]) : (DateTime?)null;
                    btnSeleccionarCliente.Text = $"👤 {nombreClienteActual}";
                    CalcularTotales();
                }
                else
                {
                    MessageBox.Show("Cliente no encontrado. Se cobrará como Público General.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar cliente: " + ex.Message);
            }
        }

        // ============ COBRAR ============
        private void BtnCobrar_Click(object sender, EventArgs e)
        {
            if (carrito.Count == 0)
            {
                MessageBox.Show("El carrito está vacío.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                using var conVerificacion = new SqlConnection(connectionString);
                conVerificacion.Open();

                // mismo método utilizado, pero pasándole 'null' en la transacción
                int? idCajaVerificada = ObtenerCajaAbierta(conVerificacion, null);

                if (idCajaVerificada == null)
                {
                    MessageBox.Show(
                        "No puede realizar ventas porque no hay una caja abierta.\n\n" +
                        "Por favor, diríjase al módulo de 'Caja' y abra su turno primero.",
                        "Caja Cerrada",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return; // Detiene la ejecución del cobro
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al verificar el estado de la caja: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 1. Calcular subtotales y descuentos
            decimal subtotal = carrito.Sum(c => c.Subtotal);

            var gestor = new GestorPromociones();

            // Evaluamos descuentos globales (Aniversario o Cumpleaños)
            decimal descAniversario = gestor.EvaluarAniversario();
            decimal descCumple = gestor.EvaluarCumpleanos(fechaNacimientoCliente);
            decimal porcentajeDescuento = Math.Max(descAniversario, descCumple);

            decimal montoDescuentoGlobal = subtotal * porcentajeDescuento;
            decimal montoDescuentoCategoria = gestor.EvaluarPromocionCategoria(carrito);

            // Descuento total en dinero
            decimal descuentoTotal = montoDescuentoGlobal + montoDescuentoCategoria;

            // 2. Calcular totales finales con descuento
            decimal subtotalConDescuento = subtotal - descuentoTotal;
            if (subtotalConDescuento < 0) subtotalConDescuento = 0; // Evitar negativos

            decimal iva = subtotalConDescuento * IVA;
            decimal totalConDescuento = subtotalConDescuento + iva;

            decimal pago = numPago.Value;

            // 3. Validar pago
            if (pago < totalConDescuento)
            {
                MessageBox.Show("El pago es insuficiente para cubrir el total.", "Pago insuficiente",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 4. Mostrar confirmación con el descuento aplicado
            string mensajeDescuento = descuentoTotal > 0 ? $"\nDescuento aplicado: -{descuentoTotal:C2}" : "";

            var confirm = MessageBox.Show(
                $"¿Confirmar venta?\n\nSubtotal: {subtotal:C2}{mensajeDescuento}\nTotal a pagar: {totalConDescuento:C2}\nPago: {pago:C2}\nCambio: {(pago - totalConDescuento):C2}",
                "Confirmar Venta",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes) return;

            try
            {
                using var con = new SqlConnection(connectionString);
                con.Open();
                using var trans = con.BeginTransaction();

                try
                {
                    // Evaluar si hay un cliente seleccionado o usar el público general
                    int idClienteFinal = idClienteActual > 0 ? idClienteActual : ObtenerClientePublico(con, trans);
                    int? idCaja = ObtenerCajaAbierta(con, trans);

                    var cmdPedido = new SqlCommand(@"
                    INSERT INTO Pedido 
                    (idCliente, idUsuario, tipo_documento, numero_documento, documento_cliente,
                    nombre_cliente, monto_pago, monto_cambio, monto_total, id_caja, fecha_pedido, estado)
                    OUTPUT INSERTED.id_pedido
                    VALUES (@idCliente, @idUsuario, 'Boleta', @numero, @docCliente,
                    @nomCliente, @pago, @cambio, @total, @idCaja, GETDATE(), 'Pagado')", con, trans);

                    cmdPedido.Parameters.AddWithValue("@idCliente", idClienteFinal);
                    cmdPedido.Parameters.AddWithValue("@idUsuario", Sesion.IdUsuario);
                    cmdPedido.Parameters.AddWithValue("@numero", "B001-" + DateTime.Now.ToString("yyyyMMddHHmmss"));

                    // Uso de las variables globales en lugar de los textos fijos
                    cmdPedido.Parameters.AddWithValue("@docCliente", docClienteActual);
                    cmdPedido.Parameters.AddWithValue("@nomCliente", nombreClienteActual);

                    cmdPedido.Parameters.AddWithValue("@pago", pago);
                    cmdPedido.Parameters.AddWithValue("@cambio", pago - totalConDescuento);
                    cmdPedido.Parameters.AddWithValue("@total", totalConDescuento);
                    cmdPedido.Parameters.AddWithValue("@idCaja", idCaja ?? (object)DBNull.Value);

                    int idPedido = (int)cmdPedido.ExecuteScalar();

                    bool primerItem = true;
                    foreach (var item in carrito)
                    {
                        decimal descuentoItem = primerItem ? descuentoTotal : 0;

                        var cmdDetalle = new SqlCommand(@"
                            INSERT INTO DetallePedido
                            (idPedido, idLibro, cantidad, precio_unitario, descuento_aplicado, fecha_registro)
                            VALUES (@idPedido, @idLibro, @cant, @precio, @descuento, GETDATE())", con, trans);

                        cmdDetalle.Parameters.AddWithValue("@idPedido", idPedido);
                        cmdDetalle.Parameters.AddWithValue("@idLibro", item.IdLibro);
                        cmdDetalle.Parameters.AddWithValue("@cant", item.Cantidad);
                        cmdDetalle.Parameters.AddWithValue("@precio", item.PrecioUnitario);
                        cmdDetalle.Parameters.AddWithValue("@descuento", descuentoItem);
                        cmdDetalle.ExecuteNonQuery();

                        primerItem = false;
                    }

                    trans.Commit();

                    MessageBox.Show(
                        $"¡Venta registrada con éxito!\n\nPedido #{idPedido}\nTotal: {totalConDescuento:C2}\nCambio: {(pago - totalConDescuento):C2}",
                        "Venta Exitosa",MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Preguntar si desea generar el ticket en PDF
                    var preguntarTicket = MessageBox.Show(
                        "¿Deseas generar el ticket en PDF?",
                        "Ticket de Venta",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (preguntarTicket == DialogResult.Yes)
                    {
                        using var sfd = new SaveFileDialog
                        {
                            Filter = "Archivo PDF (*.pdf)|*.pdf",
                            FileName = $"Ticket_{idPedido}_{DateTime.Now:yyyyMMdd}.pdf",
                            Title = "Guardar Ticket..."
                        };

                        if (sfd.ShowDialog() == DialogResult.OK)
                        {
                            try
                            {
                                TicketGenerator.GenerarTicket(idPedido, sfd.FileName);

                                MessageBox.Show($"Ticket guardado exitosamente en:\n\n{sfd.FileName}", 
                                    "Ticket Generado", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Error al generar el ticket:\n" + ex.Message, "Error",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }

                    carrito.Clear();
                    RefrescarCarrito();
                    numPago.Value = 0;
                    // RESETEAR EL CLIENTE A PÚBLICO GENERAL
                    idClienteActual = 0;
                    nombreClienteActual = "Público General";
                    docClienteActual = "00000000";
                    fechaNacimientoCliente = null;
                    btnSeleccionarCliente.Text = "👤 Público General";
                    CargarLibros(txtBuscar.Text.Trim());
                    txtBuscar.Focus();
                }
                catch
                {
                    trans.Rollback();
                    throw;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al registrar la venta:\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private int ObtenerClientePublico(SqlConnection con, SqlTransaction trans)
        {
            var cmdBuscar = new SqlCommand(
                "SELECT TOP 1 id_cliente FROM Cliente WHERE documento = '00000000'", con, trans);
            var result = cmdBuscar.ExecuteScalar();

            if (result != null)
                return Convert.ToInt32(result);

            var cmdCrear = new SqlCommand(@"
            INSERT INTO Cliente (documento, nombreCliente, correo, telefono, direccion, estado, fecha_registro, fechaNacimiento)
            OUTPUT INSERTED.id_cliente
            VALUES ('00000000', 'Publico General', 'publico@libreria.com', '0000000000', 'N/A', 1, GETDATE(), '1900-01-01')",
            con, trans);

            return (int)cmdCrear.ExecuteScalar();
        }

        private void BtnCancelar_Click(object sender, EventArgs e)
        {
            if (carrito.Count == 0) return;

            var confirm = MessageBox.Show("¿Vaciar el carrito actual?", "Confirmar",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm == DialogResult.Yes)
            {
                carrito.Clear();
                RefrescarCarrito();
                numPago.Value = 0;
            }
        }

        private void BtnCerrarSesion_Click(object sender, EventArgs e)
        {
            var confirm = MessageBox.Show("¿Cerrar sesión actual?", "Confirmar",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            // Marca que está cerrando sesión (Evita confucion en cerrar la pestaña)
            Sesion.CerrandoSesion = true;

            // Limpiar sesión
            Sesion.IdUsuario = 0;
            Sesion.IdEmpleado = 0;
            Sesion.IdRol = 0;
            Sesion.NombreUsuario = string.Empty;
            Sesion.NombreRol = string.Empty;

            // Cerrar solo esta ventana
            this.Close();
        }

        // ============ CREAR BOTÓN REDONDEADO ============
        private void RedondearBoton(Control btn, int radio)
        {
            var path = new System.Drawing.Drawing2D.GraphicsPath();

            path.AddArc(0, 0, radio, radio, 180, 90);
            path.AddArc(btn.Width - radio, 0, radio, radio, 270, 90);
            path.AddArc(btn.Width - radio, btn.Height - radio, radio, radio, 0, 90);
            path.AddArc(0, btn.Height - radio, radio, radio, 90, 90);
            path.CloseFigure();

            btn.Region = new Region(path);
        }
    }
}
