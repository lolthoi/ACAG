using System.Xml.Serialization;

namespace ACAG.Abacus.CalendarConnector.Models.Abacus.V1_0
{
  /// <summary>
  /// Model for the Abacus Report _ACAG_AbacusCalendarConnector
  /// </summary>
  [XmlType("Detail")]
  public class _ACAG_AbacusCalendarConnector
  {
    #region Properties
    [XmlAttribute("PKT_Event")]
    public string PKT_Event { get; set; }

    [XmlAttribute("EXPR_Project_RecNr")]
    public string EXPR_Project_RecNr { get; set; }

    [XmlAttribute("NBW_LeArtNr")]
    public string NBW_LeArtNr { get; set; }

    [XmlAttribute("TEXT")]
    public string TEXT { get; set; }

    [XmlAttribute("PKT_StartTime")]
    public string PKT_StartTime { get; set; }

    [XmlAttribute("PKT_EndTime")]
    public string PKT_EndTime { get; set; }

    [XmlAttribute("EXPR_E_Mail")]
    public string EXPR_E_Mail { get; set; }

    [XmlAttribute("INR")]
    public string INR { get; set; }

    [XmlAttribute("VORNAME")]
    public string VORNAME { get; set; }

    [XmlAttribute("NAME")]
    public string NAME { get; set; }
    #endregion
  }
}
