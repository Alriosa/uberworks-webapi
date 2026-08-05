// =====================================================================================
// RESUMEN DEL ARCHIVO
// Qué hace: Define con qué medio se pagó un Service (tarjeta, PayPal, Zelle). Todavía no
//           hay lógica de cobro real conectada — este enum existe porque ya está en el
//           diagrama de base de datos original (TBL_PAYMENTS), lista para cuando se
//           construya esa entidad.
// Entidades relacionadas: Payment.cs (pendiente de implementar su Repository/Service/Controller)
// Tablas relacionadas: TBL_PAYMENTS.CL_METHOD
// =====================================================================================
namespace uberworks_webapi.Common.Enums;

/// <summary>
/// Mapea al CHECK constraint de TBL_PAYMENTS.CL_METHOD.
/// </summary>
public enum PaymentMethod
{
    CreditCard,
    PayPal,
    Zelle
}
