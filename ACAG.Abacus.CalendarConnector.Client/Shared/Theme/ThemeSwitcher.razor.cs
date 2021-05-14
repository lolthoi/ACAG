using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace ACAG.Abacus.CalendarConnector.Client.Shared.Theme
{
  public partial class ThemeSwitcher
  {
    [Parameter]
    public bool Shown { get; set; }

    [Parameter]
    public Action<bool> ShownChanged { get; set; }

    string currentTheme = "default";

    public class ThemeSetModel
    {
      public string Title { get; }
      public string[] Themes { get; }
      public ThemeSetModel(string title, params string[] themes)
      {
        Title = title;
        Themes = themes;
      }
    }

    List<ThemeSetModel> themeData = new List<ThemeSetModel>() {
        new ThemeSetModel("Color Themes",  "default"),
        new ThemeSetModel("DevExpress Themes", "blazing berry", "purple", "office white"),
        new ThemeSetModel("Bootswatch Themes", "cerulean", "cyborg", "darkly", "flatly", "journal", "litera",
                        "lumen", "lux", "materia", "minty", "pulse", "sandstone", "simplex", "sketchy", "slate",
                        "solar", "spacelab", "superhero", "united", "yeti")
    };


    void OnItemClick(string theme)
    {
      currentTheme = theme;
      string themeLink = "/css/switcher-resources/themes/" + currentTheme + "/bootstrap.min.css";
      ThemeLinkService.SetTheme(themeLink);
      Shown = !Shown;
      ShownChanged?.Invoke(Shown);
    }
  }
}
