﻿using System;
using System.Collections.Generic;
using System.Text;
using BudgetTracker.Domain.Models;
using BudgetTracker.Domain.Services;

namespace BudgetTracker.WPF.ViewModels
{
    public class DashboardViewModel : ViewModelBase

    {
        public static DashboardViewModel LoadDashboardViewModel()
        {
            var dashboardViewModel = new DashboardViewModel();
            return dashboardViewModel;
        }
    }
}
