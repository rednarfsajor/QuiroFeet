using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Analisis.Models
{
    public class OrdenCompraViewModel : IValidatableObject
    {
        [Required(ErrorMessage = "Debe seleccionar un proveedor.")]
        [Display(Name = "Proveedor")]
        public int proveedor_id { get; set; }

        public DateTime fecha_creacion { get; set; } = DateTime.Now;

        [Display(Name = "Productos")]
        public List<DetalleOrdenVM> Productos { get; set; } = new List<DetalleOrdenVM>();

        // Validación personalizada para la lista de productos
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Productos == null || Productos.Count == 0)
            {
                yield return new ValidationResult("Debe agregar al menos un producto.", new[] { "Productos" });
            }
            else
            {
                for (int i = 0; i < Productos.Count; i++)
                {
                    var producto = Productos[i];

                    if (producto.id_producto <= 0)
                    {
                        yield return new ValidationResult($"Debe seleccionar un producto válido en la fila {i + 1}.", new[] { $"Productos[{i}].id_producto" });
                    }

                    if (producto.qty <= 0)
                    {
                        yield return new ValidationResult($"La cantidad debe ser mayor a 0 en la fila {i + 1}.", new[] { $"Productos[{i}].qty" });
                    }

                    // Intentar convertir el precio a decimal
                    if (!decimal.TryParse(producto.precio, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal precioDecimal) || precioDecimal <= 0)
                    {
                        yield return new ValidationResult($"El precio debe ser un número válido mayor a 0 en la fila {i + 1}.", new[] { $"Productos[{i}].precio" });
                    }
                }
            }
        }
    }

    public class DetalleOrdenVM
    {
        [Required(ErrorMessage = "Debe seleccionar un producto.")]
        public int id_producto { get; set; }

        [Required(ErrorMessage = "Debe indicar una cantidad.")]
        [Range(1, int.MaxValue, ErrorMessage = "Cantidad debe ser mayor a 0.")]
        public int qty { get; set; }

        [Required(ErrorMessage = "Debe indicar un precio.")]
        public string precio { get; set; }  // precio se recibe como string, se convierte luego
    }
}
