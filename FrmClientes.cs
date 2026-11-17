using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using FontAwesome.Sharp;

namespace P1
{
    public partial class FrmClientes : Form
    {
        // Controles
        private Label lblTitulo, lblContador;
        private TextBox txtBuscar;
        private IconPictureBox picBuscar;
        private IconButton btnNuevo, btnEditar, btnEliminar, btnRefrescar, btnCerrar;
        private DataGridView dgvClientes;
        private CheckBox chkMostrarInactivos;

        private readonly string connectionString =
            @"Server=DESKTOP-IQ00U68;Database=Libreria;Integrated Security=True;TrustServerCertificate=True;";

        public FrmClientes()
        {
            ConstruirInterfazManual();
            this.Load += (s, e) => CargarClientes();
        }

        private void ConstruirInterfazManual()
        {
            // FORMULARIO
            this.Text = "Gestión de Clientes - Librería Morita";
            this.Size = new Size(1150, 620);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.WhiteSmoke;
            this.MinimumSize = new Size(1000, 550);

            // HEADER
            Panel panelHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.FromArgb(230, 126, 34)  // Naranja
            };

            lblTitulo = new Label
            {
                Text = "👥 Gestión de Clientes",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(20, 15),
                BackColor = Color.Transparent
            };
            panelHeader.Controls.Add(lblTitulo);

