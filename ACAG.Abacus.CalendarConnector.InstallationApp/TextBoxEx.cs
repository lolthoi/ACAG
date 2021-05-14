using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ACAG.Abacus.CalendarConnector.InstallationApp
{
  public partial class TextBoxEx : UserControl, IControlEx
  {
    // The TextBox
    private TextBox textBox = new TextBox();

    // Border color of the textbox
    private Color borderColorError = Color.Red;
    private Color borderColorDefault = Color.Gray;
    private Color borderColorFocused = Color.FromArgb(0, 120, 215);
    private Color borderColor = Color.Gray;

    // Ctor
    public TextBoxEx()
    {
      this.BackColor = textBox.BackColor;
      this.Paint += new PaintEventHandler(TextBoxEx_Paint);
      this.Resize += new EventHandler(TextBoxEx_Resize);
      textBox.BorderStyle = BorderStyle.None;
      this.Controls.Add(textBox);

      textBox.GotFocus += (sender, e) =>
      {
        var color = IsError ? borderColorError : borderColorFocused;
        SetBorderColor(color);
      };

      textBox.LostFocus += (sender, e) =>
      {
        var isError = IsRequired && string.IsNullOrWhiteSpace(textBox.Text);
        var color = isError ? borderColorError : borderColorDefault;
        SetBorderColor(color);
      };

      textBox.TextChanged += (sender, e) =>
      {
        if (IsRequired && textBox.Focused)
        {
          IsError = string.IsNullOrWhiteSpace(textBox.Text);
        }
      };

      textBox.KeyPress += (sender, e) =>
      {
        if (IsOnlyNumber)
        {
          var charDelete = (Char)Keys.Delete;
          var charBack = (Char)Keys.Back;

          if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != charDelete && e.KeyChar != charBack)
          {
            e.Handled = true;
          }
        }
      };

      InvalidateSize();
    }

    public void CheckError()
    {
      IsError = IsRequired && string.IsNullOrWhiteSpace(textBox.Text);
    }

    // Exposed properties of the textbox
    public override string Text
    {
      get { return textBox.Text; }
      set
      {
        textBox.Text = value;
        if (!string.IsNullOrWhiteSpace(value))
        {
          IsError = false;
        }
      }
    }
    // ... Expose other properties you need...

    // The border color property
    public Color BorderColor
    {
      get { return borderColor; }
      set
      {
        borderColorDefault = value;
        SetBorderColor(value);
      }
    }

    private void SetBorderColor(Color color)
    {
      borderColor = color;
      Invalidate();
    }

    public event EventHandler TextChanged
    {
      add { textBox.TextChanged += value; }
      remove { textBox.TextChanged -= value; }
    }
    // ... Expose other events you need...

    private void TextBoxEx_Resize(object sender, EventArgs e)
    {
      InvalidateSize();
    }
    private void TextBoxEx_Paint(object sender, PaintEventArgs e)
    {
      ControlPaint.DrawBorder(e.Graphics, this.ClientRectangle, borderColor, ButtonBorderStyle.Solid);
    }
    private const int AMPLITUDE_SIZE = 3;
    private void InvalidateSize()
    {
      textBox.Size = new Size(this.Width - AMPLITUDE_SIZE * 2, this.Height - AMPLITUDE_SIZE * 2);
      textBox.Location = new Point(AMPLITUDE_SIZE, AMPLITUDE_SIZE);

      Refresh();
    }

    public bool IsRequired { get; set; } = true;

    private bool _isError = false;
    public bool IsError
    {
      get { return _isError; }
      set
      {
        _isError = value;
        var color = _isError ? borderColorError : (textBox.Focused ? borderColorFocused : borderColorDefault);
        SetBorderColor(color);
      }
    }

    public bool Multiline
    {
      get { return textBox.Multiline; }
      set { textBox.Multiline = value; }
    }

    public char PasswordChar
    {
      get { return textBox.PasswordChar; }
      set { textBox.PasswordChar = value; }
    }

    [DefaultValue(32767)]
    [Localizable(true)]
    public virtual int MaxLength
    {
      get { return textBox.MaxLength; }
      set { textBox.MaxLength = value; }
    }

    public bool IsOnlyNumber { get; set; }
  }
}
