using System;
using System.Collections.Generic;
using System.Linq;

namespace P1
{
    public class GestorPromociones
    {
        // 1. Aniversario (20%) - Fecha exacta: 09/11/2026
        public decimal EvaluarAniversario()
        {
            DateTime fechaAniversario = new DateTime(2026, 11, 9);
            if (DateTime.Today.Date == fechaAniversario.Date)
                return 0.20m;
            return 0m;
        }

        // 2. Cumpleaños (10%) - Verifica el mes y día del cliente
        public decimal EvaluarCumpleanos(DateTime? fechaNacimiento)
        {
            if (fechaNacimiento.HasValue &&
                fechaNacimiento.Value.Month == DateTime.Today.Month &&
                fechaNacimiento.Value.Day == DateTime.Today.Day)
            {
                return 0.10m;
            }
            return 0m;
        }

        // 3. Categorías (5%) - Aplica a los libros de ciertas categorías
        public decimal EvaluarPromocionCategoria(List<DetalleVentaItem> carrito)
        {
            // IDs DE LAS CATEGORÍAS QUE QUIERES PROMOCIONAR (ej. 1=Novela, 3=Tecnología)
            var categoriasConPromo = new List<int> { 1, 2 };

            decimal descuentoCategoria = 0;
            foreach (var item in carrito)
            {
                if (categoriasConPromo.Contains(item.IdCategoria))
                {
                    // 5% de descuento sobre el subtotal de ese libro
                    descuentoCategoria += item.Subtotal * 0.05m;
                }
            }
            return descuentoCategoria;
        }
    }
}