            lblContador = new Label
            {
                Text = "",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.White,
                AutoSize = true,
                BackColor = Color.Transparent,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            panelHeader.Controls.Add(lblContador);

            panelHeader.Resize += (s, e) =>
            {
                lblContador.Location = new Point(panelHeader.Width - 200, 22);
            };

            this.Controls.Add(panelHeader);

            // BARRA DE BÚSQUEDA
            Panel panelBuscar = new Panel
            {
                Location = new Point(20, 80),
                Size = new Size(1090, 50),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            picBuscar = new IconPictureBox
            {
                IconChar = IconChar.Search,
                IconColor = Color.Gray,
                IconSize = 20,
                Location = new Point(12, 15),
                Size = new Size(20, 20),
                BackColor = Color.Transparent
            };
            panelBuscar.Controls.Add(picBuscar);

            txtBuscar = new TextBox
            {
                Location = new Point(40, 12),
                Size = new Size(400, 28),
                Font = new Font("Segoe UI", 11),
                PlaceholderText = "Buscar por documento, nombre, correo..."
            };
            txtBuscar.TextChanged += (s, e) => CargarClientes();
            panelBuscar.Controls.Add(txtBuscar);

            chkMostrarInactivos = new CheckBox
            {
                Text = "Mostrar inactivos",
                Location = new Point(460, 15),
                AutoSize = true,
                Font = new Font("Segoe UI", 10)
            };
            chkMostrarInactivos.CheckedChanged += (s, e) => CargarClientes();
            panelBuscar.Controls.Add(chkMostrarInactivos);

            // Botones
            btnNuevo = new IconButton
            {
                Text = "Nuevo",
                IconChar = IconChar.UserPlus,
                IconColor = Color.White,
                IconSize = 16,
                Location = new Point(620, 8),
                Size = new Size(105, 35),
                BackColor = Color.FromArgb(39, 174, 96),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                ImageAlign = ContentAlignment.MiddleLeft,
                TextAlign = ContentAlignment.MiddleRight,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            btnNuevo.FlatAppearance.BorderSize = 0;
            btnNuevo.Click += BtnNuevo_Click;
            panelBuscar.Controls.Add(btnNuevo);

            btnEditar = new IconButton
            {
                Text = "Editar",
                IconChar = IconChar.Edit,
                IconColor = Color.White,
                IconSize = 16,
                Location = new Point(735, 8),
                Size = new Size(105, 35),
                BackColor = Color.FromArgb(41, 128, 185),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                ImageAlign = ContentAlignment.MiddleLeft,
                TextAlign = ContentAlignment.MiddleRight,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            btnEditar.FlatAppearance.BorderSize = 0;
            btnEditar.Click += BtnEditar_Click;
            panelBuscar.Controls.Add(btnEditar);

            btnEliminar = new IconButton
            {
                Text = "Eliminar",
                IconChar = IconChar.TrashAlt,
                IconColor = Color.White,
                IconSize = 16,
                Location = new Point(850, 8),
                Size = new Size(115, 35),
                BackColor = Color.FromArgb(192, 57, 43),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                ImageAlign = ContentAlignment.MiddleLeft,
                TextAlign = ContentAlignment.MiddleRight,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            btnEliminar.FlatAppearance.BorderSize = 0;
            btnEliminar.Click += BtnEliminar_Click;
            panelBuscar.Controls.Add(btnEliminar);

            btnRefrescar = new IconButton
            {
                IconChar = IconChar.SyncAlt,
                IconColor = Color.White,
                IconSize = 18,
                Location = new Point(975, 8),
                Size = new Size(45, 35),
                BackColor = Color.Gray,
                FlatStyle = FlatStyle.Flat
            };
            btnRefrescar.FlatAppearance.BorderSize = 0;
            btnRefrescar.Click += (s, e) => CargarClientes();
            panelBuscar.Controls.Add(btnRefrescar);

            btnCerrar = new IconButton
            {
                IconChar = IconChar.Times,
                IconColor = Color.White,
                IconSize = 18,
                Location = new Point(1030, 8),
                Size = new Size(45, 35),
                BackColor = Color.FromArgb(80, 80, 80),
                FlatStyle = FlatStyle.Flat
            };
            btnCerrar.FlatAppearance.BorderSize = 0;
            btnCerrar.Click += (s, e) => this.Close();
            panelBuscar.Controls.Add(btnCerrar);

            this.Controls.Add(panelBuscar);

            // DATAGRIDVIEW
            dgvClientes = new DataGridView
            {
                Location = new Point(20, 145),
                Size = new Size(1090, 420),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                RowHeadersVisible = false,
                Font = new Font("Segoe UI", 10),
                ColumnHeadersHeight = 35,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };
            dgvClientes.Columns.Add("IdCliente", "ID");
            dgvClientes.Columns.Add("Documento", "Documento");
            dgvClientes.Columns.Add("Nombre", "Nombre");
            dgvClientes.Columns.Add("Telefono", "Teléfono");
            dgvClientes.Columns.Add("Correo", "Correo");
            dgvClientes.Columns.Add("Direccion", "Dirección");
            dgvClientes.Columns.Add("FechaIngreso", "Registro");
            dgvClientes.Columns.Add("Estado", "Estado");
            dgvClientes.Columns["IdCliente"].Visible = false;
            dgvClientes.Columns["Documento"].Width = 100;
            dgvClientes.Columns["Telefono"].Width = 100;
            dgvClientes.Columns["FechaIngreso"].Width = 90;
            dgvClientes.Columns["Estado"].Width = 80;
            dgvClientes.Columns["Documento"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvClientes.Columns["Telefono"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvClientes.Columns["FechaIngreso"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvClientes.Columns["Estado"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dgvClientes.CellDoubleClick += BtnEditar_Click;
            dgvClientes.CellFormatting += DgvClientes_CellFormatting;

            this.Controls.Add(dgvClientes);
        }

        // ============ CARGA ============
        private void CargarClientes()
        {
            try
            {
                using var con = new SqlConnection(connectionString);
                con.Open();

                string filtro = txtBuscar.Text.Trim();
                string query = @"
                    SELECT id_cliente, documento, nombreCliente, telefono, correo, 
                           direccion, fecha_registro, estado
                    FROM Cliente
                    WHERE (@filtro = '' OR 
                           documento LIKE @likeFiltro OR
                           nombreCliente LIKE @likeFiltro OR
                           correo LIKE @likeFiltro OR
                           telefono LIKE @likeFiltro)
                      AND (@mostrarInactivos = 1 OR estado = 1)
                    ORDER BY nombreCliente";

                using var cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@filtro", filtro);
                cmd.Parameters.AddWithValue("@likeFiltro", "%" + filtro + "%");
                cmd.Parameters.AddWithValue("@mostrarInactivos", chkMostrarInactivos.Checked ? 1 : 0);

                using var reader = cmd.ExecuteReader();
                dgvClientes.Rows.Clear();
                int contador = 0;

                while (reader.Read())
                {
                    bool estado = Convert.ToBoolean(reader["estado"]);
                    dgvClientes.Rows.Add(
                        reader["id_cliente"],
                        reader["documento"].ToString(),
                        reader["nombreCliente"].ToString(),
                        reader["telefono"].ToString(),
                        reader["correo"].ToString(),
                        reader["direccion"].ToString(),
                        Convert.ToDateTime(reader["fecha_registro"]).ToString("dd/MM/yyyy"),
                        estado ? "Activo" : "Inactivo"
                    );
                    contador++;
                }

                lblContador.Text = $"{contador} cliente(s)";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar clientes:\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgvClientes_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvClientes.Rows[e.RowIndex].Cells["Estado"].Value?.ToString() == "Inactivo")
            {
                dgvClientes.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Gray;
                dgvClientes.Rows[e.RowIndex].DefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Italic);
            }
            else
            {
                // Restaurar el estilo normal
                dgvClientes.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Black;
                dgvClientes.Rows[e.RowIndex].DefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            }
        }

        // ============ NUEVO ============
        private void BtnNuevo_Click(object sender, EventArgs e)
        {
            using var frm = new FrmClienteEditar(0);
            if (frm.ShowDialog() == DialogResult.OK)
                CargarClientes();
        }

        // ============ EDITAR ============
        private void BtnEditar_Click(object sender, EventArgs e)
        {
            if (dgvClientes.CurrentRow == null)
            {
                MessageBox.Show("Selecciona un cliente para editar.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int idCliente = Convert.ToInt32(dgvClientes.CurrentRow.Cells["IdCliente"].Value);

            using var frm = new FrmClienteEditar(idCliente);
            if (frm.ShowDialog() == DialogResult.OK)
                CargarClientes();
        }

        // ============ ELIMINAR ============
        private void BtnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvClientes.CurrentRow == null)
            {
                MessageBox.Show("Selecciona un cliente para eliminar.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int idCliente = Convert.ToInt32(dgvClientes.CurrentRow.Cells["IdCliente"].Value);
            string nombre = dgvClientes.CurrentRow.Cells["Nombre"].Value.ToString();
            string documento = dgvClientes.CurrentRow.Cells["Documento"].Value.ToString();

            // No permitir eliminar al "Público General"
            if (documento == "00000000")
            {
                MessageBox.Show(
                    "El cliente \"Público General\" es un cliente especial del sistema\n" +
                    "y no puede ser eliminado ni desactivado.",
                    "Cliente protegido",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int ventasAsociadas = ContarVentasDeCliente(idCliente);

            string mensaje;
            if (ventasAsociadas > 0)
            {
                mensaje = $"El cliente \"{nombre}\" tiene {ventasAsociadas} venta(s) registrada(s).\n\n" +
                          "No se puede eliminar por completo. ¿Deseas DESACTIVARLO?\n" +
                          "(No aparecerá en la lista de ventas pero se conservará el historial)";
            }
            else
            {
                mensaje = $"¿Desactivar el cliente \"{nombre}\"?\n\n" +
                          "No aparecerá en la lista de ventas. Puedes reactivarlo después.";
            }

            var confirm = MessageBox.Show(mensaje, "Confirmar",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes) return;

            try
            {
                using var con = new SqlConnection(connectionString);
                con.Open();

                if (ventasAsociadas > 0)
                {
                    var cmd = new SqlCommand(
                        "UPDATE Cliente SET estado = 0 WHERE id_cliente = @id", con);
                    cmd.Parameters.AddWithValue("@id", idCliente);
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Cliente desactivado correctamente.", "Éxito",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    var confirmDelete = MessageBox.Show(
                        "Este cliente NO tiene ventas asociadas.\n\n" +
                        "¿Deseas ELIMINARLO PERMANENTEMENTE de la base de datos?",
                        "Eliminar definitivamente",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    if (confirmDelete == DialogResult.Yes)
                    {
                        var cmd = new SqlCommand(
                            "DELETE FROM Cliente WHERE id_cliente = @id", con);
                        cmd.Parameters.AddWithValue("@id", idCliente);
                        cmd.ExecuteNonQuery();

                        MessageBox.Show("Cliente eliminado permanentemente.", "Éxito",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        var cmd = new SqlCommand(
                            "UPDATE Cliente SET estado = 0 WHERE id_cliente = @id", con);
                        cmd.Parameters.AddWithValue("@id", idCliente);
                        cmd.ExecuteNonQuery();
                    }
                }

                CargarClientes();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Error al eliminar:\n" + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private int ContarVentasDeCliente(int idCliente)
        {
            try
            {
                using var con = new SqlConnection(connectionString);
                con.Open();
                var cmd = new SqlCommand(
                    "SELECT COUNT(*) FROM Pedido WHERE idCliente = @id", con);
                cmd.Parameters.AddWithValue("@id", idCliente);
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch
            {
                return 0;
            }
        }
    }
}
