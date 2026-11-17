using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using FontAwesome.Sharp;

namespace P1
{
    public partial class FrmProductos : Form
    {
        // Controles
        private Label lblTitulo, lblContador;
        private TextBox txtBuscar;
        private IconPictureBox picBuscar;
        private IconButton btnNuevo, btnEditar, btnEliminar, btnRefrescar, btnCerrar;
        private DataGridView dgvProductos;
        private CheckBox chkMostrarInactivos;

        private readonly string connectionString =
            @"Server=DESKTOP-IQ00U68;Database=Libreria;Integrated Security=True;TrustServerCertificate=True;";

        public FrmProductos()
        {
            ConstruirInterfazManual();
            this.Load += (s, e) => CargarProductos();
        }

        private void ConstruirInterfazManual()
        {
            // FORMULARIO
            this.Text = "Gestión de Productos - Librería Morita";
            this.Size = new Size(1150, 680); // tam pantalla
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.WhiteSmoke;
            this.MinimumSize = new Size(1000, 600);

            // HEADER
            Panel panelHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.FromArgb(41, 128, 185)
            };

            lblTitulo = new Label
            {
                Text = "📚 Gestión de Productos",
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
                lblContador.Location = new Point(panelHeader.Width - 250, 22);
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
                PlaceholderText = "Buscar por título, autor, categoría..."
            };
            txtBuscar.TextChanged += (s, e) => CargarProductos();
            panelBuscar.Controls.Add(txtBuscar);

            chkMostrarInactivos = new CheckBox
            {
                Text = "Mostrar inactivos",
                Location = new Point(460, 15),
                AutoSize = true,
                Font = new Font("Segoe UI", 10)
            };
            chkMostrarInactivos.CheckedChanged += (s, e) => CargarProductos();
            panelBuscar.Controls.Add(chkMostrarInactivos);

