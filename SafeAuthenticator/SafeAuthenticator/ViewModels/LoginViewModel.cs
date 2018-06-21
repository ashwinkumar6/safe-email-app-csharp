using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
using SafeAuthenticator.Helpers;
using SafeAuthenticator.Services;
using SafeAuthenticator.Views;
using Xamarin.Forms;

namespace SafeAuthenticator.ViewModels {
  internal class LoginViewModel : BaseViewModel {
    private string _acctPassword;
    private string _acctSecret;
    private bool _isUiEnabled;

    public string AcctPassword { get => _acctPassword; set => SetProperty(ref _acctPassword, value); }

    public string AcctSecret { get => _acctSecret; set => SetProperty(ref _acctSecret, value); }

    public ICommand CreateAcctCommand { get; }

    public ICommand LoginCommand { get; }

    public bool IsUiEnabled { get => _isUiEnabled; set => SetProperty(ref _isUiEnabled, value); }

    public bool AuthReconnect {
      get => Authenticator.AuthReconnect;
      set {
        if (Authenticator.AuthReconnect != value) {
          Authenticator.AuthReconnect = value;
        }

        OnPropertyChanged();
      }
    }

    public LoginViewModel() {
      Authenticator.PropertyChanged += (s, e) => {
        if (e.PropertyName == nameof(Authenticator.IsLogInitialised)) {
          IsUiEnabled = Authenticator.IsLogInitialised;
          }
      };

      IsUiEnabled = Authenticator.IsLogInitialised;
      CreateAcctCommand = new Command(OnCreateAcct);
      LoginCommand = new Command(OnLogin);
      AcctSecret = string.Empty;
      AcctPassword = string.Empty;

      MessagingCenter.Subscribe<LoginPage>(this, "AutoReconnectOnStartup", async _ => 
      {
        try
        {
            CredentialCacheService cache = new CredentialCacheService();
            var (location, password) = cache.Retrieve();
               using (UserDialogs.Instance.Loading("Reconnecting to Network"))
               {
                   await Task.Delay(1000);
                   await Authenticator.LoginAsync(location, password);
                   MessagingCenter.Send(this, MessengerConstants.NavHomePage);
               }
        }           
        catch (NullReferenceException)
        {
            Debug.WriteLine("Secret and Password not present in cache");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error:" + ex);
            await Application.Current.MainPage.DisplayAlert("Error", $"Log in Failed: {ex.Message}", "OK");
        }
      });

    }

    private void OnCreateAcct() {
      MessagingCenter.Send(this, MessengerConstants.NavCreateAcctPage);
    }

    public async void OnLogin() {
      IsUiEnabled = false;
      try {
        //await Authenticator.LoginAsync(AcctSecret, AcctPassword);
        await Authenticator.LoginAsync("Decde!996", "Decde!996Theone!996");
        MessagingCenter.Send(this, MessengerConstants.NavHomePage);
      } catch (Exception ex) {
        await Application.Current.MainPage.DisplayAlert("Error", $"Log in Failed: {ex.Message}", "OK");
        IsUiEnabled = true;
      }
    }
  }
}
