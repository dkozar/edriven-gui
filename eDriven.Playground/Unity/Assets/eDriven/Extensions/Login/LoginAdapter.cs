using System;
using eDriven.Extensions.Login;
using eDriven.Gui.Designer;
using eDriven.Gui.Designer.Adapters;
using eDriven.Gui.Reflection;
using Component=eDriven.Gui.Components.Component;

[Toolbox(Label = "Login", Group = "My controls", Icon = "eDriven/Editor/Controls/login")]

public class LoginAdapter : ComponentAdapter
{
    #region Saveable values

    [Saveable]
    public int LabelWidth = 150;

    [Saveable]
    public SendMode SendMode = SendMode.SimulateSending;

    [Saveable]
    public string Username = string.Empty;

    [Saveable]
    public string Password = string.Empty;

    [Saveable]
    public string UsernameLabel = "Username:";

    [Saveable]
    public string PasswordLabel = "Password:";

    [Saveable]
    public string SubmitText = "Submit";

    #endregion

    public LoginAdapter()
    {
        // setting default values   
        MinWidth = 400;
    }

    public override Type ComponentType
    {
        get { return typeof(Login); }
    }

    public override Component NewInstance()
    {
        return new Login();
    }

    public override void Apply(Component component)
    {
        base.Apply(component);

        Login login = (Login)component;
        login.LabelWidth = LabelWidth;
        login.Username = Username;
        login.Password = Password;
        login.UsernameLabel = UsernameLabel;
        login.PasswordLabel = PasswordLabel;
        login.SubmitText = SubmitText;
        login.SendMode = SendMode;
    }
}