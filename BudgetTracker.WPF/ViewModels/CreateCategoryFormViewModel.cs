﻿using System;
using System.Collections.Generic;
using System.Security.Policy;
using System.Text;
using System.Windows.Input;
using BudgetTracker.Domain.Models;
using BudgetTracker.WPF.Commands.Create;
using BudgetTracker.WPF.Commands.Remove;
using BudgetTracker.WPF.State.Navigators;
using BudgetTracker.WPF.State.Users;

namespace BudgetTracker.WPF.ViewModels
{
    public class CreateCategoryFormViewModel : ViewModelBase
    {
        // Category properties
        private string _name;
        
        // UI Errors
        private string _nameErrorMessage;
        private string _errorMessage;

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public string NameErrorMessage
        {
            get => _nameErrorMessage;
            set
            {
                _nameErrorMessage = value;
                OnPropertyChanged(nameof(NameErrorMessage));
            }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged(nameof(ErrorMessage));
            }
        }
        public bool CanSubmit => string.IsNullOrEmpty(NameErrorMessage);

        public ICommand SubmitCommand { get; }

        public CreateCategoryFormViewModel(IRenavigator renavigator, IUserStore userStore)
        {
            NameErrorMessage = string.Empty;
            SubmitCommand = new CreateCategoryCommand(this, renavigator, userStore);
        }
    }
}
