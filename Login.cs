using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using FontAwesome.Sharp;

namespace P1
{
    public partial class Login : Form
    {
        // Controles estándar
        private Label lblTitulo;
        private Label lblCorreo;
        private TextBox txtCorreo;
        private Label lblClave;
        private TextBox txtClave;
        private Label lblError;

        // Controles FontAwesome
        private IconPictureBox picLogo;
        private IconPictureBox picCorreo;
        private IconPictureBox picClave;
        private IconButton btnIngresar;

        private readonly string connectionString = @"Server=DESKTOP-IQ00U68;Database=Libreria;Integrated Security=True;TrustServerCertificate=True;";

        public Login()
        {
            ConstruirInterfazManual();
        }

        private void ConstruirInterfazManual()
        {
            this.Text = "Iniciar Sesión - Librería Morita";
            this.Size = new Size(420, 400);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.WhiteSmoke;

            // Icono del Logo
            picLogo = new IconPictureBox();
            picLogo.IconChar = IconChar.BookOpen;
            picLogo.IconColor = Color.FromArgb(41, 128, 185);
            picLogo.IconSize = 48;
            picLogo.Size = new Size(48, 48);
            picLogo.Location = new Point((this.ClientSize.Width - 48) / 2, 25);
            picLogo.BackColor = Color.Transparent;
            this.Controls.Add(picLogo);

            // Título
            lblTitulo = new Label();
            lblTitulo.Text = "Punto de Venta Librería Morita";
            lblTitulo.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblTitulo.ForeColor = Color.FromArgb(41, 128, 185);
            lblTitulo.AutoSize = true;
            lblTitulo.Location = new Point((this.ClientSize.Width - lblTitulo.PreferredWidth) / 2, 80);
            this.Controls.Add(lblTitulo);

            // Icono Correo
            picCorreo = new IconPictureBox();
            picCorreo.IconChar = IconChar.Envelope;
            picCorreo.IconColor = Color.Gray;
            picCorreo.IconSize = 20;
            picCorreo.Size = new Size(20, 20);
            picCorreo.Location = new Point(50, 140);
            picCorreo.BackColor = Color.Transparent;
            this.Controls.Add(picCorreo);

            // Correo
            lblCorreo = new Label();
            lblCorreo.Text = "Correo:";
            lblCorreo.Location = new Point(80, 140);
            lblCorreo.Font = new Font("Segoe UI", 10);
            this.Controls.Add(lblCorreo);

            txtCorreo = new TextBox();
            txtCorreo.Location = new Point(80, 165);
            txtCorreo.Size = new Size(280, 25);
            txtCorreo.Font = new Font("Segoe UI", 10);
            this.Controls.Add(txtCorreo);

            // Icono Contraseña
            picClave = new IconPictureBox();
            picClave.IconChar = IconChar.Lock;
            picClave.IconColor = Color.Gray;
            picClave.IconSize = 20;
            picClave.Size = new Size(20, 20);
            picClave.Location = new Point(50, 205);
            picClave.BackColor = Color.Transparent;
            this.Controls.Add(picClave);

            // Contraseña
            lblClave = new Label();
            lblClave.Text = "Contraseña:";
            lblClave.Location = new Point(80, 205);
            lblClave.Font = new Font("Segoe UI", 10);
            this.Controls.Add(lblClave);

            txtClave = new TextBox();
            txtClave.Location = new Point(80, 230);
            txtClave.Size = new Size(280, 25);
            txtClave.Font = new Font("Segoe UI", 10);
            txtClave.UseSystemPasswordChar = true;
            this.Controls.Add(txtClave);

            // Mensaje de Error
            lblError = new Label();
            lblError.Text = "";
            lblError.ForeColor = Color.Red;
            lblError.Location = new Point(80, 265);
            lblError.AutoSize = true;
            this.Controls.Add(lblError);

            // Botón Ingresar
            btnIngresar = new IconButton();
            btnIngresar.Text = "Ingresar";
            btnIngresar.IconChar = IconChar.SignInAlt;
            btnIngresar.IconColor = Color.White;
            btnIngresar.IconSize = 22;
            btnIngresar.ImageAlign = ContentAlignment.MiddleLeft;
            btnIngresar.TextAlign = ContentAlignment.MiddleCenter;
            btnIngresar.Location = new Point(80, 290);
            btnIngresar.Size = new Size(280, 40);
            btnIngresar.BackColor = Color.FromArgb(41, 128, 185);
            btnIngresar.ForeColor = Color.White;
            btnIngresar.FlatStyle = FlatStyle.Flat;
            btnIngresar.FlatAppearance.BorderSize = 0;
            btnIngresar.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            btnIngresar.Cursor = Cursors.Hand;

            btnIngresar.Click += BtnIngresar_Click;

            txtCorreo.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) BtnIngresar_Click(s, e); };
            txtClave.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) BtnIngresar_Click(s, e); };

            this.Controls.Add(btnIngresar);
        }

        // ============ AUTENTICACIÓN ============
        private void BtnIngresar_Click(object sender, EventArgs e)
        {
            string correo = txtCorreo.Text.Trim();
            string clave = txtClave.Text.Trim();

            if (string.IsNullOrEmpty(correo) || string.IsNullOrEmpty(clave))
            {
                lblError.Text = "Por favor, llene todos los campos.";
                return;
            }

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    // traer idRol y el nombre del rol
                    string query = @"
                        SELECT u.id_usuario, u.nombre_completo, u.idEmpleado,
                               u.idRol, r.descripcion AS nombreRol
                        FROM Usuario u
                        LEFT JOIN Rol r ON u.idRol = r.id_rol
                        WHERE u.correo = @correo 
                          AND u.clave = @clave 
                          AND u.estado = 1";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@correo", correo);
                        cmd.Parameters.AddWithValue("@clave", clave);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int idUsuario = Convert.ToInt32(reader["id_usuario"]);
                                string nombreCompleto = reader["nombre_completo"].ToString();
                                int idEmpleado = Convert.ToInt32(reader["idEmpleado"]);

                                // Leer rol con validación por si es NULL
                                int idRol = reader["idRol"] != DBNull.Value
                                    ? Convert.ToInt32(reader["idRol"]) : 0;
                                string nombreRol = reader["nombreRol"] != DBNull.Value
                                    ? reader["nombreRol"].ToString() : "Sin rol";

                                // GUARDAR EN LA SESIÓN
                                Sesion.IdUsuario = idUsuario;
                                Sesion.IdEmpleado = idEmpleado;
                                Sesion.NombreUsuario = nombreCompleto;
                                Sesion.IdRol = idRol;
                                Sesion.NombreRol = nombreRol;

                                MessageBox.Show(
                                    $"¡Bienvenido, {nombreCompleto}!\nRol: {nombreRol}",
                                    "Acceso Correcto",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);

                                // Abrir PantallaVenta y cerrar TODO si se cierra la venta
                                PantallaVenta ventanaVenta = new PantallaVenta();
                                ventanaVenta.FormClosed += (s2, args) =>
                                {
                                    // Si fue logout, volver a mostrar el Login
                                    if (Sesion.CerrandoSesion)
                                    {
                                        Sesion.CerrandoSesion = false;  // Resetear la bandera
                                        this.Show();
                                        txtCorreo.Clear();
                                        txtClave.Clear();
                                        lblError.Text = "";
                                        txtCorreo.Focus();
                                    }
                                    else
                                    {
                                        // Si el usuario cerró con la X, cerrar todo
                                        this.Close();
                                    }
                                };
                                ventanaVenta.Show();
                                this.Hide();
                            }
                            else
                            {
                                lblError.Text = "Correo o contraseña incorrectos, o usuario inactivo.";
                                txtClave.Clear();
                                txtClave.Focus();
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Error de conexión a la base de datos:\n" + ex.Message,
                    "Error Crítico", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrió un error inesperado:\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}