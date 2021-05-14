using System;

namespace ACAG.Abacus.CalendarConnector.Models.Abacus.V1_0
{
  public class AppointmentModel
  {
    #region Properties
    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    /// <value>
    /// The identifier.
    /// </value>
    public int ID { get; set; }
    /// <summary>
    /// Gets or sets the abacus identifier.
    /// </summary>
    /// <value>
    /// The abacus identifier.
    /// </value>
    public long AbacusID { get; set; }
    /// <summary>
    /// Gets or sets the exchange identifier.
    /// </summary>
    /// <value>
    /// The exchange identifier.
    /// </value>
    public string ExchangeID { get; set; }
    /// <summary>
    /// Gets or sets the insert date time.
    /// </summary>
    /// <value>
    /// The insert date time.
    /// </value>
    public DateTime InsertDateTime { get; set; }
    /// <summary>
    /// Gets or sets the mail account.
    /// </summary>
    /// <value>
    /// The mail account.
    /// </value>        
    public string MailAccount { get; set; }
    /// <summary>
    /// Gets or sets the subject.
    /// </summary>
    /// <value>
    /// The subject.
    /// </value>
    public string Subject { get; set; }
    /// <summary>
    /// Gets or sets the date time start.
    /// </summary>
    /// <value>
    /// The date time start.
    /// </value>
    public DateTime DateTimeStart { get; set; }
    /// <summary>
    /// Gets or sets the date time end.
    /// </summary>
    /// <value>
    /// The date time end.
    /// </value>
    public DateTime DateTimeEnd { get; set; }
    /// <summary>
    /// Gets or sets the status.
    /// </summary>
    /// <value>
    /// The status.
    /// </value>
    public int Status { get; set; }
    /// <summary>
    /// Gets or sets the pay type code.
    /// </summary>
    /// <value>
    /// The pay type code.
    /// </value>
    public string PayTypeCode { get; set; }
    /// <summary>
    /// Gets or sets the log information.
    /// </summary>
    /// <value>
    /// The log information.
    /// </value>
    public string LogInfo { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether this instance is private.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is private; otherwise, <c>false</c>.
    /// </value>
    public bool IsPrivate { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether this instance is out of office.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is out of office; otherwise, <c>false</c>.
    /// </value>
    public bool IsOutOfOffice { get; set; }
    #endregion

    #region Constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="AppointmentModel" /> class.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="abacusID">The abacus identifier.</param>
    /// <param name="exchangeID">The exchange identifier.</param>
    /// <param name="insertDateTime">The insert date time.</param>
    /// <param name="mailAccount">The mail account.</param>
    /// <param name="subject">The subject.</param>
    /// <param name="dateTimeStart">The date time start.</param>
    /// <param name="dateTimeEnd">The date time end.</param>
    /// <param name="isPrivate">if set to <c>true</c> [is private].</param>
    /// <param name="isOutOfOffice">if set to <c>true</c> [is out of office].</param>
    /// <param name="status">The status.</param>
    /// <param name="payTypeCode">The pay type code.</param>
    /// <param name="logInfo">The log information.</param>
    public AppointmentModel(
      int id,
      long abacusID,
      string exchangeID,
      DateTime insertDateTime,
      string mailAccount,
      string subject,
      DateTime dateTimeStart,
      DateTime dateTimeEnd,
      bool isPrivate,
      bool isOutOfOffice,
      int status,
      string payTypeCode,
      string logInfo
      )
    {
      ID = id;
      AbacusID = abacusID;
      ExchangeID = exchangeID;
      InsertDateTime = insertDateTime;
      MailAccount = mailAccount;
      Subject = subject;
      DateTimeStart = dateTimeStart;
      DateTimeEnd = dateTimeEnd;
      IsPrivate = isPrivate;
      IsOutOfOffice = isOutOfOffice;
      Status = status;
      PayTypeCode = payTypeCode;
      LogInfo = logInfo;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AppointmentModel"/> class.
    /// </summary>
    public AppointmentModel()
    {
    }
    #endregion
  }
}
