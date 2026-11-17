using System;
using System.IO;
using System.Linq;
using Microsoft.Data.SqlClient;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace P1
{
    public static class TicketGenerator
    {
        private static readonly string connectionString =
            @"Server=DESKTOP-IQ00U68;Database=Libreria;Integrated Security=True;TrustServerCertificate=True;";

        public static void GenerarTicket(int idPedido, string rutaDestino)
        {
            // 1. Obtener datos del Pedido
            int idCliente = 0;
            string nombreCliente = "";
            decimal montoTotal = 0, montoPago = 0, montoCambio = 0;
            DateTime fechaPedido = DateTime.Now;
            string numeroDocumento = "";

            // 2. Lista para los ítems del ticket
            var items = new System.Collections.Generic.List<dynamic>();

            using (var con = new SqlConnection(connectionString))
            {
                con.Open();

                // Datos de cabecera
                var cmdPedido = new SqlCommand(@"
                    SELECT idCliente, nombre_cliente, monto_total, monto_pago, monto_cambio, 
                           fecha_pedido, numero_documento
                    FROM Pedido WHERE id_pedido = @id", con);
                cmdPedido.Parameters.AddWithValue("@id", idPedido);

                using (var reader = cmdPedido.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        nombreCliente = reader["nombre_cliente"].ToString();
                        montoTotal = Convert.ToDecimal(reader["monto_total"]);
                        montoPago = Convert.ToDecimal(reader["monto_pago"]);
                        montoCambio = Convert.ToDecimal(reader["monto_cambio"]);
                        fechaPedido = Convert.ToDateTime(reader["fecha_pedido"]);
                        numeroDocumento = reader["numero_documento"].ToString();
                    }
                }

                // Detalle del ticket
                var cmdDetalle = new SqlCommand(@"
                    SELECT l.titulo, dp.cantidad, dp.precio_unitario, dp.subtotal
                    FROM DetallePedido dp
                    INNER JOIN Libro l ON dp.idLibro = l.id_libro
                    WHERE dp.idPedido = @id", con);
                cmdDetalle.Parameters.AddWithValue("@id", idPedido);

                using (var reader = cmdDetalle.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(new
                        {
                            Titulo = reader["titulo"].ToString(),
                            Cantidad = Convert.ToInt32(reader["cantidad"]),
                            Precio = Convert.ToDecimal(reader["precio_unitario"]),
                            Subtotal = Convert.ToDecimal(reader["subtotal"])
                        });
                    }
                }
            }

            // 3. Calcular subtotal e IVA (Ya que el total incluye IVA)
            // Si el total es $116, el subtotal es $100 y el IVA es $16 (16%)
            decimal ivaPorcentaje = 0.16m;
            decimal subtotal = Math.Round(montoTotal / (1 + ivaPorcentaje), 2);
            decimal ivaMonto = montoTotal - subtotal;

            // 4. Generar el PDF con QuestPDF
            Document.Create(container =>
            {
                container.Page(page =>
                {
                    // Formato de ticket (80mm de ancho x alto dinámico)
                    page.Size(80, 250, Unit.Millimetre);
                    page.Margin(5, Unit.Millimetre);
                    page.DefaultTextStyle(x => x.FontSize(9).FontFamily(Fonts.Verdana));

                    page.Header().Column(col =>
                    {
                        col.Item().AlignCenter().Text("LIBRERÍA MORITA").SemiBold().FontSize(14);
                        col.Item().AlignCenter().Text("Punto de Venta").FontSize(10);
                        col.Item().AlignCenter().Text("Tel: 555-123-4567").FontSize(8);
                        col.Item().PaddingTop(5).LineHorizontal(1);
                        col.Item().PaddingVertical(2).Text($"Ticket: {numeroDocumento}");
                        col.Item().Text($"Fecha: {fechaPedido:dd/MM/yyyy HH:mm}");
                        col.Item().Text($"Cliente: {nombreCliente}");
                    });

                    page.Content().PaddingVertical(5).Column(col =>
                    {
                        // Cabecera de la tabla
                        col.Item().Row(row =>
                        {
                            row.RelativeItem(3).Text("Cant. Producto").SemiBold().FontSize(8);
                            row.RelativeItem(2).AlignRight().Text("Total").SemiBold().FontSize(8);
                        });
                        col.Item().LineHorizontal(0.5f);

                        // Filas de productos
                        foreach (var item in items)
                        {
                            col.Item().PaddingTop(2).Row(row =>
                            {
                                row.RelativeItem(3).Text(text =>
                                {
                                    text.Span($"{item.Cantidad}x ").SemiBold().FontSize(8);
                                    text.Span($"{item.Titulo}").FontSize(8);
                                });
                                row.RelativeItem(2).AlignRight().Text($"{item.Subtotal:C2}").FontSize(8);
                            });
                        }

                        col.Item().PaddingTop(5).LineHorizontal(0.5f);

                        // Totales
                        col.Item().PaddingTop(3).Row(row =>
                        {
                            row.RelativeItem().Text("Subtotal:").FontSize(8);
                            row.RelativeItem().AlignRight().Text($"{subtotal:C2}").FontSize(8);
                        });
                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Text("IVA (16%):").FontSize(8);
                            row.RelativeItem().AlignRight().Text($"{ivaMonto:C2}").FontSize(8);
                        });
                        col.Item().PaddingTop(2).Row(row =>
                        {
                            row.RelativeItem().Text("TOTAL:").SemiBold().FontSize(10);
                            row.RelativeItem().AlignRight().Text($"{montoTotal:C2}").SemiBold().FontSize(10);
                        });

                        col.Item().PaddingTop(3).Row(row =>
                        {
                            row.RelativeItem().Text("Pago:").FontSize(8);
                            row.RelativeItem().AlignRight().Text($"{montoPago:C2}").FontSize(8);
                        });
                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Text("Cambio:").FontSize(8);
                            row.RelativeItem().AlignRight().Text($"{montoCambio:C2}").FontSize(8);
                        });
                    });

                    page.Footer().Column(col =>
                    {
                        col.Item().PaddingTop(10).AlignCenter().Text("¡Gracias por su compra!").Italic().FontSize(8);
                        col.Item().AlignCenter().Text("Librería Morita").FontSize(8);
                    });
                });
            })
            .GeneratePdf(rutaDestino);
        }
    }
}