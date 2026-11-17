using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using FontAwesome.Sharp;

namespace P1
{
    public partial class FrmProveedores : Form
    {
        // Controles
        private Label lblTitulo, lblContador;
        private TextBox txtBuscar;
        private IconPictureBox picBuscar;
        private IconButton btnNuevo, btnEditar, btnEliminar, btnRefrescar, btnCerrar;
        private DataGridView dgvProveedores;
        private CheckBox chkMostrarInactivos;

        private readonly string connectionString =
            @"Server=DESKTOP-IQ00U68;Database=Libreria;Integrated Security=True;TrustServerCertificate=True;";

        public FrmProveedores()
        {
            ConstruirInterfazManual();
            this.Load += (s, e) => CargarProveedores();
        }

        private void ConstruirInterfazManual()
        {
            // FORMULARIO
            this.Text = "Gestión de Proveedores - Librería Morita";
            this.Size = new Size(1050, 620);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.WhiteSmoke;
            this.MinimumSize = new Size(900, 550);

            // HEADER
            Panel panelHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.FromArgb(155, 89, 182)  // Morado
            };

            lblTitulo = new Label
            {
                Text = "🚚 Gestión de Proveedores",
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
                Size = new Size(990, 50),
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
                Size = new Size(340, 28),
                Font = new Font("Segoe UI", 11),
                PlaceholderText = "Buscar por nombre, correo o teléfono..."
            };
            txtBuscar.TextChanged += (s, e) => CargarProveedores();
            panelBuscar.Controls.Add(txtBuscar);

            chkMostrarInactivos = new CheckBox
            {
                Text = "Mostrar inactivos",
                Location = new Point(390, 15), 
                AutoSize = true,
                Font = new Font("Segoe UI", 10)
            };
            chkMostrarInactivos.CheckedChanged += (s, e) => CargarProveedores();
            panelBuscar.Controls.Add(chkMostrarInactivos);

            // Botones
            btnNuevo = new IconButton
            {
                Text = "Nuevo",
                IconChar = IconChar.Plus,
                IconColor = Color.White,
                IconSize = 16,
                Location = new Point(530, 8),
                Size = new Size(100, 35),
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
                Location = new Point(640, 8),
                Size = new Size(100, 35),
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
                Location = new Point(750, 8),
                Size = new Size(110, 35),
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
                Location = new Point(870, 8),
                Size = new Size(45, 35),
                BackColor = Color.Gray,
                FlatStyle = FlatStyle.Flat
            };
            btnRefrescar.FlatAppearance.BorderSize = 0;
            btnRefrescar.Click += (s, e) => CargarProveedores();
            panelBuscar.Controls.Add(btnRefrescar);

            btnCerrar = new IconButton
            {
                IconChar = IconChar.Times,
                IconColor = Color.White,
                IconSize = 18,
                Location = new Point(925, 8),
                Size = new Size(45, 35),
                BackColor = Color.FromArgb(80, 80, 80),
                FlatStyle = FlatStyle.Flat
            };
            btnCerrar.FlatAppearance.BorderSize = 0;
            btnCerrar.Click += (s, e) => this.Close();
            panelBuscar.Controls.Add(btnCerrar);

            this.Controls.Add(panelBuscar);

