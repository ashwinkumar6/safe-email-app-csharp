﻿using System;
using System.Windows.Input;
using SafeAuthenticator.Helpers;            
using Xamarin.Forms;

namespace SafeAuthenticator.ViewModels {
  internal class CreateAcctViewModel : BaseViewModel {
    private string _acctPassword;
    private string _acctSecret;
    private string _invitation;
    private bool _isUiEnabled;

    public string AcctPassword { get => _acctPassword; set => SetProperty(ref _acctPassword, value); }

        // public string AcctSecret { get => _acctSecret; set => SetProperty(ref _acctSecret, value); }

       public string AcctSecret
        {
            get{ return _acctSecret;}
            set { SetProperty(ref _acctSecret, value); OnPropertyChanged(); }
        }

        public string Invitation { get => _invitation; set => SetProperty(ref _invitation, value); }
    public ICommand CreateAcctCommand { get; }
    public bool IsUiEnabled { get => _isUiEnabled; set => SetProperty(ref _isUiEnabled, value); }

        public double SecretStrengthIndicator {
            get { return Authenticator.LoginStrength(AcctSecret).Item1; }
            set
            {
                SecretStrengthIndicator = value;
                OnPropertyChanged();
            }
        }

        public string SecretStrengthText
        {
            get { return Authenticator.LoginStrength(AcctSecret).Item2; }
            set
            {
                SecretStrengthText = value;
                OnPropertyChanged();
            }
        }

        public double PasswordStrengthIndicator
        {
            get { return Authenticator.LoginStrength(AcctPassword).Item1; }
            set
            {
                PasswordStrengthIndicator = value;
                OnPropertyChanged();
            }
        }

        public string PasswordStrengthText
        {
            get { return Authenticator.LoginStrength(AcctPassword).Item2; }
            set
            {
                PasswordStrengthText = value;
                OnPropertyChanged();
            }
        }

        public bool AuthReconnect {
      get => Authenticator.AuthReconnect;
      set {
        if (Authenticator.AuthReconnect != value) {
          Authenticator.AuthReconnect = value;
        }

        OnPropertyChanged();
      }
    }

    public CreateAcctViewModel() {
      Authenticator.PropertyChanged += (s, e) => {
        if (e.PropertyName == nameof(Authenticator.IsLogInitialised)) {
          IsUiEnabled = Authenticator.IsLogInitialised;
        }
      };

      IsUiEnabled = Authenticator.IsLogInitialised;

      CreateAcctCommand = new Command(OnCreateAcct);

      AcctSecret = string.Empty;
      AcctPassword = string.Empty;
      Invitation = string.Empty;
    }

    private async void OnCreateAcct() {
      IsUiEnabled = false;
      try {
        await Authenticator.CreateAccountAsync(AcctSecret, AcctPassword, Invitation);
        MessagingCenter.Send(this, MessengerConstants.NavHomePage);
      } catch (Exception ex) {
        await Application.Current.MainPage.DisplayAlert("Error", $"Create Acct Failed: {ex.Message}", "OK");
        IsUiEnabled = true;
      }
    }
  }
}


