// =====================================================================================
// RESUMEN DEL ARCHIVO
// Qué hace: Define el tipo de penalización que se le puede poner a un usuario (temporal o
//           permanente). Existe desde el diagrama original; la entidad Penalty.cs todavía
//           no tiene Repository/Service/Controller construidos.
// Entidades relacionadas: Penalty.cs (pendiente de implementar)
// Tablas relacionadas: TBL_PENALTIES.CL_TYPE
// =====================================================================================
namespace uberworks_webapi.Common.Enums;

/// <summary>
/// Mapea a los valores usados en TBL_PENALTIES.CL_TYPE.
/// </summary>
public enum PenaltyType
{
    Temporary,
    Permanent
}