            // DATAGRIDVIEW
            dgvProveedores = new DataGridView
            {
                Location = new Point(20, 145),
                Size = new Size(990, 420),
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
            dgvProveedores.Columns.Add("IdProveedor", "ID");
            dgvProveedores.Columns.Add("Nombre", "Nombre");
            dgvProveedores.Columns.Add("Telefono", "Teléfono");
            dgvProveedores.Columns.Add("Correo", "Correo");
            dgvProveedores.Columns.Add("Direccion", "Dirección");
            dgvProveedores.Columns.Add("FechaIngreso", "Registro");
            dgvProveedores.Columns.Add("Estado", "Estado");
            dgvProveedores.Columns["IdProveedor"].Visible = false;
            dgvProveedores.Columns["Telefono"].Width = 100;
            dgvProveedores.Columns["FechaIngreso"].Width = 100;
            dgvProveedores.Columns["Estado"].Width = 80;
            dgvProveedores.Columns["Telefono"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvProveedores.Columns["FechaIngreso"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvProveedores.Columns["Estado"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dgvProveedores.CellDoubleClick += BtnEditar_Click;
            dgvProveedores.CellFormatting += DgvProveedores_CellFormatting;

            this.Controls.Add(dgvProveedores);
        }

        // ============ CARGA ============
        private void CargarProveedores()
        {
            try
            {
                using var con = new SqlConnection(connectionString);
                con.Open();

                string filtro = txtBuscar.Text.Trim();
                string query = @"
                    SELECT id_proveedor, nombreProveedor, telefono, correo, 
                           direccion, fecha_registro, estado
                    FROM Proveedor
                    WHERE (@filtro = '' OR 
                           nombreProveedor LIKE @likeFiltro OR
                           correo LIKE @likeFiltro OR
                           telefono LIKE @likeFiltro)
                      AND (@mostrarInactivos = 1 OR estado = 1)
                    ORDER BY nombreProveedor";

                using var cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@filtro", filtro);
                cmd.Parameters.AddWithValue("@likeFiltro", "%" + filtro + "%");
                cmd.Parameters.AddWithValue("@mostrarInactivos", chkMostrarInactivos.Checked ? 1 : 0);

                using var reader = cmd.ExecuteReader();
                dgvProveedores.Rows.Clear();
                int contador = 0;

                while (reader.Read())
                {
                    bool estado = Convert.ToBoolean(reader["estado"]);
                    dgvProveedores.Rows.Add(
                        reader["id_proveedor"],
                        reader["nombreProveedor"].ToString(),
                        reader["telefono"].ToString(),
                        reader["correo"].ToString(),
                        reader["direccion"].ToString(),
                        Convert.ToDateTime(reader["fecha_registro"]).ToString("dd/MM/yyyy"),
                        estado ? "Activo" : "Inactivo"
                    );
                    contador++;
                }

                lblContador.Text = $"{contador} proveedor(es)";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar proveedores:\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgvProveedores_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvProveedores.Rows[e.RowIndex].Cells["Estado"].Value?.ToString() == "Inactivo")
            {
                dgvProveedores.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Gray;
                dgvProveedores.Rows[e.RowIndex].DefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Italic);
            }
        }

        // ============ NUEVO ============
        private void BtnNuevo_Click(object sender, EventArgs e)
        {
            using var frm = new FrmProveedorEditar(0);
            if (frm.ShowDialog() == DialogResult.OK)
                CargarProveedores();
        }

        // ============ EDITAR ============
        private void BtnEditar_Click(object sender, EventArgs e)
        {
            if (dgvProveedores.CurrentRow == null)
            {
                MessageBox.Show("Selecciona un proveedor para editar.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int idProveedor = Convert.ToInt32(dgvProveedores.CurrentRow.Cells["IdProveedor"].Value);

            using var frm = new FrmProveedorEditar(idProveedor);
            if (frm.ShowDialog() == DialogResult.OK)
                CargarProveedores();
        }

        // ============ ELIMINAR ============
        private void BtnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvProveedores.CurrentRow == null)
            {
                MessageBox.Show("Selecciona un proveedor para eliminar.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int idProveedor = Convert.ToInt32(dgvProveedores.CurrentRow.Cells["IdProveedor"].Value);
            string nombre = dgvProveedores.CurrentRow.Cells["Nombre"].Value.ToString();

            int comprasAsociadas = ContarComprasDeProveedor(idProveedor);

            string mensaje;
            if (comprasAsociadas > 0)
            {
                mensaje = $"El proveedor \"{nombre}\" tiene {comprasAsociadas} compra(s) registrada(s).\n\n" +
                          "No se puede eliminar por completo. ¿Deseas DESACTIVARLO?\n" +
                          "(No aparecerá en la lista de compras pero se conservará el historial)";
            }
            else
            {
                mensaje = $"¿Desactivar el proveedor \"{nombre}\"?\n\n" +
                          "No aparecerá en la lista de compras. Puedes reactivarlo después.";
            }

            var confirm = MessageBox.Show(mensaje, "Confirmar",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes) return;

            try
            {
                using var con = new SqlConnection(connectionString);
                con.Open();

                if (comprasAsociadas > 0)
                {
                    // Soft delete
                    var cmd = new SqlCommand(
                        "UPDATE Proveedor SET estado = 0 WHERE id_proveedor = @id", con);
                    cmd.Parameters.AddWithValue("@id", idProveedor);
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Proveedor desactivado correctamente.", "Éxito",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    var confirmDelete = MessageBox.Show(
                        "Este proveedor NO tiene compras asociadas.\n\n" +
                        "¿Deseas ELIMINARLO PERMANENTEMENTE de la base de datos?",
                        "Eliminar definitivamente",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    if (confirmDelete == DialogResult.Yes)
                    {
                        var cmd = new SqlCommand(
                            "DELETE FROM Proveedor WHERE id_proveedor = @id", con);
                        cmd.Parameters.AddWithValue("@id", idProveedor);
                        cmd.ExecuteNonQuery();

                        MessageBox.Show("Proveedor eliminado permanentemente.", "Éxito",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        var cmd = new SqlCommand(
                            "UPDATE Proveedor SET estado = 0 WHERE id_proveedor = @id", con);
                        cmd.Parameters.AddWithValue("@id", idProveedor);
                        cmd.ExecuteNonQuery();
                    }
                }

                CargarProveedores();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Error al eliminar:\n" + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private int ContarComprasDeProveedor(int idProveedor)
        {
            try
            {
                using var con = new SqlConnection(connectionString);
                con.Open();
                var cmd = new SqlCommand(
                    "SELECT COUNT(*) FROM Compra WHERE idProveedor = @id", con);
                cmd.Parameters.AddWithValue("@id", idProveedor);
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch
            {
                return 0;
            }
        }
    }
}
