using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;

namespace ACAG.Abacus.CalendarConnector.InstallationApp
{

  public interface IControlEx
  {
    bool IsError { get; set; }
    void CheckError();
  }

  public partial class ComboBoxEx : UserControl, IControlEx
  {
    // The control
    private ComboBox control = new ComboBox();

    // Border color of the control
    private Color borderColorError = Color.Red;
    private Color borderColorDefault = Color.Gray;
    private Color borderColorFocused = Color.FromArgb(0, 120, 215);
    private Color borderColor = Color.Gray;

    // Ctor
    public ComboBoxEx()
    {
      this.BackColor = control.BackColor;
      control.FlatStyle = FlatStyle.Flat;
      this.Paint += new PaintEventHandler(ComboBoxEx_Paint);
      this.Resize += new EventHandler(ComboBoxEx_Resize);
      this.Controls.Add(control);
      this.ResumeLayout(false);

      control.GotFocus += (sender, e) =>
      {
        var color = IsError ? borderColorError : borderColorFocused;
        SetBorderColor(color);
      };

      control.LostFocus += (sender, e) =>
      {
        var isError = IsRequired && IsEmpty();
        var color = isError ? borderColorError : borderColorDefault;
        SetBorderColor(color);
      };

      control.SelectedIndexChanged += (sender, e) =>
      {
        if (IsRequired && control.Focused)
        {
          IsError = IsEmpty();
        }
      };

      InvalidateSize();
    }

    private bool IsEmpty()
    {
      return control.SelectedIndex == -1;
    }

    public void CheckError()
    {
      IsError = IsEmpty();
    }

    // Exposed properties of the textbox
    public override string Text
    {
      get { return control.Text; }
      set
      {
        control.Text = value;
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

    public event EventHandler SelectedIndexChanged
    {
      add { control.SelectedIndexChanged += value; }
      remove { control.SelectedIndexChanged -= value; }
    }
    // ... Expose other events you need...

    private void ComboBoxEx_Resize(object sender, EventArgs e)
    {
      InvalidateSize();
      this.ResumeLayout(false);
    }
    private void ComboBoxEx_Paint(object sender, PaintEventArgs e)
    {
      ControlPaint.DrawBorder(e.Graphics, this.ClientRectangle, borderColor, ButtonBorderStyle.Solid);
    }
    private const int AMPLITUDE_SIZE = 1;
    private void InvalidateSize()
    {
      control.Size = new Size(this.Width - AMPLITUDE_SIZE*2, this.Height - AMPLITUDE_SIZE * 2);
      control.Location = new Point(AMPLITUDE_SIZE, AMPLITUDE_SIZE);

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
        var color = _isError ? borderColorError : (control.Focused ? borderColorFocused :  borderColorDefault);
        SetBorderColor(color);
      }
    }

    public ComboBoxStyle DropDownStyle
    {
      get { return control.DropDownStyle; }
      set { control.DropDownStyle = value; }
    }

    public bool FormattingEnabled
    {
      get { return control.FormattingEnabled; }
      set { control.FormattingEnabled = value; }
    }

    public int SelectedIndex
    {
      get { return control.SelectedIndex; }
      set {
        control.SelectedIndex = value;
        if (value >= 0)
          IsError = false;
      }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    [Editor("System.Windows.Forms.Design.ListControlStringCollectionEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
    [Localizable(true)]
    [MergableProperty(false)]
    public ComboBox.ObjectCollection Items
    {
      get { return control.Items; }
    }
  }
}