            // Botones
            btnNuevo = new IconButton
            {
                Text = "Nuevo",
                IconChar = IconChar.Plus,
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
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnRefrescar.FlatAppearance.BorderSize = 0;
            btnRefrescar.Click += (s, e) => CargarProductos();
            panelBuscar.Controls.Add(btnRefrescar);

            btnCerrar = new IconButton
            {
                IconChar = IconChar.Times,
                IconColor = Color.White,
                IconSize = 18,
                Location = new Point(1030, 8),
                Size = new Size(45, 35),
                BackColor = Color.FromArgb(80, 80, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnCerrar.FlatAppearance.BorderSize = 0;
            btnCerrar.Click += (s, e) => this.Close();
            panelBuscar.Controls.Add(btnCerrar);

            this.Controls.Add(panelBuscar);

            // DATAGRIDVIEW
            dgvProductos = new DataGridView
            {
                Location = new Point(20, 145),
                Size = new Size(1090, 480),
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
            dgvProductos.Columns.Add("IdLibro", "ID");
            dgvProductos.Columns.Add("Titulo", "Título");
            dgvProductos.Columns.Add("Autor", "Autor");
            dgvProductos.Columns.Add("Categoria", "Categoría");
            dgvProductos.Columns.Add("Editorial", "Editorial");
            dgvProductos.Columns.Add("PrecioCompra", "P. Compra");
            dgvProductos.Columns.Add("PrecioVenta", "P. Venta");
            dgvProductos.Columns.Add("Stock", "Stock");
            dgvProductos.Columns.Add("Estado", "Estado");
            dgvProductos.Columns["IdLibro"].Visible = false;
            dgvProductos.Columns["PrecioCompra"].Visible = false; // Ocultar
            dgvProductos.Columns["PrecioVenta"].Visible = false;  // Ocultar
            dgvProductos.Columns["Stock"].Visible = false; // Ocultar
            dgvProductos.Columns["Estado"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dgvProductos.CellDoubleClick += BtnEditar_Click;
            dgvProductos.CellFormatting += DgvProductos_CellFormatting;

            this.Controls.Add(dgvProductos);
        }

        // ============ CARGA DE PRODUCTOS ============
        private void CargarProductos()
        {
            try
            {
                using var con = new SqlConnection(connectionString);
                con.Open();

                string filtro = txtBuscar.Text.Trim();
                string query = @"
                    SELECT 
                        l.id_libro,
                        l.titulo,
                        a.nombre + ' ' + a.apellido AS autor,
                        c.nombreCategoria AS categoria,
                        e.nombre AS editorial,
                        ISNULL(l.precio_compra, 0) AS precio_compra,
                        l.precio_venta,
                        l.stock,
                        l.estado
                    FROM Libro l
                    INNER JOIN Autor a ON l.idAutor = a.id_autor
                    INNER JOIN Categoria c ON l.idCategoria = c.id_categoria
                    INNER JOIN Editorial e ON l.idEditorial = e.id_editorial
                    WHERE (@filtro = '' OR 
                           l.titulo LIKE @likeFiltro OR
                           a.nombre LIKE @likeFiltro OR
                           a.apellido LIKE @likeFiltro OR
                           c.nombreCategoria LIKE @likeFiltro)
                      AND (@mostrarInactivos = 1 OR l.estado = 1)
                    ORDER BY l.titulo";

                using var cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@filtro", filtro);
                cmd.Parameters.AddWithValue("@likeFiltro", "%" + filtro + "%");
                cmd.Parameters.AddWithValue("@mostrarInactivos", chkMostrarInactivos.Checked ? 1 : 0);

                using var reader = cmd.ExecuteReader();
                dgvProductos.Rows.Clear();
                int contador = 0;

                while (reader.Read())
                {
                    bool estado = Convert.ToBoolean(reader["estado"]);
                    dgvProductos.Rows.Add(
                        reader["id_libro"],
                        reader["titulo"].ToString(),
                        reader["autor"].ToString(),
                        reader["categoria"].ToString(),
                        reader["editorial"].ToString(),
                        Convert.ToDecimal(reader["precio_compra"]).ToString("N2"),
                        Convert.ToDecimal(reader["precio_venta"]).ToString("N2"),
                        reader["stock"],
                        estado ? "Activo" : "Inactivo"
                    );
                    contador++;
                }

                lblContador.Text = $"{contador} productos";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar productos:\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Colorear filas de inactivos
        private void DgvProductos_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvProductos.Rows[e.RowIndex].Cells["Estado"].Value?.ToString() == "Inactivo")
            {
                dgvProductos.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Gray;
                dgvProductos.Rows[e.RowIndex].DefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Italic);
            }
        }

        // ============ NUEVO ============
        private void BtnNuevo_Click(object sender, EventArgs e)
        {
            using var frm = new FrmLibroEditar(0);
            if (frm.ShowDialog() == DialogResult.OK)
                CargarProductos();
        }

        // ============ EDITAR ============
        private void BtnEditar_Click(object sender, EventArgs e)
        {
            if (dgvProductos.CurrentRow == null)
            {
                MessageBox.Show("Selecciona un producto para editar.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int idLibro = Convert.ToInt32(dgvProductos.CurrentRow.Cells["IdLibro"].Value);

            using var frm = new FrmLibroEditar(idLibro);
            if (frm.ShowDialog() == DialogResult.OK)
                CargarProductos();
        }

        // ============ ELIMINAR (soft delete) ============
        private void BtnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvProductos.CurrentRow == null)
            {
                MessageBox.Show("Selecciona un producto para eliminar.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int idLibro = Convert.ToInt32(dgvProductos.CurrentRow.Cells["IdLibro"].Value);
            string titulo = dgvProductos.CurrentRow.Cells["Titulo"].Value.ToString();

            // Verificar si el libro tiene ventas asociadas
            int ventasAsociadas = ContarVentasDeLibro(idLibro);

            string mensaje;
            if (ventasAsociadas > 0)
            {
                mensaje = $"El libro \"{titulo}\" tiene {ventasAsociadas} venta(s) registrada(s).\n\n" +
                          "No se puede eliminar por completo. ¿Deseas DESACTIVARLO?\n" +
                          "(No aparecerá en el catálogo pero se conservará el historial)";
            }
            else
            {
                mensaje = $"¿Desactivar el libro \"{titulo}\"?\n\n" +
                          "No aparecerá en el catálogo. Puedes reactivarlo después.";
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
                    // Soft delete: solo desactivar
                    var cmd = new SqlCommand(
                        "UPDATE Libro SET estado = 0 WHERE id_libro = @id", con);
                    cmd.Parameters.AddWithValue("@id", idLibro);
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Producto desactivado correctamente.", "Éxito",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // Sin ventas: eliminar permanentemente
                    var confirmDelete = MessageBox.Show(
                        "Este libro NO tiene ventas asociadas.\n\n" +
                        "¿Deseas ELIMINARLO PERMANENTEMENTE de la base de datos?",
                        "Eliminar definitivamente",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    if (confirmDelete == DialogResult.Yes)
                    {
                        var cmd = new SqlCommand(
                            "DELETE FROM Libro WHERE id_libro = @id", con);
                        cmd.Parameters.AddWithValue("@id", idLibro);
                        cmd.ExecuteNonQuery();

                        MessageBox.Show("Producto eliminado permanentemente.", "Éxito",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        // Desactivar de todos modos
                        var cmd = new SqlCommand(
                            "UPDATE Libro SET estado = 0 WHERE id_libro = @id", con);
                        cmd.Parameters.AddWithValue("@id", idLibro);
                        cmd.ExecuteNonQuery();
                    }
                }

                CargarProductos();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Error al eliminar:\n" + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private int ContarVentasDeLibro(int idLibro)
        {
            try
            {
                using var con = new SqlConnection(connectionString);
                con.Open();
                var cmd = new SqlCommand(
                    "SELECT COUNT(*) FROM DetallePedido WHERE idLibro = @id", con);
                cmd.Parameters.AddWithValue("@id", idLibro);
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch
            {
                return 0;
            }
        }
    }
}